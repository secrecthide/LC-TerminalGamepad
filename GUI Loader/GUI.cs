﻿using System;
using System.Data;
using System.Globalization;
using System.Linq;
using UnityEngine;
using TApi = TerminalApi.TerminalApi;
using static TerminalApi.Events.Events;
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace TerminalGamepad.GUILoader
{
    internal class TerminalGUI : MonoBehaviour
    {
        private string[] ButtonNames;
        private static string[] MainButtonNamesTEMP;
        private string[] StoreButtonNames;
        private string[] MoonsButtonNames;
        private string[] BestiaryButtonNames;
        private string[] LogsButtonNames;
        private string[] DecorButtonNames;
        private string[] UpgradeButtonNames;
        private string[] UpgradeButtonNamesTEMP;
        private string[] RadarsName;
        private string[] RadarsNameTEMP;
        private string[] StorageButtonNames;
        private string[] StorageButtonNamesTEMP;
        private string[] PlayersName;
        private string[] PlayersNameTEMP;
        private string[] CodesButtonNames;

        private static string[] MainButtonNames = { "Store", "Moons", "Scan", "Monitors", "Ping", "Flash", "Codes", "Bestiary", "Logs", "Upgrades", "Storage", "Decor"};
        private string[] ConfirmButtonNames = { "Confirm", "Deny", "Info" };
        private string[] InfoButtonNames = { "Continue", "Go Back" };
        private string[] EmptyButtonNames = { "Return" };

        private string tmp;

        private int myCount = 0;
        private int ItemAmount = 1;
        private int tempItemAmount;
        private int Page = 1;
        private int Rows = 2;  // Left and Right
        private int Columns = 8;  // Up and Down
        private int Limit = 16;
        private int ButtonIndex = 16;

        private float MenuX;
        private float MenuY;
        private float MenuWidth;
        private float MenuHeight;
        private float ButtonX;
        private float BetweenSpaceX;  // Space Between MenuX And ButtonX
        private float BetweenSpaceY;  // Space Between MenuY And ButtonY
        private float ButtonY;
        private float ButtonWidth;
        private float ButtonHeight;

        private static GUIStyle boxStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle hoverbuttonStyle;
        private static GUIStyle normalbuttonStyle;
        private static GUIStyle textFieldStyle;

        private bool isUiVisible = false;
        private bool isOnTerminal = false;
        private bool isStylesLoaded = false;
        private bool justHasBeenLoaded = true;

        private bool isOnMainMenu = true;
        private bool isOnStoreMenu = false;
        private bool isOnConfirmMenu = false;
        private bool isOnInfoMenu = false;
        private bool isOnMoonsMenu = false;
        private bool isOnBestiaryMenu = false;
        private bool isOnLogsMenu = false;
        private bool isOnStorageMenu = false;
        private bool isOnFlashMenu = false;
        private bool isOnPingMenu = false;
        private bool isOnMonitorMenu = false;
        private bool isOnCodesMenu = false;
        private bool isOnDecorMenu = false;
        private bool isOnUpgradesMenu = false;

        internal KeyBinds keybinds;
        private Texture2D background;

        private void Start()
        {
            MainButtonNamesTEMP = MainButtonNames;
            ButtonNames = MainButtonNames;
            TerminalEvents();
            UpdateMainButtonNames();
        }

        private void Update()
        {
            if (myCount < ButtonNames.Length)
            {
                LoadButtonsInfo();

                if (isOnTerminal && isUiVisible)
                {
                    if (!TApi.GetTerminalInput().Contains(ButtonNames[myCount].ToLower()))
                    {
                        if (!isOnInfoMenu)
                        {
                            if (isOnStoreMenu && !isOnConfirmMenu && !isOnInfoMenu)
                            {
                                tmp = StoreButtonNames[myCount];
                            }
                            if (isOnMoonsMenu && !isOnConfirmMenu && !isOnInfoMenu)
                            {
                                tmp = MoonsButtonNames[myCount];
                            }
                            if (isOnUpgradesMenu && !isOnConfirmMenu && !isOnInfoMenu)
                            {
                                tmp = UpgradeButtonNames[myCount];
                            }
                            TApi.SetTerminalInput(ButtonNames[myCount].ToLower());
                        }
                        if (ButtonNames[myCount].ToLower() == "logs")
                        {
                            TApi.SetTerminalInput("sigurd");
                        }
                        if (ButtonNames[myCount].ToLower() == "flash")
                            TApi.SetTerminalInput("blind");

                        if (ButtonNames[myCount].ToLower() == "go back")
                        {
                            if (isOnStoreMenu)
                                TApi.SetTerminalInput("store");
                            else if (isOnMoonsMenu)
                                TApi.SetTerminalInput("moons");
                            else if (isOnUpgradesMenu)
                                TApi.SetTerminalInput("upgrades");
                        }
                        if (ButtonNames[myCount].ToLower() == "return")
                        {
                            TApi.SetTerminalInput("help");
                        }
                        if (ButtonNames[myCount].ToLower() == "continue")
                        {
                            if (isOnStoreMenu)
                                TApi.SetTerminalInput($"{tmp} {tempItemAmount}");
                            else if (isOnMoonsMenu || isOnUpgradesMenu)
                                TApi.SetTerminalInput(tmp);

                        }
                    }

                    if (isOnMonitorMenu)
                        if (ButtonNames[myCount].ToLower() == PlayersName[myCount].ToLower())
                        {
                            TApi.SetTerminalInput("switch " + PlayersName[myCount]);
                        }
                    if (isOnMonitorMenu)
                        if (ButtonNames[myCount].ToLower() == "on/of")
                        {
                            TApi.SetTerminalInput("view monitor");
                        }

                    if (isOnStoreMenu && !isOnConfirmMenu && !isOnInfoMenu)
                    {
                        if (TApi.GetTerminalInput() != $"{ButtonNames[myCount].ToLower()} {ItemAmount}")
                        {
                            TApi.SetTerminalInput($"{ButtonNames[myCount].ToLower()} {ItemAmount}");
                        }
                    }
                }
            }
            else
                myCount = ButtonNames.Length - 1;
        }
        private void MakeGUIFitsScreen()
        {
            Rows = ModBase.Rows.Value;
            Columns = ModBase.Columns.Value;

            MenuX = Screen.width * 0.17f;
            MenuY = Screen.height * 0.59f;
            MenuWidth = Screen.width * 0.66f;
            MenuHeight = Screen.height * 0.41f;

            ButtonX = Screen.width * 0.651f;
            ButtonY = Screen.height * 0.388f;

            if (Columns % 2 == 0)
                BetweenSpaceX = Screen.width * 0.005f;
            else
                BetweenSpaceX = Screen.width * 0.0078f;

            BetweenSpaceY = Screen.height * 0.013f;
            ButtonWidth = (Screen.width * 0.643f) / Columns;
            ButtonHeight = (Screen.height * 0.36f) / Rows;
        }
        private void LoadButtonsInfo()
        {
            if (isOnStoreMenu || justHasBeenLoaded)
            {
                StoreButtonNames = new string[TApi.Terminal.buyableItemsList.Length];
                for (int i = 0; i < TApi.Terminal.buyableItemsList.Length; i++)
                {
                    StoreButtonNames[i] = TApi.Terminal.buyableItemsList[i].itemName;
                }
            }

            if (isOnMoonsMenu || justHasBeenLoaded)
            {
                MoonsButtonNames = new string[TApi.Terminal.moonsCatalogueList.Length + 1];
                for (int i = 0; i < MoonsButtonNames.Length; i++)
                {
                    if (i >= MoonsButtonNames.Length - 1)
                        MoonsButtonNames[i] = "Company";
                    else
                        MoonsButtonNames[i] = TApi.Terminal.moonsCatalogueList[i].PlanetName;
                }
            }

            if (isOnUpgradesMenu || justHasBeenLoaded)
            {
                UpgradeButtonNamesTEMP = new string[StartOfRound.Instance.unlockablesList.unlockables.Count];
                for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                {
                    if (StartOfRound.Instance.unlockablesList.unlockables[i].alwaysInStock)
                    {
                        UpgradeButtonNamesTEMP[i] = StartOfRound.Instance.unlockablesList.unlockables[i].unlockableName;
                    }
                    else
                        UpgradeButtonNamesTEMP[i] = "Remove";
                }

                int u = 0;
                int o = 0;
                foreach (string ButtonName in UpgradeButtonNamesTEMP)
                {
                    UpgradeButtonNamesTEMP[u] = "remove";
                    u++;
                    if (ButtonName.ToLower() != "remove")
                    {
                        UpgradeButtonNamesTEMP[o] = ButtonName;
                        o++;
                    }
                }
                UpgradeButtonNames = new string[o];
                for (int i = 0; i < UpgradeButtonNamesTEMP.Length; i++)
                {
                    if (UpgradeButtonNamesTEMP[i].ToLower() != "remove")
                    {
                        UpgradeButtonNames[i] = UpgradeButtonNamesTEMP[i];
                    }
                }
            }

            if (isOnStorageMenu || justHasBeenLoaded)
            {
                StorageButtonNamesTEMP = new string[StartOfRound.Instance.unlockablesList.unlockables.Count];
                for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
            {
                if (StartOfRound.Instance.unlockablesList.unlockables[i].inStorage)
                    StorageButtonNamesTEMP[i] = StartOfRound.Instance.unlockablesList.unlockables[i].unlockableName;
                else
                    StorageButtonNamesTEMP[i] = "Remove";
            }
                
                int j = 0;
                int a = 0;
                foreach (string ButtonName in StorageButtonNamesTEMP)
            {
                StorageButtonNamesTEMP[a] = "remove";
                a++;
                if (ButtonName.ToLower() != "remove")
                {
                    StorageButtonNamesTEMP[j] = ButtonName;
                    j++;
                }
            }
                StorageButtonNames = new string[j];
                for (int i = 0; i < StorageButtonNamesTEMP.Length; i++)
            {
                if (StorageButtonNamesTEMP[i].ToLower() != "remove")
                {
                    StorageButtonNames[i] = StorageButtonNamesTEMP[i];
                }
            }
            }

            if (isOnBestiaryMenu || justHasBeenLoaded)
            { 
                BestiaryButtonNames = new string[TApi.Terminal.scannedEnemyIDs.Count];
                for (int i = 0; i < TApi.Terminal.scannedEnemyIDs.Count; i++)
            {
                if (TApi.Terminal.scannedEnemyIDs.Count <= 0)
                {
                    break;
                }
                BestiaryButtonNames[i] = TApi.Terminal.enemyFiles[TApi.Terminal.scannedEnemyIDs[i]].creatureName;
            }
            }

            if (isOnLogsMenu || justHasBeenLoaded)
            {
                LogsButtonNames = new string[TApi.Terminal.unlockedStoryLogs.Count];
                for (int i = 0; i < TApi.Terminal.unlockedStoryLogs.Count; i++)
            {
                if (TApi.Terminal.unlockedStoryLogs.Count <= 0)
                    break;
                LogsButtonNames[i] = TApi.Terminal.logEntryFiles[TApi.Terminal.unlockedStoryLogs[i]].creatureName;
            }
            }

            if (isOnDecorMenu || justHasBeenLoaded)
            {
                DecorButtonNames = new string[TApi.Terminal.ShipDecorSelection.Count];
                for (int i = 0; i < TApi.Terminal.ShipDecorSelection.Count; i++)
                {
                    if (DecorButtonNames[i] != TApi.Terminal.ShipDecorSelection[i].creatureName)
                        DecorButtonNames[i] = TApi.Terminal.ShipDecorSelection[i].creatureName;
                }
            }

            if (isOnMonitorMenu || justHasBeenLoaded)
            {
                PlayersNameTEMP = new string[StartOfRound.Instance.allPlayerScripts.Length];
                for (int i = 0; i < PlayersNameTEMP.Length; i++)
                {
                    if (StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled)
                        PlayersNameTEMP[i] = StartOfRound.Instance.allPlayerScripts[i].playerUsername;
                    else
                        PlayersNameTEMP[i] = "Remove";
                }
                
                int c = 0;
                int z = 0;
                foreach (string ButtonName in PlayersNameTEMP)
                {
                    PlayersNameTEMP[c] = "remove";
                    c++;
                    if (ButtonName.ToLower() != "remove")
                    {
                        PlayersNameTEMP[z] = ButtonName;
                        z++;
                    }
                }
                PlayersName = new string[z + 1];
                for (int i = 0; i < PlayersName.Length; i++)
                {
                    if (i == 0)
                        PlayersName[i] = "On/Of";
                    else
                        PlayersName[i] = PlayersNameTEMP[i - 1];                
                }
            }

            if (isOnFlashMenu || isOnPingMenu || justHasBeenLoaded)
            {
                RadarsNameTEMP = new string[StartOfRound.Instance.mapScreen.radarTargets.Count];
                for (int i = 0; i < StartOfRound.Instance.mapScreen.radarTargets.Count; i++)
                {
                    if (StartOfRound.Instance.mapScreen.radarTargets[i].isNonPlayer)
                    {
                        RadarsNameTEMP[i] = StartOfRound.Instance.mapScreen.radarTargets[i].name;
                    }
                    else
                        RadarsNameTEMP[i] = "Remove";
                }

                int q = 0;
                int e = 0;
                foreach (string ButtonName in RadarsNameTEMP)
                {
                    RadarsNameTEMP[q] = "remove";
                    q++;
                    if (ButtonName.ToLower() != "remove")
                    {
                        RadarsNameTEMP[e] = ButtonName;
                        e++;
                    }
                }
                RadarsName = new string[e];
                for (int i = 0; i < RadarsNameTEMP.Length; i++)
                {
                    if (RadarsNameTEMP[i].ToLower() != "remove")
                    {
                        RadarsName[i] = RadarsNameTEMP[i];
                    }
                }
            }

            if (StartOfRound.Instance.shipHasLanded && CodesButtonNames.Length == 0)
            {
                TerminalAccessibleObject[] Codes = UnityEngine.Object.FindObjectsOfType<TerminalAccessibleObject>();
                if (Codes != null)
                {
                    CodesButtonNames = new string[Codes.Length];
                    for (int i = 0; i < Codes.Length; i++)
                    {
                        CodesButtonNames[i] = Codes[i].objectCode;
                    }
                }
            }
            else if (!StartOfRound.Instance.shipHasLanded)
                CodesButtonNames = new string[0];

            if (justHasBeenLoaded)
                justHasBeenLoaded = false;
        }

        public static void UpdateTextColor()
        {
            textFieldStyle.normal.textColor = ReadColor(ModBase.AmountButtonTextColor.Value);
            boxStyle.normal.textColor = ReadColor(ModBase.BoxTextColor.Value);
            normalbuttonStyle.normal.textColor = ReadColor(ModBase.ButtonTextColor.Value);
            hoverbuttonStyle.normal.textColor = ReadColor(ModBase.HighlightedButtonTextColor.Value);
        }
        public static void UpdateMainButtonNames()
        {
            string[] tmp1 = MainButtonNamesTEMP;
            string[] tmp2 = ReadText(ModBase.CustomCommads.Value);
            MainButtonNames = new string[tmp1.Length + tmp2.Length];

            for (int i = 0; i < tmp1.Length; i++)
            {
                MainButtonNames[i] = tmp1[i];
            }
            for (int i = 0; i < tmp2.Length; i++)
            {
                MainButtonNames[i + tmp1.Length] = tmp2[i];
            }
        }
        private static Color32 ReadColor(string ColorText)
        {
            var spiltColor = ColorText.Split(new char[] {',',' ', '"'}, StringSplitOptions.RemoveEmptyEntries).Select(x => byte.Parse(x.Trim(), CultureInfo.InvariantCulture)).ToArray();
            return new Color32(spiltColor[0], spiltColor[1], spiltColor[2], spiltColor[3]);
        }
        private static string[] ReadText(string Text)
        {
            var spiltText = Text.Split(new char[] { ',', '"'}, StringSplitOptions.RemoveEmptyEntries);
            return spiltText;
        }

        private void OnGUI()
        {
            if (!isStylesLoaded)
                LoadStyles();

            if (isUiVisible && isOnTerminal)
            {
                GUI.backgroundColor = ReadColor(ModBase.BoxBackgroundColor.Value);
                GUI.Box(new Rect(MenuX, MenuY, MenuWidth, MenuHeight), "TerminalGamepad", boxStyle);

                if (isOnStoreMenu && !isOnConfirmMenu)
                {
                    GUI.backgroundColor = ReadColor(ModBase.AmountBoxbackgroundColor.Value);
                    GUI.TextField(new Rect(MenuX + (Screen.width * 0.006f), MenuY - 20, 65, 20), "Amount: " + ItemAmount, textFieldStyle);
                }

                //drawing menus
                if (isOnMainMenu)
                    ButtonNames = MainButtonNames;

                if (isOnStoreMenu && !isOnConfirmMenu && !isOnInfoMenu)
                    ButtonNames = StoreButtonNames;

                if (isOnMoonsMenu && !isOnConfirmMenu && !isOnInfoMenu)
                    ButtonNames = MoonsButtonNames;

                if (isOnUpgradesMenu && !isOnConfirmMenu && !isOnInfoMenu)
                    ButtonNames = UpgradeButtonNames;

                if (isOnBestiaryMenu)
                    ButtonNames = BestiaryButtonNames;

                if (isOnLogsMenu)
                    ButtonNames = LogsButtonNames;

                if (isOnStorageMenu)
                    ButtonNames = StorageButtonNames;

                if (isOnFlashMenu || isOnPingMenu)
                    ButtonNames = RadarsName;

                if (isOnMonitorMenu)
                    ButtonNames = PlayersName;

                if (isOnDecorMenu && !isOnConfirmMenu)
                    ButtonNames = DecorButtonNames;

                if (isOnCodesMenu)
                    ButtonNames = CodesButtonNames;

                if (isOnConfirmMenu)
                    ButtonNames = ConfirmButtonNames;

                if (isOnInfoMenu)
                    ButtonNames = InfoButtonNames;

                if (ButtonNames.Length <= 0 || ButtonNames == null)
                {
                    ButtonNames = EmptyButtonNames;
                }
                DrawButtons();
            }
        }
        private void DrawButtons()
        {
            MakeGUIFitsScreen();
            Limit = Rows * Columns;
            int CurrentDrawingRow = 1;

            if (myCount > (Limit * Page) - 1)
            {
                Page++;
            }
            else if (myCount < (Limit * (Page - 1)))
            {
                Page--;
            }
            for (int i = 0; i < ButtonNames.Length; i++)
            {
                ButtonIndex = i - (Limit * (Page - 1));
                CurrentDrawingRow = ButtonIndex / Columns;
                if (i == myCount)
                {
                    GUI.backgroundColor = ReadColor(ModBase.HighlightedButtonColor.Value);
                    buttonStyle = hoverbuttonStyle;
                }
                else
                {
                    GUI.backgroundColor = ReadColor(ModBase.ButtonsColor.Value);
                    buttonStyle = normalbuttonStyle;
                }
                if (ButtonIndex >= 0)
                {
                    if (ButtonIndex < Limit)//                                                                                                                        between space
                        GUI.Button(new Rect(MenuX + BetweenSpaceX + (ButtonIndex - (Columns * CurrentDrawingRow)) * (ButtonX / Columns), MenuY + (MenuHeight / (Rows + BetweenSpaceY)) + (CurrentDrawingRow * (ButtonY / Rows)), ButtonWidth, ButtonHeight), ButtonNames[i], buttonStyle);
                }
            }
        }

        private void LoadStyles()
        {
            isStylesLoaded = true;
            MakeTex(2, 2, Color.white);

            boxStyle = new GUIStyle(GUI.skin.box);
            hoverbuttonStyle = new GUIStyle(GUI.skin.button);
            normalbuttonStyle = new GUIStyle(GUI.skin.button);
            textFieldStyle = new GUIStyle(GUI.skin.button);

            hoverbuttonStyle.normal.textColor = ReadColor(ModBase.HighlightedButtonTextColor.Value);
            hoverbuttonStyle.normal.background = background;
            hoverbuttonStyle.fontSize = 16;
            hoverbuttonStyle.fontStyle = FontStyle.Bold;
            hoverbuttonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            normalbuttonStyle.normal.textColor = ReadColor(ModBase.ButtonTextColor.Value);
            normalbuttonStyle.normal.background = background;
            normalbuttonStyle.fontSize = 14;
            normalbuttonStyle.fontStyle = FontStyle.Normal;
            normalbuttonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            boxStyle.normal.textColor = ReadColor(ModBase.BoxTextColor.Value);
            boxStyle.normal.background = background;
            boxStyle.fontSize = 10;
            boxStyle.fontStyle = FontStyle.Normal;
            boxStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            textFieldStyle.normal.textColor = ReadColor(ModBase.AmountButtonTextColor.Value);
            textFieldStyle.normal.background = background;
            textFieldStyle.fontSize = 9;
            textFieldStyle.fontStyle = FontStyle.Bold;
            textFieldStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;
        }
        private void MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            background = new Texture2D(width, height);
            background.SetPixels(pix);
            background.Apply();
        }
        private void MenuBool(bool Main, bool Store, bool Confirm, bool Info, bool Moons, bool Bestiary, bool UNUSED, bool Logs, bool Storage, bool Flash, bool Pings, bool Monitors, bool Codes, bool Decor, bool Upgrades)
        {
            isOnMainMenu = Main;
            isOnStoreMenu = Store;
            isOnConfirmMenu = Confirm;
            isOnInfoMenu = Info;
            isOnMoonsMenu = Moons;
            isOnBestiaryMenu = Bestiary;
            isOnLogsMenu = Logs;
            isOnStorageMenu = Storage;
            isOnFlashMenu = Flash;
            isOnPingMenu = Pings;
            isOnMonitorMenu = Monitors;
            isOnCodesMenu = Codes;
            isOnDecorMenu = Decor;
            isOnUpgradesMenu = Upgrades;
        }

        private void TerminalEvents()
        {
            TerminalBeginUsing += OnTerminalBeginUsing;
            TerminalExited += OnTerminalExited;
        }
        private void OnTerminalBeginUsing(object sender, TerminalEventArgs e)
        {
            keybinds.ShowKeyboardKey.Enable();
            keybinds.ShowKeyboardKey.performed += ShowHideKeyboard;

            keybinds.LeftArrow.Enable();
            keybinds.LeftArrow.performed += GoLeft;

            keybinds.RightArrow.Enable();
            keybinds.RightArrow.performed += GoRight;

            keybinds.UpArrow.Enable();
            keybinds.UpArrow.performed += GoUp;

            keybinds.DownArrow.Enable();
            keybinds.DownArrow.performed += GoDown;

            keybinds.Subbmit.Enable();
            keybinds.Subbmit.performed += Subbmit;

            keybinds.PlusAmount.Enable();
            keybinds.PlusAmount.performed += PlusAmount;

            keybinds.MinusAmount.Enable();
            keybinds.MinusAmount.performed += MinusAmount;

            keybinds.Return.Enable();
            keybinds.Return.performed += Return;

            isOnTerminal = true;
        }
        private void OnTerminalExited(object sender, TerminalEventArgs e)
        {
            keybinds.ShowKeyboardKey.Disable();
            keybinds.ShowKeyboardKey.performed -= ShowHideKeyboard;

            keybinds.LeftArrow.Disable();
            keybinds.LeftArrow.performed -= GoLeft;

            keybinds.RightArrow.Disable();
            keybinds.RightArrow.performed -= GoRight;

            keybinds.UpArrow.Disable();
            keybinds.UpArrow.performed -= GoUp;

            keybinds.DownArrow.Disable();
            keybinds.DownArrow.performed -= GoDown;

            keybinds.Subbmit.Disable();
            keybinds.Subbmit.performed -= Subbmit;

            keybinds.PlusAmount.Disable();
            keybinds.PlusAmount.performed -= PlusAmount;

            keybinds.MinusAmount.Disable();
            keybinds.MinusAmount.performed -= MinusAmount;

            keybinds.Return.Disable();
            keybinds.Return.performed -= Return;

            isOnTerminal = false;
            myCount = 0;
            ItemAmount = 1;
            Page = 1;

            MenuBool(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
        }

        private void ShowHideKeyboard(InputAction.CallbackContext context)
        {
            isUiVisible = !isUiVisible;
        }
        private void Subbmit(InputAction.CallbackContext context)
        {
            if (!isOnMonitorMenu && !isOnPingMenu && !isOnFlashMenu)
            {
                myCount = 0;
                Page = 1;
            }

            if (myCount < ButtonNames.Length)
            {
                if (isOnTerminal)
                {
                    if (TApi.GetTerminalInput().ToLower() == "help")
                    {
                        MenuBool(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
                        TApi.Terminal.OnSubmit();
                    }
                    if (ButtonNames != EmptyButtonNames)
                    {
                        if (TApi.GetTerminalInput().ToLower() == "store")
                        {
                            MenuBool(false, true, false, false, false, false, false, false, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "moons")
                        {
                            MenuBool(false, false, false, false, true, false, false, false, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "bestiary")
                        {
                            MenuBool(false, false, false, false, false, true, false, false, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "sigurd")
                        {
                            MenuBool(false, false, false, false, false, false, false, true, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "storage")
                        {
                            MenuBool(false, false, false, false, false, false, false, true, true, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "blind")
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, true, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "ping")
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, true, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "monitors")
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "codes")
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, true, true, false, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "decor")
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, true, false, true, false);
                        }
                        if (TApi.GetTerminalInput().ToLower() == "upgrades")
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                        }

                        if (isOnStoreMenu)
                            if (ButtonNames[myCount].ToLower() == StoreButtonNames[myCount].ToLower())
                            {
                                tempItemAmount = ItemAmount;
                                MenuBool(false, true, true, false, false, false, false, false, false, false, false, false, false, false, false);
                            }

                        if (isOnDecorMenu)
                            if (ButtonNames[myCount].ToLower() == DecorButtonNames[myCount].ToLower())
                            {
                                MenuBool(false, false, true, false, false, false, false, false, false, false, false, false, false, true, false);
                            }

                        if (isOnMoonsMenu)
                            if (ButtonNames[myCount].ToLower() == MoonsButtonNames[myCount].ToLower())
                            {
                                MenuBool(false, false, true, false, true, false, false, false, false, false, false, false, false, false, false);
                            }

                        if (isOnUpgradesMenu)
                            if (ButtonNames[myCount].ToLower() == UpgradeButtonNames[myCount].ToLower())
                            {
                                MenuBool(false, false, true, false, false, false, false, false, false, false, false, false, false, false, true);
                            }

                        if (isOnBestiaryMenu && BestiaryButtonNames.Length >= 1)
                            if (ButtonNames[myCount].ToLower() == BestiaryButtonNames[myCount].ToLower())
                            {
                                MenuBool(false, false, false, false, false, true, true, false, false, false, false, false, false, false, false);
                            }

                        if (isOnLogsMenu)
                            if (ButtonNames[myCount].ToLower() == LogsButtonNames[myCount].ToLower())
                            {
                                MenuBool(false, false, false, false, false, false, true, true, false, false, false, false, false, false, false);
                            }

                        if (isOnConfirmMenu)
                            if (TApi.GetTerminalInput().ToLower() == "info")
                            {
                                if (!isOnDecorMenu)
                                    MenuBool(false, isOnStoreMenu, false, true, isOnMoonsMenu, false, false, false, false, false, false, false, false, false, isOnUpgradesMenu);
                            }

                        if (isOnConfirmMenu)
                            if (TApi.GetTerminalInput().ToLower() == "deny" || TApi.GetTerminalInput().ToLower() == "confirm")
                            {
                                MenuBool(false, isOnStoreMenu, false, false, isOnMoonsMenu, false, false, false, false, false, false, false, false, isOnDecorMenu, isOnUpgradesMenu);
                            }

                        if (isOnInfoMenu)
                            if (TApi.GetTerminalInput().ToLower() == "info")
                            {
                                if (isOnDecorMenu)
                                {
                                    MenuBool(false, false, false, false, false, false, false, false, false, false, false, false, false, true, false);
                                    TApi.SetTerminalInput("decor");
                                }
                                else
                                {
                                    TApi.SetTerminalInput("deny");
                                    TApi.Terminal.OnSubmit();
                                    TApi.SetTerminalInput(tmp + " info");
                                }
                            }

                        if (isOnFlashMenu)
                            if (ButtonNames[myCount].ToLower() == RadarsName[myCount].ToLower())
                            {
                                TApi.SetTerminalInput("flash " + RadarsName[myCount]);
                            }

                        if (isOnPingMenu)
                            if (ButtonNames[myCount].ToLower() == RadarsName[myCount].ToLower())
                            {
                                TApi.SetTerminalInput("ping " + RadarsName[myCount]);
                            }

                        if (isOnInfoMenu)
                        {
                            if (ButtonNames[myCount].ToLower() == "go back")
                            {
                                MenuBool(false, isOnStoreMenu, false, false, isOnMoonsMenu, false, false, false, false, false, false, false, false, false, false);
                            }
                            else if (ButtonNames[myCount].ToLower() == "continue")
                            {
                                TApi.Terminal.OnSubmit();
                                TApi.SetTerminalInput("confirm");
                                MenuBool(false, isOnStoreMenu, false, false, isOnMoonsMenu, false, false, false, false, false, false, false, false, false, isOnUpgradesMenu);
                            }
                        }

                        ItemAmount = 1;
                        TApi.Terminal.OnSubmit();
                    }
                    else
                        myCount = 0;
                }
            }
        }
        private void Return(InputAction.CallbackContext context)
        {
            if (!isOnMainMenu)
            {
                if (ButtonNames == EmptyButtonNames)
                {
                    MenuBool(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
                    TApi.SetTerminalInput("help");
                }

                if ((isOnStoreMenu && !isOnConfirmMenu && !isOnInfoMenu) || (isOnMoonsMenu && !isOnConfirmMenu && !isOnInfoMenu) || isOnBestiaryMenu || isOnLogsMenu || isOnStorageMenu || isOnFlashMenu || isOnPingMenu || isOnMonitorMenu || isOnCodesMenu || (isOnDecorMenu && !isOnConfirmMenu) || (isOnUpgradesMenu && !isOnConfirmMenu && !isOnInfoMenu))
                {
                    MenuBool(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
                    TApi.SetTerminalInput("help");
                }
                if (isOnConfirmMenu)
                {
                    MenuBool(false, isOnStoreMenu, false, false, isOnMoonsMenu, false, false, false, false, false, false, false, false, isOnDecorMenu, isOnUpgradesMenu);
                    TApi.SetTerminalInput("deny");
                    TApi.Terminal.OnSubmit();
                    if (isOnStoreMenu)
                        TApi.SetTerminalInput("store");
                    else if (isOnMoonsMenu)
                        TApi.SetTerminalInput("moons");
                    else if (isOnDecorMenu)
                        TApi.SetTerminalInput("decor");
                    else if (isOnUpgradesMenu)
                        TApi.SetTerminalInput("upgrades");
                }
                if (isOnInfoMenu)
                {
                    MenuBool(false, isOnStoreMenu, false, false, isOnMoonsMenu, false, false, false, false, false, false, false, false, isOnDecorMenu, isOnUpgradesMenu);
                    if (isOnStoreMenu)
                        TApi.SetTerminalInput("store");
                    else if (isOnMoonsMenu)
                        TApi.SetTerminalInput("moons");
                    else if (isOnUpgradesMenu)
                        TApi.SetTerminalInput("upgrades");
                }
                TApi.Terminal.OnSubmit();
            }
            if (!isOnMonitorMenu && !isOnPingMenu && !isOnFlashMenu)
            {
                Page = 1;
                myCount = 0;
            }
        }

        private void GoDown(InputAction.CallbackContext context)
        {
            myCount += Columns;
            if (myCount > ButtonNames.Length - 1)
                myCount -= Columns;
        }
        private void GoUp(InputAction.CallbackContext context)
        {
            myCount -= Columns;
            if (myCount < 0)
                myCount += Columns;
        }
        private void GoRight(InputAction.CallbackContext context)
        {
            if (isUiVisible)
            {
                myCount += 1;
                if (myCount > ButtonNames.Length - 1)
                    myCount -= 1;
            }
        }
        private void GoLeft(InputAction.CallbackContext context)
        {
            if (isUiVisible)
            {
                myCount -= 1;
                if (myCount < 0)
                    myCount += 1;
            }
        }

        private void PlusAmount(InputAction.CallbackContext context)
        {
            if (isOnTerminal && isOnStoreMenu && isUiVisible && !isOnConfirmMenu)
            {
                ItemAmount++;
                if (ItemAmount > 12)
                    ItemAmount = 1;
            }
        }
        private void MinusAmount(InputAction.CallbackContext context)
        {
            if (isOnTerminal && isOnStoreMenu && isUiVisible && !isOnConfirmMenu)
            {
                ItemAmount--;
                if (ItemAmount < 1)
                    ItemAmount = 12;
            }
        }
    }
    public class KeyBinds : LcInputActions
    {
        [InputAction("<Keyboard>/leftCtrl", Name = "ShowKeyboard", GamepadPath = "<Gamepad>/buttonNorth")]
        public InputAction ShowKeyboardKey { get; set; }

        [InputAction("<Keyboard>/enter", Name = "Subbmit", GamepadPath = "<Gamepad>/buttonSouth")]
        public InputAction Subbmit { get; set; }

        [InputAction("<Keyboard>/alt", Name = "Return", GamepadPath = "<Gamepad>/buttonEast")]
        public InputAction Return { get; set; }

        [InputAction("<Keyboard>/e", Name = "PlusAmount", GamepadPath = "<Gamepad>/rightShoulder")]
        public InputAction PlusAmount { get; set; }

        [InputAction("<Keyboard>/q", Name = "MinusAmount", GamepadPath = "<Gamepad>/leftShoulder")]
        public InputAction MinusAmount { get; set; }

        [InputAction("<Keyboard>/leftArrow", Name = "leftArrow", GamepadPath = "<Gamepad>/dpad/left")]
        public InputAction LeftArrow { get; set; }

        [InputAction("<Keyboard>/rightArrow", Name = "rightArrow", GamepadPath = "<Gamepad>/dpad/right")]
        public InputAction RightArrow { get; set; }

        [InputAction("<Keyboard>/upArrow", Name = "upArrow", GamepadPath = "<Gamepad>/dpad/up")]
        public InputAction UpArrow { get; set; }

        [InputAction("<Keyboard>/downArrow", Name = "downArrow", GamepadPath = "<Gamepad>/dpad/down")]
        public InputAction DownArrow { get; set; }
    }
}
