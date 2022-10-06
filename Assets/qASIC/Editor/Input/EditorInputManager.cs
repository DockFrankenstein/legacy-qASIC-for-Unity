#if UNITY_EDITOR
using qASIC.Input.Map;
using qASIC.ProjectSettings;

namespace qASIC.Input.Internal
{
    public static class EditorInputManager
    {
        public static InputMap Map { get => InputProjectSettings.Instance.map; }
    }
}
#endif