using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TerminalGamepad.GUILoader;

namespace TerminalGamepad
{
    internal class LGU
    {
        private static bool? _enabled;
        public static List<string> MainButtonsName = new List<string>() { "Lategame Store", "InitAttack", "Cooldown", "Interns", "Scrap Insurance", "Extend Deadline <amount>", "Contract Info", "Demon", "Bruteforce", "Lookup" };
        public static List<string> MoreUpgradesButtonsName = new List<string>() { "Back Muscles", "Bargain Connections", "Beekeeper", "Better Scanner", "Bigger Lungs", "Charging Booster", "Climbing Gloves", "Discombobulator", "Drop Pod Thrusters", "Efficient Engines", "Fast Encryption", "Hunter", "Lethal Deals", "Lightning Rod", "Locksmith", "Malware Broadcaster", "Market Influence", "NV Headset Batteries", "Protein Powder", "Quantum Disruptor", "Running Shoes", "Shutter Batteries", "Sick Beats", "Sigurd Access", "Stimpack", "Strong Legs", "Walkie GPS" };
        public static List<string> ConfirmButtonNames = new List<string>() { "Confirm", "Deny" };
        public static List<string> DemonsButtonNames = new List<string>() { "Poltergeist", "Phantom", "Wraith", "Banshee", "Jinn", "Hantu", "Moroi", "Myling", "Goryo", "De ogen" };
        public static List<string> IPButtonNames = new List<string>() { "Submit", "Backspace", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "." };
        public static List<string> ContractButtonNames;

        public static bool enabled
        {
            get
            {
                /*if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades");
                }

                return (bool)_enabled;*/

                return false;
            }
        }

        public static void Setup()
        {
            if (enabled)
            {
                TerminalGUI.MainButtonNames.Add("LateGame");
                TerminalGUI.MainButtonNamesTEMP = TerminalGUI.MainButtonNames;
            }
        }

        public static void ContractMoons()
        {
            ContractButtonNames = new List<string> { "Random" };
            ContractButtonNames.AddRange(TerminalGUI.MoonsButtonNames.Select(item => item).ToList());
        }

        public static int CountDots(string input)
        {
            int count = 0;
            foreach (char c in input)
            {
                if (c == '.')
                {
                    count++;
                }
            }
            return count;
        }

        public static int CountNumber(string input)
        {
            if (input.Length <= 0)
                return 0;

            char lastChar = input[input.Length - 1];
            if (lastChar == '.')
                return 0;

            string[] text = input.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            return text[text.Length - 1].Length;
        }

        public static int HowManyNumbers(string input)
        {
            int dots = CountDots(input);
            int count = 0;

            if (dots == 0 || dots == 3)
                count = 3;
            else if (dots == 1 || dots == 2)
                count = 2;

            return count;
        }

        public static bool IsLettersOnly(string input)
        {
            string pattern = @"^[a-zA-Z]+$";

            // Use Regex.IsMatch() to check if the input string matches the pattern
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsNumbersOnly(string input)
        {
            string pattern = @"^[0-9]+$";

            // Use Regex.IsMatch() to check if the input string matches the pattern
            return Regex.IsMatch(input, pattern);
        }
    }
}
