/*using HarmonyLib;
using it.miketan.PilotSerial.Contexts;
using it.miketan.PilotSerial.Entities;

namespace it.miketan.PilotSerial.PilotSN
{
    [HarmonyPatch(typeof(PersistentEntity), "GetLinkedPilot")]
    public static class IDUtilityCustom
    {
        [HarmonyPostfix]
        public static void GetLinkedPilotPostfix(PersistentEntity __instance, ref PersistentEntity __result)
        {
            if (__instance == null || !__instance.hasEntityLinkPilot)
                return;

            // Recupera l'entità linkata
            var linkedEntity = ContextsCustom.sharedInstance.persistent.GetEntityWithId(__instance.entityLinkPilot.persistentID);

            // Se vuoi usare solo i custom pilot, prova a castare in sicurezza
            if (linkedEntity is PersistentEntityCustom linkedCustom)
            {
                // Qui puoi iniettare il tuo codice alla fine
                // Esempio: assegna ID memoria se non presente
                string key = "PilotID";
                if (!linkedCustom.TryGetMemorySnValue(key, out string pilotId))
                {
                    pilotId = System.Guid.NewGuid().ToString();
                    linkedCustom.SetMemorySnValue(key, pilotId);
                }

                // Aggiorna il risultato del Postfix
                __result = linkedCustom;
            }
            else
            {
                // Se non è custom, lasciamo l'oggetto originale
                __result = linkedEntity;
            }
        }
    }
}*/