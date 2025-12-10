using System;
using HarmonyLib;
using it.miketan.PilotSerial.Utilities;
using PhantomBrigade;
using UnityEngine;

namespace it.miketan.PilotSerial
{
    [HarmonyPatch]
    public class Patch
    {
        [HarmonyPatch(typeof(CIViewBasePilotInfoExtended), "RedrawForPilot")]
        [HarmonyPostfix]
        public static void ApplyPilotSn(PersistentEntity pilot)
        {
            Debug.LogFormat("[PS] - Applicazione seriale del pilota...");
            ApplyOnEditStartCustom(pilot);
        }

        [HarmonyPatch(typeof(CIViewBaseEditor), "OnEditStart")]
        [HarmonyPostfix]
        internal static void EditStartPostfix(PersistentEntity pilot)
        {
            if (pilot == null)
            {
                pilot = IDUtility.GetLinkedPilot(pilot);
            }

            ApplyOnEditStartCustom(pilot);
        }

        internal static void ApplyOnEditStartCustom(PersistentEntity pilot)
        {
            //Se non trova il pilota, esci in sicurezza.
            if (pilot == null || !pilot.isPilotTag || pilot.isDestroyed)
            {
                Debug.LogWarningFormat("[PS] - Pilota non valido.");
                return;
            }


            string pilotSn = PilotSnUtility.GetOrCreate(pilot);
            string bioValue = pilot.hasPilotBio ? pilot.pilotBio.s : string.Empty;

            //Controllo se nella descrizione del pilota vi sia già il codice o meno.
            if (!bioValue.Contains(pilotSn))
            {
                pilot.ReplacePilotBio(bioValue + "\n\n" + "[b]Pilot S/N: [/b]" + pilotSn);
                Debug.LogFormat($"[PS] - Codice Seriale pilota assegnato a {pilot}: {pilotSn} .");
            }
            else
            {
                Debug.LogWarningFormat(
                    $"[PS] - Il codice Seriale del pilota risulta essere già presente in cache. S/N: {pilotSn}");
            }
        }
    }
}