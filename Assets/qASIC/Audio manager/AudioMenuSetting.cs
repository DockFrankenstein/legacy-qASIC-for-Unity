using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace qASIC.AudioManagment.Menu
{
    [RequireComponent(typeof(Slider))]
    public class AudioMenuSetting : MonoBehaviour, IPointerUpHandler
    {
        private Slider slider;
        [Header("Updating name")]
        public TextMeshProUGUI nameText;
        public string parameterLabelName;

        [Header("Options")]
        public string parameterName;

        private void Reset()
        {
            Slider slider = GetComponent<Slider>();
            if (slider == null) slider = gameObject.AddComponent<Slider>();
            slider.minValue = -60;
            slider.maxValue = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (slider == null) return;
            AudioManager.SetFloat(parameterName, slider.value, true);
        }

        public void SetValue(float value, bool preview) => AudioManager.SetFloat(parameterName, value, preview);

        private void Start() => Initialize();

        public void Initialize()
        {
            slider = GetComponent<Slider>();
            if (slider == null) return;
            slider.onValueChanged.AddListener((float value) => SetValue(value, false));
            if (AudioManager.GetFloat(parameterName, out float newValue)) slider.SetValueWithoutNotify(newValue);
        }

        public virtual string GetLabel()
        {
            string value = "";
            if (slider != null) value = $"{Mathf.Round(slider.normalizedValue * 100)}%";
            return $"{parameterLabelName}{value}";
        }

        public virtual void Update()
        {
            if (nameText != null)
                nameText.text = GetLabel();
        }
    }
}