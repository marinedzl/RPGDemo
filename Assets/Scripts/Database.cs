using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	private AssetBundle m_effectBundle;

	public void Init()
	{
		string url = string.Format("{0}/Configs/npc.csv", Application.streamingAssetsPath);
		List<Npc.Info> npcList = Tools.LoadCsvFile<Npc.Info>(url);
		m_npcInfos = new Dictionary<int, Npc.Info>();
		foreach (var item in npcList)
			m_npcInfos.Add(item.id, item);

		url = string.Format("{0}/Configs/monster.csv", Application.streamingAssetsPath);
		List<Monster.Info> monsterList = Tools.LoadCsvFile<Monster.Info>(url);
		m_monsterInfo = new Dictionary<int, Monster.Info>();
		foreach (var item in monsterList)
			m_monsterInfo.Add(item.id, item);

		url = string.Format("{0}/Configs/level.csv", Application.streamingAssetsPath);
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

	public IEnumerator LoadEffectBundle()
	{
		if (m_effectBundle == null)
		{
			string url = string.Format("{0}/AssetBundles/effects", Application.streamingAssetsPath);
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(url);
			yield return request;
			m_effectBundle = request.assetBundle;
		}
	}

	public GameObject CreateEffect(string name)
	{
		Object prefab = m_effectBundle.LoadAsset(name);
		if (prefab != null)
			return Object.Instantiate(prefab) as GameObject;
		return null;
	}

	public IEnumerator LoadScene(string name)
	{
		string url = string.Format("file://{0}/AssetBundles/scenes/{1}", Application.streamingAssetsPath, name);
		WWW www = new WWW(url);
		yield return www;

		if (www.error != null)
		{
			Debug.LogError(www.error);
		}
		else
		{
			AssetBundle bunlde = www.assetBundle;
			yield return SceneManager.LoadSceneAsync(name);
			bunlde.Unload(false);
		}
	}

	public MapInfo LoadMapInfo(string name)
	{
		MapInfo mapInfo = null;
		string path = string.Format("{0}/Scenes/{1}.xml", Application.streamingAssetsPath, name);

		if (File.Exists(path))
			mapInfo = Tools.LoadXmlFile<MapInfo>(path);

		return mapInfo;
	}

	public void SaveMapInfo(MapInfo mapInfo, string name)
	{
		string path = string.Format("{0}/Scenes/{1}.xml", Application.streamingAssetsPath, name);
		Tools.WriteXmlFile(mapInfo, path);
	}
}
