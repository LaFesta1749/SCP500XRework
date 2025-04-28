#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using MEC;
using System;
using System.Collections.Generic;
using Exiled.API.Features.Spawn;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500U : CustomItem
    {
        public override uint Id { get; set; } = 5034;
        public override string Name { get; set; } = "SCP-500-U";
        public override string Description { get; set; } = "After 5 seconds, grants a random SCP-500 effect for 15 seconds.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        // ⏳ Закъснение преди ефекта
        private const float DelayBeforeEffect = 5f;

        // ⏳ Продължителност на ефекта
        private const float EffectDuration = 15f;

        // 🎲 Възможни ефекти
        private static readonly EffectType[] PossibleEffects = new EffectType[]
        {
            EffectType.MovementBoost,  // Ускорение (SCP-500-S)
            EffectType.Vitality,       // Регенерация (SCP-500-L)
            EffectType.BodyshotReduction, // Намаляване на щетите (SCP-500-P)
            EffectType.Scp207,         // Перманентно ускорение (SCP-500-Y)
            EffectType.DamageReduction // Намаляване на входящите щети (SCP-500-O)
        };

        private static readonly System.Random rng = new();

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

            ev.Player.Broadcast(5, "<color=orange>💊 You feel unstable... Something will happen soon.</color>");
            Timing.CallDelayed(DelayBeforeEffect, () => ApplyRandomEffect(ev.Player));
            ev.Player.RemoveItem(ev.Item);
        }

        private void ApplyRandomEffect(Player player)
        {
            if (!player.IsAlive) return;

            // 🎲 Избира случаен ефект
            EffectType chosenEffect = PossibleEffects[rng.Next(PossibleEffects.Length)];

            player.EnableEffect(chosenEffect, EffectDuration);
            player.Broadcast(5, $"<color=yellow>💊 You received an unstable effect: {chosenEffect}!</color>");

            Log.Info($"{player.Nickname} received the {chosenEffect} effect from SCP-500-U.");

            // ✅ Автоматично премахване на ефекта след 15 секунди
            Timing.CallDelayed(EffectDuration, () =>
            {
                if (player.IsAlive)
                {
                    player.DisableEffect(chosenEffect);
                    player.Broadcast(5, "<color=red>💊 The unstable effect has worn off.</color>");
                }
            });
        }
    }
}
