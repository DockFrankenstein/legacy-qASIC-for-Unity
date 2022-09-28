using UnityEngine;

namespace qASIC.InputManagement.Devices
{
    public interface IDeadZone
    {
        Vector2 DeadZone { get; set; }
    }
}