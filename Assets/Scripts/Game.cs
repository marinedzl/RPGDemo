using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

public enum Class
{
	Knight,
	Swordsman,
	Archer,
	Mage,
}

public class SaveData
{
	public class Record
	{
		public Class _class;
		public string name;
		public int level;
		public int money;
	}
	public List<Record> records = new List<Record>();
}

public class NpcInfo
{
	public int id;
	public string name;
	public string desc;
	public string model;
	public float scaling;
}

public class MapInfo
{
	public struct Position
	{
		public Vector3 t;
		public Vector3 r;

		public void CopyFrom(Transform transform)
		{
			t = transform.position;
			r = transform.rotation.eulerAngles;
		}

		public void CopyTo(Transform transform)
		{
			transform.position = t;
			transform.rotation = Quaternion.Euler(r);
		}
	}

	public struct Npc
	{
		public int id;
		public Position pos;
	}

	public Position born = new Position();
	public List<Npc> npc = new List<Npc>();
}

public class Game : MonoBehaviour
{
	static Game s_instance;
	SaveData.Record m_record;
	Dictionary<int, NpcInfo> m_npcInfos;

	public static Game Instance
	{
		get
		{
			return s_instance;
		}
	}

	SaveData.Record record
	{
		get { return m_record; }
		set { m_record = value; }
	}

	void Awake()
	{
		s_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		string url = string.Format("{0}/StreamingAssets/Configs/npc.csv", Application.dataPath);
		List<NpcInfo> npcList = Tools.LoadCsvFile<NpcInfo>(url);
		m_npcInfos = new Dictionary<int, NpcInfo>();
		foreach (var item in npcList)
			m_npcInfos.Add(item.id, item);
	}

	void Update()
	{

	}

	IEnumerator LoadScene(string name)
	{
		yield return SceneManager.LoadSceneAsync("Loading");

		string url = string.Format("file://{0}/StreamingAssets/Scenes/{1}.assetbundle", Application.dataPath, name);
		WWW www = new WWW(url);
		yield return www;

		if (www.error != null)
		{
			Debug.LogError(www.error);
		}
		else
		{
			while (!www.isDone)
				yield return null;

			AssetBundle assetbundle = www.assetBundle;

			yield return SceneManager.LoadSceneAsync(name);

			assetbundle.Unload(false);
		}

		www.Dispose();

		LoadMapInfo(name);
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

	public void ContinueGame(SaveData.Record record)
	{
		Instance.record = record;
		StartCoroutine(LoadScene("Demo2"));
	}

	public void NewGame(string name, Class _class)
	{
		SaveData.Record record = new SaveData.Record();
		record.name = name;
		record.level = 1;
		record.money = 1000;
		record._class = _class;

		SaveData data = LoadSaveData();
		data.records.Add(record);
		WriteSaveData(data);

		ContinueGame(record);
	}

	public GameObject LoadResource(string path)
	{
		return Instantiate(Resources.Load(path)) as GameObject;
	}

	public void Born(MapInfo.Position position)
	{
		GameObject role = LoadResource("Roles/" + m_record._class.ToString());

		CharacterController cc = role.AddComponent<CharacterController>();
		cc.center = new Vector3(0, 1, 0);

		const float scale = 0.6f;
		role.transform.localScale = new Vector3(scale, scale, scale);
		position.CopyTo(role.transform);

		role.AddComponent<Role>();
		role.AddComponent<Player>();

		CameraController camera = Camera.main.gameObject.AddComponent<CameraController>();
		camera.m_target = role.transform;
	}

	NpcInfo GetNpcInfo(int id)
	{
		return m_npcInfos[id];
	}

	public GameObject CreateNpc(int id, MapInfo.Position position)
	{
		NpcInfo npcInfo = GetNpcInfo(id);

		GameObject role = LoadResource("Roles/" + npcInfo.model);
		role.name = id.ToString();

		CharacterController cc = role.AddComponent<CharacterController>();
		cc.center = new Vector3(0, 1, 0);

		float scale = npcInfo.scaling;
		role.transform.localScale = new Vector3(scale, scale, scale);
		position.CopyTo(role.transform);

		return role;
	}

	public void LoadMapInfo(string name)
	{
		Transform root = gameObject.transform;
		string path = string.Format("{0}/StreamingAssets/Scenes/{1}.xml", Application.dataPath, name);

		MapInfo mapInfo = null;

		if (File.Exists(path))
			mapInfo = Tools.LoadXmlFile<MapInfo>(path);
		else
			mapInfo = new MapInfo();

		Transform born = root.FindChild("Born");
		if (born == null)
		{
			GameObject go = new GameObject("Born");
			born = go.transform;
			born.SetParent(root);
		}
		mapInfo.born.CopyTo(born);
		Born(mapInfo.born);

		Transform npcRoot = root.FindChild("Npc");
		if (npcRoot == null)
		{
			GameObject go = new GameObject("Npc");
			npcRoot = go.transform;
			npcRoot.SetParent(root);
		}

		foreach (var npc in mapInfo.npc)
		{
			GameObject go = CreateNpc(npc.id, npc.pos);
			go.transform.SetParent(npcRoot);
		}
	}

	public void SaveMapInfo(string name)
	{
		Transform root = gameObject.transform;
		MapInfo mapInfo = new MapInfo();

		Transform born = root.FindChild("Born");
		if (born != null)
		{
			mapInfo.born.CopyFrom(born);
		}

		Transform npcRoot = root.FindChild("Npc");
		if (npcRoot != null)
		{
			MapInfo.Npc npc = new MapInfo.Npc();
			int count = npcRoot.childCount;
			for (int i = 0; i < count; i++)
			{
				Transform t = npcRoot.GetChild(i);
				npc.id = int.Parse(t.name);
				npc.pos.CopyFrom(t);
				mapInfo.npc.Add(npc);
			}
		}

		string path = string.Format("{0}/StreamingAssets/Scenes/{1}.xml", Application.dataPath, name);
		Tools.WriteXmlFile(mapInfo, path);
	}
}
