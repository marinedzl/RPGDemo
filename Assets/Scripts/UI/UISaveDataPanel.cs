using System.Collections.Generic;
using UnityEngine;

public class UISaveDataPanel : MonoBehaviour
{
	public RectTransform m_contents;
	public GameObject m_recordPrefab;

	void Awake()
	{
		m_recordPrefab.gameObject.SetActive(false);
	}

	void Clear()
	{
		List<GameObject> records = new List<GameObject>();
		for (int i = 0; i < m_contents.childCount; i++)
			records.Add(m_contents.GetChild(i).gameObject);
		m_contents.DetachChildren();
		foreach (var item in records)
			Destroy(item);
	}

	void Refresh()
	{
		SaveData saveData = Game.Instance.LoadSaveData();

		Clear();

		foreach (var record in saveData.records)
		{
			GameObject go = Instantiate(m_recordPrefab);
			go.SetActive(true);
			go.transform.SetParent(m_contents);
			go.transform.localScale = Vector3.one;
			go.GetComponent<UISaveDataRecord>().SetData(record);
		}
	}

	public void Open()
	{
		Refresh();
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
}
