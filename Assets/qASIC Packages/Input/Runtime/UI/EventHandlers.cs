using UnityEngine.EventSystems;

namespace qASIC.Input.UI
{
    public interface IConfirmHandler : IEventSystemHandler
    {
        void OnConfirm(ButtonEventData eventData);
    }

    public interface ICancelHandler : IEventSystemHandler
    {
        void OnCancel(ButtonEventData eventData);
    }
}
