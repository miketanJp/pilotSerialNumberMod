using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;
using YamlDotNet.Serialization;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using it.miketan.PilotSerial.Models;
using PhantomBrigade.Data;
using PhantomBrigade.Mods;

namespace it.miketan.PilotSerial.Utilities
{
    // Classe utility per generare e immagazzinare in cache i numeri seriali dei piloti:
    internal static class PilotSnUtility
    {
        private static bool _loaded;
        private static string saveName;
        
/*        === Dictionary _serialCache crea il seguente output per il file YAML: ===
            serialCache:
              pilot_1: [pilota standard ricavato da save_internal/quicksave/custom]
                serial: [PIL-XXX-XXX]
                pilotFaction: [Phantoms/Invaders]
*/
        private static readonly Dictionary<string, PilotEntry> _serialCache = new Dictionary<string, PilotEntry>();

        private static readonly CacheRoot root = new CacheRoot
        {
            serialCache = _serialCache
        };

        private static string CacheDirectory
        {
            get
            {
                try
                {
                    saveName = DataManagerSave.saveName;
                    Debug.LogFormat("[PSN] - Cache directory name: " + saveName);
                    
                    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return Path.Combine(dir, saveName);
                }
                catch
                {
                    Debug.LogWarningFormat("[PSN] - Cache directory could not be located. Creating it.");
                    return Path.Combine(Environment.CurrentDirectory, saveName);
                }
            }
        }

        private static string CacheFilePath => Path.Combine(CacheDirectory, "cache.yaml");

        // Genera un seriale unico per l'entità del pilota.
        // Da esso si preleva l'id della relativa entità creando la relazione pilota-S/N

        public static string GetOrCreate(PersistentEntity pilot)
        {
            if (pilot == null) return null;
            var name = pilot.nameInternal.s;
            var faction = pilot.faction.s;

            List<string> bannedPilotList = new List<string>()
            {
                "pb_pilot_01",
                "pb_pilot_02",
                "pb_pilot_03",
                "pb_pilot_04",
            };
            
            EnsureLoaded();

            if (!_serialCache.TryGetValue(name, out var entry))
            {
                entry = new PilotEntry();
                _serialCache[name] = entry;
            }

            if (string.IsNullOrEmpty(entry.serial))
            {
                entry.serial = GeneratePilotSn();
            }

            if (string.IsNullOrEmpty(entry.pilotFaction))
            {
                entry.pilotFaction = faction;
            }

            //Filtra i piloti non serializzando le mappe per i piloti starter a inizio campagna;
            //Evita che in differenti salvataggi, essendo i nameInternal uguali, si ritrovano lo stesso seriale.
            switch (bannedPilotList.Contains(name))
            {
                case false:
                    TrySaveCacheYaml(CacheFilePath); //Salva le modifiche su un file di cache;
                    Debug.LogFormat("[PSN] - name: " + name + " - " + "SERIAL NOT AVAILABLE FOR STARTER PB PILOTS.");
                    break;
                case true:
                    Debug.LogFormat("[PSN] - name: " + name);
                    break;
            }

            return bannedPilotList.Contains(name) ? "" : entry.serial;

        }

        // Genera un seriale dal pattern PIL-XXXX-YYYY [A-Z][0-9]
        // Il formato può può essere cambiato liberamente.
        private static string GeneratePilotSn()
        {
            const int length = 8;
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var sb = new StringBuilder(length);
            var bytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            foreach (var t in bytes)
            {
                sb.Append(alphabet[t % alphabet.Length]);
            }

            var token = sb.ToString();
            return $"PIL-{token.Substring(0, 4)}-{token.Substring(4, 4)}";
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            
            try
            {
                if (File.Exists(CacheFilePath))
                {
                    var yaml = File.ReadAllText(CacheFilePath);
                    var deserializer = new DeserializerBuilder().Build();
                    var loaded = deserializer.Deserialize<IDictionary<string, Dictionary<string, string>>>(yaml);

                    if (loaded != null && root.serialCache != null)
                    {
                        foreach (var kvp in root.serialCache)
                            _serialCache[kvp.Key] = kvp.Value ?? new PilotEntry();
                    }
                }
                _loaded = true;
            }
            catch
            {
                // Non blocca, ma _loaded rimarrà a false.
            }

            Debug.LogFormat("[PSN] - entries loaded = " + _serialCache.Count + " | " + _loaded);
        }


        private static void TrySaveCacheYaml(string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var serializer = new SerializerBuilder().Build();
                var yaml = serializer.Serialize(root);
                File.WriteAllText(path, yaml);
            }
            catch (Exception)
            {
                // Evita di rompere il gioco
            }
        }
    }
}