// Copyright (C) 2016  PowerPlanSwitcher
// 
// This file is part of PowerPlanSwitcher.
// 
// PowerPlanSwitcher is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// PowerPlanSwitcher is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with PowerPlanSwitcher.  If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using PowerPlanSwitcher.Properties;

#endregion

namespace PowerPlanSwitcher
{
    public class SysTrayApp : Form
    {
        private readonly Icon flatBatteryIcon = new Icon(Resources.battery_512, 32, 32);

        private readonly PowerPlanManager _powerPlanManager = new PowerPlanManager();
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu = new ContextMenu();

        private readonly HotkeyManager _hotkeyManager;

        public SysTrayApp()
        {
            // init needed vars
            Console.WriteLine(@"The current culture is {0}", Thread.CurrentThread.CurrentUICulture);
            var rm = new ResourceManager("PowerPlanSwitcher.Properties.Resources", typeof(SysTrayApp).Assembly);

            _hotkeyManager = new HotkeyManager(OnHotKeyPressed);

            // create a menu item for each found power plan
            foreach (var powerPlan in _powerPlanManager.PowerPlans)
            {
                if (!_hotkeyManager.RegisterHotKey(_trayMenu.MenuItems.Count, (int)(Keys.D1 + _trayMenu.MenuItems.Count)))
                {
                    MessageBox.Show($"Couldn't register hotkey for profile at index {_trayMenu.MenuItems.Count}");
                }

                _trayMenu.MenuItems.Add($"{_trayMenu.MenuItems.Count + 1}: {powerPlan.Name}", OnSelectPowerPlan);
            }
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add("Power Options", (sender, e) => System.Diagnostics.Process.Start("control", "powercfg.cpl"));
            _trayMenu.MenuItems.Add(rm.GetString("Exit", Thread.CurrentThread.CurrentUICulture), OnExit);

            // create systray icon
            _trayIcon = new NotifyIcon
            {
                Text = _powerPlanManager.ActivePlan.Name,
                Icon = GetIcon(_powerPlanManager.ActivePlan)
            };
            _trayIcon.MouseDown += NotifyIcon_Click;
            _trayIcon.MouseClick += NotifyIcon_MouseClick;

            // add menu to systray icon
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        private Icon GetIcon(PowerPlan plan)
		{
            return flatBatteryIcon;
        }

		[STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SysTrayApp());
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            _powerPlanManager.LoadPowerPlans();

            var activePlanIndex = _powerPlanManager.GetIndexOfActivePlan();
            for (int i = 0; i < _trayMenu.MenuItems.Count; i++)
            {
                var item = _trayMenu.MenuItems[i];
                if (i == activePlanIndex)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var methodInfo = typeof(NotifyIcon).GetMethod("ShowContextMenu",
                BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(_trayIcon, null);
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // hide form window
            ShowInTaskbar = false; // remove from taskbar

            base.OnLoad(e);
        }

        private void OnSelectPowerPlan(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                _trayIcon.Icon = GetIcon(_powerPlanManager.PowerPlans[menuItem.Index]);
                _trayIcon.Text = _powerPlanManager.PowerPlans[menuItem.Index].Name;
                _powerPlanManager.SetPowerPlan(menuItem.Index);
            }
        }

        private void OnHotKeyPressed(int keyId)
        {
            _trayIcon.Icon = GetIcon(_powerPlanManager.PowerPlans[keyId]);
            _trayIcon.Text = _powerPlanManager.PowerPlans[keyId].Name;
            _powerPlanManager.SetPowerPlan(keyId);

            _trayIcon.BalloonTipTitle = "Power Plan Changed";
            _trayIcon.BalloonTipText = $"Switched to {_trayIcon.Text}";
            _trayIcon.ShowBalloonTip(2000);
        }

        private static void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _hotkeyManager?.Dispose();
                _trayIcon?.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}