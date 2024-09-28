using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Timers; // Importieren Sie den Timer-Namespace

namespace holobox_tools
{
    public partial class OverlayForm : Form
    {
        // Import notwendiger Windows API-Funktionen
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;

        [DllImport("user32.dll")]
        private static extern bool RegisterTouchWindow(IntPtr hWnd, uint flags);

        private bool isTouchOrClickActive = false;
        private System.Timers.Timer touchTimer;

        public OverlayForm()
        {
            InitializeComponent();
            MakeVisible();
            RegisterTouch();
            this.MouseDown += OverlayForm_MouseDown;   // Event-Handler für Mausklick hinzufügen
            this.MouseUp += OverlayForm_MouseUp;       // Event-Handler für Loslassen hinzufügen

            // Timer initialisieren, um alle 100 ms ein OSC-Signal zu senden
            touchTimer = new System.Timers.Timer(100);
            touchTimer.Elapsed += TouchTimer_Elapsed;
            touchTimer.AutoReset = true;
        }

        private void MakeVisible()
        {
            // Entfernen Sie WS_EX_TRANSPARENT, damit das Fenster Klicks empfangen kann
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            exStyle &= ~WS_EX_TRANSPARENT; // WS_EX_TRANSPARENT entfernen
            exStyle |= WS_EX_LAYERED | WS_EX_TOOLWINDOW;
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);

            // Setzen Sie die Opacity für Sichtbarkeit
            this.Opacity = 0.01; // 20% transparent

            // Optional: Setzen Sie eine Hintergrundfarbe für bessere Sichtbarkeit
            this.BackColor = System.Drawing.Color.Blue;
            this.TransparencyKey = System.Drawing.Color.Lime; // Verwenden Sie eine Farbe, die nicht im BackColor enthalten ist
        }

        private void RegisterTouch()
        {
            bool result = RegisterTouchWindow(this.Handle, 0);
            if (!result)
            {
                MessageBox.Show("Fehler beim Registrieren des Touch-Fensters.");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Verbergen des Fensters in Alt-Tab
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW;
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);
        }

        // Überschreiben von WndProc zur Erfassung von Touch-Ereignissen
        protected override void WndProc(ref Message m)
        {
            const int WM_TOUCH = 0x0240;
            if (m.Msg == WM_TOUCH)
            {
                // Verarbeiten der Touch-Eingabe
                TouchInputHandler.HandleTouch(m.LParam.ToInt32());
            }
            base.WndProc(ref m);
        }

        // Event-Handler für Linksklicks
        private void OverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)

            {
                // Erfassen der Position des Klicks
                int x = e.X;
                int y = e.Y;

                // Berechnen der absoluten Position auf dem Bildschirm
                Point screenPoint = this.PointToScreen(new Point(x, y));

                // Starten des Timers und Senden des ersten Touch-Ereignisses
                isTouchOrClickActive = true;
                touchTimer.Start();
                TouchInputHandler.OnTouchEvent?.Invoke(true, screenPoint.X, screenPoint.Y); // Sende 1
            }
        }

        // Event-Handler für das Loslassen des Mausklicks
        private void OverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)

            {
                // Erfassen der Position des Klicks
                int x = e.X;
                int y = e.Y;

                // Berechnen der absoluten Position auf dem Bildschirm
                Point screenPoint = this.PointToScreen(new Point(x, y));

                // Beenden des Timers und Senden des End-Touch-Ereignisses
                isTouchOrClickActive = false;
                touchTimer.Stop();
                TouchInputHandler.OnTouchEvent?.Invoke(false, screenPoint.X, screenPoint.Y); // Sende 0
            }
        }

        // Methode, die vom Timer alle 100 ms aufgerufen wird
        private void TouchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isTouchOrClickActive)
            {
                // Kontinuierliches Senden von "1", solange die Berührung/Klick aktiv ist
                var position = Cursor.Position; // Cursor-Position als Beispiel verwenden
                TouchInputHandler.OnTouchEvent?.Invoke(true, position.X, position.Y);
            }
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            // Zusätzliche Initialisierung, falls erforderlich
        }
    }
}
