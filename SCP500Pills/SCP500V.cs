#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Linq;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500V : CustomItem
    {
        public override uint Id { get; set; } = 5009;
        public override string Name { get; set; } = "SCP-500-V";
        public override string Description { get; set; } = "Steals HP from nearby enemies and converts it into AHP.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float StealRadius = 8f; // 📏 Радиус за крадене на HP
        private const int HealthToSteal = 10; // ❤️ Колко HP се краде от всеки враг

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

            int totalStolenHp = StealHealthFromEnemies(ev.Player);

            if (totalStolenHp > 0)
            {
                ev.Player.AddAhp(totalStolenHp, totalStolenHp, 0f, 0.7f, 0f, false);
                ev.Player.Broadcast(5, $"<color=#af0000>You stole {totalStolenHp} HP and converted it into AHP!</color>");
                ev.Player.RemoveItem(ev.Item);
            }
            else
            {
                ev.Player.ShowHint("<color=red>❌ No enemies nearby to drain HP from!</color>", 5);
            }
        }

        private int StealHealthFromEnemies(Player user)
        {
            int totalStolen = 0;

            foreach (Player enemy in Player.List.Where(p => p.Role.Team != user.Role.Team && p.IsAlive))
            {
                if (Vector3.Distance(user.Position, enemy.Position) <= StealRadius)
                {
                    int stolen = Mathf.Min(HealthToSteal, (int)enemy.Health - 1); // Да не убива веднага
                    if (stolen > 0)
                    {
                        enemy.Health -= stolen;
                        totalStolen += stolen;
                        enemy.ShowHint("<color=red>⚠ You feel your life force being drained!</color>", 5);
                    }
                }
            }
            return totalStolen;
        }
    }
}
