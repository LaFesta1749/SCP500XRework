#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions; // ✅ MirrorExtensions е тук!
using MEC;
using PlayerRoles;
using UnityEngine;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500D : CustomItem
    {
        public override uint Id { get; set; } = 5003;
        public override string Name { get; set; } = "SCP-500-D";
        public override string Description { get; set; } = "Temporarily disguises you as the enemy!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const int DisguiseDuration = 10; // ⏳ Времетраене на маскировката

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

            RoleTypeId disguiseRole = GetDisguiseRole(ev.Player);
            if (disguiseRole == RoleTypeId.None)
            {
                ev.Player.ShowHint("<color=red>❌ You cannot be disguised!</color>", 5);
                return;
            }

            // 🔥 Включваме точната роля в съобщението!
            ev.Player.Broadcast(5, $"<color=yellow>You used SCP-500-D!</color> You are now disguised as <color=green>{disguiseRole}</color>!");

            ApplyDisguise(ev.Player, disguiseRole);
            ev.Player.RemoveItem(ev.Item);
        }

        private void ApplyDisguise(Player player, RoleTypeId disguiseRole)
        {
            Log.Info($"{player.Nickname} is disguising as {disguiseRole}");

            // ✅ Използваме MirrorExtensions за смяна на външния вид!
            MirrorExtensions.ChangeAppearance(player, disguiseRole, false, 0);

            // ⏳ След време премахваме маскировката
            Timing.CallDelayed(DisguiseDuration, () => RemoveDisguise(player));
        }

        private void RemoveDisguise(Player player)
        {
            if (player.IsAlive)
            {
                Log.Info($"{player.Nickname} disguise expired, restoring original model.");
                MirrorExtensions.ChangeAppearance(player, player.Role.Type, false, 0);
                player.ShowHint("<color=red>Your disguise has worn off!</color>", 5);
            }
        }

        private RoleTypeId GetDisguiseRole(Player player)
        {
            return player.Role.Team switch
            {
                Team.FoundationForces => RoleTypeId.ChaosRifleman,
                Team.ChaosInsurgency => RoleTypeId.NtfPrivate,
                Team.Scientists => RoleTypeId.ClassD,
                Team.ClassD => RoleTypeId.Scientist,
                _ => RoleTypeId.None
            };
        }
    }
}
