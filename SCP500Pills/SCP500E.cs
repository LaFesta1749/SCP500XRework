#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Spawn;
using PlayerRoles;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500E : CustomItem
    {
        public override uint Id { get; set; } = 5011;
        public override string Name { get; set; } = "SCP-500-E";
        public override string Description { get; set; } = "Gives Class-D and Scientists an item.";
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
            if (!Check(ev.Item)) return; // ✅ Проверява дали използваното хапче е правилното!

            // 🚫 Проверяваме дали играчът е в асансьор или Pocket Dimension
            if (ev.Player.CurrentRoom.Type == RoomType.Pocket)
            {
                ev.Player.ShowHint("<color=red>You cannot use this pill here!</color>", 3);
                ev.IsAllowed = false;
                return;
            }

            if (ev.Player.Role.Type == RoleTypeId.ClassD || ev.Player.Role.Type == RoleTypeId.Scientist)
            {
                ev.Player.AddItem(ItemType.KeycardO5); // ✅ Дава O5 карта
                ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-E!</color> You received a <color=green>Keycard O5</color>!");
            }
            else
            {
                ev.Player.ShowHint("<color=red>❌ SCP-500-E has no effect on you.</color>", 5);
            }

            ev.Player.RemoveItem(ev.Item); // ✅ Хапчето се премахва след използване
        }
    }
}
