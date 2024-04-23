using System;
using System.Reflection;
using System.Windows.Forms;

namespace PowerPlanSwitcher
{
    internal static class NotifyIconExtensions
    {
        internal static void Notify(this NotifyIcon self, string title, string body, int duration = 2000)
        {
            self.BalloonTipTitle = title;
            self.BalloonTipText = body;
            self.ShowBalloonTip(duration);
        }
    }
}
