using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using MEC;
using UnityEngine;

namespace SCP500XRework
{
    public class EventHandlers
    {
        public readonly List<Vector3> spawnedPills = new();
        private static readonly System.Random Random = new();

        public void OnRoundStart()
        {
            //Log.Info("🚀 Round started! Attempting to spawn SCP-500 pills..."); // DEBUG лог
            spawnedPills.Clear();

            int pillsToSpawn = PillsPlugin.Instance.Config.MaxPillsPerRound;
            int spawnedCount = 0;
            List<string> pillTypes = new List<string>(PillsPlugin.Instance.Config.PillsSpawnChance.Keys);

            while (spawnedCount < pillsToSpawn)
            {
                string randomPill = pillTypes[Random.Next(pillTypes.Count)];
                if (Random.Next(0, 100) <= PillsPlugin.Instance.Config.PillsSpawnChance[randomPill])
                {
                    Vector3 spawnPosition = GetRandomSpawnPosition();
                    //Log.Info($"Attempting to spawn {randomPill} at {spawnPosition}");

                    spawnedPills.Add(spawnPosition);
                    SpawnPill(randomPill, spawnPosition);
                    spawnedCount++;

                    //Log.Info($"Successfully spawned {randomPill} at {spawnPosition}");
                }
            }
        }

        private void SpawnPill(string pillName, Vector3 position)
        {
            uint pillId = pillName switch
            {
                "SCP-500-A" => 5000,
                "SCP-500-B" => 5001,
                "SCP-500-C" => 5002,
                "SCP-500-D" => 5003,
                "SCP-500-H" => 5004,
                "SCP-500-I" => 5005,
                "SCP-500-M" => 5006,
                "SCP-500-S" => 5007,
                "SCP-500-T" => 5008,
                "SCP-500-V" => 5009,
                "SCP-500-X" => 5010,
                _ => 5000
            };

            if (!CustomItem.TryGet(pillId, out CustomItem? baseItem))
            {
                Log.Warn($"Could not find registered SCP-500 pill: {pillName}");
                return;
            }

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Pickup? spawnedPill = baseItem.Spawn(position);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (spawnedPill == null)
            {
                Log.Warn($"Failed to spawn SCP-500 pill at {position}.");
                return;
            }

            if (spawnedPill == null)
            {
                Log.Warn($"Failed to spawn pill {pillName} at {position}.");
                return;
            }

            Log.Info($"Spawned {pillName} at {position}");

            if (PillsPlugin.Instance.Config.EnableGlowEffect)
            {
                Color glowColor = GetPillColor(pillName);
                CreateLightSource(position, glowColor);
            }
        }

        private void CreateLightSource(Vector3 position, Color color)
        {
            GameObject lightObject = new GameObject("SCP500_Light");
            lightObject.transform.position = position;

            Light light = lightObject.AddComponent<Light>();
            light.color = color;
            light.intensity = PillsPlugin.Instance.Config.GlowIntensity;
            light.range = PillsPlugin.Instance.Config.GlowRange;
            light.type = LightType.Point;

            // Автоматично изчистване на светлината след 5 минути
            Timing.CallDelayed(300f, () => UnityEngine.Object.Destroy(lightObject));
        }

        private Vector3 GetRandomSpawnPosition()
        {
            List<Room> rooms = new List<Room>(Room.List);
            Room randomRoom = rooms[Random.Next(rooms.Count)];
            return randomRoom.Position + new Vector3((float)(Random.NextDouble() * 4 - 2), 0.2f, (float)(Random.NextDouble() * 4 - 2));
        }

        private Color GetPillColor(string pillName)
        {
            return pillName switch
            {
                "SCP-500-A" => Color.green,
                "SCP-500-B" => Color.red,
                "SCP-500-C" => new Color(1.0f, 0.65f, 0.0f),
                "SCP-500-D" => new Color(0.63f, 0.13f, 0.94f),
                "SCP-500-H" => new Color(0.7f, 0.0f, 0.0f),
                "SCP-500-I" => Color.white,
                "SCP-500-M" => Color.gray,
                "SCP-500-S" => Color.blue,
                "SCP-500-T" => Color.yellow,
                "SCP-500-V" => Color.magenta,
                "SCP-500-X" => new Color(0.55f, 0.27f, 0.07f),
                _ => Color.white
            };
        }
    }
}
