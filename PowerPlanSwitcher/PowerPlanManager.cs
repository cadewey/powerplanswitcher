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
using System.Diagnostics;
using System.IO;
using System.Text;

#endregion

namespace TheRefactory
{
    public class PowerPlanManager
    {
        private readonly ProcessStartInfo _startInfo = new ProcessStartInfo("cmd")
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        internal PowerPlanManager()
        {
            PowerPlans = new List<PowerPlan>();
            LoadPowerPlans();
        }

        internal List<PowerPlan> PowerPlans { get; set; }
        internal PowerPlan ActivePlan { get; set; }
        // index that points to currently active plan in guids and names arrays

        internal int GetIndexOfActivePlan()
        {
            return PowerPlans.IndexOf(ActivePlan);
        }

        /// <summary>
        ///     Retrieves a list of available power plans and determines the currently active one.
        /// </summary>
        internal void LoadPowerPlans()
        {
            string output;
            // use the powercfg cmd tool to determine existing powerplans and the currently active one.
            using (var process = new Process { StartInfo = _startInfo })
            {
                process.Start();
                process.StandardInput.WriteLine("powercfg /L");
                process.StandardInput.WriteLine("exit");

                using (var utf8Reader = new StreamReader(process.StandardOutput.BaseStream, Encoding.GetEncoding(437)))
                {
                    Console.WriteLine(utf8Reader.CurrentEncoding);
                    output = utf8Reader.ReadToEnd();
                    Console.WriteLine(output);
                    utf8Reader.Close();
                }
            }

            // parse the output from powercfg
            PowerPlans.Clear();

            foreach (var line in output.Split('\n'))
            {
                if ((line.Trim().Length == 0) || !line.Contains("GUID"))
                    continue;

                var guid = line.Substring(line.IndexOf(':') + 2).Remove(36);
                var name = line.Substring(line.IndexOf('(')).Replace('*', ' ').Trim();
                var powerPlan = new PowerPlan(guid, name.Replace('(', ' ').Replace(')', ' ').Trim());
                PowerPlans.Add(powerPlan);

                if (line.Contains("*"))
                    ActivePlan = powerPlan;
            }
        }

		/// <summary>
		///     Changes the system's power plan to the one specified by powerPlans[index].
		/// </summary>
		/// <param name="index"></param>
		internal void SetPowerPlan(int index)
        {
            ActivePlan = PowerPlans[index];

            using (var process = new Process { StartInfo = _startInfo })
            {
                process.Start();
                process.StandardInput.WriteLine("powercfg /SETACTIVE " + ActivePlan.Guid);
                process.StandardInput.WriteLine("exit");
                process.StandardOutput.ReadToEnd();
            }
        }
    }
}