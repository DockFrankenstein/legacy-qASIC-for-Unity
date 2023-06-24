using qASIC.Input.Serialization;
using System.Collections.Generic;

namespace qASIC.Input.Map.ItemData
{
    [System.Serializable]
    public class InputBindingData : InputMapItemData
    {
        public List<string> keys = new List<string>();
    }
}