using System;

using UnityEngine;
using ToolbarControl_NS;
using UnityEngine.UIElements;

namespace FRS
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        public static KSP_Log.Log Log;

        void Start()
        {
            ToolbarControl.RegisterMod(FRS.MODID, FRS.MODNAME);
            Log = new KSP_Log.Log("FRS"
#if DEBUG
                , KSP_Log.Log.LEVEL.DETAIL
#endif
                    );

        }

        bool initted = false;
        void OnGUI()
        {
            if (!initted)
            {
                InitStyle();
                initted = true;
            }
        }
        internal static void InitStyle()
        {
            {
                FRS.myLabelStyle = new GUIStyle(GUI.skin.label);
                FRS.myLabelStyle.fontSize = GlobalConfig.FontSize;
                FRS.myLabelStyle.richText = true;
            }
        }

    }
}