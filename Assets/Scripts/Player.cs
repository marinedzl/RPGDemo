using UnityEngine;

public enum Class
{
	Knight,
	Swordsman,
	Archer,
	Mage,
}

public class Player : MonoBehaviour
{
	Role m_role;

	void Start()
	{
		m_role = GetComponent<Role>();
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			m_role.Attack();
			return;
		}

		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");

		if (m_role.IsAttacking())
		{
			// do nothing
		}
		else
		{
			if (x != 0 || y != 0)
			{
				m_role.Move();

				if (m_role.IsMoving())
				{
					float moveSpeed = m_role.m_MoveSpeed;
					float rotateSpeed = moveSpeed * 3;

					Vector3 dir = Camera.main.transform.TransformDirection(new Vector3(x, 0, y));
					dir = dir.normalized;

					Vector3 motion = dir * moveSpeed * Time.deltaTime;
					motion.y = -5;

					m_role.Move(motion);
					m_role.Rotate(dir, rotateSpeed * Time.deltaTime);
				}
			}
			else
			{
				if (m_role.IsMoving())
				{
					m_role.Stand();
				}
			}
		}
	}
}
