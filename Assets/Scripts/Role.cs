using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
	public enum Team
	{
		Red,
		Blue,
		Yellow,
	}

	public float m_MoveSpeed = 0f;
	public Team team;
	public int m_id;

	public int hp;
	public int atk;
	public int def;
	public int exp;

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
		m_animator.ResetTrigger("Dead");
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
		return hp > 0;
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

	public void Hit()
	{
		ResetTrigger();
		m_animator.SetTrigger("Hit");
	}

	public void Dead()
	{
		ResetTrigger();
		m_animator.SetTrigger("Dead");
	}

	void OnAttackHit()
	{
		Hit(2.5f, 90);
	}

	void Hit(float range, float angle)
	{
		List<Role> units = Game.Map.FindNearbyEnemy(this, range);
		Vector3 forward = transform.forward;
		forward.y = 0;
		foreach (var unit in units)
		{
			Vector3 delta = unit.transform.position - transform.position;
			delta.y = 0;
			if (Vector3.Angle(forward, delta) < angle)
			{
				unit.Hit(this);
			}
		}
	}

	public void Hit(Role from)
	{
		CalculateDamage(from);
		if (!IsAlive())
		{
			Dead();
			if (from == Game.Map.player)
				Game.AwardExp(exp);
		}
		else
		{
			Hit();
		}
	}

	void CalculateDamage(Role from)
	{
		int damage = from.atk - def;
		if (damage > 0)
		{
			hp = Mathf.Max(hp - damage, 0);
		}
	}
}
