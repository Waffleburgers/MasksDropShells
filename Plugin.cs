using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MasksDropShells.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasksDropShells
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MaskDropShell : BaseUnityPlugin
    {
        private const string modGUID = "Waffle.MasksDropShells";
        private const string modName = "Masks Drop Shells";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static MaskDropShell Instance;

        internal ManualLogSource mls;

        internal ConfigEntry<int> ShellChance;
        internal ConfigEntry<int> ShellRolls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Masks drop shells has loaded");

            ShellChance = Config.Bind("General",      // The section under which the option is shown
                                   "ShellChance",  // The key of the configuration option in the configuration file
                                   25, // The default value
                                   "This value controls the chance of dropping a shell for each roll, as a percentage (0-100)");

            ShellRolls = Config.Bind("General",      // The section under which the option is shown
                                   "ShellRolls",  // The key of the configuration option in the configuration file
                                   2, // The default value
                                   "This value controls the ammount of times it will attempt to spawn a shotgun shell, as well as the max ammount that can be spawned");

            harmony.PatchAll(typeof(MaskDropShell));
            harmony.PatchAll(typeof(MaskedPlayerEnemy_Patches));
        }
    }
}
