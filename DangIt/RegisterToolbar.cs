using KSP.Localization;
using UnityEngine;
using ToolbarControl_NS;
using static nsDangIt.DangIt;

namespace nsDangIt
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(DangIt.MODID, DangIt.MODNAME);
        }
    }

    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class InitLog : MonoBehaviour
    {
        protected void Awake()
        {
            Log = new KSP_Log.Log("#LOC_DangIt_1"
#if DEBUG
                , KSP_Log.Log.LEVEL.INFO
#endif
                );
        }
    }
}
