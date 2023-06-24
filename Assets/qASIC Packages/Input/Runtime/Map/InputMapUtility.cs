using qASIC.Input.Internal.KeyProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace qASIC.Input.Map
{
    public static class InputMapUtility
    {
        private static KeyTypeProvider[] _keyTypeProviders = null;
        public static KeyTypeProvider[] KeyTypeProviders
        {
            get
            {
                if (_keyTypeProviders == null)
                    _keyTypeProviders = GetEveryKeyType();

                return _keyTypeProviders;
            }
        }

        private static Dictionary<string, KeyTypeProvider> _keyTypeProvidersDictionary = null;
        public static Dictionary<string, KeyTypeProvider> KeyTypeProvidersDictionary
        {
            get
            {
                if (_keyTypeProvidersDictionary == null)
                    _keyTypeProvidersDictionary = KeyTypeProviders
                        .ToDictionary(x => x.RootPath);

                return _keyTypeProvidersDictionary;
            }
        }

        public static KeyTypeProvider[] GetEveryKeyType()
        {
            IEnumerable<Type> deviceTypes = TypeFinder.FindAllTypes<KeyTypeProvider>()
                .Where(x => x != null && x.IsClass && !x.IsAbstract);

            List<KeyTypeProvider> types = new List<KeyTypeProvider>();
            foreach (var deviceType in deviceTypes)
            {
                ConstructorInfo constructor = deviceType.GetConstructor(Type.EmptyTypes);
                KeyTypeProvider device = (KeyTypeProvider)constructor.Invoke(null);
                types.Add(device);
            }

            return types
                .GroupBy(x => x.KeyType)
                .Select(x => x.First())
                .ToArray();
        }

        static string[] _keyList = null;
        public static string[] KeyList
        {
            get
            {
                if (_keyList == null)
                    _keyList = KeyTypeProviders
                        .SelectMany(x => x.GetKeyList()
                            .Select(y => $"{x.RootPath}/{y}"))
                        .ToArray();

                return _keyList;
            }
        }

        public static KeyTypeProvider GetProviderByRootPath(string rootPath)
        {
            var targets = KeyTypeProviders.Where(x => x.RootPath == rootPath);

            if (targets.Count() == 1)
                return targets.First();

            return null;
        }

        public static KeyTypeProvider GetProviderFromPath(string path)
        {
            string rootPath = path.Split('/').FirstOrDefault();
            return GetProviderByRootPath(rootPath);
        }

        public static bool IsGuidBroken<T>(InputMap map, string guid) where T : InputMapItem =>
            !string.IsNullOrWhiteSpace(guid) && map.GetItem<T>(guid) == null;

        public static bool TryGetItemFromPath(InputMap map, string groupName, string itemName, out InputMapItem item)
        {
            item = null;

            //REMOVEME: this is a temp fix, because the map gets set to null when recompiling during runtime
            if (map == null)
                return false;

            if (!map.TryGetGroup(groupName, out InputGroup group))
                return false;

            if (!group.TryGetItem(itemName, out item))
                return false;

            return true;
        }

        public static bool TryGetItemFromPath<T>(InputMap map, string groupName, string itemName, out InputMapItem<T> item)
        {
            item = null;

            if (!TryGetItemFromPath(map, groupName, itemName, out InputMapItem mapItem))
                return false;

            if (mapItem.ValueType != typeof(T))
                return false;

            item = mapItem as InputMapItem<T>;
            return true; 
        } 
    }
}