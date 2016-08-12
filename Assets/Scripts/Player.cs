using UnityEngine;

public class Player : MonoBehaviour
{
	Role m_role;

	void Start()
	{
		m_role = GetComponent<Role>();
	}
	
	void Update()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		if (h != 0 || v != 0)
		{
			float rotateSpeed = 10;
			Vector3 dir = Camera.main.transform.TransformDirection(new Vector3(h, 0, v));
			dir = dir.normalized;
			m_role.Lookat(dir, rotateSpeed * Time.deltaTime);
			m_role.Run(true);
		}
		else
		{
			m_role.Run(false);
		}

		if (Input.GetMouseButtonUp(0))
		{
			m_role.Attack();
		}
	}
}
