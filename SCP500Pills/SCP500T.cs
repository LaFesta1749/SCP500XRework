#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;
using MapGeneration;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500T : CustomItem
    {
        public override uint Id { get; set; } = 5008;
        public override string Name { get; set; } = "SCP-500-T";
        public override string Description { get; set; } = "Teleports you to a random location within the facility!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float TeleportDelay = 3f; // ⏳ Подготовка преди телепортация

        private static readonly List<ZoneType> AllowedZones = new()
        {
            ZoneType.LightContainment,
            ZoneType.HeavyContainment,
            ZoneType.Entrance,
            ZoneType.Surface
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem -= OnItemUsed;
        }

        private void OnItemUsed(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-T!</color> You start feeling dizzy...");
            ev.Player.EnableEffect(EffectType.Concussed, TeleportDelay); // 🔄 Леко замайване преди телепортация

            Timing.CallDelayed(TeleportDelay, () => TeleportPlayer(ev.Player));
            ev.Player.RemoveItem(ev.Item);
        }

        private void TeleportPlayer(Player player)
        {
            if (!player.IsAlive) return; // 🚫 Ако играчът е умрял през това време, не правим нищо

            var validRooms = Room.List.Where(room => AllowedZones.Contains(room.Zone)).ToList();
            if (validRooms.Count == 0)
            {
                Log.Warn("No valid teleport locations found!");
                return;
            }

            Room targetRoom = validRooms[UnityEngine.Random.Range(0, validRooms.Count)];

            // ✅ Телепортиране в центъра на стаята
            player.Position = targetRoom.Position + Vector3.up;

            Log.Info($"{player.Nickname} used SCP-500-T and teleported to {targetRoom.Name} ({targetRoom.Zone})!");
            player.ShowHint($"<color=yellow>You teleported to {targetRoom.Zone}!</color>", 5);
        }
    }
}
