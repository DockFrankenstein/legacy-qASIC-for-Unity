using System.Linq;
using System;
using qASIC.Tools;
using qASIC.InputManagement.Internal.KeyProviders;
using System.Collections.Generic;
using System.Reflection;

namespace qASIC.InputManagement.Map.Internal
{
    public static class InputMapEditorUtility
    {
        public static void RebuildActionsKeyList(InputMap map)
        {
            if (map == null) return;

            IEnumerable<InputAction> actions = map.Groups
                .SelectMany(x => x.actions);

            KeyTypeProvider[] providers = KeyTypeProviders;

            foreach (var action in actions)
            {
                List<InputAction.KeyList> savedKeys = new List<InputAction.KeyList>(action.keys);
                Dictionary<string, InputAction.KeyList> savedKeysDictionary = savedKeys
                    .ToDictionary(x => x.keyType);

                action.keys.Clear();
                foreach (var provider in providers)
                {
                    string typeName = provider.KeyType.AssemblyQualifiedName;
                    InputAction.KeyList list = new InputAction.KeyList(typeName);
                    if (savedKeysDictionary.ContainsKey(typeName))
                        list.keys = savedKeysDictionary[typeName].keys;

                    action.keys.Add(list);
                }
            }
        }

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

        private static Dictionary<Type, KeyTypeProvider> _keyTypeProvidersDictionary = null;
        public static Dictionary<Type, KeyTypeProvider> KeyTypeProvidersDictionary
        {
            get
            {
                if (_keyTypeProvidersDictionary == null)
                    _keyTypeProvidersDictionary = KeyTypeProviders
                        .ToDictionary(x => x.KeyType);

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
    }
}