﻿#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Components;
using System.Linq;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;
using Interactables.Interobjects;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500M : CustomItem
    {
        public override uint Id { get; set; } = 5029;
        public override string Name { get; set; } = "SCP-500-M";
        public override string Description { get; set; } = "Distorts your appearance in strange ways!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        // 🔹 Съхраняваме оригиналните модели на играчите, за да ги възстановим при нужда
        private static readonly Dictionary<Player, Vector3> OriginalScales = new();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath; // Връщаме модела при смърт
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem -= OnItemUsed;
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDeath;
        }

        private void OnItemUsed(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            // 🚫 Проверяваме дали играчът е в асансьор или Pocket Dimension
            if (ev.Player.CurrentRoom.Type == RoomType.Pocket ||
                ev.Player.CurrentRoom.Type == RoomType.HczElevatorA ||
                ev.Player.CurrentRoom.Type == RoomType.HczElevatorB ||
                ev.Player.Lift != null) // ✅ Проверяваме дали играчът е в асансьор
            {
                ev.Player.ShowHint("<color=red>You cannot use this pill here!</color>", 3);
                ev.IsAllowed = false;
                return;
            }

            // ✅ Премахваме старото изкривяване, ако има такова
            if (OriginalScales.ContainsKey(ev.Player))
            {
                ev.Player.Scale = OriginalScales[ev.Player]; // Връщаме стария модел
            }
            else
            {
                OriginalScales[ev.Player] = ev.Player.Scale; // Запазваме оригиналния модел
            }

            // ✅ Приложи новото изкривяване
            Vector3 newScale = ApplyDistortion(ev.Player);
            ev.Player.Scale = newScale;

            ev.Player.Broadcast(3, "<color=#ff8dfb>You used SCP-500-M! Your appearance has changed...</color>");
            ev.Player.RemoveItem(ev.Item);
        }

        private Vector3 ApplyDistortion(Player player)
        {
            Vector3 scale = player.Scale; // Фолбек в случай че не е в OriginalScales

            if (OriginalScales.TryGetValue(player, out Vector3 originalScale))
                scale = originalScale;

            int distortionType = UnityEngine.Random.Range(0, 6);

            switch (distortionType)
            {
                case 0:
                    scale.y = 0.5f;
                    player.Broadcast(5, "<color=#ff8dfb>You appear short and floating!</color>");
                    break;
                case 1:
                    scale.z *= -1f;
                    player.Broadcast(5, "<color=#ff8dfb>You appear to be facing backwards!</color>");
                    break;
                case 2:
                    scale.z = 0.05f;
                    player.Broadcast(5, "<color=#ff8dfb>You appear paper thin!</color>");
                    break;
                case 3:
                    scale.x = 1.3f;
                    player.Broadcast(5, "<color=#ff8dfb>You appear wider!</color>");
                    break;
                case 4:
                    scale.y = 1.2f;
                    player.Broadcast(5, "<color=#ff8dfb>You appear taller!</color>");
                    break;
                case 5:
                    scale.y = 0.8f;
                    player.Broadcast(5, "<color=#ff8dfb>You appear squashed!</color>");
                    break;
            }

            return scale;
        }

        private void OnPlayerDeath(DyingEventArgs ev)
        {
            if (OriginalScales.ContainsKey(ev.Player))
            {
                ev.Player.Scale = OriginalScales[ev.Player]; // Възстановяваме нормалния модел
                OriginalScales.Remove(ev.Player);
            }
        }
    }
}
