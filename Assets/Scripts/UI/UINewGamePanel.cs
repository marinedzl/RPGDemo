using UnityEngine;
using UnityEngine.UI;

public class UINewGamePanel : MonoBehaviour
{
	public Button[] m_classButtons;
	public Text m_classText;
	public InputField m_inputField;
	
	Class m_class;
	GameObject[] m_roles = new GameObject[4];

	void ChangeClass(int index)
	{
		m_class = (Class)index;
		m_classText.text = m_class.ToString();

		if (m_roles[index] == null)
		{
			m_roles[index] = Game.Database.LoadResource("Roles/" + m_class.ToString());
		}

		for (int i = 0; i < m_roles.Length; i++)
		{
			if (m_roles[i] != null)
				m_roles[i].SetActive(index == i);
			m_classButtons[i].interactable = index != i;
		}
	}

	public void Open()
	{
		ChangeClass(0);
		gameObject.SetActive(true);
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void OnClickClose()
	{
		Close();
	}

	public void OnClickClass(int index)
	{
		ChangeClass(index);
	}

	public void OnClickStart()
	{
		string name = "Hero";
		if (!string.IsNullOrEmpty(m_inputField.text))
			name = m_inputField.text;
		Game.NewGame(name, m_class);
	}
}
