// Copyright (C) 2023  PowerPlanSwitcher
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
    internal class NvidiaDriverDownloadForm : Form
    {
        public ProgressBar pbDownload;
        public Button btnCancel;

        internal NvidiaDriverDownloadForm()
        {
            InitializeComponent();

            CreateHandle();
        }

       
        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NvidiaDriverDownloadForm));
            this.pbDownload = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(107, 13);
            label1.TabIndex = 0;
            label1.Text = "Downloading driver...";
            // 
            // pbDownload
            // 
            this.pbDownload.Location = new System.Drawing.Point(16, 40);
            this.pbDownload.Name = "pbDownload";
            this.pbDownload.Size = new System.Drawing.Size(256, 23);
            this.pbDownload.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(102, 69);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // NvidiaDriverDownloadForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 103);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbDownload);
            this.Controls.Add(label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NvidiaDriverDownloadForm";
            this.Text = "Downloading";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}