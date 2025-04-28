#nullable disable
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server; // ✅ Уточняваме `Server.RoundEnded`
using Exiled.Events.Handlers;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500H : CustomItem
    {
        public override uint Id { get; set; } = 5026;
        public override string Name { get; set; } = "SCP-500-H";
        public override string Description { get; set; } = "Increases your maximum health.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const int HealthIncrease = 20; // ✅ Увеличение на максималния HP
        private static readonly Dictionary<int, float> ModifiedHealth = new(); // 📌 Запазва оригиналния HP на база Player ID

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd; // ✅ Връщане на HP след рунда
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem -= OnItemUsed;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
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

            // ✅ Запазваме оригиналния HP, ако не е вече запазен
            if (!ModifiedHealth.ContainsKey(ev.Player.Id))
                ModifiedHealth[ev.Player.Id] = ev.Player.MaxHealth;

            // ✅ Увеличаваме максималния HP и го лекуваме до новия максимум
            ev.Player.MaxHealth += HealthIncrease;
            ev.Player.Health = ev.Player.MaxHealth;

            ev.Player.Broadcast(5, $"<color=yellow>You used SCP-500-H!</color> Your maximum health increased to <color=green>{ev.Player.MaxHealth}</color>.");
            ev.Player.RemoveItem(ev.Item);
        }

        private void OnRoundEnd(RoundEndedEventArgs ev)
        {
            // ✅ Връщаме оригиналния HP на всички играчи
            foreach (var entry in ModifiedHealth)
            {
                Exiled.API.Features.Player player = Exiled.API.Features.Player.List.FirstOrDefault(p => p.Id == entry.Key);
                if (player != null && player.IsAlive)
                    player.MaxHealth = 100;
            }

            // ✅ Изчистваме списъка за следващия рунд
            ModifiedHealth.Clear();
        }
    }
}
