using System.Collections.Generic;
using KSP.UI.Screens;
using UnityEngine;
using ClickThroughFix;
using SpaceTuxUtility;

using ToolbarControl_NS;


namespace FRS
{
    internal static class GlobalConfig
    {
        static ConfigNode configFile, configFileNode;
        internal static string PLUGINDATADIR { get { return KSPUtil.ApplicationRootPath + "GameData/FRS/PluginData/"; } }
        internal static string PLUGINDATA { get { return PLUGINDATADIR + "FRS.cfg"; } }
        internal const string NODENAME = "FRS";

        internal static bool KspSkin = true;
        internal static bool showAll = true;
        internal static bool showInstructions = true;
        internal const  int DEFAULT_FONT_SIZE = 13;
        internal static int FontSize = DEFAULT_FONT_SIZE;

        public const int MIN_FONT_SIZE = 10;
        public const int MAX_FONT_SIZE = 36;

        internal static float winX, winY;

        public static void LoadCfg()
        {
            configFile = ConfigNode.Load(PLUGINDATA);
            if (configFile != null)
            {
                configFileNode = configFile.GetNode(NODENAME);
                if (configFileNode != null)
                {
                    KspSkin = configFileNode.SafeLoad("KspSkin", true);
                    FontSize = configFileNode.SafeLoad("FontSize", 13);

                    winX = configFileNode.SafeLoad("winX", 200f);
                    winY = configFileNode.SafeLoad("winY", 200f);
                    showAll = configFileNode.SafeLoad("showAll", true);
                    showInstructions = configFileNode.SafeLoad("showInstructions", true);
                }
            }
        }

        public static void SaveCfg()
        {
            configFile = new ConfigNode(NODENAME);
            configFileNode = new ConfigNode(NODENAME);

            configFileNode.AddValue("KspSkin", KspSkin);
            configFileNode.AddValue("FontSize", FontSize);
            configFileNode.AddValue("winX", winX);
            configFileNode.AddValue("winY", winY);
            configFileNode.AddValue("showAll", showAll);
            configFileNode.AddValue("showInstructions", showInstructions);
            configFile.AddNode(NODENAME, configFileNode);
            configFile.Save(PLUGINDATA);
        }

    }
}
