using KSP.UI.Screens;
using UnityEngine;
using static FRS.RegisterToolbar;
using ToolbarControl_NS;
using ClickThroughFix;
using SpaceTuxUtility;


namespace FRS
{
    [KSPAddon(KSPAddon.Startup.EveryScene, true)]
    public class FRS : MonoBehaviour
    {
        internal const string MODID = "FRS";
        internal const string MODNAME = "FRS";

        static ToolbarControl toolbarControl;
        bool Active = false;
        int enterExitWinId;

        private GUIContent fundsContent;
        private GUIContent repContent;
        private GUIContent scienceContent;

#pragma warning disable 0414
        private Texture2D fundsIconGreen = new Texture2D(2, 2);
        private Texture2D fundsIconRed = new Texture2D(2, 2);
        private Texture2D reputationIconGreen = new Texture2D(2, 2);
        private Texture2D reputationIconRed = new Texture2D(2, 2);
        private Texture2D scienceIcon = new Texture2D(2, 2);
#pragma warning restore 0414

        public double lastFundsChange { get; private set; }
        public float lastRepChange { get; private set; }
        public float lastScienceChange { get; private set; }
        public double currentFunds { get; private set; }
        public float currentReputation { get; private set; }
        public float currentScience { get; private set; }

        bool Career = false;

        private Rect _windowRect;

        const float DEFAULT_WIDTH = 150;
        const float DEFAULT_HEIGHT = 100;
        float width = DEFAULT_WIDTH;
        float height = DEFAULT_HEIGHT;
        public static GUIStyle myLabelStyle = null;

        void Start()
        {
            DontDestroyOnLoad(this);

            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(this.ToggleMainWindow, this.ToggleMainWindow,
                    ApplicationLauncher.AppScenes.VAB |
                        ApplicationLauncher.AppScenes.SPH |
                        ApplicationLauncher.AppScenes.FLIGHT |
                        ApplicationLauncher.AppScenes.MAPVIEW |
                        ApplicationLauncher.AppScenes.SPACECENTER |
                        ApplicationLauncher.AppScenes.TRACKSTATION,
                    MODID,
                    "FRSButton",
                    "FRS/PluginData/FRS",
                    "FRS/PluginData/FRS",
                    MODNAME
                );
            }
            GameEvents.OnFundsChanged.Add(this.onFundsChange);
            GameEvents.OnReputationChanged.Add(this.onRepChange);
            GameEvents.OnScienceChanged.Add(this.onScienceChange);
            GameEvents.onGameStateLoad.Add(this.onGameStateLoad);

            enterExitWinId = WindowHelper.NextWindowId("FRSWin");
            GlobalConfig.LoadCfg();

            ToolbarControl.LoadImageFromFile(ref fundsIconGreen, KSPUtil.ApplicationRootPath + "GameData/FRS/Textures/fundsgreen.png");
            ToolbarControl.LoadImageFromFile(ref fundsIconRed, KSPUtil.ApplicationRootPath + "GameData/FRS/Textures/fundsred.png");
            ToolbarControl.LoadImageFromFile(ref reputationIconGreen, KSPUtil.ApplicationRootPath + "GameData/FRS/Textures/repgreen.png");
            ToolbarControl.LoadImageFromFile(ref reputationIconRed, KSPUtil.ApplicationRootPath + "GameData/FRS/Textures/repred.png");
            ToolbarControl.LoadImageFromFile(ref scienceIcon, KSPUtil.ApplicationRootPath + "GameData/FRS/Textures/science.png");

            this.fundsContent = new GUIContent();
            this.repContent = new GUIContent();
            this.scienceContent = new GUIContent();
            this.fundsContent.image = this.fundsIconGreen;
            this.repContent.image = this.reputationIconGreen;
            this.scienceContent.image = this.scienceIcon;

            GlobalConfig.LoadCfg();
            width = ((float)GlobalConfig.FontSize / (float)GlobalConfig.DEFAULT_FONT_SIZE * DEFAULT_WIDTH);
            height = ((float)GlobalConfig.FontSize / (float)GlobalConfig.DEFAULT_FONT_SIZE * DEFAULT_HEIGHT);
            _windowRect.width = width;
            _windowRect.height = height;
            _windowRect.x = GlobalConfig.winX;
            _windowRect.y = GlobalConfig.winY;
        }

        void ToggleMainWindow() { this.Active = !this.Active; }


        private void onFundsChange(double newValue, TransactionReasons reasons)
        {
            this.lastFundsChange = newValue - this.currentFunds;
            this.currentFunds = newValue;
        }

        private void onRepChange(float newValue, TransactionReasons reasons)
        {
            this.lastRepChange = newValue - this.currentReputation;
            this.currentReputation = newValue;
        }

        private void onScienceChange(float newValue, TransactionReasons reasons)
        {
            this.lastScienceChange = newValue - this.currentScience;
            this.currentScience = newValue;
        }

        private void onGameStateLoad(ConfigNode node) { initCurrencies(); }

        private void initCurrencies()
        {
#if false
            Log.Info(
                "Initializing currencies." +
                "\n\tFunding.Instance={0}" +
                "ResearchAndDevelopment.Instance={1}" +
                "Reputation.Instance={2}",
                Funding.Instance == null ? "NULL" : Funding.Instance.ToString(),
                ResearchAndDevelopment.Instance == null ? "NULL" : ResearchAndDevelopment.Instance.ToString(),
                Reputation.Instance == null ? "NULL" : Reputation.Instance.ToString()
            );
#endif

            Career = Funding.Instance != null;
            if (Career)
            {
                this.currentFunds = Funding.Instance != null ? Funding.Instance.Funds : 0;
                this.currentReputation = Reputation.Instance != null ? Reputation.Instance.reputation : 0;
                this.currentScience = ResearchAndDevelopment.Instance != null ?
                    ResearchAndDevelopment.Instance.Science : 0;
            }
        }
        protected Rect WindowRect
        {
            get
            {
                if (_windowRect == default(Rect))
                {
                    return new Rect(200f, 200f, width, height);
                }
                return _windowRect;
            }
            set
            {
                _windowRect = value;
            }
        }

        public void OnGUI()
        {
            if (Active)
            {
                if (GlobalConfig.KspSkin)
                    GUI.skin = HighLogic.Skin;

                WindowRect = ClickThruBlocker.GUILayoutWindow(enterExitWinId, WindowRect, Win, "FRS");
                _windowRect.width = width;
                _windowRect.height = height;

            }
        }

        void SaveWinInfo()
        {
            myLabelStyle.fontSize = GlobalConfig.FontSize;
            myLabelStyle.richText = true;

            myLabelStyle.fontSize = GlobalConfig.FontSize;
            width = ((float)GlobalConfig.FontSize / (float)GlobalConfig.DEFAULT_FONT_SIZE * DEFAULT_WIDTH);
            height = ((float)GlobalConfig.FontSize / (float)GlobalConfig.DEFAULT_FONT_SIZE * DEFAULT_HEIGHT);

            GlobalConfig.winX = _windowRect.x;
            GlobalConfig.winY = _windowRect.y;
            width = ((float)GlobalConfig.FontSize / (float)GlobalConfig.DEFAULT_FONT_SIZE * DEFAULT_WIDTH);
            height = ((float)GlobalConfig.FontSize / (float)GlobalConfig.DEFAULT_FONT_SIZE * DEFAULT_HEIGHT);
            GlobalConfig.SaveCfg();
        }

        void Win(int id)
        {
            if (GUI.Button(new Rect(2, 2f, 15f, 15f), "-"))
            {
                // Who wants a 0 size font?
                if (GlobalConfig.FontSize > GlobalConfig.MIN_FONT_SIZE)
                    GlobalConfig.FontSize--;
                SaveWinInfo();
            }

            GUI.Label(new Rect(22f, 0f, 60f, 20f), "Font");
            if (GUI.Button(new Rect(22f + 30f, 2f, 15f, 15f), "+"))
            {
                // Big big big!!!
                if (GlobalConfig.FontSize < GlobalConfig.MAX_FONT_SIZE)
                    GlobalConfig.FontSize++;
                SaveWinInfo();
            }
            if (GUI.Button(new Rect(_windowRect.width - 24 - 28, 3f, 23, 15f), new GUIContent("S")))
            {
                GlobalConfig.KspSkin = !GlobalConfig.KspSkin;
                SaveWinInfo();
            }
            if (GUI.Button(new Rect(_windowRect.width - 24, 3f, 23, 15f), new GUIContent("X")))
                Active = false;


            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Funds: ", myLabelStyle);
            GUILayout.FlexibleSpace();
            this.fundsContent.text = currentFunds.ToString("#,#.##");
            GUILayout.Label(fundsContent, myLabelStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Reputation:", myLabelStyle);
            GUILayout.FlexibleSpace();
            this.repContent.text = currentReputation.ToString("#,#.##");
            GUILayout.Label(repContent, myLabelStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Science:", myLabelStyle);
            GUILayout.FlexibleSpace();
            this.scienceContent.text = currentScience.ToString("#,#.##");
            GUILayout.Label(scienceContent, myLabelStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
