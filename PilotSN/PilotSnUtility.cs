using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace it.miketan.PilotSerial.PilotSN
{
    public class PilotSnUtility
    {
        private static Dictionary<int, string> _stringCache = new Dictionary<int, string>();
        private static Dictionary<int, float> _floatCache = new Dictionary<int, float>();


        public static string GetOrCreate(PersistentEntity pilot)
        {
            if (pilot == null) return null;
            int id = pilot.id.id;

            if (!_stringCache.TryGetValue(id, out string serial))
            {
                serial = GeneratePilotSn();
                _stringCache[id] = serial;
            }

            return serial;
        }
        
        public static float GetOrCreateFloat(PersistentEntity pilot)
        {
            if (pilot == null) return 0f;

            int id = pilot.id.id;

            if (!_floatCache.TryGetValue(id, out float value))
            {
                value = GenerateFloatValue();
                _floatCache[id] = value;
            }

            return value;
        }
        
        

        private static string GeneratePilotSn()
        {
            const int length = 8;
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(alphabet[b % alphabet.Length]);

            return $"PIL-{sb.ToString().Substring(0, 4)}-{sb.ToString().Substring(4, 4)}";
        }
        
        private static float GenerateFloatValue()
        {
            // Genera un numero compreso tra 0.0 e 1.0
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            uint raw = BitConverter.ToUInt32(bytes, 0);
            return (raw / (float)uint.MaxValue);
        }

    }
}