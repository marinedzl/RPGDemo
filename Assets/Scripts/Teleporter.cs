using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public string m_toMap;

	void OnTriggerEnter(Collider collider)
	{
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null)
		{
			Game.ChangeMap(m_toMap);
		}
	}
}
