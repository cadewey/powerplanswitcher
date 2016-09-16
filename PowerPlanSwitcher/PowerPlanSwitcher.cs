using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace TheRefactory
{
    public class SysTrayApp : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        private readonly NotifyIcon trayIcon;
        private readonly ContextMenu trayMenu;
        private readonly PowerPlanManager powerPlanManager;

        public SysTrayApp()
        {
            // init needed vars
            powerPlanManager = new PowerPlanManager();
            trayMenu = new ContextMenu();
            Console.WriteLine("The current culture is {0}", Thread.CurrentThread.CurrentUICulture);
            ResourceManager rm = new ResourceManager("TheRefactory.Properties.Resources", typeof(SysTrayApp).Assembly);

            // create a menu item for each found power plan
            foreach (PowerPlan powerPlan in powerPlanManager.powerPlans)
            {
                trayMenu.MenuItems.Add(powerPlan.name, OnSelectPowerPlan);
            }
            trayMenu.MenuItems[powerPlanManager.GetIndexOfActivePlan()].DefaultItem = true;
            trayMenu.MenuItems.Add("-");
           
            // check for new version
            if (UpdateChecker.isUpdateAvailable(this, false, Properties.Resources.urlCheckUpdate + UpdateChecker.getUuid(typeof(SysTrayApp)), UpdateChecker.getAssemblyVersionAsInteger(), UpdateChecker.getUuid(typeof(SysTrayApp))))
            {
                trayMenu.MenuItems.Add(rm.GetString("Update", Thread.CurrentThread.CurrentUICulture), OnUpdate);
            }
            trayMenu.MenuItems.Add(rm.GetString("Exit", Thread.CurrentThread.CurrentUICulture), OnExit);

            // create systray icon
            trayIcon = new NotifyIcon();
            trayIcon.Text = rm.GetString("Powerplan") + " Switcher";
            trayIcon.Icon = new Icon(Properties.Resources._1474044465_battery, 40, 40);
            trayIcon.MouseDown += NotifyIcon_Click;
            trayIcon.MouseClick += NotifyIcon_MouseClick;

            // add menu to systray icon
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            UpdateChecker.isUpdateAvailable(this, true, Properties.Resources.urlCheckUpdate + UpdateChecker.getUuid(typeof(SysTrayApp)), UpdateChecker.getAssemblyVersionAsInteger(), UpdateChecker.getUuid(typeof(SysTrayApp)));
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            powerPlanManager.LoadPowerPlans();
            trayMenu.MenuItems[powerPlanManager.GetIndexOfActivePlan()].DefaultItem = true;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo methodInfo = typeof(NotifyIcon).GetMethod("ShowContextMenu",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                methodInfo.Invoke(trayIcon, null);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // hide form window
            ShowInTaskbar = false; // remove from taskbar
            base.OnLoad(e);
        }

        private void OnSelectPowerPlan(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(MenuItem))
            {
                MenuItem menuItem = (MenuItem)sender;
                menuItem.DefaultItem = true;
                powerPlanManager.SetPowerPlan(menuItem.Index);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                trayIcon.Dispose();
            }
            base.Dispose(isDisposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // SysTrayApp
            // 
            ClientSize = new System.Drawing.Size(284, 261);
            Name = "SysTrayApp";
            Load += new EventHandler(this.SysTrayApp_Load);
            ResumeLayout(false);

        }

        private void SysTrayApp_Load(object sender, EventArgs e)
        {

        }
    }
}
