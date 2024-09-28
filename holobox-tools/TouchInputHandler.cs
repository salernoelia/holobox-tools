using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace holobox_tools
{
    public static class TouchInputHandler
    {
        // Import notwendiger Funktionen zur Erfassung von Touch-Informationen
        [DllImport("user32.dll")]
        private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [Out] TOUCHINPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern void CloseTouchInputHandle(IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct TOUCHINPUT
        {
            public int x;
            public int y;
            public IntPtr hSource;
            public int dwID;
            public int dwFlags;
            public int dwMask;
            public int dwTime;
            public IntPtr dwExtraInfo;
            public int cxContact;
            public int cyContact;
        }

        // Delegate für die Verarbeitung von Touch-Ereignissen
        public static Action<bool, int, int> OnTouchEvent;

        public static void HandleTouch(int lParam)
        {
            IntPtr handle = new IntPtr(lParam);
            int inputCount = 10; // Anzahl der Touch-Eingaben
            TOUCHINPUT[] inputs = new TOUCHINPUT[inputCount];
            if (GetTouchInputInfo(handle, inputCount, inputs, Marshal.SizeOf(typeof(TOUCHINPUT))))
            {
                foreach (var ti in inputs)
                {
                    bool touched = (ti.dwFlags & 0x0002) == 0; // TOUCHEVENTF_UP
                    int x = ti.x / 100; // Umrechnung in Pixel
                    int y = ti.y / 100;
                    OnTouchEvent?.Invoke(!touched, x, y);
                }
            }
            CloseTouchInputHandle(handle);
        }
    }
}
