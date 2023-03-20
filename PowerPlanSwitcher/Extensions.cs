using System;
using System.Reflection;
using System.Windows.Forms;

namespace PowerPlanSwitcher
{
    internal static class NotifyIconExtensions
    {
        // A bug in the .NET Framework's WinForms code limits icon text to 64 chars, when in reality the underlying
        // control supports 127 chars. This extension method works around the bug.
        // Found at https://stackoverflow.com/a/580264
        public static void SetText(this NotifyIcon self, string text)
        {
            // If we would exceed the length cap, just ignore and leave the text as-is
            if (text.Length >= 128)
                return;

            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;

            t.GetField("text", hidden).SetValue(self, text);

            if ((bool)t.GetField("added", hidden).GetValue(self))
                t.GetMethod("UpdateIcon", hidden).Invoke(self, new object[] { true });
        }
    }
}
