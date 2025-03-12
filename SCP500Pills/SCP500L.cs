#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using MEC;
using System.Collections.Generic;
using Exiled.API.Features.Spawn;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500L : CustomItem
    {
        public override uint Id { get; set; } = 5013;
        public override string Name { get; set; } = "SCP-500-L";
        public override string Description { get; set; } = "Grants rapid healing and speed boost for 30 seconds.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float EffectDuration = 30f; // ⏳ Продължителност на ефекта
        private const int RegenIntensity = 25; // ❤️ Бонус HP възстановяване (по-бързо лечение)
        private const int SpeedIntensity = 15; // 🏃‍♂️ Скорост

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

            ApplyLuckyEffects(ev.Player);
            ev.Player.Broadcast(5, "<color=yellow>🍀 You feel incredibly lucky! Rapid healing and speed boost activated!</color>");
            ev.Player.RemoveItem(ev.Item);
        }

        private void ApplyLuckyEffects(Player player)
        {
            // ✅ Активира ефектите
            player.EnableEffect(EffectType.Vitality, EffectDuration); // 🏥 Бързо възстановяване
            player.EnableEffect(EffectType.MovementBoost, EffectDuration); // 🏃‍♂️ Скорост

            // ✅ Променя интензивността им
            player.ChangeEffectIntensity(EffectType.Vitality, (byte)RegenIntensity);
            player.ChangeEffectIntensity(EffectType.MovementBoost, (byte)SpeedIntensity);

            // ✅ Автоматично премахване на ефектите след 30 секунди
            Timing.CallDelayed(EffectDuration, () =>
            {
                if (player.IsAlive)
                {
                    player.Broadcast(5, "<color=red>🍀 Your luck has run out. Effects have worn off.</color>");

                    // ❌ Изрично премахваме ефектите
                    player.DisableEffect(EffectType.Vitality);
                    player.DisableEffect(EffectType.MovementBoost);
                }
            });
        }
    }
}
