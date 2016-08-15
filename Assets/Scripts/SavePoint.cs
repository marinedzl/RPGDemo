using UnityEngine;
using System.Collections;

public class SavePoint : MonoBehaviour
{
	void OnTriggerEnter(Collider collider)
	{
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null)
		{
			Game.OpenSaveData();
		}
	}
}
