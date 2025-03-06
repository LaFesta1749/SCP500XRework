#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features.Spawn;

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

        // 📌 Запазваме текущите изкривявания на всеки играч
        private static readonly Dictionary<Player, Vector3> PlayerDistortions = new();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath; // ✅ Изчистване при смърт
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

            // ✅ Ако играчът вече има изкривяване, взимаме текущото му скалиране
            Vector3 currentScale = PlayerDistortions.ContainsKey(ev.Player)
                ? PlayerDistortions[ev.Player]
                : ev.Player.GameObject.transform.localScale;

            // ✅ Приложи ново изкривяване
            Vector3 newScale = ApplyDistortion(ev.Player, currentScale);

            // ✅ Запази изкривяването, за да не се губи
            PlayerDistortions[ev.Player] = newScale;

            // ✅ Изпрати съобщение за ефекта
            ev.Player.Broadcast(3, $"<color=#ff8dfb>You used SCP-500-M!</color> Your appearance has changed...");

            ev.Player.RemoveItem(ev.Item);
        }

        private Vector3 ApplyDistortion(Player player, Vector3 scale)
        {
            int distortionType = UnityEngine.Random.Range(0, 11); // Общо 11 различни модификации

            switch (distortionType)
            {
                case 0:
                    scale.y *= -1f; // 🔄 Обърнат модел с главата надолу
                    player.Broadcast(5, "<color=#ff8dfb>You appear upside down!</color>");
                    break;
                case 1:
                    scale.y = 0.5f; // 🔼 Става нисък и левитиращ
                    player.Broadcast(5, "<color=#ff8dfb>You appear short and floating!</color>");
                    break;
                case 2:
                    scale.z *= -1f; // 🔄 Гледа назад
                    player.Broadcast(5, "<color=#ff8dfb>You appear to be facing backwards!</color>");
                    break;
                case 3:
                    scale.z *= 0.05f; // 📝 Хартиен модел (много тънък)
                    player.Broadcast(5, "<color=#ff8dfb>You appear paper thin!</color>");
                    break;
                case 4:
                    scale.x += 1f; // ➡️ Разширяване на тялото
                    player.Broadcast(5, "<color=#ff8dfb>You appear wider!</color>");
                    break;
                case 5:
                    scale.y += 0.5f; // ⬆️ По-висок модел
                    player.Broadcast(5, "<color=#ff8dfb>You appear taller!</color>");
                    break;
                case 6:
                    scale.x += 1.5f; // 📏 Става още по-широк
                    player.Broadcast(5, "<color=#ff8dfb>You appear even wider than before!</color>");
                    break;
                case 7:
                    scale.y += 1.2f; // 🧠 Огромна глава (Big Head Mode)
                    player.Broadcast(5, "<color=#ff8dfb>Your head appears gigantic!</color>");
                    break;
                case 8:
                    scale.x += 0.6f; // 👐 Огромни ръце
                    scale.z += 0.6f;
                    player.Broadcast(5, "<color=#ff8dfb>Your hands look massive!</color>");
                    break;
                case 9:
                    player.GameObject.transform.Rotate(new Vector3(0f, 15f, 10f)); // 🤕 Извита поза
                    player.Broadcast(5, "<color=#ff8dfb>You appear crooked!</color>");
                    break;
                case 10:
                    scale.y += 0.5f; // 🦵 Гигантски крака
                    player.Position += Vector3.up * 0.4f; // 📏 Повдига играча, за да не потъне в земята
                    player.Broadcast(5, "<color=#ff8dfb>Your legs seem way too long!</color>");
                    break;
            }

            // ✅ Приложи новата форма към играча
            player.GameObject.transform.localScale = scale;

            return scale;
        }

        private void OnPlayerDeath(DyingEventArgs ev)
        {
            if (PlayerDistortions.ContainsKey(ev.Player))
            {
                ev.Player.GameObject.transform.localScale = Vector3.one; // ✅ Нулирай модела
                PlayerDistortions.Remove(ev.Player);
            }
        }
    }
}
