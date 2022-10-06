using UnityEngine;
using qASIC.Samples.Pedestal.ColorZones;

namespace qASIC.Samples.Pedestal.Dialogue
{
#if !qASIC_DEV
	[AddComponentMenu("")]
#endif
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
            if (ColorZoneManager.Singleton == null) return;
            Color startColor = ColorZoneManager.Singleton.current.backgroundColorMain;
            Color endColor = ColorZoneManager.Singleton.current.backgroundColorSecondary;

            time += Time.deltaTime;
            can.backgroundColor = Color.Lerp(startColor, endColor, curve.Evaluate(time / duration));
            time %= duration;
        }
    }
}