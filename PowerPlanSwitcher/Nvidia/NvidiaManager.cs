﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerPlanSwitcher.Nvidia
{
    internal class NvidiaManager : IGpuManager, IDisposable
    {
        private class DriverVersionNumber
        {
            public int Major { get; private set; }
            public int Minor { get; private set; }

            public DriverVersionNumber(string versionString)
            {
                string[] parts = versionString.Split('.');
                Major = int.Parse(parts[0]);
                Minor = int.Parse(parts[1]);
            }

            public override string ToString() => $"{Major}.{Minor}";

            public bool IsNewerThan(DriverVersionNumber other)
            {
                return other == null ||
                    this.Major > other.Major ||
                    (this.Major == other.Major && this.Minor > other.Minor);
            }
        }

        private IntPtr _nvmlDeviceHandle;
        private IntPtr _nvapiDeviceHandle;

        private string _deviceName;
        private string _driverVersion;
        private string _vbiosVersion;
        private uint _pcieLinkWidth;
        private uint _pcieLinkGeneration;
        private uint _cudaCores;
        private uint _memoryBusWidth;
        
        private uint _defaultPowerLimit;
        private uint _minPowerLimit;
        private uint _maxPowerLimit;

        private bool _initialized = false;
        private readonly NvidiaConfig _config;
        private NvidiaInfoForm _infoForm;

        internal NvidiaManager(JObject configJson)
        {
            _config = configJson.ToObject<NvidiaConfig>();
            
            if (_config == null)
            {
                throw new ArgumentException($"Couldn't convert JObject to {nameof(NvidiaConfig)}. JSON: {configJson.ToString(Formatting.None)}");
            }
        }

        public (InitializationResult Result, string Error) Initialize()
        {
            (bool Success, string Error) result = InitializeNvml();

            if (!result.Success)
            {
                return (InitializationResult.Error, result.Error); // Nvml failure is fatal
            }

            result = InitializeNvApi();
            ReadNonCriticalValues();

            _initialized = true;

            return (result.Success ? InitializationResult.Ok : InitializationResult.Partial, result.Error);
        }

        private (bool Success, string Error) InitializeNvml()
        {
            if (!NvmlApi.NvmlInitV2().IsSuccess())
            {
                return (false, "Couldn't initialize NVML API. GPU control will not be available.");
            }

            if (!NvmlApi.NvmlDeviceGetHandleByIndex(_config.DeviceIndex, out _nvmlDeviceHandle).IsSuccess())
            {
                return (false, $"Couldn't get handle to NVML device with index {_config.DeviceIndex}. GPU control will not be available.");
            }

            StringBuilder name = new StringBuilder();
            if (NvmlApi.NvmlDeviceGetName(_nvmlDeviceHandle, name, NvmlApi.DeviceNameBufferSize).IsSuccess())
            {
                _deviceName = name.ToString();
            }
            else
            {
                return (false, $"Couldn't get device name for NVML device with index {_config.DeviceIndex}. GPU control will not be available.");
            }

            if (!NvmlApi.NvmlDeviceGetPowerManagementDefaultLimit(_nvmlDeviceHandle, out _defaultPowerLimit).IsSuccess())
            {
                return (false, $"Couldn't get default power limit for NVML device with index {_config.DeviceIndex}. GPU control will not be available.");
            }

            if (!NvmlApi.NvmlDeviceGetPowerManagementLimitConstraints(_nvmlDeviceHandle, out _minPowerLimit, out _maxPowerLimit).IsSuccess())
            {
                return (false, $"Couldn't get min/max power limits for NVML device with index {_config.DeviceIndex}. GPU control will not be available.");
            }

            return (true, String.Empty);
        }

        private (bool Success, string Error) InitializeNvApi()
        {
            IntPtr[] gpusPtr = new IntPtr[NvApi.MaxPhysicalGPUs];

            NvApi.Initialize();
            NvApi.EnumPhysicalGpus(gpusPtr, out uint pGpuCount);

            _nvapiDeviceHandle = gpusPtr[_config.DeviceIndex];
            
            return (true, String.Empty);
        }

        private void ReadNonCriticalValues()
        {
            StringBuilder buffer = new StringBuilder();

            if (NvmlApi.NvmlSystemGetDriverVersion(buffer, NvmlApi.SystemDriverVersionBufferSize).IsSuccess())
            {
                _driverVersion = buffer.ToString();
            }

            if (NvmlApi.NvmlDeviceGetVbiosVersion(_nvmlDeviceHandle, buffer.Clear(), NvmlApi.DeviceVbiosVersionBufferSize).IsSuccess())
            {
                _vbiosVersion = buffer.ToString();
            }

            NvmlApi.NvmlDeviceGetNumGpuCores(_nvmlDeviceHandle, out _cudaCores);
            NvmlApi.NvmlDeviceGetCurrPcieLinkGeneration(_nvmlDeviceHandle, out _pcieLinkGeneration);
            NvmlApi.NvmlDeviceGetCurrPcieLinkWidth(_nvmlDeviceHandle, out _pcieLinkWidth);
            NvmlApi.NvmlDeviceGetMemoryBusWidth(_nvmlDeviceHandle, out _memoryBusWidth);
        }

        public string DeviceName => _deviceName;
        public string VbiosVersion => _vbiosVersion;
        public string DriverVersion => _driverVersion;
        public uint CoreCount => _cudaCores;
        public string PCIeLinkStatus => $"PCIe {_pcieLinkGeneration}.0 x{_pcieLinkWidth}";
        public uint MemoryBusWidth => _memoryBusWidth;

        public uint GetActivePowerLimit()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetPowerManagementLimit(_nvmlDeviceHandle, out uint limit).IsSuccess())
            {
                return limit;
            }

            return _defaultPowerLimit;
        }

        public int GetActivePowerLimitIndex()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetPowerManagementLimit(_nvmlDeviceHandle, out uint limit).IsSuccess())
            {
                double activeLimitScaling = (double)limit / _defaultPowerLimit;
                return Array.IndexOf(_config.AvailablePowerScaling, activeLimitScaling);
            }

            return -1;
        }

        public double[] GetAvailablePowerLimits()
        {
            EnsureInitialized();

            return _config.AvailablePowerScaling;
        }

        public uint GetDefaultPowerLimit() => _defaultPowerLimit;

        public uint GetCurrentPowerDraw()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetPowerUsage(_nvmlDeviceHandle, out uint power).IsSuccess())
            {
                return power;
            }

            return 0;
        }

        public NvmlMemory GetMemoryInfo()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetMemoryInfo(_nvmlDeviceHandle, out NvmlMemory memory).IsSuccess())
            {
                return memory;
            }

            return default(NvmlMemory);
        }

        public (uint Gpu, uint Mem) GetCurrentClockFrequencies() => GetClockFrequencies(NvApiClockType.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);

        public (uint Gpu, uint Mem) GetBaseClockFrequencies() => GetClockFrequencies(NvApiClockType.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);

        public (uint Gpu, uint Mem) GetBoostClockFrequencies() => GetClockFrequencies(NvApiClockType.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);

        private (uint Gpu, uint Mem) GetClockFrequencies(NvApiClockType type)
        {
            if (_nvapiDeviceHandle != IntPtr.Zero)
            {
                NvApiClockFrequenciesV2 clockFrequencies = new NvApiClockFrequenciesV2(type);

                if (NvApi.GetAllClockFrequencies(_nvapiDeviceHandle, ref clockFrequencies).IsSuccess())
                {
                    return (
                        clockFrequencies.Domain[(int)NvApiClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS].Frequency,
                        clockFrequencies.Domain[(int)NvApiClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY].Frequency
                    );
                }
            }

            return (0, 0);
        }

        public NvmlUtilization GetUtilization()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetUtilizationRates(_nvmlDeviceHandle, out NvmlUtilization utilization).IsSuccess())
            {
                return utilization;
            }

            return default(NvmlUtilization);
        }

        public uint GetGpuTemperature()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetTemperature(_nvmlDeviceHandle, NvmlTempteratureSensor.NVML_TEMPERATURE_GPU, out uint temp).IsSuccess())
            {
                return temp;
            }

            return 0;
        }

        public uint GetGpuTemperatureThreshold()
        {
            EnsureInitialized();

            if (NvmlApi.NvmlDeviceGetTemperatureThreshold(_nvmlDeviceHandle, NvmlTemperatureThreshold.NVML_TEMPERATURE_THRESHOLD_GPU_MAX, out uint temp).IsSuccess())
            {
                return temp;
            }

            return 100;
        }

        public double SetPowerLimit(int powerLimitIndex)
        {
            EnsureInitialized();

            double scaling = _config.AvailablePowerScaling[powerLimitIndex];

            return ApplyScaling(scaling);
        }

        public double CpuProfileChanged(int index)
        {
            if (_config.PlanAutoScaling.Any())
            {
                NvidiaAutoScaling autoScaling = _config.PlanAutoScaling.FirstOrDefault(s => s.PlanIndex == index);

                if (autoScaling != null)
                {
                    return ApplyScaling(autoScaling.Scaling);
                }
            }

            return 0.0;
        }

        public bool SetHdrMode(bool enabled)
        {
            uint displayIdCount = 1;
            NvGpuDisplayIds[] displayIds = null;

            if (!NvApi.GetAllDisplayIds(_nvapiDeviceHandle, displayIds, ref displayIdCount).IsSuccess())
            {
                return false;
            }

            Array.Resize(ref displayIds, (int)displayIdCount);
            displayIds[0].Version = NvApi.MakeNvApiVersion<NvGpuDisplayIds>(3);

            if (NvApi.GetAllDisplayIds(_nvapiDeviceHandle, displayIds, ref displayIdCount).IsSuccess())
            {
                IEnumerable<NvGpuDisplayIds> connectedDisplays = displayIds.Where(d => d.IsPhysicallyConnected);

                foreach (NvGpuDisplayIds display in connectedDisplays)
                {
                    var colorData = new NvHdrColorDataV2()
                    {
                        Version = NvApi.MakeNvApiVersion<NvHdrColorDataV2>(2),
                        Cmd = NvHdrCmd.NV_HDR_CMD_SET,
                        StaticMetadataDescriptorId = NvStaticMetadataDescriptorId.NV_STATIC_METADATA_TYPE_1,
                        HdrMode = enabled ? NvHdrMode.NV_HDR_MODE_UHDA : NvHdrMode.NV_HDR_MODE_SDR,
                        DynamicRange = NvDynamicRange.NV_DYNAMIC_RANGE_AUTO
                    };

                    if (!NvApi.DispHdrColorMode(display.DisplayId, ref colorData).IsSuccess())
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public void ShowInfoForm()
        {
            if (_infoForm != null)
            {
                if (_infoForm.WindowState == FormWindowState.Minimized)
                {
                    _infoForm.WindowState = FormWindowState.Normal;
                    _infoForm.Activate();       
                }

                return;
            }

            _infoForm = new NvidiaInfoForm(this);
            _infoForm.StartPosition = FormStartPosition.CenterScreen;
            _infoForm.FormClosed += (sender, e) => _infoForm = null;
            _infoForm.ShowDialog();
        }

        public async void CheckForDriverUpdate()
        {
            if (String.IsNullOrEmpty(_config.DriverSearchUrl) || !Uri.TryCreate(_config.DriverSearchUrl, UriKind.Absolute, out _))
            {
                MessageBox.Show(@"DriverSearchUrl is blank or invalid. Please check your config.");
                return;
            }

            HtmlAgilityPack.HtmlDocument doc = await new HtmlWeb().LoadFromWebAsync(_config.DriverSearchUrl);
            HtmlNode latestDriver = doc.DocumentNode.Descendants("tr")
                .Where(n => n.GetAttributeValue("id", "") == "driverList")
                .Where(n => n.Descendants("a").Where(d => d.InnerText.Contains("GeForce")).Any())
                .FirstOrDefault();

            if (latestDriver != null)
            {
                DriverVersionNumber latestVersion = new DriverVersionNumber(latestDriver.Descendants("td").ElementAt(2).InnerText);
                DriverVersionNumber currentDriver = new DriverVersionNumber(DriverVersion);
                string driverDate = latestDriver.Descendants("td").ElementAt(3).InnerText;

                if (latestVersion.IsNewerThan(currentDriver))
                {
                    DialogResult result = MessageBox.Show($"Version: {latestVersion}. Published: {driverDate}.\n\nWould you like to download it now?", "New Driver Available", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        DownloadDriver($"https:{latestDriver.Descendants("a").First().GetAttributeValue("href", "")}");
                    }
                }
                else
                {
                    MessageBox.Show($"You have the latest driver.\n\nVersion: {latestVersion}. Published: {driverDate}.", "Driver Up to Date");
                }
            }
            else
            {
                MessageBox.Show("Driver search failed");
            }
        }

        private async void DownloadDriver(string downloadPageUrl)
        {
            HtmlAgilityPack.HtmlDocument downloadPage = await new HtmlWeb().LoadFromWebAsync(downloadPageUrl);
            HtmlNode downloadButton = downloadPage.DocumentNode.Descendants("a").Where(n => n.GetAttributeValue("id", "") == "lnkDwnldBtn").First();
            Regex urlParamRegex = new Regex(@"[?]url=(?<downloadPath>[^&]+)&", RegexOptions.Compiled);
            Match m = urlParamRegex.Match(downloadButton.GetAttributeValue("href", ""));

            if (m.Success)
            {
                string downloadUrl = $"https://us.download.nvidia.com{m.Groups["downloadPath"].Value}";
                string downloadsFolder = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads");
                string localFilePath = Path.Combine(downloadsFolder, downloadUrl.Split('/').Last());

                using (var clientHandler = new HttpClientHandler() { AllowAutoRedirect = true })
                using (var progressHandler = new ProgressMessageHandler(clientHandler))
                using (var cancellationSource = new CancellationTokenSource())
                using (var downloadForm = new NvidiaDriverDownloadForm())
                {
                    progressHandler.HttpReceiveProgress += (sender, ev) =>
                    {
                        try
                        {
                            downloadForm.BeginInvoke((MethodInvoker)delegate { downloadForm.pbDownload.Value = ev.ProgressPercentage; });
                        }
                        catch { }
                    };

                    downloadForm.Shown += async (o, e) =>
                    {
                        try
                        {
                            var cancellationToken = cancellationSource.Token;
                            using (HttpClient httpClient = new HttpClient(progressHandler, disposeHandler: false))
                            using (HttpResponseMessage result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, downloadUrl), HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                            using (Stream responseStream = await result.Content.ReadAsStreamAsync())
                            using (var fs = new FileStream(localFilePath, FileMode.CreateNew))
                            {
                                await responseStream.CopyToAsync(fs, bufferSize: 81920, cancellationToken);
                            }

                            downloadForm.BeginInvoke((MethodInvoker)delegate { downloadForm.Close(); });
                            Process.Start("explorer.exe", $"/select,\"{localFilePath}\"");
                        }
                        catch (TaskCanceledException)
                        {
                            try
                            {
                                File.Delete(localFilePath);
                            }
                            catch { }

                            downloadForm.Close();
                        }
                    };

                    downloadForm.btnCancel.Click += (o, e) =>
                    {
                        cancellationSource.Cancel();
                    };

                    downloadForm.StartPosition = FormStartPosition.CenterScreen;
                    downloadForm.ShowDialog();
                }
            }
        }

        private double ApplyScaling(double scaling)
        {
            uint newLimit = (uint)((_defaultPowerLimit / 1000) * scaling) * 1000;

            newLimit = Math.Min(Math.Max(newLimit, _minPowerLimit), _maxPowerLimit);

            return NvmlApi.NvmlDeviceSetPowerManagementLimit(_nvmlDeviceHandle, newLimit).IsSuccess()
                ? scaling
                : 0.0;
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new ApplicationException($"{nameof(NvidiaManager)} instance not initialized!");
            }
        }

        public void Dispose()
        {
            if (_nvapiDeviceHandle != IntPtr.Zero)
            {
                _nvapiDeviceHandle = IntPtr.Zero;
                NvApi.Unload();
            }

            _nvmlDeviceHandle = IntPtr.Zero;
            NvmlApi.NvmlShutdown();
        }
    }
}
