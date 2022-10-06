using System.Linq;
using System;
using qASIC.Tools;
using qASIC.Input.Internal.KeyProviders;
using System.Collections.Generic;
using System.Reflection;

namespace qASIC.Input.Map.Internal
{
    public static class InputMapEditorUtility
    {
        public static void RebuildActionsKeyList(InputMap map)
        {
            //if (map == null) return;

            //IEnumerable<InputBinding> items = map.groups
            //    .SelectMany(x => x.items)
            //    .OfType<InputBinding>();

            //KeyTypeProvider[] providers = KeyTypeProviders;

            //foreach (var item in items)
            //{
            //    List<string> savedKeys = new List<string>(item.keys);
            //    Dictionary<string, InputBinding.KeyList> savedKeysDictionary = item.keys
            //        .ToDictionary(x => )

            //    item.keys.Clear();
            //    foreach (var provider in providers)
            //    {
            //        string typeName = provider.KeyType.AssemblyQualifiedName;
            //        InputBinding.KeyList list = new InputBinding.KeyList(typeName);
            //        if (savedKeysDictionary.ContainsKey(typeName))
            //            list.keys = savedKeysDictionary[typeName].keys;

            //        item.keys.Add(list);
            //    }
            //}
        }
    }
}