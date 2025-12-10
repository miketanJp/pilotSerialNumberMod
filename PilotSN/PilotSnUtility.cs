using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace it.miketan.PilotSerial.Utilities
{
    public class PilotSnUtility
    {
        private static Dictionary<int, string> _cache = new Dictionary<int, string>();
        
        public static string GetOrCreate(PersistentEntity pilot)
        {
            if (pilot == null) return null;
            int id = pilot.id.id;

            if (!_cache.TryGetValue(id, out string serial))
            {
                serial = GeneratePilotSn();
                _cache[id] = serial;
            }

            return serial;
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
    }
}