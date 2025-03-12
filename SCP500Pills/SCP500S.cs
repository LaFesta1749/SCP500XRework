#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using System;
using MEC;
using UnityEngine;
using Exiled.API.Features.Spawn;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500S : CustomItem
    {
        public override uint Id { get; set; } = 5007;
        public override string Name { get; set; } = "SCP-500-S";
        public override string Description { get; set; } = "Gives a random speed boost or slowdown.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float EffectDuration = 10f; // ⏳ Продължителност на ефекта
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

            bool isBoost = rng.NextDouble() <= 0.7; // 70% шанс за ускорение, 30% шанс за забавяне
            byte intensity = isBoost ? (byte)rng.Next(70, 201) : (byte)rng.Next(10, 40); // 70-200 boost, 10-40 slow

            ApplySpeedEffect(ev.Player, isBoost, intensity);
            ev.Player.Broadcast(5, $"<color=yellow>You used SCP-500-S!</color> {(isBoost ? "<color=green>Speed Boost!</color>" : "<color=red>Speed Reduction!</color>")}");
            ev.Player.RemoveItem(ev.Item);
        }

        private void ApplySpeedEffect(Player player, bool isBoost, byte intensity)
        {
            EffectType effect = isBoost ? EffectType.MovementBoost : EffectType.SinkHole;

            player.EnableEffect(effect, EffectDuration);
            player.ChangeEffectIntensity(effect, intensity);

            Log.Info($"{player.Nickname} used SCP-500-S and received {(isBoost ? "Speed Boost" : "Speed Reduction")} (Intensity {intensity}) for {EffectDuration} seconds.");

            // ❌ Премахваме ефекта след изтичане на времето
            Timing.CallDelayed(EffectDuration, () =>
            {
                if (player.IsAlive)
                {
                    player.DisableEffect(effect);
                    player.ShowHint("<color=red>Your movement speed has returned to normal.</color>", 5);
                }
            });
        }
    }
}
