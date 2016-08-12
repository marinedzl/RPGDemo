using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

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

public class Game : MonoBehaviour
{
	static Game s_instance;
	SaveData.Record m_record;

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
		
		MapInfo mapInfo = Tools.Load<MapInfo>(string.Format("{0}/StreamingAssets/Scenes/{1}.xml", Application.dataPath, name));
		Born(mapInfo.born);
	}

	const string SaveDataFileName = "SaveData.xml";

	public SaveData LoadSaveData()
	{
		string path = string.Format("{0}/{1}", Application.persistentDataPath, SaveDataFileName);
		SaveData saveData = Tools.Load<SaveData>(path);
		if (saveData == null)
			saveData = new SaveData();
		return saveData;
	}

	public void WriteSaveData(SaveData data)
	{
		Tools.Save(data, string.Format("{0}/{1}", Application.persistentDataPath, SaveDataFileName));
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
}
