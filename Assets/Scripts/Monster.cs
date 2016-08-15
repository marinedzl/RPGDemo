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
	}

	public int m_id;

	public int id
	{
		get { return m_id; }
	}

	void Start()
	{
	}

	void Update()
	{

	}
}
