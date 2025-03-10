#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using MEC;
using UnityEngine;
using PlayerRoles;
using PlayerStatsSystem;
using Exiled.API.Features.Spawn;
using System.Collections.Generic;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500F : CustomItem
    {
        public override uint Id { get; set; } = 5012;
        public override string Name { get; set; } = "SCP-500-F";
        public override string Description { get; set; } = "Simulates death, then revives you.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private const float FakeDeathDuration = 10f; // ⏳ Време, през което изглежда "мъртъв"
        private const float ReviveHealth = 50f; // ❤️ Колко живот ще има след възкресение

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

            ev.Player.Broadcast(5, "<color=yellow>You swallowed SCP-500-F... You feel dizzy.</color>");
            FakeDeath(ev.Player);
            ev.Player.RemoveItem(ev.Item);
        }

        private void FakeDeath(Player player)
        {
            Log.Info($"{player.Nickname} has faked their death.");

            Vector3 fakeDeathPosition = player.Position; // Запазваме позицията
            Quaternion fakeDeathRotation = player.GameObject.transform.rotation;

            // ✅ Създаваме "мъртво" тяло
            Ragdoll fakeRagdoll = Ragdoll.CreateAndSpawn(
                roleType: player.Role.Type,
                name: player.Nickname, // ✅ Добавяме името на играча
                damageHandler: new UniversalDamageHandler(0f, DeathTranslations.Poisoned), // Използваме Poisoned като причина за "смърт"
                position: fakeDeathPosition,
                rotation: fakeDeathRotation,
                owner: player
            );

            // ✅ Правим играча "мъртъв", но всъщност е жив
            player.EnableEffect(EffectType.Invisible, FakeDeathDuration); // ✅ Правим го невидим за другите
            player.EnableEffect(EffectType.Blinded, FakeDeathDuration); // ✅ Добавяме "замаяност"
            player.IsGodModeEnabled = true; // ✅ Правим го неуязвим за времето на фалшивата смърт
            player.IsIntercomMuted = true; // ✅ Заглушаваме гласа му, за да не издаде, че е жив

            Map.Broadcast(5, $"📢 <color=red>{player.Nickname} is down! (Dead Body)</color>");

            // ✅ След 10 секунди го "възкресяваме"
            Timing.CallDelayed(FakeDeathDuration, () =>
            {
                player.DisableEffect(EffectType.Blinded);
                player.DisableEffect(EffectType.Invisible);
                player.DisableAllEffects();
                player.IsGodModeEnabled = false; // ✅ Вече може да бъде убит
                player.IsIntercomMuted = false; // ✅ Връщаме гласа му

                // ✅ Принудително нулиране на камерата (използва се за оправяне на черния екран)
                player.Teleport(fakeDeathPosition);

                fakeRagdoll.Destroy(); // ✅ Премахваме тялото от земята

                player.Broadcast(5, "<color=green>😱 You have returned from the dead!</color>");
                Map.Broadcast(5, $"😱 <color=yellow>{player.Nickname} has returned from the dead!</color>");

                Log.Info($"{player.Nickname} has revived after faking death.");
            });
        }
    }
}