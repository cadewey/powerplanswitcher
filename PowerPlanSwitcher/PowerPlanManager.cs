using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
        private PowerPlan ActivePlan { get; set; }
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
            // use the powercfg cmd tool to determine existing powerplans and the currently active one.
            var process = new Process {StartInfo = _startInfo};
            process.Start();
            process.StandardInput.WriteLine("powercfg /L");
            process.StandardInput.WriteLine("exit");
            var utf8Reader = new StreamReader(process.StandardOutput.BaseStream, Encoding.GetEncoding(437));
            Console.WriteLine(utf8Reader.CurrentEncoding);
            var output = utf8Reader.ReadToEnd();
            Console.WriteLine(output);
            utf8Reader.Close();
            process.Dispose();

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
            var process = new Process {StartInfo = _startInfo};
            process.Start();
            process.StandardInput.WriteLine("powercfg /SETACTIVE " + ActivePlan.Guid);
            process.StandardInput.WriteLine("exit");
            process.StandardOutput.ReadToEnd();
            process.Dispose();
        }
    }
}