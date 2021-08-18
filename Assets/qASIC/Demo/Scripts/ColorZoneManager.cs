using System.Collections;
using UnityEngine;

namespace qASIC.Demo.ColorZones
{
	public class ColorZoneManager : MonoBehaviour
	{
		[System.Serializable]
		public class ColorZone
        {
			public Color backgroundColorMain;
			public Color backgroundColorSecondary;

			[Space]
			public Color wallColor;
			public Color playerColor;
			public Color interactableColor;
			public Color textColor;

			public static ColorZone Lerp(ColorZone a, ColorZone b, float t)
            {
				ColorZone zone = new ColorZone();
				zone.backgroundColorMain = Color.Lerp(a.backgroundColorMain, b.backgroundColorMain, t);
				zone.backgroundColorSecondary = Color.Lerp(a.backgroundColorSecondary, b.backgroundColorSecondary, t);

				zone.wallColor = Color.Lerp(a.wallColor, b.wallColor, t);
				zone.playerColor = Color.Lerp(a.playerColor, b.playerColor, t);
				zone.interactableColor = Color.Lerp(a.interactableColor, b.interactableColor, t);
				zone.textColor = Color.Lerp(a.textColor, b.textColor, t);

				return zone;
			}
        }

		public static ColorZoneManager Singleton { get; private set; }

		public ColorZone[] colorZones;

		public ColorZone current;
		public int index = 0;

		private void Awake()
        {
            if(Singleton == null)
            {
				Singleton = this;
				return;
            }
			Destroy(this);
        }

        private void Start()
        {
			if (colorZones.Length > 0) current = colorZones[0];
		}

        public void ChangeColorZone(int index)
        {
			if (index == this.index) return;

			if(index >= colorZones.Length || index < 0)
            {
				qDebug.LogError("Color zone index is out of range!");
				return;
            }

			this.index = index;
			StartCoroutine(ChangeColorZone(colorZones[index]));
        }

		IEnumerator ChangeColorZone(ColorZone newZone)
        {
			ColorZone start = current;
			float time = 0f;
			while(time <= 1f)
            {
				yield return null;
				time += Time.deltaTime;
				current = ColorZone.Lerp(start, newZone, Mathf.Clamp01(time));
            }
        }
    }
}