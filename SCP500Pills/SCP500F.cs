#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using PlayerRoles;
using PlayerStatsSystem;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500F : CustomItem
    {
        public override uint Id { get; set; } = 5012;
        public override string Name { get; set; } = "SCP-500-F";
        public override string Description { get; set; } = "Fake your own death and return after 10 seconds!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float FakeDeathDuration = 10f; // ⏳ Колко време ще изглежда мъртъв
        private const float ReviveHealth = 30f; // ❤️ Колко живот ще има след възраждането

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

            ev.Player.Broadcast(5, "<color=yellow>You took SCP-500-F...</color> You feel weak...");
            FakeDeath(ev.Player);
            ev.Player.RemoveItem(ev.Item);
        }

        private void FakeDeath(Player player)
        {
            Log.Info($"{player.Nickname} has faked their death.");

            Vector3 fakeDeathPosition = player.Position; // Запазваме позицията

            // ✅ Убиваме играча (ще изглежда мъртъв)
            player.Kill("SCP-500-F Effect");
            //Map.Broadcast(5, $"📢 <color=red>{player.Nickname} is down! (Dead Body)</color>");

            // ✅ След 10 секунди го връщаме към живот
            Timing.CallDelayed(10f, () =>
            {
                if (!player.IsAlive) // Проверяваме дали още е "мъртъв"
                {
                    player.Role.Set(player.Role.Type, SpawnReason.Respawn);
                    player.Position = fakeDeathPosition; // ✅ Връщаме го на същото място
                    player.Health = 50; // 🔴 Връщаме го с малко живот
                    player.EnableEffect(EffectType.Concussed, 5f); // 😵 Замаяност за баланс

                    player.Broadcast(5, "<color=green>😱 You have returned from the dead!</color>");
                    Map.Broadcast(5, $"😱 <color=yellow>{player.Nickname} has returned from the dead!</color>");

                    Log.Info($"{player.Nickname} has revived after faking death.");
                }
            });
        }

    }
}
