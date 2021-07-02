using UnityEngine;
using qASIC.Demo.ColorZones;

namespace qASIC.Demo.Dialogue
{
	public class BackgroundAnimation : MonoBehaviour
	{
		public AnimationCurve curve;

        public float duration = 2f;

        Camera can;
        float time;

        private void Awake()
        {
            can = GetComponent<Camera>();
        }

        private void Update()
        {
            if (ColorZoneManager.singleton == null) return;
            Color startColor = ColorZoneManager.singleton.current.backgroundColorMain;
            Color endColor = ColorZoneManager.singleton.current.backgroundColorSecondary;

            time += Time.deltaTime;
            can.backgroundColor = Color.Lerp(startColor, endColor, curve.Evaluate(time / duration));
            time %= duration;
        }
    }
}