using UnityEngine;

public class Monster : MonoBehaviour
{
	public class Info
	{
		public int id;
		public string name;
		public string desc;
		public string model;
		public float scaling;
		public int hp;
		public int atk;
		public int def;
		public int exp;
	}

	public int m_id;

	public int id
	{
		get { return m_id; }
	}
}
