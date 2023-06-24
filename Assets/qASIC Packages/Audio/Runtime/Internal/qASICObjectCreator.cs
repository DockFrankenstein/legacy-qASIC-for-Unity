using UnityEditor;
using UnityEngine;

namespace qASIC.Audio.Internal
{
    public static partial class qASICObjectCreator
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/Audio/Controlled Audio Source", false, 3)]
        static void CreateAudioSource(MenuCommand command)
        {
            GameObject obj = new GameObject("Audio Source", typeof(AudioSource), typeof(Audio.AudioSourceController));
            if (command?.context is GameObject)
                obj.transform.SetParent((command.context as GameObject).transform);

            qASIC.Internal.qASICObjectCreator.FinishObject(obj);
        }
#endif
    }
}