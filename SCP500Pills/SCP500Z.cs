#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Exiled.API.Features.Spawn;
using MEC;
using UnityEngine;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500Z : CustomItem
    {
        public override uint Id { get; set; } = 5018;
        public override string Name { get; set; } = "SCP-500-Z";
        public override string Description { get; set; } = "You die and return as an enhanced SCP-049-2.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

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

            if (ev.Player.Role.Team == Team.SCPs)
            {
                ev.Player.ShowHint("<color=red>❌ SCPs cannot use SCP-500-Z!</color>", 5);
                return;
            }

            ev.Player.Broadcast(5, "<color=yellow>You consumed SCP-500-Z!</color> You feel... strange...");
            TransformIntoZombie(ev.Player);
            ev.Player.RemoveItem(ev.Item);
        }

        private void TransformIntoZombie(Player player)
        {
            Vector3 deathPosition = player.Position; // ✅ Запазваме локацията

            player.Kill("SCP-500-Z Effect"); // 💀 Убиваме играча с custom съобщение

            // ✅ Изчакваме малко и го възраждаме като SCP-049-2
            Timing.CallDelayed(3f, () =>
            {
                if (!player.IsAlive) // Проверяваме дали още е мъртъв
                {
                    player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.Respawn);
                    player.Position = deathPosition; // ✅ Връщаме го на същото място
                    player.MaxHealth = 400; // 💪 Повече HP
                    player.Health = 400;
                    player.EnableEffect(EffectType.MovementBoost, 999f);
                    player.ChangeEffectIntensity(EffectType.MovementBoost, 20); // 🚀 По-бърз

                    player.Broadcast(5, "<color=green>🧟 You have resurrected as an enhanced SCP-049-2!</color>");
                    Log.Info($"{player.Nickname} has transformed into an enhanced SCP-049-2.");
                }
            });
        }
    }
}
