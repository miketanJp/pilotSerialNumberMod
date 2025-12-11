using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;

namespace it.miketan.PilotSerial.utilities
{
    // Classe utility per generare e immagazzinare in cache i numeri seriali dei piloti.
    public static class PilotSnUtility
    {
        private static readonly Dictionary<string, string>
            _serialCache =
                new Dictionary<string, string>(); // Cache in memoria con i seguenti input: pilot id -> serial

        private static bool _loaded;

        private static string CacheDirectory
        {
            get
            {
                try
                {
                    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return Path.Combine(dir, "it.miketan.PilotSerial.cache");
                }
                catch
                {
                    return Path.Combine(Environment.CurrentDirectory, "PilotSerialCache");
                }
            }
        }

        private static string CacheFilePath => Path.Combine(CacheDirectory, "pilot_sn_cache_session.yaml");

        // Genera un seriale unico per l'entità del pilota, da cui si preleva l'id della relativa entità creando la relazione pilota-S/N
        public static string GetOrCreate(PersistentEntity pilot)
        {
            if (pilot == null) return null;
            var name = pilot.nameInternal.s;

            Debug.LogFormat("PilotSerial: name = " + name);

            EnsureLoaded();

            if (!_serialCache.TryGetValue(name, out var serial))
            {
                serial = GeneratePilotSn();
                _serialCache[name] = serial;

                TrySaveCacheYaml(CacheFilePath,
                    _serialCache); //Salva le modifiche su un file di cache; ciò eviterà di ricreare gli ID a ogni sessione.
            }

            Debug.LogFormat("PilotSerial: name = " + name);

            return serial;
        }

        // Genera un seriale dal pattern PIL-XXXX-YYYY (A-Z, 0-9),
        // Il cui formato può essere cambiato a proprio piacimento
        private static string GeneratePilotSn()
        {
            const int length = 8;
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(length);
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(alphabet[bytes[i] % alphabet.Length]);
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
                    var loaded = deserializer.Deserialize<IDictionary<string, string>>(yaml);

                    if (loaded != null)
                    {
                        foreach (var kvp in loaded)
                            _serialCache[kvp.Key] = kvp.Value;
                    }
                }

                _loaded = true;

            }
            catch
            {
                // Non blocca, ma NON imposta _loaded = true
            }
            
            Debug.LogFormat("PilotSerial: entries loaded = " + _serialCache.Count + " | " + _loaded);
        }


        private static void TrySaveCacheYaml(string path, object data)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var serializer = new SerializerBuilder().Build();
                var yaml = serializer.Serialize(data);

                File.WriteAllText(path, yaml);
            }
            catch (Exception)
            {
                // Evita di rompere il gioco
            }
        }
    }
}