using UnityEngine;
using qASIC.Input.Map;
using System.Linq;

namespace qASIC.Input
{
    [System.Serializable]
    public class InputMapItemReference
    {
        [SerializeField] protected string guid;

        public string Guid => guid;

        public InputMapItemReference() { }

        public InputMapItemReference(string guid)
        {
            this.guid = guid;
        }

        public bool ItemExists() =>
            InputManager.Map?.ItemsDictionary.ContainsKey(guid) ?? false;

        public InputGroup GetGroup()
        {
            var item = GetItem();
            if (!InputManager.MapLoaded)
                return null;

            var targets = InputManager.Map.groups
            .Where(x => x.items.Contains(item));

            return targets.Count() == 1 ? targets.First() : null;
        }

        public InputMapItem GetItem()
        {
            if (!ItemExists())
                return null;

            return InputManager.Map.ItemsDictionary[guid];
        }

        public string GetGroupName() =>
            GetGroup()?.ItemName ?? string.Empty;

        public string GetItemName() =>
            GetItem()?.ItemName ?? string.Empty;

        #region Getting Input From Manager
        public bool GetInput() =>
            InputManager.GetInputFromGUID(guid);

        public bool GetInputUp() =>
            InputManager.GetInputUpFromGUID(guid);

        public bool GetInputDown() =>
            InputManager.GetInputDownFromGUID(guid);

        public T GetInputValue<T>() =>
            InputManager.GetInputValueFromGUID<T>(guid);

        public object GetInputValue() =>
            InputManager.GetInputValueFromGUID(guid);

        public InputEventType GetInputEvent() =>
            InputManager.GetInputEventFromGUID(guid);
        #endregion

        #region Getting Input From Players
        public bool GetInputFromPlayer(int playerIndex) =>
            InputManager.Players.IndexInRange(playerIndex) ? InputManager.Players[playerIndex].GetInputFromGUID(guid) : false;

        public bool GetInputUpFromPlayer(int playerIndex) =>
            InputManager.Players.IndexInRange(playerIndex) ? InputManager.Players[playerIndex].GetInputUpFromGUID(guid) : false;

        public bool GetInputDownFromPlayer(int playerIndex) =>
            InputManager.Players.IndexInRange(playerIndex) ? InputManager.Players[playerIndex].GetInputDownFromGUID(guid) : false;

        public T GetInputValueFromPlayer<T>(int playerIndex) =>
            InputManager.Players.IndexInRange(playerIndex) ? InputManager.Players[playerIndex].GetInputValueFromGUID<T>(guid) : default;

        public object GetInputValueFromPlayer(int playerIndex) =>
            InputManager.Players.IndexInRange(playerIndex) ? InputManager.Players[playerIndex].GetInputValueFromGUID(guid) : default;

        public InputEventType GetInputEventFromPlayer(int playerIndex) =>
            InputManager.Players.IndexInRange(playerIndex) ? InputManager.Players[playerIndex].GetInputEventFromGUID(guid) : default;
        #endregion
    }
}