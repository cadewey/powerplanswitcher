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

using Microsoft.Win32;
using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using TheRefactory.Properties;

#endregion

namespace TheRefactory
{
    public class SysTrayApp : Form
    {
        private readonly Icon batteryIcon = new Icon(Resources.battery, 32, 32);
        private readonly Icon greenBatteryIcon = new Icon(Resources.battery_green, 32, 32);
        private readonly Icon redBatteryIcon = new Icon(Resources.battery_red, 32, 32);

        private readonly PowerPlanManager _powerPlanManager = new PowerPlanManager();
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu = new ContextMenu();
        private readonly string PersonalizeKey = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public SysTrayApp()
        {
            // init needed vars
            Console.WriteLine(@"The current culture is {0}", Thread.CurrentThread.CurrentUICulture);
            var rm = new ResourceManager("TheRefactory.Properties.Resources", typeof(SysTrayApp).Assembly);

            // create a menu item for each found power plan
            foreach (var powerPlan in _powerPlanManager.PowerPlans)
            {
                _trayMenu.MenuItems.Add(powerPlan.Name, OnSelectPowerPlan);
            }
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add("Light", ActivateLightMode);
            _trayMenu.MenuItems.Add("Dark", ActivateDarkMode);
            _trayMenu.MenuItems.Add("-");
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

        private bool HasDarkMode()
        {
            return (int)Registry.GetValue(PersonalizeKey, "AppsUseLightTheme", 1) == 0 ||
               (int)Registry.GetValue(PersonalizeKey, "SystemUsesLightTheme", 1) == 0;
        }


        private void ActivateDarkMode(object sender, EventArgs e)
        {
            Registry.SetValue(PersonalizeKey, "AppsUseLightTheme", 0);
            Registry.SetValue(PersonalizeKey, "SystemUsesLightTheme", 0);
        }

        private void ActivateLightMode(object sender, EventArgs e)
        {
            Registry.SetValue(PersonalizeKey, "AppsUseLightTheme", 1);
            Registry.SetValue(PersonalizeKey, "SystemUsesLightTheme", 1);
        }

        private Icon GetIcon(PowerPlan plan)
		{
			if (ContainsInsensitive(plan.Name, "high") || ContainsInsensitive(plan.Name, "performance"))
			{
                return redBatteryIcon;
			}
            else if (ContainsInsensitive(plan.Name, "save"))
            {
                return greenBatteryIcon;
            }
            return batteryIcon;
        }

        private bool ContainsInsensitive(string text, string searchTerm)
		{
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(text, searchTerm, CompareOptions.IgnoreCase) >= 0;
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
            var hasDarkMode = HasDarkMode();
            var activePlanIndex = _powerPlanManager.GetIndexOfActivePlan();
            for (int i = 0; i < _trayMenu.MenuItems.Count; i++)
            {
                var item = _trayMenu.MenuItems[i];
                if (i == activePlanIndex || item.Text == "Dark" && hasDarkMode || item.Text == "Light" && !hasDarkMode)
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
            if (sender.GetType() != typeof(MenuItem)) return;
            var menuItem = (MenuItem) sender;
            _trayIcon.Icon = GetIcon(_powerPlanManager.PowerPlans[menuItem.Index]);
            _trayIcon.Text = _powerPlanManager.PowerPlans[menuItem.Index].Name;
            _powerPlanManager.SetPowerPlan(menuItem.Index);
        }

        private static void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
                _trayIcon.Dispose();
            base.Dispose(isDisposing);
        }
    }
}