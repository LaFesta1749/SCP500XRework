#nullable disable
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500A : CustomItem
    {
        public override uint Id { get; set; } = 5000; // Уникално ID за SCP-500-A
        public override string Name { get; set; } = "SCP-500-A";
        public override string Description { get; set; } = "Summons a dead player to help you.";
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
            if (!Check(ev.Item)) return; // ✅ Проверява дали използваното хапче е SCP-500-A

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

            if (TrySummonPlayer(ev.Player))
            {
                ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-A!</color> Summoning a teammate...");
                ev.Player.RemoveItem(ev.Item);
            }
            else
            {
                ev.Player.ShowHint("<color=red>❌ No dead players available to summon!</color>", 5);
                ev.IsAllowed = false; // ❌ Хапчето НЕ се използва
            }
        }

        private bool TrySummonPlayer(Player user)
        {
            var deadPlayers = Player.List.Where(p => p.Role == RoleTypeId.Spectator).ToList();
            if (deadPlayers.Count == 0) return false; // ❌ Няма мъртви играчи

            Player selectedPlayer = deadPlayers[UnityEngine.Random.Range(0, deadPlayers.Count)];
            RoleTypeId newRole = GetResurrectedRole(user); // 🛠️ ВЗЕМАМЕ РОЛЯ НА БАЗА НА ТОЗИ, КОЙТО ИЗПОЛЗВА ХАПЧЕТО

            // ✅ Запазваме текущата позиция
            Vector3 respawnPosition = user.Position;

            selectedPlayer.Role.Set(newRole, SpawnReason.ForceClass);
            selectedPlayer.Position = respawnPosition;

            // ✅ Премахваме Spawn Protection веднага след възкресение
            selectedPlayer.DisableEffect(EffectType.SpawnProtected);

            selectedPlayer.ShowHint($"✨ You have been revived as {newRole}!", 5);
            Log.Info($"{selectedPlayer.Nickname} has been resurrected as {newRole} at {respawnPosition}.");

            return true;
        }

        private RoleTypeId GetResurrectedRole(Player user)
        {
            // 🔄 ВЗЕМАМЕ РОЛЯ НА БАЗА НА ИГРАЧА, КОЙТО ИЗПОЛЗВА ХАПЧЕТО (НЕ НА МЪРТВИЯ)
            return user.Role.Type switch
            {
                RoleTypeId.ClassD => GetRandomRole(RoleTypeId.ChaosRifleman, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor),
                RoleTypeId.Scientist => GetRandomRole(RoleTypeId.NtfCaptain, RoleTypeId.NtfSergeant, RoleTypeId.NtfPrivate),
                RoleTypeId.FacilityGuard => GetRandomRole(RoleTypeId.NtfCaptain, RoleTypeId.NtfSergeant, RoleTypeId.NtfPrivate),
                RoleTypeId.NtfPrivate => GetRandomRole(RoleTypeId.NtfCaptain, RoleTypeId.NtfSergeant, RoleTypeId.NtfPrivate),
                RoleTypeId.NtfSergeant => GetRandomRole(RoleTypeId.NtfCaptain, RoleTypeId.NtfSergeant, RoleTypeId.NtfPrivate),
                RoleTypeId.NtfCaptain => GetRandomRole(RoleTypeId.NtfCaptain, RoleTypeId.NtfSergeant, RoleTypeId.NtfPrivate),
                RoleTypeId.ChaosRifleman => GetRandomRole(RoleTypeId.ChaosRifleman, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor),
                RoleTypeId.ChaosConscript => GetRandomRole(RoleTypeId.ChaosRifleman, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor),
                RoleTypeId.ChaosMarauder => GetRandomRole(RoleTypeId.ChaosRifleman, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor),
                RoleTypeId.ChaosRepressor => GetRandomRole(RoleTypeId.ChaosRifleman, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor),
                _ => RoleTypeId.ClassD // Ако нещо не е в списъка, ще върне Class-D като fallback
            };
        }

        private RoleTypeId GetRandomRole(params RoleTypeId[] roles)
        {
            return roles[UnityEngine.Random.Range(0, roles.Length)];
        }
    }
}
