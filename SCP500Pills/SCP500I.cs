#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using Exiled.API.Enums;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500I : CustomItem
    {
        public override uint Id { get; set; } = 5027;
        public override string Name { get; set; } = "SCP-500-I";
        public override string Description { get; set; } = "Grants temporary invisibility for 8 seconds, removed if interacting with a door.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float InvisibilityDuration = 8f; // ⏳ Времетраене на невидимостта
        private readonly HashSet<Player> invisiblePlayers = new(); // ✅ Запазва кой е невидим

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem += OnItemUsed;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.UsingItem -= OnItemUsed;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
        }

        private void OnItemUsed(UsingItemEventArgs ev)
        {
            if (ev.Item == null || ev.Player == null)
            {
                Log.Warn("[SCP500-I] Item or Player was null during OnItemUsed.");
                return;
            }

            if (!Check(ev.Item)) return; // ✅ Проверява дали използваното хапче е SCP-500-I

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

            ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-I!</color> You are now invisible for 8 seconds!");
            ev.Player.EnableEffect(EffectType.Invisible, InvisibilityDuration);

            // ✅ Добавяме играча в списъка с невидими
            invisiblePlayers.Add(ev.Player);

            ev.Player.RemoveItem(ev.Item);

            // ✅ След 8 секунди премахваме невидимостта
            Timing.CallDelayed(InvisibilityDuration, () => RemoveInvisibility(ev.Player));
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            // ✅ Ако играчът е невидим и докосне врата, премахваме ефекта
            if (invisiblePlayers.Contains(ev.Player))
            {
                RemoveInvisibility(ev.Player);
                ev.Player.ShowHint("<color=red>Your invisibility has worn off!</color>", 5);
            }
        }

        private void RemoveInvisibility(Player player)
        {
            if (player != null && player.IsAlive)
            {
                player.DisableEffect(EffectType.Invisible);
                invisiblePlayers.Remove(player);
                Log.Info($"{player.Nickname} is no longer invisible.");
            }
        }
    }
}
