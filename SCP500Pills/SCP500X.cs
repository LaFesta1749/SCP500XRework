#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Linq;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
using Exiled.API.Features.Doors;
using Exiled.API.Extensions;
using Exiled.API.Interfaces;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500X : CustomItem
    {
        public override uint Id { get; set; } = 5010;
        public override string Name { get; set; } = "SCP-500-X";
        public override string Description { get; set; } = "Explodes all nearby doors.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float ExplosionRadius = 8f; // 📏 Радиус на експлозията

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

            // 🚫 Проверяваме дали играчът е в асансьор или Pocket Dimension
            if (ev.Player.CurrentRoom.Type == RoomType.Pocket)
            {
                ev.Player.ShowHint("<color=red>You cannot use this pill here!</color>", 3);
                ev.IsAllowed = false;
                return;
            }

            bool exploded = ExplodeNearbyDoors(ev.Player);

            if (exploded)
            {
                ev.Player.Broadcast(5, "<color=red>💥 You destroyed all nearby doors!</color>");
                ev.Player.RemoveItem(ev.Item);
            }
            else
            {
                ev.Player.ShowHint("<color=red>❌ No doors nearby to destroy!</color>", 5);
            }
        }

        private bool ExplodeNearbyDoors(Player player)
        {
            bool anyDoorExploded = false;

            foreach (var door in Door.List.Where(d => Vector3.Distance(player.Position, d.Position) <= ExplosionRadius))
            {
                // ❌ Пропускаме вратите "079_FIRST" и "079_SECOND"
                if (door.Name == "079_FIRST" || door.Name == "079_SECOND")
                    continue; // ❌ Пропуска и не прилага експлозията

                if (door is Exiled.API.Interfaces.IDamageableDoor damageableDoor) // ✅ Проверка дали вратата може да бъде унищожена
                {
                    damageableDoor.Break();
                    anyDoorExploded = true;
                }
                else if (!door.IsOpen && !door.IsLocked) // Ако не може да се счупи, просто я отваряме
                {
                    door.IsOpen = true;
                    anyDoorExploded = true;
                }
            }

            return anyDoorExploded;
        }
    }
}
