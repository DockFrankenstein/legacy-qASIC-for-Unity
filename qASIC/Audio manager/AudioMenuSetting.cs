using UnityEngine;
using UnityEngine.UI;

namespace qASIC.AudioManagment.Menu
{
    [RequireComponent(typeof(Slider))]
    public class AudioMenuSetting : MonoBehaviour
    {
        public string ParameterName;
        private float _value;

        public void PreviewValue(float value)
        {
            _value = value;
            AudioManager.ChangeParameterFloat(ParameterName, value, false);
        }
        public void SetValue() => AudioManager.ChangeParameterFloat(ParameterName, _value, true);

        private void Start() => Initialize();

        public void Initialize()
        {
            Slider slider = GetComponent<Slider>();
            if (slider != null && AudioManager.GetParameterFloat(ParameterName, out float value)) slider.value = value;
        }
    }
}