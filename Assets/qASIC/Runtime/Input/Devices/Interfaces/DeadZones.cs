using UnityEngine;

namespace qASIC.Input.Devices
{
    public interface ITriggerDeadZone : ILeftTriggerDeadZone, IRightTriggerDeadZone
    {

    }

    public interface ILeftTriggerDeadZone
    {
        Vector2 LeftTriggerDeadZone { get; set; }
    }

    public interface IRightTriggerDeadZone
    {
        Vector2 RightTriggerDeadZone { get; set; }
    }

    public interface IStickDeadZone : ILeftStickDeadZone, IRightStickDeadZone
    {

    }

    public interface ILeftStickDeadZone
    {
        Vector2 LeftStickDeadZone { get; set; }
    }

    public interface IRightStickDeadZone
    {
        Vector2 RightStickDeadZone { get; set; }
    }
}