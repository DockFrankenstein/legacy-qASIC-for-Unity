using qASIC.Input.Internal.KeyProviders;
using qASIC.Tools;
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
                        .ToDictionary(x => x.KeyName);

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

        public static KeyTypeProvider GetProviderByRootPath(string rootPath)
        {
            var targets = KeyTypeProviders.Where(x => x.KeyName == rootPath);

            if (targets.Count() == 1)
                return targets.First();

            return null;
        }

        public static KeyTypeProvider GetProviderFromPath(string path)
        {
            string rootPath = path.Split('/').FirstOrDefault();
            return GetProviderByRootPath(rootPath);
        }
    }
}