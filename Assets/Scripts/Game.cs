using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
	static Game s_instance;

	public static Game Instance
	{
		get { return s_instance; }
	}

	public static SaveData.Record Record
	{
		get { return s_instance.m_record; }
	}

	public static Database Database
	{
		get { return s_instance.m_database; }
	}

	public static Map Map
	{
		get { return s_instance.m_map; }
	}

	public static new CameraController camera
	{
		get { return s_instance.m_camera; }
	}

	void Awake()
	{
		s_instance = this;
		DontDestroyOnLoad(gameObject);

		m_database = new Database();
		m_database.Init();
	}

	void OnApplicationQuit()
	{
		s_instance = null;
	}

	Database m_database;
	Map m_map;
	SaveData.Record m_record;
	public CameraController m_camera;
	UIConfirm m_uiConfirm;

	void Start()
	{
	}

	void Update()
	{

	}

	public static void ChangeMap(string name)
	{
		camera.target = null;
		s_instance.m_map = new Map();
		s_instance.StartCoroutine(s_instance.m_map.Load(name));
	}

	public static void ContinueGame(SaveData.Record record)
	{
		s_instance.m_record = record;
		ChangeMap("Demo2");
	}

	public static void NewGame(string name, Class _class)
	{
		SaveData.Record record = new SaveData.Record();
		record.name = name;
		record.level = 1;
		record.money = 1000;
		record.exp = 0;
		record._class = _class;

		SaveData data = Database.LoadSaveData();
		data.records.Add(record);
		Database.WriteSaveData(data);

		ContinueGame(record);
	}

	public static void SaveMap()
	{
		Map.SaveMapInfo();
	}

	public static void AwardExp(int exp)
	{
		LevelInfo levelInfo = Database.GetLevelInfo(Record.level);
		Record.exp += exp;
		if (Record.exp >= levelInfo.exp) // level up
		{
			Record.level += 1;
			Record.exp -= levelInfo.exp;
			levelInfo = Database.GetLevelInfo(Record.level);
			Map.player.hp = levelInfo.hp;
			Map.player.atk = levelInfo.atk;
			Map.player.def = levelInfo.def;
		}
	}

	public static void CreateUI()
	{
		GameObject uiRoot = Database.LoadResource("UI");
		s_instance.m_uiConfirm = uiRoot.transform.FindChild("UIConfirm").GetComponent<UIConfirm>();
		s_instance.m_uiConfirm.gameObject.SetActive(false);
	}

	public static void OpenSaveData()
	{
		s_instance.StartCoroutine(s_instance.WaitingSaveData());
	}

	public IEnumerator WaitingSaveData()
	{
		yield return m_uiConfirm.Open("Save", "Cancel");
		if (m_uiConfirm.selection)
		{
			Debug.Log("Save data");
		}
	}
}
