using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.UI.ViewManagement;

namespace PowerPlanSwitcher
{
    internal class RPCServer : IDisposable
    {
        private NamedPipeServerStream _server;
        private readonly PowerPlanManager _powerPlanManager;

        internal RPCServer(PowerPlanManager powerPlanManager)
        {
            _powerPlanManager = powerPlanManager;

            InitializeRPCServer();
        }

        internal void InitializeRPCServer()
        {
            Task.Run(() =>
            {
                var pipeSecurity = new PipeSecurity();
                var currentUser = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

                pipeSecurity.AddAccessRule(new PipeAccessRule(currentUser, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));

                while (true)
                {
                    using (_server = NamedPipeServerStreamAcl.Create("PowerPlanSwitcher", PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.None, 0, 0, pipeSecurity))
                    {
                        _server.WaitForConnection();

                        using (var reader = new StreamReader(_server))
                        {
                            ProcessCommand(reader.ReadToEnd());
                        }
                    }
                }
            });
        }

        private void ProcessCommand(string command)
        {
            command = command.Trim();

            if (command.Split(" ") is [var cmd, var parameter])
            {
                switch (cmd)
                {
                    case "set-power-plan":
                        if (Int32.TryParse(parameter, out int planIndex))
                        {
                            _powerPlanManager.SetPowerPlan(planIndex, PowerPlanChangedEventSource.RPC);
                        }
                        break;
                    case "set-resolution":
                        if (parameter.Split("@") is [var resolution, var freq] && resolution.Split("x") is [var resX, var resY])
                        {
                            if (Int32.TryParse(resX, out int width) && Int32.TryParse(resY, out int height) && Int32.TryParse(freq, out int refreshRate))
                            {
                                var devMode = new WinApi.DevMode();

                                if (WinApi.EnumDisplaySettings(null, -1, ref devMode))
                                {
                                    devMode.width = width;
                                    devMode.height = height;
                                    devMode.frequency = refreshRate;

                                    WinApi.ChangeDisplaySettings(ref devMode, 1);
                                }
                            }
                        }

                        break;
                    /*case "set-hdr":
                        if (WinApi.GetDisplayConfigBufferSizes(WinApi.DisplayConfigFlags.QDC_ONLY_ACTIVE_PATHS, out int pathArrayElements, out int modeInfoArrayElements) == WinApi.WinErrorCode.ERROR_SUCCESS)
                        {
                            var pathInfoArray = new WinApi.DisplayConfigPathInfo[pathArrayElements];
                            var modeInfoArray = new WinApi.DisplayConfigModeInfo[modeInfoArrayElements];

                            var result = WinApi.QueryDisplayConfig(WinApi.DisplayConfigFlags.QDC_ONLY_ACTIVE_PATHS, ref pathArrayElements, pathInfoArray, ref modeInfoArrayElements, modeInfoArray, IntPtr.Zero);

                            foreach (var pathInfo in pathInfoArray)
                            {
                                var deviceNameInfo = new WinApi.DisplayConfigTargetDeviceName();
                                deviceNameInfo.Header.AdapterId = pathInfo.targetInfo.AdapterId;
                                deviceNameInfo.Header.Id = pathInfo.targetInfo.Id;

                                result = WinApi.DisplayConfigGetDeviceInfo(ref deviceNameInfo);

                                string deviceName = result == WinApi.WinErrorCode.ERROR_SUCCESS ? deviceNameInfo.MonitorFriendlyDeviceName.ToString() : String.Empty;
                            }

                            bool success = result == WinApi.WinErrorCode.ERROR_SUCCESS;
                        }
                        break;*/
                    default:
                        break;
                }
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
