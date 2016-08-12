using UnityEngine;

public class UIStartScreen : MonoBehaviour
{
	public UISaveDataPanel m_saveDataPanel;
	public UINewGamePanel m_newGamePanel;

	void Awake()
	{
		m_saveDataPanel.gameObject.SetActive(false);
		m_newGamePanel.gameObject.SetActive(false);
	}

	public void OnClickNewGame()
	{
		m_newGamePanel.Open();
	}

	public void OnClickContinue()
	{
		m_saveDataPanel.Open();
	}

	public void OnClickOptions()
	{

	}
}
