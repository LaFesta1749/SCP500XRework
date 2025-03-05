using Exiled.API.Features;
using System;
using SCP500XRework.SCP500Pills; // 📌 Добавяме новите класове за хапчетата
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API;
using InventorySystem.Items.Usables;

namespace SCP500XRework
{
    public class PillsPlugin : Plugin<Config>
    {
        public override string Author => "LaFesta1749";
        public override string Name => "SCP-500 Pills";
        public override string Prefix => "SCP500Pills";
        public override Version Version => new Version(1, 0, 0);

        public static PillsPlugin Instance { get; private set; } = null!;
        public EventHandlers eventHandlers = null!;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Config.IsEnabled)
            {
                Log.Info("SCP-500 Pills Plugin is disabled in the config.");
                return;
            }

            eventHandlers = new EventHandlers();

            // ✅ Регистрираме всяко хапче ръчно
            new SCP500A().Register();
            new SCP500B().Register();
            new SCP500C().Register();
            new SCP500D().Register();
            new SCP500H().Register();
            new SCP500I().Register();
            new SCP500M().Register();
            new SCP500S().Register();
            new SCP500T().Register();
            new SCP500V().Register();
            new SCP500X().Register();
            new SCP500Z().Register();

            // ✅ Регистрираме event-ите
            Exiled.Events.Handlers.Server.RoundStarted += eventHandlers.OnRoundStart;

            Log.Info("SCP-500 Pills Plugin has been enabled!");
        }

        public override void OnDisabled()
        {
            // ❌ Изключваме event-ите
            Exiled.Events.Handlers.Server.RoundStarted -= eventHandlers.OnRoundStart;

            // ❌ Дерегистрираме всички хапчета
            CustomItem.UnregisterItems(); // Премахва всички регистрирани CustomItems

            Log.Info("SCP-500 Pills Plugin has been disabled!");
        }

    }
}
