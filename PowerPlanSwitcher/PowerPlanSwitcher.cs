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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using PowerPlanSwitcher.Nvidia;
using PowerPlanSwitcher.Properties;

namespace PowerPlanSwitcher
{
    public class SysTrayApp : Form
    {
        private struct StoredPlanState
        {
            public int PlanIndex;
            public string ChangedByProcessName;
            public int ProcessCount;
        }
        
        private Config _config;
        private readonly PowerPlanManager _powerPlanManager;
        private readonly NotifyIcon _trayIcon;
        private RPCServer _rpcServer;

        private HotkeyManager _hotkeyManager;
        private readonly List<IGpuManager> _gpuManagers = new List<IGpuManager>();

        private readonly string _appName = Assembly.GetExecutingAssembly().GetName().Name;
        private readonly RegistryKey _startupRegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private readonly ResourceManager _rm = new ResourceManager("PowerPlanSwitcher.Properties.Resources", typeof(SysTrayApp).Assembly);

        ManagementEventWatcher _procStartWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
        ManagementEventWatcher _procStopWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));

        private StoredPlanState _storedState = new StoredPlanState();

        public SysTrayApp()
        {
            Console.WriteLine(@"The current culture is {0}", Thread.CurrentThread.CurrentUICulture);

            _powerPlanManager = new PowerPlanManager();
            _powerPlanManager.PowerPlanChanged += OnPowerPlanChanged;

            // create systray icon
            _trayIcon = new NotifyIcon
            {
                Icon = new Icon(Resources.battery_512_green, 32, 32),
                Visible = true
            };

            _trayIcon.MouseDown += NotifyIcon_Click;
            _trayIcon.MouseClick += NotifyIcon_MouseClick;

            LoadConfig();
            InitializeMenus();
            InitializeProcessAutoSwitching();

            if (_config.EnableRPCServer)
            {
                _rpcServer = new RPCServer(_powerPlanManager);
            }
        }

        internal void SetHdrMode(bool enabled)
        {
            _gpuManagers[0].SetHdrMode(enabled);
        }

		[STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SysTrayApp());
        }

        private void LoadConfig()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            catch (FileNotFoundException)
            {
                // No gpu config; this is fine
                return;
            }
        }

        private void InitializeProcessAutoSwitching()
        {
            foreach (ProcessAutoSwitchingConfig conf in _config.ProcessAutoSwitching)
            {
                conf.ProcessNames = conf.ProcessNames.Select(n => n.ToLowerInvariant()).ToArray();
            }

            void startEvent(object sender, EventArrivedEventArgs args)
            {
                string procName = args.NewEvent.Properties["ProcessName"].Value.ToString().ToLowerInvariant();

                // Don't change the plan if we already changed for some other process
                if (String.IsNullOrEmpty(_storedState.ChangedByProcessName))
                {
                    foreach (ProcessAutoSwitchingConfig conf in _config.ProcessAutoSwitching)
                    {
                        if (conf.ProcessNames.Contains(procName))
                        {
                            _storedState = new StoredPlanState()
                            {
                                PlanIndex = _powerPlanManager.GetIndexOfActivePlan(),
                                ChangedByProcessName = procName,
                                ProcessCount = 1
                            };

                            _powerPlanManager.SetPowerPlan(conf.PlanIndex, PowerPlanChangedEventSource.AutoSwitch);
                        }
                    }
                }
                else if (_storedState.ChangedByProcessName == procName)
                {
                    // Some processes can do weird things like spin up sub-processes with the same name for auto-update purposes
                    // If we changed our plan due to a process starting up and then we see it again, add to the counter
                    _storedState.ProcessCount++;
                }

                // Explicitly dispose because in the path where we're not doing anything these WMI objects actually won't get marked for collection
                // https://social.msdn.microsoft.com/Forums/en-US/158d5f4b-1238-4854-a66c-b51e37550c52/memory-leak-in-wmi-when-querying-event-logs?forum=netfxbcl
                args.NewEvent.Dispose();
            };
            
            void stopEvent(object sender, EventArrivedEventArgs args)
            {
                if (!String.IsNullOrEmpty(_storedState.ChangedByProcessName))
                {
                    string procName = args.NewEvent.Properties["ProcessName"].Value.ToString().ToLowerInvariant();

                    if (_storedState.ChangedByProcessName == procName)
                    {
                        // Decrement the counter, and if we've hit 0 (i.e. all instances of the process have exited), switch the plan back
                        if (--_storedState.ProcessCount == 0)
                        {
                            _powerPlanManager.SetPowerPlan(_storedState.PlanIndex, PowerPlanChangedEventSource.AutoSwitch);
                            _storedState.ChangedByProcessName = null;
                        }
                    }
                }

                // Explicitly dispose because in the path where we're not doing anything these WMI objects actually won't get marked for collection
                // https://social.msdn.microsoft.com/Forums/en-US/158d5f4b-1238-4854-a66c-b51e37550c52/memory-leak-in-wmi-when-querying-event-logs?forum=netfxbcl
                args.NewEvent.Dispose();
            };

            _procStartWatcher.EventArrived -= startEvent;
            _procStartWatcher.EventArrived += startEvent;
            _procStartWatcher.Start();

            _procStopWatcher.EventArrived -= stopEvent;
            _procStopWatcher.EventArrived += stopEvent;
            _procStopWatcher.Start();
        }

        private void InitializeMenus()
        {
            ContextMenuStrip trayMenu = new ContextMenuStrip();

            InitializePowerPlans(trayMenu);
            InitializeGpuConfigs(trayMenu);
            InitializeMiscMenuItems(trayMenu);

            if (_config != null && _config.StartupPlanIndex >= 0)
            {
                _powerPlanManager.SetPowerPlan(_config.StartupPlanIndex, PowerPlanChangedEventSource.TrayMenu);
            }

            _trayIcon.ContextMenuStrip = trayMenu;
            _trayIcon.Text = GetProfileHoverText(_powerPlanManager.ActivePlan.Name);

            foreach (IGpuManager manager in _gpuManagers)
            {
                int activeLimitIndex = manager.GetActivePowerLimitIndex();

                if (activeLimitIndex > -1 && activeLimitIndex < manager.GetAvailablePowerLimits().Length)
                {
                    _trayIcon.Text = _trayIcon.Text + GetGpuHoverText(manager.DeviceName, manager.GetAvailablePowerLimits()[activeLimitIndex]);
                }
            }
        }

        private void InitializePowerPlans(ContextMenuStrip trayMenu)
        {
            _hotkeyManager?.Dispose();
            _hotkeyManager = new HotkeyManager(OnHotKeyPressed);

            List<ToolStripMenuItem> _cpuItems = new List<ToolStripMenuItem>();

            // create a menu item for each found power plan
            foreach (var powerPlan in _powerPlanManager.PowerPlans)
            {
                if (!_hotkeyManager.RegisterHotKey(_cpuItems.Count, (int)(Keys.D1 + _cpuItems.Count)))
                {
                    MessageBox.Show($"Couldn't register hotkey for profile at index {_cpuItems.Count}");
                }

                _cpuItems.Add(new ToolStripMenuItem($"{_cpuItems.Count + 1}: {powerPlan.Name}", image: null, OnSelectPowerPlan));
            }

            trayMenu.Items.Add(new ToolStripMenuItem("CPU Profiles", image: null, _cpuItems.ToArray()));
        }

        private void InitializeGpuConfigs(ContextMenuStrip trayMenu)
        {
            if (_config == null)
                return;

            _gpuManagers?.ForEach(gpm => gpm.Dispose());
            _gpuManagers?.Clear();

            foreach (var gpuConfig in _config.Gpus)
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
                List<ToolStripMenuItem> _gpuSubEntries = manager.GetAvailablePowerLimits().Select(d => new ToolStripMenuItem($"{(int)(100 * d)}%", image: null, OnSelectGpuPowerLimit)).ToList();
                ToolStripMenuItem gpuItem = new ToolStripMenuItem(manager.DeviceName);

                if (_gpuSubEntries.Any())
                {
                    gpuItem.DropDownItems.AddRange(_gpuSubEntries.ToArray());
                    gpuItem.DropDownItems.Add("-");
                }

                gpuItem.DropDownItems.Add(new ToolStripMenuItem("Check For Driver Update", image: null, (sender, e) => manager.CheckForDriverUpdate()));
                gpuItem.DropDownItems.Add(new ToolStripMenuItem("Device Info", image: null, (sender, e) => manager.ShowInfoForm()));

                trayMenu.Items.Add("-");
                trayMenu.Items.Add(gpuItem);
            }
        }

        private void InitializeMiscMenuItems(ContextMenuStrip trayMenu)
        {
            trayMenu.Items.Add("-");
            trayMenu.Items.Add(GetStringResource("Power Options"), image: null, (sender, e) => System.Diagnostics.Process.Start("control", "powercfg.cpl"));
            trayMenu.Items.Add("Reload", image: null, (sender, e) =>
            {
                _trayIcon.ContextMenuStrip = null;
                LoadConfig();
                InitializeMenus();
                InitializeProcessAutoSwitching();
                _trayIcon.Notify("Reload Complete", "Configuration reloaded successfully");
            });

            trayMenu.Items.Add(GetStringResource("Exit"), image: null, OnExit);
        }

        private string GetStringResource(string s) => _rm.GetString(s, Thread.CurrentThread.CurrentUICulture);

        private string GetProfileHoverText(string name) => $"Active plan: {name}";

        private string GetGpuHoverText(string name, double scaling) => $"\n\n{name}: {(int)(scaling * 100)}% power";

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            _powerPlanManager.LoadPowerPlans();

            var activePlanIndex = _powerPlanManager.GetIndexOfActivePlan();
            var cpuItem = (ToolStripMenuItem)_trayIcon.ContextMenuStrip.Items[0];

            for (int i = 0; i < cpuItem.DropDownItems.Count; i++)
            {
                ((ToolStripMenuItem)cpuItem.DropDownItems[i]).Checked = (i == activePlanIndex);
            }

            for (int i = 1; i < _gpuManagers.Count + 1; ++i)
            {
                int activePowerLimitIndex = _gpuManagers[i - 1].GetActivePowerLimitIndex();
                var gpuItem = (ToolStripMenuItem)_trayIcon.ContextMenuStrip.Items[2 * i];

                for (int j = 0; j < gpuItem.DropDownItems.Count; ++j)
                {
                    if (gpuItem.DropDownItems[j] is ToolStripMenuItem tsmi)
                    {
                        tsmi.Checked = (j == activePowerLimitIndex);
                    }
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

        private int IndexOfMenuItem(ToolStripMenuItem item, ToolStripItemCollection items)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                if (item.Text == items[i].Text)
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnSelectPowerPlan(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                ToolStripMenuItem parent = menuItem.OwnerItem as ToolStripMenuItem;
                _storedState.ChangedByProcessName = null;
                _powerPlanManager.SetPowerPlan(IndexOfMenuItem(menuItem, parent.DropDownItems), PowerPlanChangedEventSource.TrayMenu);
            }
        }

        private void OnSelectGpuPowerLimit(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                ToolStripMenuItem parent = menuItem.OwnerItem as ToolStripMenuItem;
                IGpuManager manager = _gpuManagers[(IndexOfMenuItem(parent, _trayIcon.ContextMenuStrip.Items) / 2) - 1];
                int gpuMenuIndex = IndexOfMenuItem(menuItem, parent.DropDownItems);

                if (manager.GetActivePowerLimitIndex() == gpuMenuIndex)
                {
                    return;
                }

                manager.SetPowerLimit(gpuMenuIndex);

                _trayIcon.Text = GetProfileHoverText(_powerPlanManager.ActivePlan.Name);

                foreach (IGpuManager gpuManager in _gpuManagers)
                {
                    _trayIcon.Text = _trayIcon.Text + GetGpuHoverText(gpuManager.DeviceName, gpuManager.GetAvailablePowerLimits()[gpuManager.GetActivePowerLimitIndex()]);
                }
            }
        }

        private void OnHotKeyPressed(int keyId)
        {
            _storedState.ChangedByProcessName = null;
            _powerPlanManager.SetPowerPlan(keyId, PowerPlanChangedEventSource.HotKey);
        }

        private void OnPowerPlanChanged(PowerPlanChangedEventArgs args)
        {
            int index = args.Index;
            _trayIcon.Icon = TryGetIconForPlan(index);
            _trayIcon.Text = GetProfileHoverText(_powerPlanManager.PowerPlans[index].Name);

            StringBuilder notificationText = new StringBuilder();
            notificationText.Append($"Switched to {_trayIcon.Text} profile");

            foreach (IGpuManager manager in _gpuManagers)
            {
                double newPowerLimit = manager.CpuProfileChanged(index);

                if (newPowerLimit > 0.0)
                {
                    _trayIcon.Text = _trayIcon.Text + GetGpuHoverText(manager.DeviceName, newPowerLimit);
                    notificationText.Append($"\n\n{manager.DeviceName} power limit set to {(int)(newPowerLimit * 100)}%");
                }
            }

            if (args.Source != PowerPlanChangedEventSource.TrayMenu)
            {
                _trayIcon.Notify("Power Plan Changed", notificationText.ToString());
            }
        }

        private Icon TryGetIconForPlan(int index)
        {
            if (_config.PlanIconColors?.TryGetValue(index.ToString(), out string color) == true)
            {
                switch (color)
                {
                    case "red":
                        return Resources.battery_512_red;
                    case "yellow":
                        return Resources.battery_512_yellow;
                    default:
                        return Resources.battery_512_green;
                }
            }

            return Resources.battery_512_green;
        }

        private static void OnExit(object sender, EventArgs e) => Application.Exit();

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _procStartWatcher?.Dispose();
                _procStopWatcher?.Dispose();
                _startupRegKey?.Dispose();
                _gpuManagers.ForEach(gm => gm.Dispose());
                _hotkeyManager?.Dispose();
                _trayIcon?.Dispose();
                _rpcServer?.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}