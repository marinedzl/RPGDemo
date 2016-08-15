using UnityEngine;
using UnityEngine.UI;

public class UISaveDataRecord : MonoBehaviour
{
	public Image m_class;
	public Text m_name;
	public Text m_level;
	public Text m_money;
	SaveData.Record m_record;

	public void SetData(SaveData.Record record)
	{
		m_record = record;
		m_name.text = m_record.name;
		m_level.text = m_record.level.ToString();
		m_money.text = m_record.money.ToString();

		Sprite[] sprites = Resources.LoadAll<Sprite>("Icons/class_icons");
		foreach (var item in sprites)
		{
			if (item.name == record._class.ToString())
				m_class.sprite = item;
		}
	}

	public void OnClick()
	{
		Game.ContinueGame(m_record);
	}
}
