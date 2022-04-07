#if UNITY_EDITOR
using qASIC.InputManagement.Map;
using qASIC.ProjectSettings;

namespace qASIC.InputManagement.Internal
{
    public static class EditorInputManager
    {
        public static InputMap Map { get => InputProjectSettings.Instance.map; }
    }
}
#endif