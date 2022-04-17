using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace qASIC.AudioManagement.Menu
{
    [AddComponentMenu("qASIC/Options/Audio Menu Setting")]
    [RequireComponent(typeof(Slider))]
    public class AudioMenuSetting : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] Slider slider;
        [Header("Updating name")]
        public TextMeshProUGUI nameText;
        public string parameterLabelName;

        [Header("Settings")]
        public string parameterName;

#if UNITY_EDITOR
        private void Reset()
        {
            slider = GetComponent<Slider>();
            if (slider == null) return;

            slider.minValue = 0;
            slider.maxValue = 1;

            UnityEditor.Events.UnityEventTools.AddPersistentListener(slider.onValueChanged, PreviewVolume);
        }
#endif

        public void OnPointerUp(PointerEventData eventData)
        {
            if (slider == null) return;
            SetVolume(slider.value, false);
        }

        public void PreviewVolume(float value) =>
            SetVolume(value, true);

        public void SetVolume(float value, bool preview) =>
            AudioManager.SetVolume(parameterName, value, preview);

        private void Start() =>
            Initialize();

        public void Initialize()
        {
            if (slider == null)
                InitSlider();

            if (AudioManager.GetVolume(parameterName, out float newValue))
                slider.SetValueWithoutNotify(newValue);
        }

        void InitSlider()
        {
            slider = GetComponent<Slider>();
            if (slider == null) return;
            slider.onValueChanged.AddListener((float value) => SetVolume(value, true));
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