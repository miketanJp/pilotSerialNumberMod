using HarmonyLib;
using it.miketan.PilotSerial.UI;

namespace it.miketan.PilotSerial
{
    [HarmonyPatch]
    public class Patch
    {
        // Postfix sul redraw della vista del pilota: aggiunge/riusa una label NGUI con il seriale senza toccare la bio
        [HarmonyPatch(typeof(CIViewBasePilotInfoExtended), "RedrawForPilot")]
        [HarmonyPostfix]
        public static void RedrawForPilot_Postfix(CIViewBasePilotInfoExtended __instance, PersistentEntity pilot)
        {
            UIInjector.InjectLabel(__instance?.coreLabelSummary, pilot);
        }

        // Effettua il postfix di InjectForAnchor anche nell'editor (CIViewBaseEditor) in fase di avvio editing
        [HarmonyPatch(typeof(CIViewBaseEditor), "OnEditStart")]
        [HarmonyPostfix]
        public static void Editor_OnEditStart_Postfix(CIViewBaseEditor __instance, PersistentEntity pilot)
        {
            UIInjector.InjectLabel(__instance?.inputBio != null ? __instance.inputBio.label : null, pilot);
        }
    }
}