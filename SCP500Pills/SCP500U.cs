#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500U : CustomItem
    {
        public override uint Id { get; set; } = 5015;
        public override string Name { get; set; } = "SCP-500-U";
        public override string Description { get; set; } = "Summons a teammate to help you.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new(); // ✅ Поправено

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

        private void OnItemUsed(Exiled.Events.EventArgs.Player.UsingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return; // ✅ Проверява дали използваното хапче е правилното!

            ev.Player.Broadcast(5, "You used SCP-500-U! Summoning a teammate...");
            SummonTeammate(ev.Player);
            ev.Player.RemoveItem(ev.Item);
        }

        private void SummonTeammate(Player player)
        {
            Log.Info($"{player.Nickname} used SCP-500-U, but summoning logic is not implemented yet.");
        }
    }
}
