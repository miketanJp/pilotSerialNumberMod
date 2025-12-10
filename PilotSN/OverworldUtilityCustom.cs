using System;
using System.Collections.Generic;

namespace it.miketan.PilotSerial.PilotSN
{
    public static class OverworldUtilityCustom
    {
        // Mappa statica per associare dati custom a qualsiasi PersistentEntity
        private static Dictionary<PersistentEntity, SortedDictionary<string, string>> memory
            = new Dictionary<PersistentEntity, SortedDictionary<string, string>>();

        public static bool TryGetMemorySnValue(this PersistentEntity entity, string key, out string value)
        {
            value = "";
            if (entity == null || string.IsNullOrEmpty(key))
                return false;

            if (memory.TryGetValue(entity, out var dict) && dict.ContainsKey(key))
            {
                value = dict[key];
                return true;
            }

            return false;
        }

        public static void SetMemorySnValue(this PersistentEntity entity, string key, string value)
        {
            if (entity == null || string.IsNullOrEmpty(key))
                return;

            if (!memory.TryGetValue(entity, out var dict))
            {
                dict = new SortedDictionary<string, string>();
                memory[entity] = dict;
            }

            dict[key] = value;
        }
    }
}