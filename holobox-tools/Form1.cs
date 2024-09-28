using System;
using System.Drawing;
using System.Windows.Forms;
using SharpOSC;
using AForge.Video;
using AForge.Video.DirectShow;
using NAudio.Wave;
using System.Net.Sockets;
using System.Text;

namespace holobox_tools
{
    public partial class Form1 : Form
    {
        // OSC Sender using SharpOSC
        private UDPSender oscSender;

        // Video devices
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        // Audio devices
        private WaveInEvent waveIn;
        private UdpClient audioClient;

        // UDP for video
        private UdpClient videoClient;
        private int videoPort = 5005; // Beispielport

        // Touch state
        private bool isTouched = false;

        // Overlay form
        private OverlayForm overlay;

        public Form1()
        {
            InitializeComponent();
            InitializeOSC();
            InitializeMediaDevices();
            InitializeTouch();
            InitializeOverlay();
        }

        private void InitializeOSC()
        {
            // Standard OSC-Adresse und Port
            string oscAddress = txtOSCAddress.Text; // Beispiel: "127.0.0.1"
            if (!int.TryParse(txtOSCPort.Text, out int oscPort))
            {
                oscPort = 8000; // Standardport, falls Parsing fehlschlägt
            }

            // Erstellen der UDPSender-Instanz
            oscSender = new UDPSender(oscAddress, oscPort);
            Log($"OSC Sender initialisiert mit Adresse {oscAddress} und Port {oscPort}.");
        }

        private void InitializeMediaDevices()
        {
            // Initialisieren der Video-Geräte
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                cmbWebcam.Items.Add(device.Name);
            }
            if (cmbWebcam.Items.Count > 0)
                cmbWebcam.SelectedIndex = 0;

            // Initialisieren der Audio-Geräte
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                cmbMicrophone.Items.Add(deviceInfo.ProductName);
            }
            if (cmbMicrophone.Items.Count > 0)
                cmbMicrophone.SelectedIndex = 0;

            Log("Medien-Geräte initialisiert.");
        }

        private void InitializeTouch()
        {
            // Abonnieren der Touch-Ereignisse
            TouchInputHandler.OnTouchEvent += TouchInputHandler_OnTouchEvent;
            Log("Touch-Eingabe initialisiert.");
        }

        private void InitializeOverlay()
        {
            overlay = new OverlayForm();
            overlay.Show();
            overlay.Hide(); // Start verborgen; anzeigen bei Start des Streamings
        }

        private void TouchInputHandler_OnTouchEvent(bool touched, int x, int y)
        {
            if (touched && !isTouched)
            {
                isTouched = true;
                SendOSCTouch(true, x, y);
                Log($"Touch gestartet bei ({x}, {y})");
            }
            else if (!touched && isTouched)
            {
                isTouched = false;
                SendOSCTouch(false, x, y);
                Log($"Touch beendet bei ({x}, {y})");
            }
        }


        private void SendOSCTouch(bool touched, int x, int y)
        {
            // Erstellen der OSC-Nachricht
            var message = new OscMessage("/touch", touched ? 1 : 0, x, y);
            oscSender.Send(message);
            Log($"OSC Nachricht gesendet: /touch {(touched ? 1 : 0)} bei ({x}, {y})");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartStreaming();
            overlay.Show(); // Anzeigen des Overlays beim Start des Streamings
            Log("Streaming gestartet.");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopStreaming();
            overlay.Hide(); // Verbergen des Overlays beim Stoppen des Streamings
            Log("Streaming gestoppt.");
        }

        private void StartStreaming()
        {
            // Schließen vorheriger Verbindungen, falls vorhanden
            if (oscSender != null)
            {
                oscSender = null;
            }

            string oscAddress = txtOSCAddress.Text;
            if (!int.TryParse(txtOSCPort.Text, out int oscPort))
            {
                oscPort = 8000; // Standardport
            }

            // OSC-Sender initialisieren
            oscSender = new UDPSender(oscAddress, oscPort);
            Log($"OSC-Sender initialisiert: Adresse {oscAddress}, Port {oscPort}");

            // Starten des Video-Streamings
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[cmbWebcam.SelectedIndex].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
                Log($"Video-Streaming gestartet. Sende auf IP: {oscAddress}, Port: {videoPort}");
            }
            else
            {
                Log("Keine Video-Geräte gefunden.");
            }

            // Starten des Audio-Streamings
            if (cmbMicrophone.Items.Count > 0)
            {
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = cmbMicrophone.SelectedIndex;
                waveIn.WaveFormat = new WaveFormat(44100, 1);
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.StartRecording();

                audioClient = new UdpClient();
                Log($"Audio-Streaming gestartet. Sende auf IP: {oscAddress}, Port: 5006");
            }
            else
            {
                Log("Keine Audio-Geräte gefunden.");
            }
        }


        private void StopStreaming()
        {
            // Stoppen des Video-Streamings
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.NewFrame -= VideoSource_NewFrame;
                videoSource.SignalToStop();
                videoSource = null;
                Log("Video-Streaming gestoppt.");
            }

            // Stoppen des Audio-Streamings
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;

                audioClient.Close();
                audioClient = null;
                Log("Audio-Streaming gestoppt.");
            }

            // Verwerfen der UDPSender-Instanz
            if (oscSender != null)
            {
                oscSender = null;
                Log("OSC Sender verworfen.");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Konvertieren des Frames zu JPEG und Senden über UDP
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            // Optional: Skalieren des Bildes auf eine kleinere Auflösung
            int newWidth = 320; // Beispielbreite
            int newHeight = 240; // Beispielhöhe
            Bitmap resizedBitmap = new Bitmap(bitmap, new Size(newWidth, newHeight));

            using (var ms = new System.IO.MemoryStream())
            {
                // Speichern mit niedrigerer JPEG-Qualität
                System.Drawing.Imaging.ImageCodecInfo jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(encoder, 50L); // Qualität von 50
                encoderParams.Param[0] = encoderParam;

                resizedBitmap.Save(ms, jpgEncoder, encoderParams);
                byte[] imageBytes = ms.ToArray();

                try
                {
                    if (videoClient == null)
                    {
                        videoClient = new UdpClient();
                    }

                    // Überprüfen der Größe des Pakets
                    if (imageBytes.Length > 65000) // Etwas unter dem UDP-Limit
                    {
                        Log($"Fehler beim Video-Streaming: Paketgröße {imageBytes.Length} Bytes überschreitet das Limit.");
                        return;
                    }

                    videoClient.Send(imageBytes, imageBytes.Length, oscSender.Address, videoPort);
                }
                catch (Exception ex)
                {
                    Log($"Fehler beim Video-Streaming: {ex.Message}");
                }
            }

            // Ressourcen freigeben
            bitmap.Dispose();
            resizedBitmap.Dispose();
        }

        // Methode zum Abrufen des Encoders
        private System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Senden der Audio-Daten über UDP
            try
            {
                if (audioClient == null)
                {
                    audioClient = new UdpClient();
                }
                audioClient.Send(e.Buffer, e.BytesRecorded, oscSender.Address, 5006); // Beispiel-Audioport
            }
            catch (Exception ex)
            {
                Log($"Fehler beim Audio-Streaming: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action<string>(Log), message);
            }
            else
            {
                lstLog.Items.Add($"{DateTime.Now}: {message}");
                lstLog.TopIndex = lstLog.Items.Count - 1; // Automatisches Scrollen
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopStreaming();
            if (overlay != null)
            {
                overlay.Close();
            }
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Zusätzliche Initialisierung, falls erforderlich
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Code, der ausgeführt wird, wenn label1 geklickt wird
        }

        private void cmbWebcam_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtOSCPort_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
