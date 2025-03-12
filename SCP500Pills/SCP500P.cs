#nullable disable
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500P : CustomItem
    {
        public override uint Id { get; set; } = 5019;
        public override string Name { get; set; } = "SCP-500-P";
        public override string Description { get; set; } = "Boosts your damage output by 25% for 20 seconds.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float EffectDuration = 20f; // ⏳ Продължителност на ефекта
        private const float DamageMultiplier = 1.25f; // ⚔️ Увеличение на щетите с 25%
        private readonly Dictionary<Player, bool> boostedPlayers = new(); // Следи кои играчи имат бууст

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
            Exiled.Events.Handlers.Player.Hurting += OnPlayerHurting;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem -= OnItemUsed;
            Exiled.Events.Handlers.Player.Hurting -= OnPlayerHurting;
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

            ActivatePowerMode(ev.Player);
            ev.Player.Broadcast(5, "<color=yellow>💪 You feel an immense surge of strength!</color>");
            ev.Player.RemoveItem(ev.Item);
        }

        private void ActivatePowerMode(Player player)
        {
            if (!boostedPlayers.ContainsKey(player))
                boostedPlayers[player] = true; // ✅ Добавяме играча към списъка

            // ✅ Автоматично премахване на ефекта след 20 секунди
            Timing.CallDelayed(EffectDuration, () =>
            {
                if (boostedPlayers.ContainsKey(player))
                {
                    boostedPlayers.Remove(player); // ❌ Премахваме играча от списъка
                    player.Broadcast(5, "<color=red>💀 Your strength boost has worn off...</color>");
                }
            });
        }

        private void OnPlayerHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && boostedPlayers.ContainsKey(ev.Attacker))
            {
                ev.Amount *= DamageMultiplier; // ✅ Увеличаваме нанесените щети с 25%
            }
        }
    }
}
