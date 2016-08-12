using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform m_target;

	public float m_maxDistance = 5;
	public float m_minDistance = 1;
	public float m_defaultDistance = 2;

	public float m_defaultPitch = -30;
	public float m_minPitch = -45;
	public float m_maxPitch = 45;

	public float m_scrollSpeed = 7;
	public float m_rotateSpeed = 10;

	public Vector3 m_offset = new Vector3(0, 1, 0);

	float m_distanceEnd = 0;
	Vector3 m_angleEnd = Vector3.zero;

	float m_distance = 0;
	Vector3 m_angle = Vector3.zero;

	Vector3 m_downAngle = Vector3.zero;
	Vector3 m_downPos;
	bool m_down = false;

	void Start()
	{
		m_distanceEnd = m_defaultDistance;
		m_angleEnd = new Vector3(m_defaultPitch, -m_target.eulerAngles.y, 0);

		m_distance = m_distanceEnd;
		m_angle = m_angleEnd;

		UpdateTransform();
	}

	void UpdateTransform()
	{
		Vector3 pos = new Vector3(0, 0, m_distance);
		pos = Quaternion.Euler(m_angle) * pos;
		Vector3 target = m_offset + m_target.position;
		transform.position = target + pos;
		transform.LookAt(target);
	}

	void UpdateLerp()
	{
		m_distance = Mathf.Lerp(m_distance, m_distanceEnd, Time.deltaTime * m_scrollSpeed);
		m_angle = Vector3.Lerp(m_angle, m_angleEnd, Time.deltaTime * m_rotateSpeed);
	}
	
	void Update()
	{
		Vector2 mouseScrollDelta = Input.mouseScrollDelta;
		if (mouseScrollDelta.y != 0)
		{
			m_distanceEnd -= mouseScrollDelta.y;
			m_distanceEnd = Mathf.Clamp(m_distanceEnd, m_minDistance, m_maxDistance);
		}

		if (Input.GetMouseButtonDown(1))
		{
			m_down = true;
			m_downPos = Input.mousePosition;
			m_downAngle = m_angle;
		}

		if (Input.GetMouseButtonUp(1))
		{
			m_down = false;
		}

		if (m_down)
		{
			Vector3 delta = Input.mousePosition - m_downPos;
			float swap = delta.x;
			delta.x = delta.y;
			delta.y = swap;
			m_angleEnd = m_downAngle + delta;
			m_angleEnd.x = Mathf.Clamp(m_angleEnd.x, m_minPitch, m_maxPitch);
		}

		UpdateLerp();
		UpdateTransform();
	}
}
