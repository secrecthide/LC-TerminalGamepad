using BepInEx;
using HarmonyLib;
using static TerminalApi.Events.Events;
using System.Reflection;
using TerminalGamepad.GUILoader;
using UnityEngine;

namespace TerminalGamepad
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency("atomic.terminalapi", MinimumDependencyVersion: "1.5.0")]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", MinimumDependencyVersion: "0.5.5")]
    public class ModBase : BaseUnityPlugin
    {
        private const string GUID = "Secrecthide.TerminalGamepad";
        private const string Name = "TerminalGamepad";
        private const string Version = "1.0.0";

        private readonly Harmony harmony = new Harmony(GUID);
        private static ModBase instance;

        private TerminalGUI myGUI;
        private KeyBinds keybinds = new KeyBinds();

        private void Awake()
        {
            instance = this;
            Logger.LogInfo("TermialGamepad has launched successfully! ENJOY :)");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            TerminalStarting += OnTerminalBeginUsing;
        }

        private void OnTerminalBeginUsing(object sender, TerminalEventArgs e)
        {
            if (!GameObject.Find("TerminalGUI"))
            {
                var gameObject = new UnityEngine.GameObject("TerminalGUI");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
                gameObject.AddComponent<TerminalGUI>();
                myGUI = (TerminalGUI)gameObject.GetComponent("TerminalGUI");
                myGUI.keybinds = keybinds;
            }
        }
    }
}
