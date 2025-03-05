using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace SCP500XRework
{
    public class Config : IConfig
    {
        [Description("Дали плъгинът да бъде активиран.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Дали debug да бъде активиран.")]
        public bool Debug { get; set; } = false;

        [Description("Максимален брой SCP-500 хапчета, които могат да spawn-нат в един рунд.")]
        public int MaxPillsPerRound { get; set; } = 20;

        [Description("Процентен шанс за spawn на всяко хапче.")]
        public Dictionary<string, int> PillsSpawnChance { get; set; } = new Dictionary<string, int>()
        {
            { "SCP-500-A", 50 },
            { "SCP-500-B", 40 },
            { "SCP-500-C", 30 },
            { "SCP-500-D", 45 },
            { "SCP-500-H", 50 },
            { "SCP-500-I", 35 },
            { "SCP-500-M", 30 },
            { "SCP-500-S", 60 },
            { "SCP-500-T", 25 },
            { "SCP-500-V", 40 },
            { "SCP-500-X", 20 },
            { "SCP-500-E", 50 },
            { "SCP-500-F", 30 },
            { "SCP-500-L", 50 },
            { "SCP-500-O", 5 },
            { "SCP-500-U", 30 },
            { "SCP-500-W", 25 },
            { "SCP-500-Y", 40 },
            { "SCP-500-Z", 15 },
            { "SCP-500-P", 45 }
        };

        [Description("Дали хапчетата да имат glow ефект при spawn.")]
        public bool EnableGlowEffect { get; set; } = true;

        [Description("Колко ярка да бъде светлината около хапчетата.")]
        public float GlowIntensity { get; set; } = 5.0f;

        [Description("Обхватът на светлината около хапчетата.")]
        public float GlowRange { get; set; } = 1.5f;

        [Description("Съобщение, което се показва при взимане на хапче.")]
        public string PickedUpHint { get; set; } = "You have picked up a {0}\n{1}";

        [Description("Съобщение, което се показва при избиране на хапче в инвентара.")]
        public string SelectedHint { get; set; } = "You have selected a {0}\n{1}";
    }
}
