#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500Y : CustomItem
    {
        public override uint Id { get; set; } = 5017;
        public override string Name { get; set; } = "SCP-500-Y";
        public override string Description { get; set; } = "An unpredictable pill that oscillates between good and bad effects.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const int TotalCycles = 5; // 🔄 Общо 5 смени
        private const float CycleDuration = 5f; // ⏳ Продължителност на един ефект

        private static readonly EffectType[] PositiveEffects =
        {
            EffectType.MovementBoost, // 🚀 Скорост
            EffectType.Scp207,        // 🍷 Бавна регенерация (замяна за BodyRegen)
            EffectType.Vitality       // 💪 Повишен Max HP
        };

        private static readonly EffectType[] NegativeEffects =
        {
            EffectType.SinkHole,  // 🐌 Забавяне
            EffectType.Bleeding,  // 🩸 Бавно губене на HP
            EffectType.Poisoned   // ☠️ Постепенен DMG
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

            ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-Y!</color> Effects will change every 5 seconds!");
            Timing.RunCoroutine(ApplyYoyoEffect(ev.Player));
            ev.Player.RemoveItem(ev.Item);
        }

        private IEnumerator<float> ApplyYoyoEffect(Player player)
        {
            for (int i = 0; i < TotalCycles; i++)
            {
                bool isPositive = i % 2 == 0;
                EffectType effect = isPositive
                    ? PositiveEffects[UnityEngine.Random.Range(0, PositiveEffects.Length)]
                    : NegativeEffects[UnityEngine.Random.Range(0, NegativeEffects.Length)];

                byte intensity = (byte)UnityEngine.Random.Range(50, 151); // 🔧 Интензитет на ефекта
                player.EnableEffect(effect, CycleDuration);
                player.ChangeEffectIntensity(effect, intensity);

                player.Broadcast(3, isPositive
                    ? $"<color=green>✅ You feel empowered! ({effect})</color>"
                    : $"<color=red>⚠ Something feels wrong... ({effect})</color>");

                // ✅ Гарантирано премахване на ефекта след 5 секунди
                Timing.CallDelayed(CycleDuration, () =>
                {
                    if (player.IsAlive)
                    {
                        player.DisableEffect(effect);
                    }
                });

                yield return Timing.WaitForSeconds(CycleDuration);
            }
        }
    }
}
