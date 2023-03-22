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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using PowerPlanSwitcher.Properties;
using PowerPlanSwitcher.Nvidia;
using Microsoft.Win32;

#endregion

namespace PowerPlanSwitcher
{
    public class SysTrayApp : Form
    {
        private readonly PowerPlanManager _powerPlanManager = new PowerPlanManager();
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu = new ContextMenu();

        private HotkeyManager _hotkeyManager;
        private readonly List<IGpuManager> _gpuManagers = new List<IGpuManager>();

        private readonly string _appName = Assembly.GetExecutingAssembly().GetName().Name;
        private readonly RegistryKey _startupRegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private readonly ResourceManager _rm = new ResourceManager("PowerPlanSwitcher.Properties.Resources", typeof(SysTrayApp).Assembly);

        public SysTrayApp()
        {
            Console.WriteLine(@"The current culture is {0}", Thread.CurrentThread.CurrentUICulture);

            InitializePowerPlans();
            InitializeGpuConfigs();
            InitializeMiscMenuItems();
            
            // create systray icon
            _trayIcon = new NotifyIcon
            {
                Text = GetProfileHoverText(_powerPlanManager.ActivePlan.Name),
                Icon = new Icon(Resources.battery_512, 32, 32),
                ContextMenu = _trayMenu,
                Visible = true
            };

            foreach (IGpuManager manager in _gpuManagers)
            {
                _trayIcon.SetText(_trayIcon.Text + GetGpuHoverText(manager.DeviceName, manager.GetAvailablePowerLimits()[manager.GetActivePowerLimitIndex()]));
            }

            _trayIcon.MouseDown += NotifyIcon_Click;
            _trayIcon.MouseClick += NotifyIcon_MouseClick;
        }

		[STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SysTrayApp());
        }

        private void InitializePowerPlans()
        {
            List<MenuItem> _cpuItems = new List<MenuItem>();
            _hotkeyManager = new HotkeyManager(OnHotKeyPressed);

            // create a menu item for each found power plan
            foreach (var powerPlan in _powerPlanManager.PowerPlans)
            {
                if (!_hotkeyManager.RegisterHotKey(_cpuItems.Count, (int)(Keys.D1 + _cpuItems.Count)))
                {
                    MessageBox.Show($"Couldn't register hotkey for profile at index {_cpuItems.Count}");
                }

                _cpuItems.Add(new MenuItem($"{_cpuItems.Count + 1}: {powerPlan.Name}", OnSelectPowerPlan));
            }

            _trayMenu.MenuItems.Add(GetStringResource("CPU Profiles"), _cpuItems.ToArray());
        }

        private void InitializeGpuConfigs()
        {
            GpuConfigList gpuConfigs;

            try
            {
                gpuConfigs = JsonConvert.DeserializeObject<GpuConfigList>(File.ReadAllText("config.json"));
            }
            catch (FileNotFoundException)
            {
                // No gpu config; this is fine
                return;
            }

            foreach (var gpuConfig in gpuConfigs.Gpus)
            {
                switch (gpuConfig.Type)
                {
                    case GpuType.Nvidia:
                        NvidiaManager nvidiaManager = new NvidiaManager(gpuConfig.Config);
                        (InitializationResult result, string error) ret = nvidiaManager.Initialize();

                        if (ret.result == InitializationResult.Error)
                        {
                            MessageBox.Show(ret.error, $"{nameof(NvidiaManager)}: Initialization Error");
                        }
                        else
                        {
                            if (ret.result == InitializationResult.Partial)
                            {
                                MessageBox.Show(ret.error, $"{nameof(NvidiaManager)}: Initialization Warning");
                            }
                            _gpuManagers.Add(nvidiaManager);
                        }
                        break;
                }
            }

            foreach (var manager in _gpuManagers)
            {
                List<MenuItem> _gpuSubEntries = manager.GetAvailablePowerLimits().Select(d => new MenuItem($"{(int)(100 * d)}%", OnSelectGpuPowerLimit)).ToList();

                _gpuSubEntries.Add(new MenuItem("More info...", (sender, e) => manager.ShowInfoForm()));

                if (_gpuSubEntries.Any())
                {
                    _trayMenu.MenuItems.Add("-");
                    _trayMenu.MenuItems.Add(manager.DeviceName, _gpuSubEntries.ToArray());
                }
            }
        }

        private void InitializeMiscMenuItems()
        {
            MenuItem startupItem = new MenuItem(GetStringResource("Run at Startup"), (sender, e) =>
            {
                if (sender is MenuItem menuItem)
                {
                    if (menuItem.Checked)
                    {
                        menuItem.Checked = false;
                        _startupRegKey.DeleteValue(_appName);
                    }
                    else
                    {
                        menuItem.Checked = true;
                        _startupRegKey.SetValue(_appName, Application.ExecutablePath);
                    }
                }
            })
            {
                Checked = _startupRegKey.GetValue(_appName) != null
            };

            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add(startupItem);
            _trayMenu.MenuItems.Add(GetStringResource("Power Options"), (sender, e) => System.Diagnostics.Process.Start("control", "powercfg.cpl"));
            _trayMenu.MenuItems.Add(GetStringResource("Exit"), OnExit);
        }

        private string GetStringResource(string s) => _rm.GetString(s, Thread.CurrentThread.CurrentUICulture);

        private string GetProfileHoverText(string name) => $"Active plan: {name}";

        private string GetGpuHoverText(string name, double scaling) => $"\n\n{name}: {(int)(scaling * 100)}% power";

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            _powerPlanManager.LoadPowerPlans();

            var activePlanIndex = _powerPlanManager.GetIndexOfActivePlan();
            for (int i = 0; i < _trayMenu.MenuItems[0].MenuItems.Count; i++)
            {
                _trayMenu.MenuItems[0].MenuItems[i].Checked = (i == activePlanIndex);
            }

            for (int i = 1; i < _gpuManagers.Count + 1; ++i)
            {
                int activePowerLimitIndex = _gpuManagers[i - 1].GetActivePowerLimitIndex();
                for (int j = 0; j < _trayMenu.MenuItems[2*i].MenuItems.Count; ++j)
                {
                    _trayMenu.MenuItems[2*i].MenuItems[j].Checked = (j == activePowerLimitIndex);
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
                ChangePowerPlan(menuItem.Index);
            }
        }

        private void OnSelectGpuPowerLimit(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                MenuItem parent = menuItem.Parent as MenuItem;
                IGpuManager manager = _gpuManagers[(parent.Index / 2) - 1];

                if (manager.GetActivePowerLimitIndex() == menuItem.Index)
                {
                    return;
                }

                manager.SetPowerLimit(menuItem.Index);

                _trayIcon.Text = GetProfileHoverText(_powerPlanManager.ActivePlan.Name);

                foreach (IGpuManager gpuManager in _gpuManagers)
                {
                    _trayIcon.SetText(_trayIcon.Text + GetGpuHoverText(gpuManager.DeviceName, gpuManager.GetAvailablePowerLimits()[gpuManager.GetActivePowerLimitIndex()]));
                }
            }
        }

        private void OnHotKeyPressed(int keyId) => ChangePowerPlan(keyId, notify: true);

        private void ChangePowerPlan(int index, bool notify = false)
        {
            if (_powerPlanManager.GetIndexOfActivePlan() == index)
            {
                return;
            }

            _trayIcon.Text = GetProfileHoverText(_powerPlanManager.PowerPlans[index].Name);
            _powerPlanManager.SetPowerPlan(index);

            if (notify)
            {
                _trayIcon.BalloonTipTitle = "Power Plan Changed";
                _trayIcon.BalloonTipText = $"Switched to {_trayIcon.Text} profile";
            }

            foreach (IGpuManager manager in _gpuManagers)
            {
                double newPowerLimit = manager.CpuProfileChanged(index);

                if (newPowerLimit > 0.0)
                {
                    _trayIcon.SetText(_trayIcon.Text + GetGpuHoverText(manager.DeviceName, newPowerLimit));

                    if (notify)
                    {
                        _trayIcon.BalloonTipText += $"\n\n{manager.DeviceName} power limit set to {(int)(newPowerLimit * 100)}%";
                    }
                }
            }
            
            if (notify)
            {
                _trayIcon.ShowBalloonTip(2000);
            }
        }

        private static void OnExit(object sender, EventArgs e) => Application.Exit();

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _gpuManagers.ForEach(gm => gm.Dispose());
                _hotkeyManager?.Dispose();
                _trayIcon?.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}