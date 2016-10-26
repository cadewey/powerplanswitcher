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

namespace TheRefactory
{
    internal class PowerPlan
    {
        public PowerPlan(string guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public string Guid { get; private set; }
        public string Name { get; private set; }
    }
}