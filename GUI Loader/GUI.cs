using BepInEx.Logging;
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
        private string[] CodesButtonNames;


        private string[] MainButtonNames = { "Store", "Moons", "Scan", "Monitors", "Ping", "Flash", "Codes", "Bestiary", "Logs", "Upgrades", "Storage", "Decor"};
        private string[] ConfirmButtonNames = { "Confirm", "Deny", "Info" };
        private string[] InfoButtonNames = { "Continue", "Go Back" };
        private string[] ReturnButtonNames = { "Return" };
        private string[] EmptyButtonNames = { "Return"};

        private string tmp;

        private int myCount = 0;
        private int ItemAmount = 1;
        private int tempItemAmount;
        private int Page = 1;
        private int Limit = 16;
        private int ButtonIndex = 16;

        private float MENUX;
        private float MENUY;

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle hoverbuttonStyle;
        private GUIStyle normalbuttonStyle;
        private GUIStyle textFieldStyle;

        private bool isUiVisible = false;
        private bool isOnTerminal = false;
        private bool isStylesLoaded = false;

        private bool isOnMainMenu = true;
        private bool isOnStoreMenu = false;
        private bool isOnConfirmMenu = false;
        private bool isOnInfoMenu = false;
        private bool isOnMoonsMenu = false;
        private bool isOnBestiaryMenu = false;
        private bool isOnReturnMenu = false;
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
            ButtonNames = MainButtonNames;
            TerminalEvents();

            StoreButtonNames = new string[TApi.Terminal.buyableItemsList.Length];
            for (int i = 0; i < TApi.Terminal.buyableItemsList.Length; i++)
            {
                StoreButtonNames[i] = TApi.Terminal.buyableItemsList[i].itemName;
            }

            MoonsButtonNames = new string[TApi.Terminal.moonsCatalogueList.Length];
            for (int i = 0; i < TApi.Terminal.moonsCatalogueList.Length; i++)
            {
                MoonsButtonNames[i] = TApi.Terminal.moonsCatalogueList[i].PlanetName;
            }

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

        void Update()
        {
            if (myCount < ButtonNames.Length)
            {
                LoadButtonsInfo();

                MENUX = Screen.width * 0.17f;
                MENUY = Screen.height * 0.59f;

                if (isOnTerminal && isUiVisible)
                {
                    if (!TApi.GetTerminalInput().Contains(ButtonNames[myCount].ToLower()))
                    {
                        if (!isOnInfoMenu && !isOnReturnMenu)
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
                            if (isOnBestiaryMenu)
                                TApi.SetTerminalInput("bestiary");
                            else if (isOnLogsMenu)
                                TApi.SetTerminalInput("sigurd");
                            else
                                TApi.SetTerminalInput("help");
                        }
                        if (ButtonNames[myCount].ToLower() == "continue")
                        {
                            if (isOnStoreMenu)
                                TApi.SetTerminalInput(tmp + " " + tempItemAmount);
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
                        if (!TApi.GetTerminalInput().Contains(ItemAmount.ToString()))
                        {
                            TApi.SetTerminalInput(ButtonNames[myCount].ToLower() + " " + ItemAmount);
                        }
                    }
                }
            }
            else
                myCount = ButtonNames.Length - 1;
        }
        private void LoadButtonsInfo()
        {
            if (isOnStorageMenu)
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

            if (isOnBestiaryMenu)
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

            if (isOnLogsMenu)
            {
                LogsButtonNames = new string[TApi.Terminal.unlockedStoryLogs.Count];
                for (int i = 0; i < TApi.Terminal.unlockedStoryLogs.Count; i++)
            {
                if (TApi.Terminal.unlockedStoryLogs.Count <= 0)
                    break;
                LogsButtonNames[i] = TApi.Terminal.logEntryFiles[TApi.Terminal.unlockedStoryLogs[i]].creatureName;
            }
            }

            if (isOnDecorMenu)
            {
                DecorButtonNames = new string[TApi.Terminal.ShipDecorSelection.Count];
                for (int i = 0; i < TApi.Terminal.ShipDecorSelection.Count; i++)
                {
                    if (DecorButtonNames[i] != TApi.Terminal.ShipDecorSelection[i].creatureName)
                        DecorButtonNames[i] = TApi.Terminal.ShipDecorSelection[i].creatureName;
                }
            }

            if (isOnMonitorMenu)
            {
                PlayersName = new string[StartOfRound.Instance.mapScreen.radarTargets.Count + 1];
                for (int i = 0; i < StartOfRound.Instance.mapScreen.radarTargets.Count + 1; i++)
                {
                    if (i == 0)
                        PlayersName[i] = "On/Of";
                    else
                        PlayersName[i] = StartOfRound.Instance.mapScreen.radarTargets[i - 1].name;
                }
            }

            if (isOnFlashMenu || isOnPingMenu)
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
        }

        private void OnGUI()
        {
            if (!isStylesLoaded)
                LoadStyles();

            if (isUiVisible && isOnTerminal)
            {
                GUI.backgroundColor = new Color32(0, 0, 15, 130);
                GUI.Box(new Rect(MENUX, MENUY, Screen.width * 0.66f, Screen.height * 0.41f), "TerminalGamepad", boxStyle);

                if (isOnStoreMenu && !isOnConfirmMenu)
                {
                    GUI.backgroundColor = new Color32(0, 0, 50, 255);
                    GUI.TextField(new Rect(MENUX + (Screen.width * 0.006f), MENUY - 20, 65, 20), "Amount: " + ItemAmount, textFieldStyle);
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

                if (isOnBestiaryMenu && !isOnReturnMenu)
                    ButtonNames = BestiaryButtonNames;

                if (isOnLogsMenu && !isOnReturnMenu)
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

                if (isOnReturnMenu)
                    ButtonNames = ReturnButtonNames;

                if (ButtonNames.Length <= 0 || ButtonNames == null)
                {
                    ButtonNames = EmptyButtonNames;
                }
                DrawButtons();
            }
        }
        private void DrawButtons()
        {
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
                if (i == myCount)
                {
                    GUI.backgroundColor = new Color32(0, 0, 50, 150);
                    buttonStyle = hoverbuttonStyle;
                }
                else
                {
                    GUI.backgroundColor = new Color32(0, 0, 20, 150);
                    buttonStyle = normalbuttonStyle;
                }
                if (ButtonIndex >= 0)
                {
                    if (ButtonIndex < Limit / 2)
                        GUI.Button(new Rect(MENUX + (Screen.width * 0.0078f) + ButtonIndex * (Screen.width * 0.081f), MENUY + (Screen.height * 0.0232f), Screen.width * 0.078f, Screen.height * 0.18f), ButtonNames[i], buttonStyle);
                    else if (ButtonIndex < Limit)
                        GUI.Button(new Rect(MENUX + (Screen.width * 0.0078f) + (ButtonIndex - 8) * (Screen.width * 0.081f), MENUY + (Screen.height * 0.212f), Screen.width * 0.078f, Screen.height * 0.18f), ButtonNames[i], buttonStyle);
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

            hoverbuttonStyle.normal.textColor = new Color32(36, 255, 0, 255);
            hoverbuttonStyle.normal.background = background;
            hoverbuttonStyle.fontSize = 16;
            hoverbuttonStyle.fontStyle = FontStyle.Bold;
            hoverbuttonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            normalbuttonStyle.normal.textColor = new Color32(20, 142, 0, 255);
            normalbuttonStyle.normal.background = background;
            normalbuttonStyle.fontSize = 14;
            normalbuttonStyle.fontStyle = FontStyle.Normal;
            normalbuttonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            boxStyle.normal.textColor = new Color32(30, 255, 0, 255);
            boxStyle.normal.background = background;
            boxStyle.fontSize = 10;
            boxStyle.fontStyle = FontStyle.Normal;
            boxStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            textFieldStyle.normal.textColor = new Color32(24, 203, 0, 255);
            textFieldStyle.normal.background = background;
            textFieldStyle.fontSize = 10;
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
        private void MenuBool(bool Main, bool Store, bool Confirm, bool Info, bool Moons, bool Bestiary, bool Back, bool Logs, bool Storage, bool Flash, bool Pings, bool Monitors, bool Codes, bool Decor, bool Upgrades)
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
            Cursor.visible = false;
            myCount = 0;
            ItemAmount = 1;
            Page = 1;

            MenuBool(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
        }

        private void ShowHideKeyboard(InputAction.CallbackContext context)
        {
            if (!isUiVisible)
            {
                isUiVisible = true;
            }
            else
            {
                isUiVisible = false;
            }
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
                    if (TApi.GetTerminalInput().Contains("help"))
                    {
                        MenuBool(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
                        TApi.Terminal.OnSubmit();
                    }
                    if (ButtonNames != EmptyButtonNames)
                    {
                        if (TApi.GetTerminalInput().Contains("store"))
                        {
                            MenuBool(false, true, false, false, false, false, false, false, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("moons"))
                        {
                            MenuBool(false, false, false, false, true, false, false, false, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("bestiary"))
                        {
                            MenuBool(false, false, false, false, false, true, false, false, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("sigurd"))
                        {
                            MenuBool(false, false, false, false, false, false, false, true, false, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("storage"))
                        {
                            MenuBool(false, false, false, false, false, false, false, true, true, false, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("blind"))
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, true, false, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("ping"))
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, true, false, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("monitors"))
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("codes"))
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, true, true, false, false);
                        }
                        if (TApi.GetTerminalInput().Contains("decor"))
                        {
                            MenuBool(false, false, false, false, false, false, false, false, false, false, false, true, false, true, false);
                        }
                        if (TApi.GetTerminalInput().Contains("upgrades"))
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
                            if (TApi.GetTerminalInput().Contains("info"))
                            {
                                if (!isOnDecorMenu)
                                    MenuBool(false, isOnStoreMenu, false, true, isOnMoonsMenu, false, false, false, false, false, false, false, false, false, isOnUpgradesMenu);
                            }

                        if (isOnConfirmMenu)
                            if (TApi.GetTerminalInput().ToLower().Contains("deny") || (TApi.GetTerminalInput().ToLower().Contains("confirm")))
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
                            if (ButtonNames[myCount].ToLower().Contains("go back"))
                            {
                                MenuBool(false, isOnStoreMenu, false, false, isOnMoonsMenu, false, false, false, false, false, false, false, false, false, false);
                            }
                            else if (ButtonNames[myCount].ToLower().Contains("continue"))
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

                if ((isOnStoreMenu && !isOnConfirmMenu && !isOnInfoMenu) || (isOnMoonsMenu && !isOnConfirmMenu && !isOnInfoMenu) || (isOnBestiaryMenu && !isOnReturnMenu) || (isOnLogsMenu && !isOnReturnMenu) || isOnStorageMenu || isOnFlashMenu || isOnPingMenu || isOnMonitorMenu || isOnCodesMenu || (isOnDecorMenu && !isOnConfirmMenu) || (isOnUpgradesMenu && !isOnConfirmMenu && !isOnInfoMenu))
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
                if (isOnReturnMenu)
                {
                    MenuBool(false, false, false, false, false, isOnBestiaryMenu, false, isOnLogsMenu, false, false, false, false, false, false, false);
                    if (isOnBestiaryMenu)
                        TApi.SetTerminalInput("bestiary");
                    else
                        TApi.SetTerminalInput("sigurd");
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
            if (isUiVisible && myCount + 8 <= (Limit * Page) - 1)
            {
                myCount += 8;
                if (myCount > ButtonNames.Length - 1)
                    myCount -= 8;
            }
        }
        private void GoUp(InputAction.CallbackContext context)
        {
            if (isUiVisible && myCount - 8 >= (Limit * (Page - 1)))
            {
                myCount -= 8;
                if (myCount < 0)
                    myCount += 8;
            }
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

        [InputAction("<Keyboard>/leftArrow", Name = "leftArrow", GamepadPath = "<Gamepad>/dpad/left")]
        public InputAction LeftArrow { get; set; }

        [InputAction("<Keyboard>/rightArrow", Name = "rightArrow", GamepadPath = "<Gamepad>/dpad/right")]
        public InputAction RightArrow { get; set; }

        [InputAction("<Keyboard>/upArrow", Name = "upArrow", GamepadPath = "<Gamepad>/dpad/up")]
        public InputAction UpArrow { get; set; }

        [InputAction("<Keyboard>/downArrow", Name = "downArrow", GamepadPath = "<Gamepad>/dpad/down")]
        public InputAction DownArrow { get; set; }

        [InputAction("numpadPlus", Name = "PlusAmount", GamepadPath = "<Gamepad>/rightShoulder")]
        public InputAction PlusAmount { get; set; }

        [InputAction("numpadMinus", Name = "MinusAmount", GamepadPath = "<Gamepad>/leftShoulder")]
        public InputAction MinusAmount { get; set; }

        [InputAction("<Keyboard>/enter", Name = "Subbmit", GamepadPath = "<Gamepad>/buttonSouth")]
        public InputAction Subbmit { get; set; }

        [InputAction("<Keyboard>/alt", Name = "Return", GamepadPath = "<Gamepad>/buttonEast")]
        public InputAction Return { get; set; }
    }
}
