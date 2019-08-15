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

using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using UniversalUnityHooks;

namespace BP_NoDeathmatch
{
    public static class Hooks
    {
        #region Private Members

        private static readonly string ConfigFilePath = Path.Combine("Plugins", "NoDeathmatch-Settings.json");
        private static Variables Vars = new Variables();

        #endregion

        #region Hooks

        [Hook("SvManager.StartServer")]
        public static void StartServer(SvManager svManager)
        {
            Debug.Log("Initializing NoDeathmatch...");
            if (File.Exists(ConfigFilePath))
            {
                try { Vars = JsonConvert.DeserializeObject<Variables>(File.ReadAllText(ConfigFilePath)); }
                catch (Exception) { Debug.Log($"[ERROR] Can't deserialize {ConfigFilePath}. Using default values."); }
            }
            else
            {
                Debug.Log("Config file for NoDeathmatch not found. Creating one...");
                File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(Vars, Formatting.Indented));
            }
        }

        [Hook("SvPlayer.Damage")]
        public static bool Damage(SvPlayer target, ref DamageIndex type, ref float amount, ref ShPlayer attacker, ref Collider collider)
        {
            if (attacker == null || attacker == target || attacker.IsClientBot() || (Vars.IgnoreDamageToBots && target.player.IsClientBot()))
                return false;

            Faction attackerFaction, playerFaction;
            {
                var attackerJob = attacker.job.jobIndex;
                attackerFaction = Vars.Factions.Find(x => x.Jobs.Contains(attackerJob));
                playerFaction = Vars.Factions.Find(x => x.Jobs.Contains(target.player.job.jobIndex));
            }

            if (target.player.wantedLevel >= Vars.MinTargetWantedLevelToIgnoreDamageRules
                || attackerFaction.CanDamage.Contains(playerFaction.Name)
                || attackerFaction.CanDamage.Contains("*"))
            {
                if (target.WillDieByDamage(type, amount, collider))
                {
                    if (target.player.wantedLevel >= Vars.MinTargetWantedLevelToIgnoreKillRules
                        || attackerFaction.CanKill.Contains(playerFaction.Name)
                        || attackerFaction.CanKill.Contains("*"))
                        return false;

                    attacker.svPlayer.ShowMessageInChat(Vars.CantKillMessage);
                    return true;
                }
                return false;
            }
            attacker.svPlayer.ShowMessageInChat(Vars.CantDamageMessage);
            return true;
        }

        #endregion

        #region Private Extensions

        private static void ShowMessageInChat(this SvPlayer player, string message) => player.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, message);

        private static bool WillDieByDamage(this SvPlayer sv, DamageIndex type, float amount, Collider collider)
        {
            if (sv.player.IsDead() || sv.player.IsShielded(type, collider))
                return false;

            if (sv.player.IsBlocking(type))
                amount *= 0.3f;
            else if (collider == sv.player.headCollider)
                amount *= 2f;

            if (sv.serverside)
                amount /= sv.svManager.settings.difficulty;

            amount -= amount * (sv.player.armorLevel / sv.player.maxStat * 0.5f);

            return sv.player.health - amount <= 0;
        }

        #endregion
    }
}
