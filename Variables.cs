/*
    This file is part of BP-NoDeathmatch.
    
    BP-NoDeathmatch is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    BP-NoDeathmatch is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    
    You should have received a copy of the GNU General Public License
    along with BP-NoDeathmatch.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace BP_NoDeathmatch
{
    public class Variables
    {
        public Variables()
        {
            Factions.Add(new Faction
            {
                Name = "Civilians",
                Jobs = { 0, 4, 5, 10, 11 },
                CanDamage = { "Gangsters" },
                CanKill = { "Gangsters" }
            });
            Factions.Add(new Faction
            {
                Name = "Government",
                Jobs = { 3, 9, 12 },
                CanDamage = { "*" },
                CanKill = { "Gangsters" }
            });
            Factions.Add(new Faction
            {
                Name = "Gangsters",
                Jobs = { 1, 2, 6, 7, 8 },
                CanDamage = { "*" },
                CanKill = { "*" }
            });
        }

        public int MinTargetWantedLevelToIgnoreDamageRules { get; set; } = 1;
        public int MinTargetWantedLevelToIgnoreKillRules { get; set; } = 2;

        public bool IgnoreDamageToBots { get; set; } = false;

        public string CantDamageMessage { get; set; } = "You can't damage people for no reason.";
        public string CantKillMessage { get; set; } = "You can't kill people for no reason.";

        public List<Faction> Factions { get; set; } = new List<Faction>();
    }
}
