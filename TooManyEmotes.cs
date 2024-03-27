using System.Collections.Generic;
using System.Linq;
using TerminalGamepad.GUILoader;
using TooManyEmotes.Patches;

namespace TerminalGamepad
{
    internal class TooManyEmotes
    {
        private static bool? _enabled;
        public static List<string> StoreButtonsName = new List<string>() { "Lategame Store", "InitAttack", "Cooldown", "Interns", "Scrap Insurance", "Extend Deadline 1", "Contract Info", "Demon", "Bruteforce", "Lookup" };

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("FlipMods.TooManyEmotes");
                }

                return (bool)_enabled;
            }
        }
        public static void Setup()
        {
            if (enabled)
            {
                TerminalGUI.MainButtonNames.Add("Emotes");
                TerminalGUI.MainButtonNamesTEMP = TerminalGUI.MainButtonNames;
            }
        }

        public static void GetEmotesList()
        {
            StoreButtonsName = TerminalPatcher.emoteSelection.Select(e => e.displayName).ToList();
        }
    }
}