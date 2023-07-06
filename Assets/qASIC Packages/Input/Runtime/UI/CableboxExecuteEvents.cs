using UnityEngine.EventSystems;

namespace qASIC.Input.UI
{
    public static class CableboxExecuteEvents
    {
        public static ExecuteEvents.EventFunction<IConfirmHandler> ConfirmHandler { get; } = Execute;
        private static void Execute(IConfirmHandler handler, BaseEventData eventData) =>
            handler.OnConfirm(ExecuteEvents.ValidateEventData<ButtonEventData>(eventData));

        public static ExecuteEvents.EventFunction<ICancelHandler> CancelHandler { get; } = Execute;
        private static void Execute(ICancelHandler handler, BaseEventData eventData) =>
            handler.OnCancel(ExecuteEvents.ValidateEventData<ButtonEventData>(eventData));
    }
}
