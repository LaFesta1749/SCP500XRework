using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using RemoteAdmin;
using UnityEngine;

namespace SCP500XRework
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GivePillCommand : ICommand
    {
        public string Command { get; } = "gp";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Gives an SCP-500 pill to a player.";

        private static readonly Dictionary<string, uint> PillIds = new()
        {
            { "A", 5000 }, { "B", 5001 }, { "C", 5002 }, { "D", 5003 },
            { "E", 5011 }, { "F", 5012 }, { "H", 5004 }, { "I", 5005 },
            { "L", 5013 }, { "M", 5006 }, { "O", 5014 }, { "P", 5019 },
            { "S", 5007 }, { "T", 5008 }, { "U", 5015 }, { "V", 5009 },
            { "W", 5016 }, { "X", 5010 }, { "Y", 5017 }, { "Z", 5018 }
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = "❌ This command can only be used by a player!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "❌ Usage: .gp <pill_letter> (e.g., .gp B)";
                return false;
            }

            string pillKey = arguments.At(0).ToUpper();
            if (!PillIds.TryGetValue(pillKey, out uint pillId))
            {
                response = "❌ Invalid pill name!";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            if (player == null)
            {
                response = "❌ Could not find player!";
                return false;
            }

            if (!CustomItem.TryGet(pillId, out CustomItem? pillItem) || pillItem == null)
            {
                response = $"❌ Pill SCP-500-{pillKey} is not registered!";
                return false;
            }

            // Дава на играча персонализираното хапче
            pillItem.Give(player);

            response = $"✅ [GP] {player.Nickname} received a SCP-500-{pillKey} pill!";
            return true;

        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PillsListCommand : ICommand
    {
        public string Command { get; } = "pills";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Lists all SCP-500 pills and their effects.";

        private static readonly Dictionary<string, string> PillEffects = new()
        {
            { "A", "Summons a teammate." },
            { "B", "Switches your team." },
            { "C", "Grants an O5 keycard." },
            { "D", "Disguises you as the enemy." },
            { "E", "Random teleport." },
            { "F", "Grants extra movement speed." },
            { "H", "Increases max HP." },
            { "I", "Grants invisibility." },
            { "L", "Spawns an allied SCP-939." },
            { "M", "Distorts your model." },
            { "O", "Summons a mini-warhead explosion." },
            { "P", "Randomly shuffles player's inventory." },
            { "S", "Random speed boost or slow." },
            { "T", "Teleports you to a teammate." },
            { "U", "Summons a random SCP entity." },
            { "V", "Drains HP from nearby enemies." },
            { "W", "Cleans all negative effects." },
            { "X", "Explodes all doors in a small radius." },
            { "Y", "Applies a random debuff." },
            { "Z", "Cures all SCP-008 infections." }
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string pillsList = "📜 **SCP-500 Pills List:**\n";
            foreach (var pill in PillEffects)
            {
                pillsList += $"🔹 SCP-500-{pill.Key}: {pill.Value}\n";
            }

            response = pillsList;
            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnAllPillsCommand : ICommand
    {
        public string Command { get; } = "sap";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Forces all SCP-500 pills to spawn.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (PillsPlugin.Instance == null || PillsPlugin.Instance.eventHandlers == null)
            {
                response = "❌ Plugin is not initialized!";
                return false;
            }

            PillsPlugin.Instance.eventHandlers.OnRoundStart();
            response = "✅ All SCP-500 pills have been spawned!";
            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TeleportToNearestPillCommand : ICommand
    {
        public string Command { get; } = "tpnp";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Teleports the player to the nearest SCP-500 pill.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = "❌ This command can only be used by a player!";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            if (player == null)
            {
                response = "❌ Could not find player!";
                return false;
            }

            if (PillsPlugin.Instance == null || PillsPlugin.Instance.eventHandlers == null)
            {
                response = "❌ Plugin is not initialized!";
                return false;
            }

            if (PillsPlugin.Instance.eventHandlers.spawnedPills.Count == 0)
            {
                response = "❌ No SCP-500 pills found!";
                return false;
            }

            Vector3 nearestPill = Vector3.zero;
            float minDistance = float.MaxValue;

            foreach (Vector3 pillPosition in PillsPlugin.Instance.eventHandlers.spawnedPills)
            {
                float distance = Vector3.Distance(player.Position, pillPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPill = pillPosition;
                }
            }

            if (nearestPill == Vector3.zero)
            {
                response = "❌ Could not find the nearest SCP-500 pill!";
                return false;
            }

            player.Position = nearestPill;
            response = $"✅ Teleported {player.Nickname} to the nearest SCP-500 pill at {nearestPill}!";
            return true;
        }
    }
}
