namespace qASIC.InputManagement.Devices
{
    public interface IGamepadDevice : IInputDevice
    {
        float DeadZoneInner { get; set; }
        float DeadZoneOuter { get; set; }

        void SetName(string name);
    }
}