using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
	public class Record
	{
		public Class _class;
		public string name;
		public int level;
		public int money;
		public int exp;
	}
	public List<Record> records = new List<Record>();
}

public class LevelInfo
{
	public int level;
	public int hp;
	public int atk;
	public int def;
	public int exp;
}

public class Database
{
	public Dictionary<int, Npc.Info> m_npcInfos;
	public Dictionary<int, Monster.Info> m_monsterInfo;
	public List<LevelInfo> m_levelInfos;

	public void Init()
	{
		string url = string.Format("{0}/StreamingAssets/Configs/npc.csv", Application.dataPath);
		List<Npc.Info> npcList = Tools.LoadCsvFile<Npc.Info>(url);
		m_npcInfos = new Dictionary<int, Npc.Info>();
		foreach (var item in npcList)
			m_npcInfos.Add(item.id, item);

		url = string.Format("{0}/StreamingAssets/Configs/monster.csv", Application.dataPath);
		List<Monster.Info> monsterList = Tools.LoadCsvFile<Monster.Info>(url);
		m_monsterInfo = new Dictionary<int, Monster.Info>();
		foreach (var item in monsterList)
			m_monsterInfo.Add(item.id, item);

		url = string.Format("{0}/StreamingAssets/Configs/level.csv", Application.dataPath);
		m_levelInfos = Tools.LoadCsvFile<LevelInfo>(url);
	}

	public Npc.Info GetNpcInfo(int id)
	{
		return m_npcInfos[id];
	}

	public Monster.Info GetMonsterInfo(int id)
	{
		return m_monsterInfo[id];
	}

	public LevelInfo GetLevelInfo(int level)
	{
		return m_levelInfos[level];
	}

	const string SaveDataFileName = "SaveData.xml";

	public SaveData LoadSaveData()
	{
		string path = string.Format("{0}/{1}", Application.persistentDataPath, SaveDataFileName);
		SaveData saveData = Tools.LoadXmlFile<SaveData>(path);
		if (saveData == null)
			saveData = new SaveData();
		return saveData;
	}

	public void WriteSaveData(SaveData data)
	{
		Tools.WriteXmlFile(data, string.Format("{0}/{1}", Application.persistentDataPath, SaveDataFileName));
	}

	public GameObject LoadResource(string path)
	{
		return Object.Instantiate(Resources.Load(path)) as GameObject;
	}
}
