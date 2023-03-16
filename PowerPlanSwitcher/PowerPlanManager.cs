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
using System.Runtime.InteropServices;

#endregion

namespace PowerPlanSwitcher
{
    public class PowerPlanManager
    {
        enum PowerDataAccessor : uint
        {
            ACCESS_SCHEME = 16,
            ACCESS_SUBGROUP = 17,
            ACCESS_INDIVIDUAL_SETTING = 18,
            ACCESS_ACTIVE_SCHEME = 19,
            ACCESS_CREATE_SCHEME = 20
        }

        [DllImport("powrprof.dll")]
        static extern uint PowerEnumerate(
            IntPtr rootPowerKey,
            IntPtr schemeGuid,
            IntPtr subGroupOfPowerSettingsGuid,
            PowerDataAccessor accessFlags,
            uint index,
            IntPtr buffer,
            ref uint bufferSize);

        [DllImport("powrprof.dll")]
        static extern uint PowerReadFriendlyName(
            IntPtr rootPowerKey,
            IntPtr schemeGuid,
            IntPtr subGroupOfPowerSettingGuid,
            IntPtr powerSettingGuid,
            IntPtr buffer,
            ref uint bufferSize);

        [DllImport("powrprof.dll")]
        static extern uint PowerSetActiveScheme(IntPtr userRootPowerKey, ref Guid schemeGuid);

        [DllImport("powrprof.dll")]
        public static extern uint PowerGetActiveScheme(IntPtr userRootPowerKey, ref IntPtr activePolicyGuid);

        internal PowerPlanManager()
        {
            LoadPowerPlans();
            LoadActivePlan();
        }

        internal List<PowerPlan> PowerPlans { get; set; }
        internal PowerPlan ActivePlan { get; set; }
        // index that points to currently active plan in guids and names arrays

        internal int GetIndexOfActivePlan()
        {
            return PowerPlans.FindIndex(p => p.Guid == ActivePlan.Guid);
        }

        internal void LoadPowerPlans()
        {
            PowerPlans = new List<PowerPlan>();

            IntPtr ptrPlanGuid;
            uint bufferSize = 16; // Size of System.Guid

            uint index = 0;
            uint res = 0;

            while (res == 0)
            {
                ptrPlanGuid = Marshal.AllocHGlobal((int)bufferSize);

                try
                {
                    res = PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, PowerDataAccessor.ACCESS_SCHEME, index, ptrPlanGuid, ref bufferSize);

                    if (res == 259)
                        break; // no more data

                    if (res != 0)
                    {
                        throw new COMException("Error occurred while enumerating power schemes. Win32 error code: " + res);
                    }

                    Guid guid = Marshal.PtrToStructure<Guid>(ptrPlanGuid);
                    string name = GetPlanName(ptrPlanGuid);

                    PowerPlans.Add(new PowerPlan(guid, name));
                }
                finally
                {
                    Marshal.FreeHGlobal(ptrPlanGuid);
                }

                index++;
            }
        }

        private void LoadActivePlan()
        {
            IntPtr ptrActiveGuid = IntPtr.Zero;
            uint res = PowerGetActiveScheme(IntPtr.Zero, ref ptrActiveGuid);
            if (res == 0)
            {
                Guid guid = Marshal.PtrToStructure<Guid>(ptrActiveGuid);
                string planName = GetPlanName(ptrActiveGuid);

                Marshal.FreeHGlobal(ptrActiveGuid);

                ActivePlan = new PowerPlan(guid, planName);
            }
            else
            {
                throw new Exception("Error reading current power scheme. Native Win32 error code = " + res);
            }
        }

        private string GetPlanName(IntPtr ptrPlanGuid)
        {
            uint buffSize = 0;
            uint res = PowerReadFriendlyName(IntPtr.Zero, ptrPlanGuid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref buffSize);
            if (res == 0)
            {
                IntPtr ptrName = Marshal.AllocHGlobal((int)buffSize);
                res = PowerReadFriendlyName(IntPtr.Zero, ptrPlanGuid, IntPtr.Zero, IntPtr.Zero, ptrName, ref buffSize);
                if (res == 0)
                {
                    string name = Marshal.PtrToStringUni(ptrName);
                    Marshal.FreeHGlobal(ptrName);

                    return name;
                }
                Marshal.FreeHGlobal(ptrName);
            }

            throw new Exception("Error reading power scheme name. Native Win32 error code = " + res);
        }

        /// <summary>
        ///     Changes the system's power plan to the one specified by powerPlans[index].
        /// </summary>
        /// <param name="index"></param>
        internal void SetPowerPlan(int index)
        {
            ActivePlan = PowerPlans[index];
            Guid planGuid = ActivePlan.Guid;

            uint res = PowerSetActiveScheme(IntPtr.Zero, ref planGuid);
            
            if (res != 0) 
                throw new COMException($"Error occurred. Win32 error code: {res}");
        }
    }
}