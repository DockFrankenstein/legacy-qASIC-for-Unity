using System;

namespace qASIC.Input.Map
{
    public interface ISerializableMapItem
    {
        Type DataHolderType { get; }

        InputMapItemData CreateDataHolder();
    }
}