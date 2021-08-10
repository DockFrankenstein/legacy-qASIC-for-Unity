using UnityEngine;

namespace qASIC.Demo.ColorZones
{
	public class WallColorAssigner : MonoBehaviour
	{
        SpriteRenderer[] walls;

        private void Awake()
        {
            walls = new SpriteRenderer[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                walls[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (ColorZoneManager.Singleton == null) return;

            for (int i = 0; i < walls.Length; i++)
                if (walls[i] != null)
                    walls[i].color = ColorZoneManager.Singleton.current.wallColor;
        }
    }
}