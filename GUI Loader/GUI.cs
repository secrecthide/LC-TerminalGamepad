using System;
using System.Data;
using System.Globalization;
using System.Linq;
using UnityEngine;
using TApi = TerminalApi.TerminalApi;
using static TerminalApi.Events.Events;
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using BepInEx.Logging;
using System.Collections;
using TerminalGamepad.Data;

namespace TerminalGamepad.GUILoader
{
    internal class TerminalGUI : MonoBehaviour
    {
        private string[] ButtonNames;
        public static string[] MainButtonNamesTEMP;
        private string[] StoreButtonNames;
        public static string[] MoonsButtonNames;
        private string[] BestiaryButtonNames;
        private string[] LogsButtonNames;
        private string[] DecorButtonNames;
        private string[] UpgradeButtonNames;
        private string[] RadarsName;
        private string[] StorageButtonNames;
        private string[] PlayersName;
        private string[] CodesButtonNames;

        public static string[] MainButtonNames = { "Store", "Moons", "Scan", "Monitors", "Ping", "Flash", "Transmit", "Codes", "Bestiary", "Logs", "Upgrades", "Storage", "Decor" };
        private string[] ConfirmButtonNames = { "Confirm", "Deny", "Info" };
        private string[] InfoButtonNames = { "Continue", "Go Back" };
        private string[] EmptyButtonNames = { "Return" };
        private string[] Keyboard = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
                                      "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
                                      "A", "S", "D", "F", "G", "H", "J", "K", "L", "SPACE",
                                      "Backspace", "Z", "X", "C", "V", "B", "N", "M", "!", "Submit"};

        private string tmp;

        private int myCount = 0;
        private int myCountInedx = 0;
        private int ItemAmount = 1;
        private int tempItemAmount;
        private int Page = 1;
        private int totalOfPages = 1;
        private int Rows = 2;  // Left and Right
        private int currentRow = 1;
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
        private bool isStylesLoaded = false;
        private bool justHasBeenLoaded = true;
        private bool boxVisible = true;
        public static bool leftRight = false;
        public static bool upDown = false;
        public static bool dynamic = false;

        public static Pages currentPage;
        public static SubPages currentSubPage;

        internal KeyBinds keybinds;
        private ManualLogSource mls;

        private void Start()
        {
            MainButtonNamesTEMP = MainButtonNames;
            ButtonNames = MainButtonNames;
            mls = BepInEx.Logging.Logger.CreateLogSource("TerminalGamepad");

            LGU.AddLguButton();
            TerminalEvents();
            UpdateMainButtonNames();
            UpdatePagesMode();
            DrawBoxForOneSecond();

        }

        private void Update()
        {
            try { if (ButtonNames[myCount] != null){ } }
            catch (IndexOutOfRangeException) { myCount = ButtonNames.Length - 1; myCountInedx = myCount - (Limit * (Page - 1)); }

            if (StartOfRound.Instance.localPlayerController.inTerminalMenu && isUiVisible)
            {
                LoadButtonsInfo();

                for (int i = 1; i <= Rows; i++)
                {
                    if (myCountInedx <= Columns * i && myCountInedx >= Columns * (i - 1))
                    {
                        currentRow = i; 
                    }
                }
                if (TApi.GetTerminalInput().ToLower() != ButtonNames[myCount].ToLower())
                {
                    if (currentSubPage != SubPages.Info && currentPage != Pages.Bruteforce && currentPage != Pages.Transmit && currentPage != Pages.Lookup)
                    {
                        if (currentPage == Pages.Store && currentSubPage == SubPages.None)
                        {
                            tmp = StoreButtonNames[myCount];
                        }
                        if (currentPage == Pages.Moons && currentSubPage == SubPages.None)
                        {
                            tmp = MoonsButtonNames[myCount];
                        }
                        if (currentPage == Pages.Upgrades && currentSubPage == SubPages.None && UpgradeButtonNames.Length > 0)
                        {
                            tmp = UpgradeButtonNames[myCount];
                        }
                        if (currentPage == Pages.moreUpgrades && currentSubPage == SubPages.None)
                        {
                            tmp = LGU.MoreUpgradesButtonsName[myCount];
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
                        if (currentPage == Pages.Store)
                            TApi.SetTerminalInput("store");
                        else if (currentPage == Pages.Moons)
                            TApi.SetTerminalInput("moons");
                        else if (currentPage == Pages.Upgrades)
                            TApi.SetTerminalInput("upgrades");
                    }

                    if (ButtonNames[myCount].ToLower() == "return")
                    {
                        TApi.SetTerminalInput("help");
                    }

                    if (currentPage == Pages.moreUpgrades && currentSubPage == SubPages.None)
                    {
                        TApi.SetTerminalInput(LGU.MoreUpgradesButtonsName[myCount] + " info");
                    }

                    if (ButtonNames[myCount].ToLower() == "continue")
                    {
                        if (currentPage == Pages.Store)
                            TApi.SetTerminalInput($"{tmp} {tempItemAmount}");
                        else if (currentPage == Pages.Moons || currentPage == Pages.Upgrades)
                            TApi.SetTerminalInput(tmp);
                    }
                    if (currentSubPage == SubPages.LGUConfirm)
                    {
                        if (currentPage == Pages.moreUpgrades)
                        {
                            if (ButtonNames[myCount].ToLower() == "confirm")
                            {
                                TApi.SetTerminalInput(tmp);
                            }
                            else if (ButtonNames[myCount].ToLower() == "deny")
                            {
                                TApi.SetTerminalInput("Lategame Store");
                            }
                        }
                    }

                    if (currentPage == Pages.Monitor)
                    {
                        if (ButtonNames[myCount].ToLower() == "on/off")
                        {
                            TApi.SetTerminalInput("view monitor");
                        }
                        else
                        {
                            TApi.SetTerminalInput("switch " + PlayersName[myCount]);
                        }
                    }

                    if (currentPage == Pages.Demon)
                    {
                        TApi.SetTerminalInput("demon " + LGU.DemonsButtonNames[myCount]);
                    }
                }

                if (currentPage == Pages.Store && currentSubPage == SubPages.None)
                {
                    if (TApi.GetTerminalInput() != $"{ButtonNames[myCount].ToLower()} {ItemAmount}")
                    {
                        TApi.SetTerminalInput($"{ButtonNames[myCount].ToLower()} {ItemAmount}");
                    }
                }

                if (currentPage == Pages.Contract && currentSubPage == SubPages.None)
                {
                    if (ButtonNames[myCount].ToLower() == "random")
                        TApi.SetTerminalInput("Contract");
                    else if (ButtonNames[myCount].ToLower() == "41 experimentation")
                        TApi.SetTerminalInput("contract experimentation");
                    else
                        TApi.SetTerminalInput("contract " + LGU.ContractButtonNames[myCount]);
                }
            }
            
        }
        private void MakeGUIFitsScreen()
        {
            Rows = ModBase.Rows.Value;
            Columns = ModBase.Columns.Value;

            if (dynamic)
            {
                if (Rows > Columns)
                {
                    leftRight = false;
                    upDown = true;
                }
                else if (Rows < Columns)
                {
                    leftRight = true;
                    upDown = false;
                }
                else
                {
                    leftRight = true;
                    upDown = true;
                }
            }

            MenuX = Screen.width * 0.17f;
            MenuY = Screen.height * 0.59f;
            MenuWidth = Screen.width * 0.66f;
            MenuHeight = Screen.height * 0.41f;

            ButtonX = Screen.width * 0.651f;
            ButtonY = Screen.height * 0.388f;

            if (Columns == 1)
                BetweenSpaceX = Screen.width * 0.0135f;
            else if (Columns % 2 == 0)
                BetweenSpaceX = Screen.width * 0.005f;
            else
                BetweenSpaceX = Screen.width * 0.0078f;

            BetweenSpaceY = Screen.height * 0.013f;
            ButtonWidth = (Screen.width * 0.63f) / Columns;
            ButtonHeight = (Screen.height * 0.36f) / Rows;
        }
        private void LoadButtonsInfo()
        {
            if (currentPage == Pages.Store || justHasBeenLoaded)
            {
                StoreButtonNames = TApi.Terminal.buyableItemsList.Select(item => item.itemName).ToArray();
            }

            if (currentPage == Pages.Moons || justHasBeenLoaded)
            {
                MoonsButtonNames = new string[TApi.Terminal.moonsCatalogueList.Length + 1];

                for (int i = 0; i < TApi.Terminal.moonsCatalogueList.Length; i++)
                {
                    MoonsButtonNames[i] = TApi.Terminal.moonsCatalogueList[i].PlanetName;
                }

                MoonsButtonNames[MoonsButtonNames.Length - 1] = "Company";
            }

            if (currentPage == Pages.Upgrades || justHasBeenLoaded)
            {
                List<string> upgradeButtonNamesTemp = new List<string>();

                foreach (var unlockable in StartOfRound.Instance.unlockablesList.unlockables)
                {
                    if (unlockable.alwaysInStock && !unlockable.alreadyUnlocked && !unlockable.hasBeenUnlockedByPlayer)
                    {
                        upgradeButtonNamesTemp.Add(unlockable.unlockableName);
                    }
                }
                UpgradeButtonNames = upgradeButtonNamesTemp.ToArray();
            }

            if (currentPage == Pages.Storage || justHasBeenLoaded)
            {
                List<string> storageButtonNamesTemp = new List<string>();

                foreach (var unlockable in StartOfRound.Instance.unlockablesList.unlockables)
                {
                    if (unlockable.inStorage)
                    {
                        storageButtonNamesTemp.Add(unlockable.unlockableName);
                    }
                }

                StorageButtonNames = storageButtonNamesTemp.ToArray();
            }

            if (currentPage == Pages.Bestiary || justHasBeenLoaded)
            {
                if (TApi.Terminal.scannedEnemyIDs.Count <= 0)
                {
                    BestiaryButtonNames = new string[0];
                }
                else
                {
                    BestiaryButtonNames = new string[TApi.Terminal.scannedEnemyIDs.Count];
                    for (int i = 0; i < TApi.Terminal.scannedEnemyIDs.Count; i++)
                    {
                        BestiaryButtonNames[i] = TApi.Terminal.enemyFiles[TApi.Terminal.scannedEnemyIDs[i]].creatureName;
                    }
                }
            }

            if (currentPage == Pages.Logs || justHasBeenLoaded)
            {
                if (TApi.Terminal.unlockedStoryLogs.Count <= 0)
                {
                    LogsButtonNames = new string[0];
                }
                else
                {
                    LogsButtonNames = new string[TApi.Terminal.unlockedStoryLogs.Count];
                    for (int i = 0; i < TApi.Terminal.unlockedStoryLogs.Count; i++)
                    {
                        LogsButtonNames[i] = TApi.Terminal.logEntryFiles[TApi.Terminal.unlockedStoryLogs[i]].creatureName;
                    }
                }
            }

            if (currentPage == Pages.Decor || justHasBeenLoaded)
            {
                DecorButtonNames = new string[TApi.Terminal.ShipDecorSelection.Count];
                for (int i = 0; i < DecorButtonNames.Length; i++)
                {
                    DecorButtonNames[i] = TApi.Terminal.ShipDecorSelection[i].creatureName;
                }
            }

            if (currentPage == Pages.Flash || currentPage == Pages.Ping || justHasBeenLoaded)
            {
                var radarsNameTemp = new List<string>();

                foreach (var radarTarget in StartOfRound.Instance.mapScreen.radarTargets)
                {
                    if (radarTarget.isNonPlayer)
                    {
                        radarsNameTemp.Add(radarTarget.name);
                    }
                }

                RadarsName = radarsNameTemp.ToArray();
            }

            if (currentPage == Pages.Monitor || justHasBeenLoaded)
            {
                var playersNameTemp = new List<string>();

                foreach (var playerScript in StartOfRound.Instance.allPlayerScripts)
                {
                    if (playerScript.isPlayerControlled)
                    {
                        playersNameTemp.Add(playerScript.playerUsername);
                    }
                }
                foreach (var radarTarget in StartOfRound.Instance.mapScreen.radarTargets)
                {
                    if (radarTarget.isNonPlayer)
                    {
                        playersNameTemp.Add(radarTarget.name);
                    }
                }

                PlayersName = new string[playersNameTemp.Count + 1];
                PlayersName[0] = "On/Off";

                for (int i = 0; i < playersNameTemp.Count; i++)
                {
                    PlayersName[i + 1] = playersNameTemp[i];
                }
            }

            if (!StartOfRound.Instance.inShipPhase)
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
            else
                CodesButtonNames = new string[0];

            if (currentPage == Pages.Contract)
                LGU.ContractMoons();

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
        public static void UpdatePagesMode()
        {
            switch(ModBase.PagesMode.Value)
            {
                case Data.PagesScrollType.Dynamic:
                    TerminalGUI.dynamic = true;
                    TerminalGUI.leftRight = false;
                    TerminalGUI.upDown = false;
                    break;

                case Data.PagesScrollType.UpDown:
                    TerminalGUI.upDown = true;
                    TerminalGUI.leftRight = false;
                    TerminalGUI.dynamic = false;
                    break;

                case Data.PagesScrollType.LeftRight:
                    TerminalGUI.leftRight = true;
                    TerminalGUI.upDown = false;
                    TerminalGUI.dynamic = false;
                    break;

                case Data.PagesScrollType.Both:
                    TerminalGUI.upDown = true;
                    TerminalGUI.leftRight = true;
                    TerminalGUI.dynamic = false;
                    break;
            }
        }
        private static Color32 ReadColor(string ColorText)
        {
            var spiltColor = ColorText.Split(new char[] { ',', ' ', '"' }, StringSplitOptions.RemoveEmptyEntries).Select(x => byte.Parse(x.Trim(), CultureInfo.InvariantCulture)).ToArray();
            return new Color32(spiltColor[0], spiltColor[1], spiltColor[2], spiltColor[3]);
        }
        private static string[] ReadText(string Text)
        {
            var spiltText = Text.Split(new char[] { ',', '"' }, StringSplitOptions.RemoveEmptyEntries);
            return spiltText;
        }

        private void OnGUI()
        {
            if (!isStylesLoaded)
                LoadStyles();
            if (boxVisible)
                GUI.Box(new Rect(Screen.width + 100, 0, 30, 30),"test");

            if (isUiVisible && StartOfRound.Instance.localPlayerController.inTerminalMenu)
            {
                totalOfPages = Mathf.CeilToInt((float)ButtonNames.Length / (float)Limit);
                string boxText = $"Page <{Page}/{totalOfPages}>";

                GUI.backgroundColor = ReadColor(ModBase.BoxBackgroundColor.Value);
                GUI.Box(new Rect(MenuX, MenuY, MenuWidth, MenuHeight), boxText, boxStyle);

                if (currentPage == Pages.Store && currentSubPage == SubPages.None)
                {
                    GUI.backgroundColor = ReadColor(ModBase.AmountBoxbackgroundColor.Value);
                    GUI.TextField(new Rect(MenuX + (Screen.width * 0.006f), MenuY - 20, 65, 20), "Amount: " + ItemAmount, textFieldStyle);
                }

                //drawing menus
                if (currentPage == Pages.Main)
                    ButtonNames = MainButtonNames;

                if (currentPage == Pages.Store && currentSubPage == SubPages.None)
                    ButtonNames = StoreButtonNames;

                if (currentPage == Pages.Moons && currentSubPage == SubPages.None)
                    ButtonNames = MoonsButtonNames;

                if (currentPage == Pages.Upgrades && currentSubPage == SubPages.None)
                    ButtonNames = UpgradeButtonNames;

                if (currentPage == Pages.Bestiary)
                    ButtonNames = BestiaryButtonNames;

                if (currentPage == Pages.Logs)
                    ButtonNames = LogsButtonNames;

                if (currentPage == Pages.Storage)
                    ButtonNames = StorageButtonNames;

                if (currentPage == Pages.Flash || currentPage == Pages.Ping)
                    ButtonNames = RadarsName;

                if (currentPage == Pages.Monitor)
                    ButtonNames = PlayersName;

                if (currentPage == Pages.Decor && currentSubPage == SubPages.None)
                    ButtonNames = DecorButtonNames;

                if (currentPage == Pages.Codes)
                    ButtonNames = CodesButtonNames;

                if (currentSubPage == SubPages.Confirm)
                    ButtonNames = ConfirmButtonNames;

                if (currentSubPage == SubPages.Info)
                    ButtonNames = InfoButtonNames;

                if (currentPage == Pages.LateGame)
                    ButtonNames = LGU.MainButtonsName;

                if (currentPage == Pages.moreUpgrades && currentSubPage == SubPages.None)
                    ButtonNames = LGU.MoreUpgradesButtonsName;

                if (currentSubPage == SubPages.LGUConfirm)
                    ButtonNames = LGU.ConfirmButtonNames;

                if (currentPage == Pages.Contract && currentSubPage == SubPages.None)
                    ButtonNames = LGU.ContractButtonNames;

                if (currentPage == Pages.Demon)
                    ButtonNames = LGU.DemonsButtonNames;

                if (currentPage == Pages.Bruteforce)
                    ButtonNames = LGU.IPButtonNames;

                if (currentPage == Pages.Transmit || currentPage == Pages.Lookup)
                    ButtonNames = Keyboard;

                if (ButtonNames.Length <= 0 || ButtonNames == null)
                {
                    ButtonNames = EmptyButtonNames;
                }
                DrawButtons();
            }
        }
        private void DrawButtons()
        {
            if (ButtonNames != Keyboard)
            {
                MakeGUIFitsScreen();
                Limit = Rows * Columns;

                if (myCount > (Limit * Page) - 1)
                {
                    Page++;
                    myCountInedx = myCount - (Limit * (Page - 1));
                }
                else if (myCount < (Limit * (Page - 1)))
                {
                    Page--;
                    myCountInedx = myCount - (Limit * (Page - 1));
                }

                int CurrentDrawingRow = 1;

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
            else
                DrawKeyboard();
        }
        private void DrawKeyboard()
        {
            MakeGUIFitsScreen();

            int CurrentDrawingRow = 1;
            Rows = 4;
            Columns = 10;
            Limit = Rows * Columns;

            for (int i = 0; i < ButtonNames.Length; i++)
            {
                float ButtonWidth = (Screen.width * 0.63f) / Columns;
                float ButtonHeight = (Screen.height * 0.36f) / Rows;

                CurrentDrawingRow = i / Columns;

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
                if (i >= 0)
                {
                    if (i < Limit)//                                                                                                                        between space
                        GUI.Button(new Rect(MenuX + BetweenSpaceX + (i - (Columns * CurrentDrawingRow)) * (ButtonX / Columns), MenuY + (MenuHeight / (Rows + BetweenSpaceY)) + (CurrentDrawingRow * (ButtonY / Rows)), ButtonWidth, ButtonHeight), ButtonNames[i], buttonStyle);
                }
            }
        }
        IEnumerator DrawBoxForOneSecond()
        {
            yield return new WaitForSeconds(1f);
            boxVisible = false;
        }

        private void LoadStyles()
        {
            isStylesLoaded = true;
            Texture2D background = new Texture2D(10,10);
            MakeTex(2, 2, Color.white, background);

            boxStyle = new GUIStyle(GUI.skin.box);
            hoverbuttonStyle = new GUIStyle(GUI.skin.button);
            normalbuttonStyle = new GUIStyle(GUI.skin.button);
            textFieldStyle = new GUIStyle(GUI.skin.button);

            hoverbuttonStyle.normal.textColor = ReadColor(ModBase.HighlightedButtonTextColor.Value);
            hoverbuttonStyle.normal.background = background;
            hoverbuttonStyle.fontSize = 16;
            hoverbuttonStyle.fontStyle = FontStyle.Bold;
            hoverbuttonStyle.wordWrap = true;
            hoverbuttonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            normalbuttonStyle.normal.textColor = ReadColor(ModBase.ButtonTextColor.Value);
            normalbuttonStyle.normal.background = background;
            normalbuttonStyle.fontSize = 14;
            normalbuttonStyle.fontStyle = FontStyle.Normal;
            normalbuttonStyle.wordWrap = true;
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
        private void MakeTex(int width, int height, Color col, Texture2D background)
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

            currentPage = Pages.Main;
            currentSubPage = SubPages.None;
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

            myCount = 0;
            ItemAmount = 1;
            Page = 1;
        }

        private void ShowHideKeyboard(InputAction.CallbackContext context)
        {
            isUiVisible = !isUiVisible;
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio ,TApi.Terminal.keyboardClips);
        }
        private void Subbmit(InputAction.CallbackContext context)
        {
            if (myCount < ButtonNames.Length)
            {

                if (Gamepad.all.Count >= 1)
                    RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);

                if (ButtonNames == EmptyButtonNames)
                {
                    currentPage = Pages.Main;
                    TApi.Terminal.OnSubmit();
                }
                if (ButtonNames != EmptyButtonNames)
                {
                    bool skip = false;
                    if (currentPage == Pages.Main)
                    {
                        if (TApi.GetTerminalInput().ToLower() == "store")
                        {
                            currentPage = Pages.Store;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "moons")
                        {
                            currentPage = Pages.Moons;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "bestiary")
                        {
                            currentPage = Pages.Bestiary;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "sigurd")
                        {
                            currentPage = Pages.Logs;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "storage")
                        {
                            currentPage = Pages.Storage;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "blind")
                        {
                            currentPage = Pages.Flash;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "ping")
                        {
                            currentPage = Pages.Ping;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "monitors")
                        {
                            currentPage = Pages.Monitor;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "codes")
                        {
                            currentPage = Pages.Codes;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "decor")
                        {
                            currentPage = Pages.Decor;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "upgrades")
                        {
                            currentPage = Pages.Upgrades;
                        }
                        if (TApi.GetTerminalInput().ToLower() == "transmit")
                        {
                            currentPage = Pages.Transmit;
                            TApi.SetTerminalInput("");
                        }
                        if (TApi.GetTerminalInput().ToLower() == "lategame" && LGU.enabled)
                        {
                            currentPage = Pages.LateGame;
                        }
                        skip = true;
                    }
                    if (currentPage == Pages.LateGame && !skip)
                    {
                        if (TApi.GetTerminalInput().ToLower() == "lategame store")
                            currentPage = Pages.moreUpgrades;

                        if (TApi.GetTerminalInput().ToLower() == "contract info")
                            currentPage = Pages.Contract;

                        if (TApi.GetTerminalInput().ToLower() == "demon")
                            currentPage = Pages.Demon;

                        if (TApi.GetTerminalInput().ToLower() == "bruteforce")
                        {
                            currentPage = Pages.Bruteforce;
                            TApi.SetTerminalInput("");
                        }

                        if (TApi.GetTerminalInput().ToLower() == "lookup")
                        {
                            currentPage = Pages.Lookup;
                            TApi.SetTerminalInput("");
                        }

                        skip = true;
                    }

                    if (currentPage == Pages.Flash && !skip)
                        if (ButtonNames[myCount].ToLower() == RadarsName[myCount].ToLower())
                        {
                            TApi.SetTerminalInput("flash " + RadarsName[myCount]);
                        }

                    if (currentPage == Pages.Ping && !skip)
                        if (ButtonNames[myCount].ToLower() == RadarsName[myCount].ToLower())
                        {
                            TApi.SetTerminalInput("ping " + RadarsName[myCount]);
                        }

                    if (currentPage == Pages.Store && !skip)
                        if (ButtonNames[myCount].ToLower() == StoreButtonNames[myCount].ToLower())
                        {
                            tempItemAmount = ItemAmount;
                            currentSubPage = SubPages.Confirm;
                        }

                    if (currentPage == Pages.Decor && !skip)
                        if (ButtonNames[myCount].ToLower() == DecorButtonNames[myCount].ToLower())
                        {
                            currentSubPage = SubPages.Confirm;
                        }

                    if (currentPage == Pages.Moons && !skip)
                        if (ButtonNames[myCount].ToLower() == MoonsButtonNames[myCount].ToLower())
                        {
                            currentSubPage = SubPages.Confirm;
                        }

                    if (currentPage == Pages.Upgrades && !skip)
                        if (ButtonNames[myCount].ToLower() == UpgradeButtonNames[myCount].ToLower())
                        {
                            currentSubPage = SubPages.Confirm;
                        }

                    if (currentPage == Pages.moreUpgrades && currentSubPage == SubPages.None && !skip)
                    {
                        currentSubPage = SubPages.LGUConfirm;
                        skip = true;
                    }

                    if (currentPage == Pages.Contract && currentSubPage == SubPages.None && !skip)
                    {
                        if (ButtonNames[myCount].ToLower() == "random")
                            currentSubPage = SubPages.None;
                        else
                            currentSubPage = SubPages.LGUConfirm;
                        skip = true;
                    }

                    if (currentSubPage == SubPages.LGUConfirm && !skip)
                    {
                        currentSubPage = SubPages.None;
                    }

                    if (currentSubPage == SubPages.Confirm && !skip)
                    {
                        if (TApi.GetTerminalInput().ToLower() == "info")
                        {
                            if (currentPage != Pages.Decor)
                                currentSubPage = SubPages.Info;
                        }
                        else if (TApi.GetTerminalInput().ToLower() == "deny" || TApi.GetTerminalInput().ToLower() == "confirm")
                        {
                            currentSubPage = SubPages.None;
                        }

                    }

                    if (currentSubPage == SubPages.Info && !skip)
                        if (TApi.GetTerminalInput().ToLower() == "info")
                        {
                            if (currentPage == Pages.Decor)
                            {
                                currentPage = Pages.Decor;
                                TApi.SetTerminalInput("decor");
                            }
                            else
                            {
                                TApi.SetTerminalInput("deny");
                                TApi.Terminal.OnSubmit();
                                TApi.SetTerminalInput(tmp + " info");
                            }
                        }

                    if (currentSubPage == SubPages.Info && !skip)
                    {
                        if (TApi.GetTerminalInput().ToLower() == "store")
                        {
                            currentSubPage = SubPages.None;
                        }
                        else if (ButtonNames[myCount].ToLower() == "continue")
                        {
                            TApi.Terminal.OnSubmit();
                            TApi.SetTerminalInput("confirm");
                            currentSubPage = SubPages.None;
                        }
                    }

                    if (currentPage == Pages.Bruteforce && !skip)
                    {
                        if (ButtonNames[myCount] == ".")
                        {
                            if (LGU.CountDots(TApi.GetTerminalInput()) < 3 && LGU.CountNumber(TApi.GetTerminalInput()) > 0)
                                TApi.SetTerminalInput($"{TApi.GetTerminalInput()}{ButtonNames[myCount]}");
                        }
                        else if (ButtonNames[myCount].ToLower() != "submit" && ButtonNames[myCount].ToLower() != "backspace")
                        {
                            if (LGU.CountNumber(TApi.GetTerminalInput()) < LGU.HowManyNumbers(TApi.GetTerminalInput()))
                                TApi.SetTerminalInput($"{TApi.GetTerminalInput()}{ButtonNames[myCount]}");

                            mls.LogMessage(LGU.CountNumber(TApi.GetTerminalInput()));  
                        }
                        else if (ButtonNames[myCount].ToLower() == "backspace")
                        {
                            if (TApi.GetTerminalInput().Length > 0)
                            {
                                TApi.SetTerminalInput(TApi.GetTerminalInput().Substring(0, TApi.GetTerminalInput().Length - 1));
                            }
                        }
                        else
                        {
                            TApi.SetTerminalInput("bruteforce " + TApi.GetTerminalInput());
                            TApi.Terminal.OnSubmit();
                        }

                    }

                    if (currentPage == Pages.Transmit && !skip)
                    {
                        if (ButtonNames[myCount].ToLower() != "submit" && ButtonNames[myCount].ToLower() != "space" && ButtonNames[myCount].ToLower() != "backspace")
                        {
                            TApi.SetTerminalInput($"{TApi.GetTerminalInput()}{ButtonNames[myCount]}");
                        }
                        else if (ButtonNames[myCount].ToLower() == "space")
                        {
                            TApi.SetTerminalInput($"{TApi.GetTerminalInput()} ");
                        }
                        else if (ButtonNames[myCount].ToLower() == "backspace")
                        {
                            if (TApi.GetTerminalInput().Length > 0)
                            {
                                TApi.SetTerminalInput(TApi.GetTerminalInput().Substring(0, TApi.GetTerminalInput().Length - 1));
                            }
                        }
                        else
                        {
                            TApi.SetTerminalInput("transmit " + TApi.GetTerminalInput());
                            TApi.Terminal.OnSubmit();
                        }

                    }

                    if (currentPage == Pages.Lookup && !skip)
                    {
                        if (ButtonNames[myCount].ToLower() != "submit" && ButtonNames[myCount].ToLower() != "space" && ButtonNames[myCount].ToLower() != "backspace")
                        {
                            if (TApi.GetTerminalInput().Length < 2 && LGU.IsLettersOnly(ButtonNames[myCount]))
                                TApi.SetTerminalInput($"{TApi.GetTerminalInput()}{ButtonNames[myCount]}");
                            if (TApi.GetTerminalInput().Length == 2)
                                TApi.SetTerminalInput($"{TApi.GetTerminalInput()}-");
                            else if (TApi.GetTerminalInput().Length > 2 && TApi.GetTerminalInput().Length < 6 && LGU.IsNumbersOnly(ButtonNames[myCount]))
                                TApi.SetTerminalInput($"{TApi.GetTerminalInput()}{ButtonNames[myCount]}");
                        }
                        else if (ButtonNames[myCount].ToLower() == "backspace")
                        {
                            if (TApi.GetTerminalInput().Length > 0)
                            {
                                TApi.SetTerminalInput(TApi.GetTerminalInput().Substring(0, TApi.GetTerminalInput().Length - 1));
                            }
                        }
                        else if (ButtonNames[myCount].ToLower() == "submit")
                        {
                            TApi.SetTerminalInput("lookup " + TApi.GetTerminalInput());
                            TApi.Terminal.OnSubmit();
                        }

                    }

                    ItemAmount = 1;
                    if (currentPage != Pages.Bruteforce && currentPage != Pages.Transmit && currentPage != Pages.Lookup)
                        TApi.Terminal.OnSubmit();
                }
                else
                {
                    myCount = 0;
                    myCountInedx = myCount - (Limit * (Page - 1));
                }

                if (currentPage != Pages.Main && currentPage != Pages.Flash && currentPage != Pages.Ping && currentPage != Pages.Monitor && currentPage != Pages.Bruteforce && currentPage != Pages.LateGame && currentPage != Pages.Transmit && currentPage != Pages.Lookup)
                {
                    myCount = 0;
                    Page = 1;
                    myCountInedx = myCount - (Limit * (Page - 1));
                }

            }
        }
        private void Return(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);

            if (currentPage != Pages.Bruteforce && currentPage != Pages.Main)
            {
                Page = 1;
                myCount = 0;
                myCountInedx = myCount - (Limit * (Page - 1));
            }

            if (currentPage != Pages.Main)
            {
                if (ButtonNames == EmptyButtonNames)
                {
                    currentPage = Pages.Main;
                    TApi.SetTerminalInput("help");
                }  

                if ((currentPage == Pages.Store && currentSubPage == SubPages.None) || (currentPage == Pages.Moons && currentSubPage == SubPages.None) || currentPage == Pages.Bestiary || currentPage == Pages.Logs || currentPage == Pages.Storage || currentPage == Pages.Flash || currentPage == Pages.Ping || currentPage == Pages.Monitor || currentPage == Pages.Codes || (currentPage == Pages.Decor && currentSubPage == SubPages.None) || (currentPage == Pages.Upgrades && currentSubPage == SubPages.None) || (currentPage == Pages.LateGame && currentSubPage == SubPages.None) || currentPage == Pages.Transmit)
                {
                    currentPage = Pages.Main;
                    TApi.SetTerminalInput("help");
                }
                if (currentSubPage == SubPages.Confirm)
                {
                    currentSubPage = SubPages.None;
                    TApi.SetTerminalInput("deny");
                    TApi.Terminal.OnSubmit();
                    if (currentPage == Pages.Store)
                        TApi.SetTerminalInput("store");
                    else if (currentPage == Pages.Moons)
                        TApi.SetTerminalInput("moons");
                    else if (currentPage == Pages.Decor)
                        TApi.SetTerminalInput("decor");
                    else if (currentPage == Pages.Upgrades)
                        TApi.SetTerminalInput("upgrades");
                }
                if (currentSubPage == SubPages.Info)
                {
                    currentSubPage = SubPages.None;
                    if (currentPage == Pages.Store)
                        TApi.SetTerminalInput("store");
                    else if (currentPage == Pages.Moons)
                        TApi.SetTerminalInput("moons");
                    else if (currentPage == Pages.Upgrades)
                        TApi.SetTerminalInput("upgrades");
                }
                if ((currentPage == Pages.moreUpgrades || currentPage == Pages.Contract) && currentSubPage == SubPages.None || currentPage == Pages.Demon || currentPage == Pages.Lookup || currentPage == Pages.Bruteforce)
                {
                    currentPage = Pages.LateGame;
                    TApi.SetTerminalInput("Lategame");
                }
                if (currentSubPage == SubPages.LGUConfirm)
                {
                    currentSubPage = SubPages.None;

                    if (currentPage == Pages.moreUpgrades)
                        TApi.SetTerminalInput("Lategame Store");
                    else if (currentPage == Pages.Contract)
                    {
                        TApi.SetTerminalInput("contract info");
                        TApi.Terminal.OnSubmit();
                        TApi.SetTerminalInput("contract info");
                    }
                }

                TApi.Terminal.OnSubmit();
            }
        }

        private void GoDown(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);
            
            if (isUiVisible)
            {
                myCount += Columns;
                if (myCount > ButtonNames.Length - 1 || (!upDown && currentRow == Rows))
                    myCount -= Columns;

                myCountInedx = myCount - (Limit * (Page - 1));
            }
        }
        private void GoUp(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);
            
            if (isUiVisible)
            {
                myCount -= Columns;
                if (myCount < 0 || (!upDown && currentRow == 1))
                    myCount += Columns;
                myCountInedx = myCount - (Limit * (Page - 1));
            }
        }
        private void GoRight(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);

            if (isUiVisible)
            {
                myCount += 1;
                myCountInedx = myCount - (Limit * (Page - 1));
                if (myCount > ButtonNames.Length - 1)
                { 
                    myCount -= 1;
                    myCountInedx = myCount - (Limit * (Page - 1));
                }
                else if (myCountInedx > currentRow * Columns - 1)
                {
                    if (currentRow != Rows && leftRight)
                    {
                        myCount -= 1;
                        myCountInedx = myCount - (Limit * (Page - 1));
                    }
                    else if (!leftRight)
                    {
                        myCount -= 1;
                        myCountInedx = myCount - (Limit * (Page - 1));   
                    }
                }
            }
        }
        private void GoLeft(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);

            if (isUiVisible)
            {
                myCount -= 1;
                myCountInedx = myCount - (Limit * (Page - 1));
                if (myCount < 0)
                {
                    myCount += 1;
                    myCountInedx = myCount - (Limit * (Page - 1));
                }  
                else if (myCountInedx < (currentRow * Columns - 1) - Columns + 1)
                {
                    if (currentRow != 1 && leftRight)
                    {
                        myCount += 1;
                        myCountInedx = myCount - (Limit * (Page - 1));
                    }
                    else if (!leftRight)
                    {
                        myCount += 1;
                        myCountInedx = myCount - (Limit * (Page - 1));
                    }
                }
            }
        }

        private void PlusAmount(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);

            if (currentPage == Pages.Store && isUiVisible && currentSubPage == SubPages.None)
            {
                ItemAmount++;
                if (ItemAmount > 12)
                    ItemAmount = 1;
            }
        }
        private void MinusAmount(InputAction.CallbackContext context)
        {
            if (Gamepad.all.Count >= 1)
                RoundManager.PlayRandomClip(TApi.Terminal.terminalAudio, TApi.Terminal.keyboardClips);

            if (currentPage == Pages.Store && isUiVisible && currentSubPage == SubPages.None)
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

        [InputAction("<Keyboard>/upArrow", Name = "upArrow", GamepadPath = "<Gamepad>/dpad/up")]
        public InputAction UpArrow { get; set; }

        [InputAction("<Keyboard>/downArrow", Name = "downArrow", GamepadPath = "<Gamepad>/dpad/down")]
        public InputAction DownArrow { get; set; }

        [InputAction("<Keyboard>/leftArrow", Name = "leftArrow", GamepadPath = "<Gamepad>/dpad/left")]
        public InputAction LeftArrow { get; set; }

        [InputAction("<Keyboard>/rightArrow", Name = "rightArrow", GamepadPath = "<Gamepad>/dpad/right")]
        public InputAction RightArrow { get; set; }
    }
}
