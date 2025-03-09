#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Doors;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500W : CustomItem
    {
        public override uint Id { get; set; } = 5016;
        public override string Name { get; set; } = "SCP-500-W";
        public override string Description { get; set; } = "Causes nearby doors to open and close randomly!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float EffectDuration = 10f; // ⏳ Продължителност на ефекта (10 секунди)
        private const float ChaosInterval = 0.8f; // ⏳ Време между всяко хаотично отваряне/затваряне
        private const float EffectRadius = 10f; // 📏 Радиус на ефекта
        private static readonly List<string> ExcludedDoors = new() { "079_FIRST", "079_SECOND" }; // 🚫 Забранени врати

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

            ev.Player.Broadcast(5, "<color=yellow>You used SCP-500-W!</color> Nearby doors are going wild!");
            ev.Player.RemoveItem(ev.Item);

            Timing.RunCoroutine(StartDoorChaos(ev.Player));
        }

        private IEnumerator<float> StartDoorChaos(Player player)
        {
            float timer = 0f;
            List<Door> affectedDoors = Door.List
                .Where(d => Vector3.Distance(player.Position, d.Position) <= EffectRadius && !ExcludedDoors.Contains(d.Name))
                .ToList();

            while (timer < EffectDuration)
            {
                foreach (var door in affectedDoors)
                {
                    if (UnityEngine.Random.value > 0.5f)
                        door.IsOpen = !door.IsOpen; // 🔁 Превключва състоянието (отворена → затворена, затворена → отворена)
                }

                yield return Timing.WaitForSeconds(ChaosInterval);
                timer += ChaosInterval;
            }
        }
    }
}
