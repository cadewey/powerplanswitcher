using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TheRefactory
{
    public class PowerPlanManager
    {
        internal List<PowerPlan> powerPlans { get; set; }
        private PowerPlan activePlan { get; set; } // index that points to currently active plan in guids and names arrays
        private ProcessStartInfo startInfo = new ProcessStartInfo("cmd")
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        internal PowerPlanManager()
        {
            powerPlans = new List<PowerPlan>();
            LoadPowerPlans();
        }

        internal int GetIndexOfActivePlan()
        {
            return powerPlans.IndexOf(activePlan);
        }

        /// <summary>
        /// Retrieves a list of available power plans and determines the currently active one.
        /// </summary>
        internal void LoadPowerPlans()
        {
            // use the powercfg cmd tool to determine existing powerplans and the currently active one.
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.StandardInput.WriteLine("powercfg /L");
            process.StandardInput.WriteLine("exit");
            StreamReader utf8Reader = new StreamReader(process.StandardOutput.BaseStream, Encoding.GetEncoding(437));
            Console.WriteLine(utf8Reader.CurrentEncoding);
            string output = utf8Reader.ReadToEnd();
            Console.WriteLine(output);
            utf8Reader.Close();
            process.Dispose();
 
            // parse the output from powercfg
            powerPlans.Clear();
            foreach (string line in output.Split(new Char[] { '\n' }))
            {
                if (line.Trim().Length == 0 || !line.Contains("GUID"))
                {
                    continue;
                }

                string guid = line.Substring(line.IndexOf(':') + 2).Remove(36);
                string name = line.Substring(line.IndexOf('(')).Replace('*', ' ').Trim();
                PowerPlan powerPlan = new PowerPlan(guid, name.Replace('(', ' ').Replace(')', ' ').Trim());
                powerPlans.Add(powerPlan);

                if (line.Contains("*"))
                {
                    activePlan = powerPlan;
                }
            }
        }

        /// <summary>
        /// Changes the system's power plan to the one specified by powerPlans[index].
        /// </summary>
        /// <param name="index"></param>
        internal void SetPowerPlan(int index)
        {
            activePlan = powerPlans[index];
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.StandardInput.WriteLine("powercfg /SETACTIVE " + activePlan.guid);
            process.StandardInput.WriteLine("exit");
            string output = process.StandardOutput.ReadToEnd();
            process.Dispose();
        }
    }

 
}
