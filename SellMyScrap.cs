using System.Collections.Generic;
using TerminalGamepad.GUILoader;
using UnityEngine;

namespace TerminalGamepad
{
    internal class SellMyScrap
    {
        private static bool? _enabled;
        public static List<string> scrapList = new List<string>();
        public static List<string> MainButtonsName = new List<string>() { "sell <amount>", "sell quota", "sell all", "sell item", "view overtime", "view scrap", "view config" }; // view all scrap

        private static GameObject hangarShip;
        public static GameObject HangarShip
        {
            get
            {
                if (hangarShip == null)
                {
                    hangarShip = GameObject.Find("/Environment/HangarShip");
                }

                return hangarShip;
            }
        }

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.github.zehsteam.SellMyScrap");
                }

                return (bool)_enabled;
            }
        }
        public static void Setup()
        {
            if (enabled)
            {
                TerminalGUI.MainButtonNames.Add("Sell");
                TerminalGUI.MainButtonNamesTEMP = TerminalGUI.MainButtonNames;
            }
        }

        public static void GetScrapList()
        {
            scrapList = GetScrapFromShip();
        }

        public static List<string> GetScrapFromShip()
        {
            GrabbableObject[] itemsInShip = HangarShip.GetComponentsInChildren<GrabbableObject>();
            List<string> scrap = new List<string>();

            foreach (var item in itemsInShip)
            {
                if (!IsScrapItem(item)) continue;   
                
                if (!scrap.Contains(item.itemProperties.itemName))
                    scrap.Add(item.itemProperties.itemName);
            }

            return scrap;
        }

        private static bool IsScrapItem(GrabbableObject item)
        {
            if (!item.itemProperties.isScrap) 
                return false;

            if (item.isHeld || item.isPocketed || !item.grabbable) 
                return false;
            
            return true;
        }
    }
}