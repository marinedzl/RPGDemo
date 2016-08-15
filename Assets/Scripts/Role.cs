using UnityEngine;

public class Role : MonoBehaviour
{
	public float m_MoveSpeed = 0f;

	Animator m_animator;
	CharacterController m_cc;
	AnimatorStateInfo m_currentState;
	AnimatorStateInfo m_nextState;

	void Start()
	{
		m_animator = GetComponent<Animator>();
		m_cc = GetComponent<CharacterController>();
	}

	void Update()
	{
		m_currentState = m_animator.GetCurrentAnimatorStateInfo(0);
		m_nextState = m_animator.GetNextAnimatorStateInfo(0);
	}

	void ResetTrigger()
	{
		m_animator.ResetTrigger("Stand");
		m_animator.ResetTrigger("Move");
		m_animator.ResetTrigger("Attack");
		m_animator.ResetTrigger("Hit");
	}

	public void Rotate(Vector3 dir, float lerp)
	{
		dir.y = 0;
		Quaternion lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, lerp);
	}

	public void Move(Vector3 motion)
	{
		m_cc.Move(motion);
	}

	public bool IsMoving()
	{
		return (m_currentState.IsTag("Move") && !m_nextState.IsTag("Stand")) || m_nextState.IsTag("Move");
	}

	public bool IsAttacking()
	{
		return m_currentState.IsTag("Attack") || m_nextState.IsTag("Attack");
	}

	public bool IsAlive()
	{
		return true;
	}

	public void Move()
	{
		ResetTrigger();
		m_animator.SetTrigger("Move");
	}

	public void Stand()
	{
		ResetTrigger();
		m_animator.SetTrigger("Stand");
	}

	public void Attack()
	{
		ResetTrigger();
		m_animator.SetTrigger("Attack");
	}

	public void Hit(Role from)
	{
		ResetTrigger();
		m_animator.SetTrigger("Hit");
	}
}
