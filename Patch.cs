using System.Collections.Generic;
using HarmonyLib;
using it.miketan.PilotSerial.PilotSN;
using PhantomBrigade;
using PhantomBrigade.Data;
using PhantomBrigade.Overworld;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;

namespace it.miketan.PilotSerial
{
    [HarmonyPatch]
    public class Patch
    {
        [HarmonyPatch(typeof(CIViewBasePilotInfoExtended), "RedrawForPilot")]
        [HarmonyPostfix]
        internal static void ApplyPilotSn(PersistentEntity pilot)
        {
            Debug.LogFormat("[PS] - In esecuzione...");

            ApplyOnEditStartCustom(pilot);

            Debug.LogFormat("[PS] - Esecuzione completata.");
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


        private static void ApplyOnEditStartCustom(PersistentEntity pilot)
        {
            if (pilot == null)
            {
                Debug.LogWarning("[PS] - Pilota nullo.");
                return;
            }
            
            float pilotSnFloat = PilotSnUtility.GetOrCreateFloat(pilot);
            
            //Metodi usati per generare la stringa alfanumerica; OverworldUtility non ha un TryGetMemory per il tipo Stringa.
            //string bioValue = pilot.hasPilotBio ? pilot.pilotBio.s : string.Empty;
            //string pilotSn = PilotSnUtility.GetOrCreate(pilot);

            //bool snMemoryFound = OverworldUtility.TryGetMemoryFloat(pilot, "pilot_info_stats_sn", out string value);
            //value = pilotSnSn;

            // Controllo se il pilota è valido
            if (!pilot.isPilotTag)
            {
                Debug.LogWarningFormat("[PS] - Pilota non valido.");
                return;
            }

            bool snFMemoryFound = OverworldUtility.TryGetMemoryFloat(pilot, "pilot_info_stats_sn", out float value);
            value = pilotSnFloat;
            
            // Controllo se al pilota è già stato assegnato il S/N
            if (!snFMemoryFound)
            {
                OverworldUtility.SetMemoryFloat(pilot, "pilot_info_stats_sn", value);
                Debug.LogFormat($"[PS] - Codice Seriale pilota assegnato a {pilot.nameInternal.s}: {value}.");
            }
            else
            {
                Debug.LogWarningFormat($"[PS] - Il codice Seriale del pilota è già presente in cache. S/N: {value}");
            }
        }
    }
}