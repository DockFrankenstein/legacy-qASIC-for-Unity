using UnityEngine;
using UnityEditor;

namespace qASIC.Input.Internal
{
    [CustomPropertyDrawer(typeof(MapItemTypeAttribute))]
    internal class MapItemTypeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type != nameof(InputMapItemReference))
            {
                GUI.Label(position, "Use [MapItemType] on InputMapItemReference only.");
                return;
            }

            var itemTypeAttribute = attribute as MapItemTypeAttribute;
            InputMapItemReferenceDrawer.DrawItemReferenceProperty(position, property, label, itemTypeAttribute.ItemType);
        }
    }
}