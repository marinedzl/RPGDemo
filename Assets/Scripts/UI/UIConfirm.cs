using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIConfirm : MonoBehaviour
{
	public Text m_yesText;
	public Text m_noText;
	enum Selection
	{
		Waiting,
		Yes,
		No,
	}
	Selection m_selection;

	public bool selection
	{
		get { return m_selection == Selection.Yes; }
	}

	public IEnumerator Open(string yes, string no)
	{
		gameObject.SetActive(true);
		float timeScale = Time.timeScale;
		Time.timeScale = 0;
		m_yesText.text = yes;
		m_noText.text = no;
		m_selection = Selection.Waiting;
		while (m_selection == Selection.Waiting)
		{
			yield return null;
		}
		gameObject.SetActive(false);
		Time.timeScale = timeScale;
	}

	public void OnOk()
	{
		m_selection = Selection.Yes;
	}

	public void OnCancel()
	{
		m_selection = Selection.No;
	}
}
