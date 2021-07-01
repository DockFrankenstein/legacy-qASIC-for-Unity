using UnityEngine;

namespace qASIC.Demo.Dialogue
{
	public class BackgroundAnimation : MonoBehaviour
	{
		public AnimationCurve curve;
		public Color startColor;
		public Color endColor;

        public float duration = 2f;

        Camera can;
        float time;

        private void Awake()
        {
            can = GetComponent<Camera>();
        }

        private void Update()
        {
            time += Time.deltaTime;
            can.backgroundColor = Color.Lerp(startColor, endColor, curve.Evaluate(time / duration));
            time %= duration;
        }
    }
}