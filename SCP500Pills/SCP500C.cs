#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Linq;
using UnityEngine;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500C : CustomItem
    {
        public override uint Id { get; set; } = 5002;
        public override string Name { get; set; } = "SCP-500-C";
        public override string Description { get; set; } = "Causes chaos by confusing all nearby enemies!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float EffectRadius = 10f; // Радиус на хаоса
        private const int BlurDuration = 8; // Времетраене на замъгленото зрение
        private const int MovementChaosDuration = 6; // Времетраене на случайните движения
        private const int AmnesiaDuration = 7; // Времетраене на амнезията в секунди

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

            ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-C!</color> Nearby enemies are confused!");
            CauseChaos(ev.Player);
            ev.Player.RemoveItem(ev.Item);
        }

        private void CauseChaos(Player player)
        {
            foreach (Player enemy in Player.List.Where(p => p.Role.Team != player.Role.Team && p.IsAlive))
            {
                if (Vector3.Distance(player.Position, enemy.Position) <= EffectRadius)
                {
                    enemy.EnableEffect(EffectType.Blinded, BlurDuration);
                    enemy.EnableEffect(EffectType.Concussed, MovementChaosDuration);
                    enemy.EnableEffect(EffectType.AmnesiaVision, AmnesiaDuration);
                    enemy.ShowHint("<color=red>You feel dizzy and disoriented!</color>", 5);

                    Log.Info($"{player.Nickname} used SCP-500-C and confused {enemy.Nickname}!");
                }
            }
        }
    }
}
