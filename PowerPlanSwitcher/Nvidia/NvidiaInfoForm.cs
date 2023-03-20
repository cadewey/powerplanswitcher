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
using System.Timers;

#endregion

namespace PowerPlanSwitcher.Nvidia
{
    internal class NvidiaInfoForm : Form
    {
        #region Designer generated code
        private TextBox txtDeviceName;

        #endregion

        private TextBox txtVbiosVersion;
        private TextBox txtDriverVersion;
        private TextBox txtPCIeInfo;
        private TextBox txtCudaCores;
        private TextBox txtMemoryUsage;
        private TextBox txtMemoryBusWidth;
        private readonly NvidiaManager _nvidiaManager;
        private Label label7;
        private TextBox txtGpuClocks;
        private TextBox txtMemoryClocks;
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
        }

        private void PollDynamicValues()
        {
            NvmlMemory memoryInfo = _nvidiaManager.GetMemoryInfo();
            (uint gpuCurrentClock, uint memCurrentClock) = _nvidiaManager.GetCurrentClockFrequencies();
            (uint gpuBaseClock, uint memBaseClock) = _nvidiaManager.GetBaseClockFrequencies();
            (uint gpuBoostClock, uint memBoostClock) = _nvidiaManager.GetBoostClockFrequencies();

            try
            {
                if (IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        txtMemoryUsage.Text = $"{memoryInfo.used / (1024 * 1024)} MB / {memoryInfo.total / (1024 * 1024)} MB";
                        txtGpuClocks.Text = $"{gpuCurrentClock / 1000} MHz / {gpuBaseClock / 1000} MHz / {gpuBoostClock / 1000} MHz";
                        txtMemoryClocks.Text = $"{memCurrentClock / 1000} MHz / {memBaseClock / 1000} MHz / {memBoostClock / 1000} MHz";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NvidiaInfoForm));
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.txtVbiosVersion = new System.Windows.Forms.TextBox();
            this.txtDriverVersion = new System.Windows.Forms.TextBox();
            this.txtPCIeInfo = new System.Windows.Forms.TextBox();
            this.txtCudaCores = new System.Windows.Forms.TextBox();
            this.txtMemoryUsage = new System.Windows.Forms.TextBox();
            this.txtMemoryBusWidth = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGpuClocks = new System.Windows.Forms.TextBox();
            this.txtMemoryClocks = new System.Windows.Forms.TextBox();
            lblName = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
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
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "GPU:";
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
            // NvidiaInfoForm
            // 
            this.ClientSize = new System.Drawing.Size(382, 435);
            this.Controls.Add(this.txtMemoryClocks);
            this.Controls.Add(label8);
            this.Controls.Add(this.txtGpuClocks);
            this.Controls.Add(this.label7);
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