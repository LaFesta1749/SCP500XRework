#nullable disable
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions; // ✅ MirrorExtensions за промяна на модела
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features.Spawn;

namespace SCP500XRework.SCP500Pills
{
    public class SCP500M : CustomItem
    {
        public override uint Id { get; set; } = 5006;
        public override string Name { get; set; } = "SCP-500-M";
        public override string Description { get; set; } = "Distorts your appearance in strange ways!";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; } = 0.1f;
        public override SpawnProperties SpawnProperties { get; set; } = new();

        private static readonly List<string> Distortions = new()
        {
            "You appear to be upside down.",
            "Your model is stretched vertically.",
            "Your model is stretched horizontally.",
            "Your arms seem way too long.",
            "Your head is too big.",
            "Your legs are too short.",
            "Your body is slightly floating above the ground.",
            "Your model flickers randomly.",
            "Your body is twisted at a strange angle.",
            "Your entire model is slightly shaking."
        };

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

            string randomDistortion = Distortions[UnityEngine.Random.Range(0, Distortions.Count)];
            ev.Player.Broadcast(5, $"<color=yellow>You used SCP-500-M!</color> {randomDistortion}");

            ApplyDistortion(ev.Player);
            ev.Player.RemoveItem(ev.Item);
        }

        private void ApplyDistortion(Player player)
        {
            int distortionType = UnityEngine.Random.Range(0, Distortions.Count);

            switch (distortionType)
            {
                case 0:
                    MirrorExtensions.ChangeAppearance(player, player.Role.Type, true, 180); // Обърнат с главата надолу
                    break;
                case 1:
                    ScaleModel(player, new Vector3(1f, 1.5f, 1f)); // Вертикално разтегнат
                    break;
                case 2:
                    ScaleModel(player, new Vector3(1.5f, 1f, 1f)); // Хоризонтално разтегнат
                    break;
                case 3:
                    ScaleModel(player, new Vector3(1f, 1.2f, 1.2f)); // По-дълги ръце
                    break;
                case 4:
                    ScaleModel(player, new Vector3(1.2f, 1.2f, 1.2f)); // Голяма глава
                    break;
                case 5:
                    ScaleModel(player, new Vector3(1f, 0.8f, 1f)); // Къси крака
                    break;
                case 6:
                    player.Position += Vector3.up * 0.3f; // Леко лети над земята
                    break;
                case 7:
                    Timing.RunCoroutine(FlickerModel(player)); // Мигане на модела
                    break;
                case 8:
                    RotateModel(player, new Vector3(20f, 0f, 10f)); // Усукана поза
                    break;
                case 9:
                    Timing.RunCoroutine(ShakeModel(player)); // Леко трептене
                    break;
            }
        }

        private void ScaleModel(Player player, Vector3 scale)
        {
            player.GameObject.transform.localScale = scale;
        }

        private void RotateModel(Player player, Vector3 rotation)
        {
            player.GameObject.transform.Rotate(rotation);
        }

        private IEnumerator<float> FlickerModel(Player player)
        {
            Renderer renderer = player.GameObject.GetComponentInChildren<Renderer>();
            if (renderer == null) yield break;

            for (int i = 0; i < 10; i++)
            {
                renderer.enabled = false; // Скрива модела
                yield return Timing.WaitForSeconds(0.2f);
                renderer.enabled = true; // Показва модела
                yield return Timing.WaitForSeconds(0.2f);
            }
        }

        private IEnumerator<float> ShakeModel(Player player)
        {
            for (int i = 0; i < 20; i++)
            {
                player.GameObject.transform.position += new Vector3(0.01f, 0f, 0.01f);
                yield return Timing.WaitForSeconds(0.1f);
                player.GameObject.transform.position -= new Vector3(0.01f, 0f, 0.01f);
            }
        }
    }
}
