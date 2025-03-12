#nullable disable
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500B : CustomItem
    {
        public override uint Id { get; set; } = 5001; // Уникално ID за SCP-500-B
        public override string Name { get; set; } = "SCP-500-B";
        public override string Description { get; set; } = "Swaps your team to the opposite faction.";
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

            ev.Player.RemoveItem(ev.Item); // ✅ Премахваме хапчето веднага
            ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-B!</color> Switching teams...");
            SwapTeam(ev.Player);
        }

        private void SwapTeam(Player player)
        {
            if (player.Role.Team == Team.SCPs || player.Role == RoleTypeId.Spectator)
            {
                player.ShowHint("⚠ You cannot swap teams!", 5);
                return;
            }

            // ✅ Запазваме текущата позиция преди смяната
            Vector3 currentPosition = player.Position;

            RoleTypeId newRole = player.Role.Type switch
            {
                RoleTypeId.ClassD => RoleTypeId.Scientist,  // Class-D → Scientist
                RoleTypeId.Scientist => RoleTypeId.ClassD,  // Scientist → Class-D
                RoleTypeId.NtfPrivate => RoleTypeId.ChaosRifleman,  // MTF → Chaos
                RoleTypeId.NtfSergeant => RoleTypeId.ChaosMarauder,
                RoleTypeId.NtfCaptain => RoleTypeId.ChaosRepressor,
                RoleTypeId.NtfSpecialist => RoleTypeId.ChaosConscript,
                RoleTypeId.ChaosRifleman => RoleTypeId.NtfPrivate,  // Chaos → MTF
                RoleTypeId.ChaosMarauder => RoleTypeId.NtfSergeant,
                RoleTypeId.ChaosRepressor => RoleTypeId.NtfCaptain,
                RoleTypeId.ChaosConscript => RoleTypeId.NtfSpecialist,
                RoleTypeId.FacilityGuard => GetRandomFacilityGuardTransformation(), // FacilityGuard → SCP-049-2, Scientist или Class-D
                _ => player.Role.Type // Ако не попада в списъка, не се променя
            };

            if (newRole == player.Role.Type)
            {
                player.ShowHint("⚠ Your team cannot be swapped!", 5);
                return;
            }

            // ✅ Drop-ваме всички предмети на земята преди смяната на ролята
            foreach (var item in player.Items.ToList()) // Взима всички предмети
            {
                player.DropItem(item); // Дропва предмета
            }

            // ✅ Смяна на отбора
            player.RoleManager.ServerSetRole(newRole, RoleChangeReason.RemoteAdmin);

            // ✅ Връщаме играча на същото място след смяната
            Timing.CallDelayed(0.1f, () => player.Position = currentPosition);

            player.ShowHint($"✨ You have transformed into a {newRole}!", 5);
            Log.Info($"{player.Nickname} used SCP-500-B and swapped to {newRole}.");
        }

        // ✅ Логика за Facility Guard – избира случайно между SCP-049-2, Scientist или Class-D
        private RoleTypeId GetRandomFacilityGuardTransformation()
        {
            RoleTypeId[] possibleRoles = { RoleTypeId.Scp0492, RoleTypeId.Scientist, RoleTypeId.ClassD };
            return possibleRoles[UnityEngine.Random.Range(0, possibleRoles.Length)];
        }
    }
}