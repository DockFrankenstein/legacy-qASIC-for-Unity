using System;
using qASIC.Input.Map;
using UnityEngine;

namespace qASIC.Input
{
    public enum MapItemType
    {
        InputBinding = 0,
        Input1DAxis = 1,
        Input2DAxis = 2,
        Input3DAxis = 3,
        Shortcut = 4,
    }

    public class MapItemTypeAttribute : PropertyAttribute
    {
        public Type ItemType { get; private set; }

        public MapItemTypeAttribute(Type type)
        {
            ItemType = type;
        }

        public MapItemTypeAttribute(MapItemType type)
        {
            switch (type)
            {
                case MapItemType.InputBinding:
                    ItemType = typeof(InputBinding);
                    break;
                case MapItemType.Input1DAxis:
                    ItemType = typeof(Input1DAxis);
                    break;
                case MapItemType.Input2DAxis:
                    ItemType = typeof(Input2DAxis);
                    break;
                case MapItemType.Input3DAxis:
                    ItemType = typeof(Input3DAxis);
                    break;
                case MapItemType.Shortcut:
                    ItemType = typeof(Shortcut);
                    break;
            }
        }
    }
}