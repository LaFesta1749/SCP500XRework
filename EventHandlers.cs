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
            if (!TryGetPillId(pillName, out uint pillId))
            {
                Log.Warn($"Pill type '{pillName}' is not recognized and will not be spawned.");
                return;
            }

            if (!CustomItem.TryGet(pillId, out CustomItem? baseItem))
            {
                Log.Warn($"Could not find registered SCP-500 pill: {pillName}");
                return;
            }

            Pickup? spawnedPill = baseItem!.Spawn(position);
            if (spawnedPill == null)
            {
                Log.Warn($"Failed to spawn SCP-500 pill at {position}.");
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
        private bool TryGetPillId(string pillName, out uint id)
        {
            return pillName switch
            {
                "SCP-500-A" => (id = 5000) > 0,
                "SCP-500-B" => (id = 5001) > 0,
                "SCP-500-C" => (id = 5002) > 0,
                "SCP-500-D" => (id = 5003) > 0,
                "SCP-500-E" => (id = 5011) > 0,
                "SCP-500-F" => (id = 5012) > 0,
                "SCP-500-H" => (id = 5004) > 0,
                "SCP-500-I" => (id = 5005) > 0,
                "SCP-500-L" => (id = 5013) > 0,
                "SCP-500-M" => (id = 5006) > 0,
                "SCP-500-O" => (id = 5014) > 0,
                "SCP-500-P" => (id = 5019) > 0,
                "SCP-500-S" => (id = 5007) > 0,
                "SCP-500-T" => (id = 5008) > 0,
                "SCP-500-U" => (id = 5015) > 0,
                "SCP-500-V" => (id = 5009) > 0,
                "SCP-500-W" => (id = 5016) > 0,
                "SCP-500-X" => (id = 5010) > 0,
                "SCP-500-Y" => (id = 5017) > 0,
                "SCP-500-Z" => (id = 5018) > 0,
                _ => (id = 0) > 1 // false
            };
        }
    }
}
