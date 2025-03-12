#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using MEC;
using System.Collections.Generic;
using Exiled.API.Features.Spawn;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500O : CustomItem
    {
        public override uint Id { get; set; } = 5014;
        public override string Name { get; set; } = "SCP-500-O";
        public override string Description { get; set; } = "Applies all SCP-500 effects at once, but weakened.";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        // 📌 Намалена продължителност на ефектите (50%)
        private const float EffectDuration = 15f;

        // 📌 Намалена интензивност (50%)
        private const int SpeedIntensity = 10;
        private const int RegenIntensity = 15;
        private const int DamageBoostIntensity = 12;
        private const int JumpBoostIntensity = 8;

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

            ApplyOverdoseEffects(ev.Player);
            ev.Player.Broadcast(5, "<color=purple>💊 You feel an extreme rush of effects! (SCP-500-O Activated)</color>");
            ev.Player.RemoveItem(ev.Item);
        }

        private void ApplyOverdoseEffects(Player player)
        {
            // ✅ Активира всички ефекти с 50% намалена сила
            player.EnableEffect(EffectType.MovementBoost, EffectDuration);
            player.EnableEffect(EffectType.Vitality, EffectDuration);
            player.EnableEffect(EffectType.DamageReduction, EffectDuration);
            player.EnableEffect(EffectType.Scp207, EffectDuration);
            player.EnableEffect(EffectType.BodyshotReduction, EffectDuration);

            // ✅ Променя интензивността им
            player.ChangeEffectIntensity(EffectType.MovementBoost, (byte)SpeedIntensity);
            player.ChangeEffectIntensity(EffectType.Vitality, (byte)RegenIntensity);
            player.ChangeEffectIntensity(EffectType.DamageReduction, (byte)DamageBoostIntensity);
            player.ChangeEffectIntensity(EffectType.BodyshotReduction, (byte)JumpBoostIntensity);

            // ✅ Автоматично премахване на ефектите след 15 секунди
            Timing.CallDelayed(EffectDuration, () =>
            {
                if (player.IsAlive)
                {
                    player.Broadcast(5, "<color=red>💊 The overdose effects have worn off.</color>");

                    // ❌ Изрично премахваме ефектите
                    player.DisableEffect(EffectType.MovementBoost);
                    player.DisableEffect(EffectType.Vitality);
                    player.DisableEffect(EffectType.DamageReduction);
                    player.DisableEffect(EffectType.Scp207);
                    player.DisableEffect(EffectType.BodyshotReduction);
                }
            });
        }
    }
}
