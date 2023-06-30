using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace qASIC.Files.Serialization
{
    [Serializable]
    public class ObjectSerializer
    {
        public ObjectSerializer() { }
        public ObjectSerializer(SerializationProvider provider) : this()
        {
            this.provider = provider;
        }

        public SerializationProvider Provider => provider;

        [SerializeReference] SerializationProvider provider;
        public AdvancedGenericFilePath filepath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "input.txt", "input-editor.txt");

        #region Static
        private static List<Type> _providerTypes = null;
        public static List<Type> ProviderTypes 
        { 
            get
            {
                if (_providerTypes == null)
                    _providerTypes = TypeFinder.FindAllTypesList<SerializationProvider>();

                return _providerTypes;
            }
        }

        public static List<Type> GetAvaliableProviderTypes(Type objectType = null, bool useGenericProviders = true)
        {
            var providers = TypeFinder.CreateConstructorsFromTypesList<SerializationProvider>(ProviderTypes);

            Dictionary<string, Type> types = new Dictionary<string, Type>();
            foreach (var item in providers)
            {
                if (item.ObjectType == objectType)
                {
                    types.SetOrAdd(item.SerializationType, item.GetType());
                    continue;
                }

                if (!types.ContainsKey(item.SerializationType) && useGenericProviders)
                {
                    types.Add(item.SerializationType, item.GetType());
                    continue;
                }
            }

            return types
                .Select(x => x.Value)
                .ToList();
        }
        #endregion

        public void Serialize(object obj)
        {
            if (provider == null)
            {
                qDebug.LogError("Cannot serialize, provider has not been selected!");
                return;
            }

            string txt = provider.SerializeObject(obj);
            if (provider.SavesToFile)
            {
                string path = filepath.GetFullPath();
                File.WriteAllText(path, txt);
            }
        }

        public T Deserialize<T>() =>
            (T)Deserialize(typeof(T));

        public object Deserialize(Type type)
        {
            if (provider == null)
            {
                qDebug.LogError("Cannot deserialize, provider has not been selected!");
                return null;
            }

            string txt = string.Empty;

            if (provider.SavesToFile)
            {
                string path = filepath.GetFullPath();
                if (!File.Exists(path))
                    return null;

                txt = File.ReadAllText(path);
            }

            return provider.DeserializeObject(txt, type);
        }
    }
}