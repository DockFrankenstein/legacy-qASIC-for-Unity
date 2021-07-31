using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace qASIC.AudioManagment.Menu
{
    [RequireComponent(typeof(Slider))]
    public class AudioMenuSetting : MonoBehaviour, IPointerUpHandler
    {
        private Slider _slider;

        public string ParameterName;

        private void Reset()
        {
            Slider slider = GetComponent<Slider>();
            if (slider == null) slider = gameObject.AddComponent<Slider>();
            slider.minValue = -60;
            slider.maxValue = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_slider == null) return;
            AudioManager.SetFloat(ParameterName, _slider.value, true);
        }

        public void SetValue(float value, bool preview) => AudioManager.SetFloat(ParameterName, value, preview);

        private void Start() => Initialize();

        public void Initialize()
        {
            _slider = GetComponent<Slider>();
            if (_slider == null) return;
            _slider.onValueChanged.AddListener((float value) => SetValue(value, false));
            if (AudioManager.GetFloat(ParameterName, out float newValue)) _slider.SetValueWithoutNotify(newValue);
        }
    }
}