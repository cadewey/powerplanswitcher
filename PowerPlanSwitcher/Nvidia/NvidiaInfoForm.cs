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
using System.Windows.Forms;

namespace PowerPlanSwitcher.Nvidia
{
    internal class NvidiaInfoForm : Form
    {
        #region Designer generated code

        private TextBox txtDeviceName;
        private TextBox txtVbiosVersion;
        private TextBox txtDriverVersion;
        private TextBox txtPCIeInfo;
        private TextBox txtCudaCores;
        private TextBox txtMemoryUsage;
        private TextBox txtMemoryBusWidth;
        private TextBox txtGpuClocks;
        private TextBox txtMemoryClocks;
        private TextBox txtCurrentPowerDraw;
        private TextBox txtCurrentPowerLimit;
        private TextBox txtDefaultPowerLimit;
        private CircularProgressBar.CircularProgressBar prgGpuUtilization;
        private CircularProgressBar.CircularProgressBar prgMemUtilization;
        private CircularProgressBar.CircularProgressBar progGpuTemp;

        #endregion

        private readonly NvidiaManager _nvidiaManager;
        private readonly System.Timers.Timer _pollingTimer = new System.Timers.Timer();

        internal NvidiaInfoForm(NvidiaManager manager)
        {
            _nvidiaManager = manager;
            InitializeComponent();

            CreateHandle();

            ReadStaticValues();
            PollDynamicValues();

            _pollingTimer.Interval = 1000; /* ms */
            _pollingTimer.Elapsed += (sender, args) => PollDynamicValues();
            _pollingTimer.Start();
        }

        private void ReadStaticValues()
        {
            txtDeviceName.Text = _nvidiaManager.DeviceName;
            txtVbiosVersion.Text = _nvidiaManager.VbiosVersion;
            txtDriverVersion.Text = _nvidiaManager.DriverVersion;
            txtPCIeInfo.Text = _nvidiaManager.PCIeLinkStatus;
            txtCudaCores.Text = _nvidiaManager.CoreCount.ToString();
            txtMemoryBusWidth.Text = $"{_nvidiaManager.MemoryBusWidth}-bit";
            txtDefaultPowerLimit.Text = $"{_nvidiaManager.GetDefaultPowerLimit() / 1000} W";
        }

        private void PollDynamicValues()
        {
            NvmlMemory memoryInfo = _nvidiaManager.GetMemoryInfo();
            (uint gpuCurrentClock, uint memCurrentClock) = _nvidiaManager.GetCurrentClockFrequencies();
            (uint gpuBaseClock, uint memBaseClock) = _nvidiaManager.GetBaseClockFrequencies();
            (uint gpuBoostClock, uint memBoostClock) = _nvidiaManager.GetBoostClockFrequencies();
            uint powerLimit = _nvidiaManager.GetActivePowerLimit();
            uint powerDraw = _nvidiaManager.GetCurrentPowerDraw();
            NvmlUtilization utilization = _nvidiaManager.GetUtilization();
            uint temp = _nvidiaManager.GetGpuTemperature();
            uint tempThreshold = _nvidiaManager.GetGpuTemperatureThreshold();

            try
            {
                if (IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        txtMemoryUsage.Text = $"{memoryInfo.Used / (1024 * 1024)} MB / {memoryInfo.Total / (1024 * 1024)} MB";
                        txtGpuClocks.Text = $"{gpuCurrentClock / 1000} MHz / {gpuBaseClock / 1000} MHz / {gpuBoostClock / 1000} MHz";
                        txtMemoryClocks.Text = $"{memCurrentClock / 1000} MHz / {memBaseClock / 1000} MHz / {memBoostClock / 1000} MHz";
                        txtCurrentPowerLimit.Text = $"{powerLimit / 1000} W";
                        txtCurrentPowerDraw.Text = $"{powerDraw / 1000} W";
                        prgGpuUtilization.Value = (int)utilization.Gpu;
                        prgGpuUtilization.Text = $"{utilization.Gpu}%";
                        prgMemUtilization.Value = (int)utilization.Memory;
                        prgMemUtilization.Text = $"{utilization.Memory}%";
                        progGpuTemp.Text = $"{temp}°C";
                        progGpuTemp.Maximum = (int)tempThreshold;
                        progGpuTemp.Value = (int)temp;
                    });
                }
            }
            catch (InvalidOperationException)
            {
                // This can occur if the timer triggers right as the form is being closed and then finishes the polling just after,
                // but we can safely swallow it
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _pollingTimer?.Stop();
                _pollingTimer?.Dispose();
            }

            base.Dispose(isDisposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.Label lblName;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label12;
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label14;
            System.Windows.Forms.Label label16;
            System.Windows.Forms.Label label15;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label11;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NvidiaInfoForm));
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.txtVbiosVersion = new System.Windows.Forms.TextBox();
            this.txtDriverVersion = new System.Windows.Forms.TextBox();
            this.txtPCIeInfo = new System.Windows.Forms.TextBox();
            this.txtCudaCores = new System.Windows.Forms.TextBox();
            this.txtMemoryUsage = new System.Windows.Forms.TextBox();
            this.txtMemoryBusWidth = new System.Windows.Forms.TextBox();
            this.txtGpuClocks = new System.Windows.Forms.TextBox();
            this.txtMemoryClocks = new System.Windows.Forms.TextBox();
            this.txtCurrentPowerDraw = new System.Windows.Forms.TextBox();
            this.txtCurrentPowerLimit = new System.Windows.Forms.TextBox();
            this.txtDefaultPowerLimit = new System.Windows.Forms.TextBox();
            this.prgGpuUtilization = new CircularProgressBar.CircularProgressBar();
            this.prgMemUtilization = new CircularProgressBar.CircularProgressBar();
            this.progGpuTemp = new CircularProgressBar.CircularProgressBar();
            lblName = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(11, 13);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(38, 13);
            lblName.TabIndex = 0;
            lblName.Text = "Name:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 44);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 13);
            label1.TabIndex = 1;
            label1.Text = "VBIOS:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(202, 44);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(38, 13);
            label2.TabIndex = 3;
            label2.Text = "Driver:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(16, 73);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(33, 13);
            label3.TabIndex = 5;
            label3.Text = "PCIe:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(203, 73);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(37, 13);
            label4.TabIndex = 7;
            label4.Text = "Cores:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(8, 102);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(41, 13);
            label5.TabIndex = 9;
            label5.Text = "VRAM:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(11, 137);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(173, 13);
            label6.TabIndex = 12;
            label6.Text = "Clock Values (Current/Base/Boost)";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(16, 196);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(33, 13);
            label8.TabIndex = 15;
            label8.Text = "Mem:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(9, 230);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(102, 13);
            label9.TabIndex = 17;
            label9.Text = "Power Management";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(16, 254);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(35, 13);
            label10.TabIndex = 18;
            label10.Text = "Draw:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(240, 254);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(44, 13);
            label12.TabIndex = 22;
            label12.Text = "Default:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(11, 290);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(52, 13);
            label13.TabIndex = 24;
            label13.Text = "Utilization";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(46, 403);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(30, 13);
            label14.TabIndex = 29;
            label14.Text = "GPU";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(297, 403);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(34, 13);
            label16.TabIndex = 33;
            label16.Text = "Temp";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(168, 404);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(44, 13);
            label15.TabIndex = 31;
            label15.Text = "Memory";
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.BackColor = System.Drawing.Color.White;
            this.txtDeviceName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtDeviceName.Location = new System.Drawing.Point(57, 10);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.ReadOnly = true;
            this.txtDeviceName.Size = new System.Drawing.Size(298, 20);
            this.txtDeviceName.TabIndex = 0;
            this.txtDeviceName.TabStop = false;
            this.txtDeviceName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtVbiosVersion
            // 
            this.txtVbiosVersion.BackColor = System.Drawing.Color.White;
            this.txtVbiosVersion.Location = new System.Drawing.Point(57, 41);
            this.txtVbiosVersion.Name = "txtVbiosVersion";
            this.txtVbiosVersion.ReadOnly = true;
            this.txtVbiosVersion.Size = new System.Drawing.Size(139, 20);
            this.txtVbiosVersion.TabIndex = 2;
            this.txtVbiosVersion.TabStop = false;
            this.txtVbiosVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtDriverVersion
            // 
            this.txtDriverVersion.BackColor = System.Drawing.Color.White;
            this.txtDriverVersion.Location = new System.Drawing.Point(246, 41);
            this.txtDriverVersion.Name = "txtDriverVersion";
            this.txtDriverVersion.ReadOnly = true;
            this.txtDriverVersion.Size = new System.Drawing.Size(109, 20);
            this.txtDriverVersion.TabIndex = 4;
            this.txtDriverVersion.TabStop = false;
            this.txtDriverVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPCIeInfo
            // 
            this.txtPCIeInfo.BackColor = System.Drawing.Color.White;
            this.txtPCIeInfo.Location = new System.Drawing.Point(57, 70);
            this.txtPCIeInfo.Name = "txtPCIeInfo";
            this.txtPCIeInfo.ReadOnly = true;
            this.txtPCIeInfo.Size = new System.Drawing.Size(139, 20);
            this.txtPCIeInfo.TabIndex = 6;
            this.txtPCIeInfo.TabStop = false;
            this.txtPCIeInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtCudaCores
            // 
            this.txtCudaCores.BackColor = System.Drawing.Color.White;
            this.txtCudaCores.Location = new System.Drawing.Point(246, 70);
            this.txtCudaCores.Name = "txtCudaCores";
            this.txtCudaCores.ReadOnly = true;
            this.txtCudaCores.Size = new System.Drawing.Size(109, 20);
            this.txtCudaCores.TabIndex = 8;
            this.txtCudaCores.TabStop = false;
            this.txtCudaCores.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMemoryUsage
            // 
            this.txtMemoryUsage.BackColor = System.Drawing.Color.White;
            this.txtMemoryUsage.Location = new System.Drawing.Point(57, 99);
            this.txtMemoryUsage.Name = "txtMemoryUsage";
            this.txtMemoryUsage.ReadOnly = true;
            this.txtMemoryUsage.Size = new System.Drawing.Size(183, 20);
            this.txtMemoryUsage.TabIndex = 10;
            this.txtMemoryUsage.TabStop = false;
            this.txtMemoryUsage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMemoryBusWidth
            // 
            this.txtMemoryBusWidth.BackColor = System.Drawing.Color.White;
            this.txtMemoryBusWidth.Location = new System.Drawing.Point(246, 99);
            this.txtMemoryBusWidth.Name = "txtMemoryBusWidth";
            this.txtMemoryBusWidth.ReadOnly = true;
            this.txtMemoryBusWidth.Size = new System.Drawing.Size(109, 20);
            this.txtMemoryBusWidth.TabIndex = 11;
            this.txtMemoryBusWidth.TabStop = false;
            this.txtMemoryBusWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(16, 166);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(33, 13);
            label7.TabIndex = 13;
            label7.Text = "GPU:";
            // 
            // txtGpuClocks
            // 
            this.txtGpuClocks.BackColor = System.Drawing.Color.White;
            this.txtGpuClocks.Location = new System.Drawing.Point(57, 163);
            this.txtGpuClocks.Name = "txtGpuClocks";
            this.txtGpuClocks.ReadOnly = true;
            this.txtGpuClocks.Size = new System.Drawing.Size(298, 20);
            this.txtGpuClocks.TabIndex = 14;
            this.txtGpuClocks.TabStop = false;
            this.txtGpuClocks.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMemoryClocks
            // 
            this.txtMemoryClocks.BackColor = System.Drawing.Color.White;
            this.txtMemoryClocks.Location = new System.Drawing.Point(57, 193);
            this.txtMemoryClocks.Name = "txtMemoryClocks";
            this.txtMemoryClocks.ReadOnly = true;
            this.txtMemoryClocks.Size = new System.Drawing.Size(298, 20);
            this.txtMemoryClocks.TabIndex = 16;
            this.txtMemoryClocks.TabStop = false;
            this.txtMemoryClocks.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtCurrentPowerDraw
            // 
            this.txtCurrentPowerDraw.BackColor = System.Drawing.Color.White;
            this.txtCurrentPowerDraw.Location = new System.Drawing.Point(57, 251);
            this.txtCurrentPowerDraw.Name = "txtCurrentPowerDraw";
            this.txtCurrentPowerDraw.ReadOnly = true;
            this.txtCurrentPowerDraw.Size = new System.Drawing.Size(65, 20);
            this.txtCurrentPowerDraw.TabIndex = 19;
            this.txtCurrentPowerDraw.TabStop = false;
            this.txtCurrentPowerDraw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(128, 254);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(31, 13);
            label11.TabIndex = 20;
            label11.Text = "Limit:";
            // 
            // txtCurrentPowerLimit
            // 
            this.txtCurrentPowerLimit.BackColor = System.Drawing.Color.White;
            this.txtCurrentPowerLimit.Location = new System.Drawing.Point(165, 251);
            this.txtCurrentPowerLimit.Name = "txtCurrentPowerLimit";
            this.txtCurrentPowerLimit.ReadOnly = true;
            this.txtCurrentPowerLimit.Size = new System.Drawing.Size(65, 20);
            this.txtCurrentPowerLimit.TabIndex = 21;
            this.txtCurrentPowerLimit.TabStop = false;
            this.txtCurrentPowerLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtDefaultPowerLimit
            // 
            this.txtDefaultPowerLimit.BackColor = System.Drawing.Color.White;
            this.txtDefaultPowerLimit.Location = new System.Drawing.Point(290, 251);
            this.txtDefaultPowerLimit.Name = "txtDefaultPowerLimit";
            this.txtDefaultPowerLimit.ReadOnly = true;
            this.txtDefaultPowerLimit.Size = new System.Drawing.Size(65, 20);
            this.txtDefaultPowerLimit.TabIndex = 23;
            this.txtDefaultPowerLimit.TabStop = false;
            this.txtDefaultPowerLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // prgGpuUtilization
            // 
            this.prgGpuUtilization.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.prgGpuUtilization.AnimationSpeed = 0;
            this.prgGpuUtilization.BackColor = System.Drawing.Color.Transparent;
            this.prgGpuUtilization.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prgGpuUtilization.ForeColor = System.Drawing.Color.Black;
            this.prgGpuUtilization.InnerColor = System.Drawing.Color.Transparent;
            this.prgGpuUtilization.InnerMargin = 0;
            this.prgGpuUtilization.InnerWidth = -1;
            this.prgGpuUtilization.Location = new System.Drawing.Point(19, 315);
            this.prgGpuUtilization.MarqueeAnimationSpeed = 0;
            this.prgGpuUtilization.Name = "prgGpuUtilization";
            this.prgGpuUtilization.OuterColor = System.Drawing.Color.Silver;
            this.prgGpuUtilization.OuterMargin = -25;
            this.prgGpuUtilization.OuterWidth = 26;
            this.prgGpuUtilization.ProgressColor = System.Drawing.Color.ForestGreen;
            this.prgGpuUtilization.ProgressWidth = 14;
            this.prgGpuUtilization.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.prgGpuUtilization.Size = new System.Drawing.Size(85, 85);
            this.prgGpuUtilization.StartAngle = 270;
            this.prgGpuUtilization.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgGpuUtilization.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.prgGpuUtilization.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.prgGpuUtilization.SubscriptText = "";
            this.prgGpuUtilization.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.prgGpuUtilization.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.prgGpuUtilization.SuperscriptText = "";
            this.prgGpuUtilization.TabIndex = 28;
            this.prgGpuUtilization.Text = "98%";
            this.prgGpuUtilization.TextMargin = new System.Windows.Forms.Padding(2, 1, 0, 0);
            this.prgGpuUtilization.Value = 68;
            // 
            // prgMemUtilization
            // 
            this.prgMemUtilization.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.prgMemUtilization.AnimationSpeed = 0;
            this.prgMemUtilization.BackColor = System.Drawing.Color.Transparent;
            this.prgMemUtilization.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prgMemUtilization.ForeColor = System.Drawing.Color.Black;
            this.prgMemUtilization.InnerColor = System.Drawing.Color.Transparent;
            this.prgMemUtilization.InnerMargin = 0;
            this.prgMemUtilization.InnerWidth = -1;
            this.prgMemUtilization.Location = new System.Drawing.Point(145, 315);
            this.prgMemUtilization.MarqueeAnimationSpeed = 0;
            this.prgMemUtilization.Name = "prgMemUtilization";
            this.prgMemUtilization.OuterColor = System.Drawing.Color.Silver;
            this.prgMemUtilization.OuterMargin = -25;
            this.prgMemUtilization.OuterWidth = 26;
            this.prgMemUtilization.ProgressColor = System.Drawing.Color.DodgerBlue;
            this.prgMemUtilization.ProgressWidth = 14;
            this.prgMemUtilization.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.prgMemUtilization.Size = new System.Drawing.Size(85, 85);
            this.prgMemUtilization.StartAngle = 270;
            this.prgMemUtilization.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgMemUtilization.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.prgMemUtilization.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.prgMemUtilization.SubscriptText = "";
            this.prgMemUtilization.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.prgMemUtilization.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.prgMemUtilization.SuperscriptText = "";
            this.prgMemUtilization.TabIndex = 30;
            this.prgMemUtilization.Text = "98%";
            this.prgMemUtilization.TextMargin = new System.Windows.Forms.Padding(2, 1, 0, 0);
            this.prgMemUtilization.Value = 68;
            // 
            // progGpuTemp
            // 
            this.progGpuTemp.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.progGpuTemp.AnimationSpeed = 0;
            this.progGpuTemp.BackColor = System.Drawing.Color.Transparent;
            this.progGpuTemp.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progGpuTemp.ForeColor = System.Drawing.Color.Black;
            this.progGpuTemp.InnerColor = System.Drawing.Color.Transparent;
            this.progGpuTemp.InnerMargin = 0;
            this.progGpuTemp.InnerWidth = -1;
            this.progGpuTemp.Location = new System.Drawing.Point(270, 315);
            this.progGpuTemp.MarqueeAnimationSpeed = 0;
            this.progGpuTemp.Name = "progGpuTemp";
            this.progGpuTemp.OuterColor = System.Drawing.Color.Silver;
            this.progGpuTemp.OuterMargin = -25;
            this.progGpuTemp.OuterWidth = 26;
            this.progGpuTemp.ProgressColor = System.Drawing.Color.Firebrick;
            this.progGpuTemp.ProgressWidth = 14;
            this.progGpuTemp.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.progGpuTemp.Size = new System.Drawing.Size(85, 85);
            this.progGpuTemp.StartAngle = 270;
            this.progGpuTemp.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progGpuTemp.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.progGpuTemp.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.progGpuTemp.SubscriptText = "";
            this.progGpuTemp.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.progGpuTemp.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.progGpuTemp.SuperscriptText = "";
            this.progGpuTemp.TabIndex = 32;
            this.progGpuTemp.Text = "55°C";
            this.progGpuTemp.TextMargin = new System.Windows.Forms.Padding(2, 1, 0, 0);
            this.progGpuTemp.Value = 68;
            // 
            // NvidiaInfoForm
            // 
            this.ClientSize = new System.Drawing.Size(382, 435);
            this.Controls.Add(label16);
            this.Controls.Add(this.progGpuTemp);
            this.Controls.Add(label15);
            this.Controls.Add(this.prgMemUtilization);
            this.Controls.Add(label14);
            this.Controls.Add(this.prgGpuUtilization);
            this.Controls.Add(label13);
            this.Controls.Add(this.txtDefaultPowerLimit);
            this.Controls.Add(label12);
            this.Controls.Add(this.txtCurrentPowerLimit);
            this.Controls.Add(label11);
            this.Controls.Add(this.txtCurrentPowerDraw);
            this.Controls.Add(label10);
            this.Controls.Add(label9);
            this.Controls.Add(this.txtMemoryClocks);
            this.Controls.Add(label8);
            this.Controls.Add(this.txtGpuClocks);
            this.Controls.Add(label7);
            this.Controls.Add(label6);
            this.Controls.Add(this.txtMemoryBusWidth);
            this.Controls.Add(this.txtMemoryUsage);
            this.Controls.Add(label5);
            this.Controls.Add(this.txtCudaCores);
            this.Controls.Add(label4);
            this.Controls.Add(this.txtPCIeInfo);
            this.Controls.Add(label3);
            this.Controls.Add(this.txtDriverVersion);
            this.Controls.Add(label2);
            this.Controls.Add(this.txtVbiosVersion);
            this.Controls.Add(label1);
            this.Controls.Add(this.txtDeviceName);
            this.Controls.Add(lblName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NvidiaInfoForm";
            this.Text = "Nvidia Device Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}