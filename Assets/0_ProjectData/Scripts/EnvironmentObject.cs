using UnityEngine;

namespace PiratesNS
{
	public class EnvironmentObject : MonoBehaviour
	{
		public void Move(Vector2 direction) {
            transform.position += (Vector3)direction;
        }
	}
}
