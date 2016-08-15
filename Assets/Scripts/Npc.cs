using UnityEngine;

public class Npc : MonoBehaviour
{
	public class Info
	{
		public int id;
		public string name;
		public string desc;
		public string model;
		public float scaling;
	}

	public int m_id;

	public int id
	{
		get { return m_id; }
	}
}
