using UnityEngine;

public class Role : MonoBehaviour
{
	Animator m_animator;

	void Start()
	{
		m_animator = GetComponent<Animator>();
	}
	
	void Update()
	{

	}

	public void Run(bool run)
	{
		m_animator.SetBool("Run", run);
	}

	public void Attack()
	{
		m_animator.SetTrigger("Attack");
	}

	public void Lookat(Vector3 dir, float lerp)
	{
		dir.y = 0;
		Quaternion lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, lerp);
	}
}
