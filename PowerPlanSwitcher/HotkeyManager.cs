using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerPlanSwitcher
{
    enum KeyModifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        WinKey = 8
    }

    public sealed class HotkeyManager : NativeWindow, IDisposable
    {
        const int WM_HOTKEY = 0x0312;

        private readonly Action<int> _onHotKeyPressed;
        private readonly List<int> _registrationIds;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public HotkeyManager(Action<int> onHotKeyPressed)
        {
            CreateHandle(new CreateParams());
            _onHotKeyPressed = onHotKeyPressed;
            _registrationIds = new List<int>();
        }

        public bool RegisterHotKey(int id, int keyId)
        {
            if (RegisterHotKey(this.Handle, id, (int)(KeyModifier.Alt | KeyModifier.Shift), keyId))
            {
                _registrationIds.Add(id);
                return true;
            }

            return false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                if (_onHotKeyPressed != null)
                {
                    // The ID of the hotkey that was pressed, which in our case maps to the index of the profile requested
                    _onHotKeyPressed(m.WParam.ToInt32());
                }
            }

            base.WndProc(ref m);
        }

        public void Dispose()
        {
            foreach (int id in _registrationIds)
            {
                UnregisterHotKey(this.Handle, id);
            }

            DestroyHandle();
        }
    }
}
