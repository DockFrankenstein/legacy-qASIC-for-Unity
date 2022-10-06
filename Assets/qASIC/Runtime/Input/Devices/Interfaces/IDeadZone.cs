using UnityEngine;

namespace qASIC.Input.Devices
{
    public interface IDeadZone
    {
        Vector2 DeadZone { get; set; }
    }
}