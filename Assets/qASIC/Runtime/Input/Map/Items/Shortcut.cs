using System;
using qASIC.Input.Map.ItemData;
using UnityEngine;
using System.Collections.Generic;
using qASIC.Input.Devices;
using System.Linq;

namespace qASIC.Input.Map
{
    public class Shortcut : InputMapItem<bool>, ISerializableMapItem
    {
        public Type DataHolderType => typeof(ShortcutData);
        public override Color ItemColor => new Color(0.6f, 0f, 1f);

        public List<string> keys = new List<string>();

        public InputMapItemData CreateDataHolder()
        {
            return new ShortcutData()
            {
                keys = keys,
            };
        }

        public override bool GetHighestValue(bool a, bool b) =>
            a || b;

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device)
        {
            var keyPaths = keys
                .GroupBy(x => x.Split('/').First())
                .Select(x => x.Key)
                .ToList();

            InputEventType eventType = InputEventType.None;
            foreach (var path in keyPaths)
                eventType |= GetInputEventFromKeyPath(device, path);

            return eventType;
        }

        InputEventType GetInputEventFromKeyPath(IInputDevice device, string path)
        {
            var pathKeys = keys
                .Where(x => x.Split('/').First() == path)
                .ToList();

            var pressedKeys = device.Values
                .Where(x => x.Value > 0f)
                .Select(x => x.Key);

            var additionalPressedKeys = pressedKeys
                .Except(pathKeys)
                .ToList();

            bool isUp = false;
            foreach (var key in additionalPressedKeys)
            {
                var flags = device.GetInputEvent(key);

                //If the key has been pressed this tick, the shortcut will be up
                if (flags.HasFlag(InputEventType.Down))
                {
                    isUp = true;
                    continue;
                }

                //If the key has been pressed before, the shortcut cannot be triggered
                if (flags.HasFlag(InputEventType.Pressed))
                    return InputEventType.None;
            }

            bool isDown = false;
            foreach (var key in pathKeys)
            {
                var flags = device.GetInputEvent(key);
                if (flags.HasFlag(InputEventType.Down))
                    isDown = true;

                if (flags.HasFlag(InputEventType.Up))
                    isUp = true;

                if (flags == InputEventType.None)
                    return InputEventType.None;
            }

            if (isUp)
                return InputEventType.Up;

            return isDown ?
                InputEventType.Pressed | InputEventType.Down :
                InputEventType.Pressed;
        }

        public override bool ReadValue(InputMapData data, IInputDevice device) =>
            GetInputEvent(data, device).HasFlag(InputEventType.Pressed);

        /// <summary>Checks if there are any unassigned paths in the binding</summary>
        /// <returns>A list of all unassigned item indexes</returns>
        public List<int> HasUnassignedPaths() =>
            keys
            .Select((x, i) => InputMapUtility.GetProviderFromPath(x) == null ? i : -1)
            .Where(x => x != -1)
            .ToList();
    }
}