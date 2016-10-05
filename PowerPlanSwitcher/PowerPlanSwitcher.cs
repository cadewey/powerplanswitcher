using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using TheRefactory.Properties;

namespace TheRefactory
{
    public class SysTrayApp : Form
    {
        private readonly PowerPlanManager _powerPlanManager = new PowerPlanManager();
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu = new ContextMenu();

        private readonly Icon[] _icons = {
            new Icon(Resources.battery, 32, 32),
            new Icon(Resources.battery_red, 32, 32),
            new Icon(Resources.battery_green, 32, 32)
        };

        public SysTrayApp()
        {
            // init needed vars
            Console.WriteLine(@"The current culture is {0}", Thread.CurrentThread.CurrentUICulture);
            var rm = new ResourceManager("TheRefactory.Properties.Resources", typeof(SysTrayApp).Assembly);

            // create a menu item for each found power plan
            foreach (var powerPlan in _powerPlanManager.PowerPlans)
                _trayMenu.MenuItems.Add(powerPlan.Name, OnSelectPowerPlan);
            _trayMenu.MenuItems[_powerPlanManager.GetIndexOfActivePlan()].DefaultItem = true;
            _trayMenu.MenuItems.Add("-");

            // check for new version
            if (UpdateChecker.IsUpdateAvailable(this, false,
                Resources.urlCheckUpdate + UpdateChecker.GetUuid(typeof(SysTrayApp)),
                UpdateChecker.GetAssemblyVersionAsInteger(), UpdateChecker.GetUuid(typeof(SysTrayApp))))
                _trayMenu.MenuItems.Add(rm.GetString("Update", Thread.CurrentThread.CurrentUICulture), OnUpdate);
            _trayMenu.MenuItems.Add(rm.GetString("Exit", Thread.CurrentThread.CurrentUICulture), OnExit);

            // create systray icon
            _trayIcon = new NotifyIcon
            {
                Text = rm.GetString("Powerplan") + @" Switcher",
                Icon = _icons[_powerPlanManager.GetIndexOfActivePlan()]
            };
            _trayIcon.MouseDown += NotifyIcon_Click;
            _trayIcon.MouseClick += NotifyIcon_MouseClick;

            // add menu to systray icon
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            UpdateChecker.IsUpdateAvailable(this, true,
                Resources.urlCheckUpdate + UpdateChecker.GetUuid(typeof(SysTrayApp)),
                UpdateChecker.GetAssemblyVersionAsInteger(), UpdateChecker.GetUuid(typeof(SysTrayApp)));
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            _powerPlanManager.LoadPowerPlans();
            _trayMenu.MenuItems[_powerPlanManager.GetIndexOfActivePlan()].DefaultItem = true;
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
            menuItem.DefaultItem = true;
            _powerPlanManager.SetPowerPlan(menuItem.Index);
            _trayIcon.Icon = _icons[_powerPlanManager.GetIndexOfActivePlan()];
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