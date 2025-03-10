#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Extensions;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500M : CustomItem
    {
        public override uint Id { get; set; } = 5006;
        public override string Name { get; set; } = "SCP-500-M";
        public override string Description { get; set; } = "Distorts your appearance in strange ways!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        // Запазваме текущите промени на играчите
        private static readonly Dictionary<Player, Vector3> PlayerDistortions = new();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath; // Изчистване на ефекта при смърт
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

            Vector3 currentScale = PlayerDistortions.ContainsKey(ev.Player)
                ? PlayerDistortions[ev.Player]
                : ev.Player.Scale; // Използва се `Scale`, който работи с MirrorExtended

            Vector3 newScale = ApplyDistortion(ev.Player, currentScale);
            PlayerDistortions[ev.Player] = newScale;

            ev.Player.Broadcast(3, "<color=#ff8dfb>You used SCP-500-M!</color> Your appearance has changed...");
            ev.Player.RemoveItem(ev.Item);
        }

        private Vector3 ApplyDistortion(Player player, Vector3 scale)
        {
            int distortionType = UnityEngine.Random.Range(0, 6); // Избира един от 6 ефекта

            switch (distortionType)
            {
                case 0:
                    scale.y = 0.5f; // Малък и левитиращ
                    player.Broadcast(5, "<color=#ff8dfb>You appear short and floating!</color>");
                    break;
                case 1:
                    scale.z *= -1f; // Обърнат назад
                    player.Broadcast(5, "<color=#ff8dfb>You appear to be facing backwards!</color>");
                    break;
                case 2:
                    scale.z *= 0.05f; // Хартиен модел (много тънък)
                    player.Broadcast(5, "<color=#ff8dfb>You appear paper thin!</color>");
                    break;
                case 3:
                    scale.x += 1f; // Разширен
                    player.Broadcast(5, "<color=#ff8dfb>You appear wider!</color>");
                    break;
                case 4:
                    scale.y += 0.5f; // По-висок
                    player.Position += Vector3.up * 0.3f; // Компенсира височината, за да не падне под картата
                    player.Broadcast(5, "<color=#ff8dfb>You appear taller!</color>");
                    break;
                case 5:
                    scale.x += 1.5f; // Още по-широк
                    player.Broadcast(5, "<color=#ff8dfb>You appear even wider than before!</color>");
                    break;
            }

            // Използваме MirrorExtended за безопасно прилагане на скалирането
            player.Scale = scale;

            return scale;
        }

        private void OnPlayerDeath(DyingEventArgs ev)
        {
            if (PlayerDistortions.ContainsKey(ev.Player))
            {
                ev.Player.Scale = Vector3.one; // Възстановява нормалния модел
                PlayerDistortions.Remove(ev.Player);
            }
        }
    }
}
