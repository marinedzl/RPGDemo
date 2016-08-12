using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
	public float m_speed = 5;
	public Image m_loading;

	void Start()
	{

	}

	void Update()
	{
		m_loading.transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 100 * m_speed));
	}
}
