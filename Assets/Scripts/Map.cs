using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public struct Monster
	{
		public int id;
		public Position pos;
	}

	public struct Teleporter
	{
		public string toMap;
		public Position pos;
		public Vector3 scaling;
	}

	public Position born = new Position();
	public List<Npc> npc = new List<Npc>();
	public List<Monster> monster = new List<Monster>();
	public List<Position> savepoint = new List<Position>();
	public List<Teleporter> teleporter = new List<Teleporter>();
}

public class Map
{
	string m_name;

	Transform m_root;
	Transform m_bornPosition;
	Transform m_savePointRoot;
	Transform m_teleporterRoot;
	Transform m_npcRoot;
	Transform m_playerRoot;
	Transform m_monsterRoot;

	List<Role> m_roles;
	List<Role> m_erase = new List<Role>();
	Role m_player;

	public Role player
	{
		get { return m_player; }
	}

	void Update()
	{
		foreach (var role in m_roles)
		{
			if (role == player) // 跳过主角
				continue;
			if (!role.IsAlive())
			{
				Object.Destroy(role.gameObject, 4);
				m_erase.Add(role);
			}
		}
		if (m_erase.Count > 0)
		{
			foreach (var unit in m_erase)
				m_roles.Remove(unit);
			m_erase.Clear();
		}
	}

	void Clear()
	{
		m_roles = new List<Role>();
		m_player = null;
	}

	public IEnumerator Load(string name)
	{
		Clear();
		m_name = name;
		yield return SceneManager.LoadSceneAsync("Loading");
		yield return Game.Database.LoadEffectBundle();
		yield return Game.Database.LoadScene(name);
		CreateTree();
		LoadMapInfo();
		Game.CreateUI();
	}

	void CreateTree()
	{
		GameObject go = new GameObject("Map");
		m_root = go.transform;

		go = new GameObject("Born");
		m_bornPosition = go.transform;
		m_bornPosition.SetParent(m_root);

		go = new GameObject("SavePoint");
		m_savePointRoot = go.transform;
		m_savePointRoot.SetParent(m_root);

		go = new GameObject("Teleporter");
		m_teleporterRoot = go.transform;
		m_teleporterRoot.SetParent(m_root);

		go = new GameObject("Npc");
		m_npcRoot = go.transform;
		m_npcRoot.SetParent(m_root);

		go = new GameObject("Player");
		m_playerRoot = go.transform;
		m_playerRoot.SetParent(m_root);

		go = new GameObject("Monster");
		m_monsterRoot = go.transform;
		m_monsterRoot.SetParent(m_root);
	}

	public void LoadMapInfo()
	{
		MapInfo mapInfo = Game.Database.LoadMapInfo(m_name);

		if (mapInfo == null)
			mapInfo = new MapInfo();

		mapInfo.born.CopyTo(m_bornPosition);

		foreach (var item in mapInfo.npc)
			CreateNpc(item);
		
		foreach (var item in mapInfo.monster)
			CreateMonster(item);

		foreach (var item in mapInfo.savepoint)
			CreateSavePoint(item);

		foreach (var item in mapInfo.teleporter)
			CreateTeleporter(item);

		Born(mapInfo.born);
	}

	public void SaveMapInfo()
	{
		MapInfo mapInfo = new MapInfo();
		
		mapInfo.born.CopyFrom(m_bornPosition);

		MapInfo.Npc mapNpc = new MapInfo.Npc();
		int count = m_npcRoot.childCount;
		for (int i = 0; i < count; i++)
		{
			Transform t = m_npcRoot.GetChild(i);
			Npc npc = t.GetComponent<Npc>();
			if (npc != null)
			{
				mapNpc.id = npc.id;
				mapNpc.pos.CopyFrom(t);
				mapInfo.npc.Add(mapNpc);
			}
		}

		MapInfo.Monster mapMonster = new MapInfo.Monster();
		count = m_monsterRoot.childCount;
		for (int i = 0; i < count; i++)
		{
			Transform t = m_monsterRoot.GetChild(i);
			Monster monster = t.GetComponent<Monster>();
			if (monster != null)
			{
				mapMonster.id = monster.id;
				mapMonster.pos.CopyFrom(t);
				mapInfo.monster.Add(mapMonster);
			}
		}

		MapInfo.Position savePoint = new MapInfo.Position();
		count = m_savePointRoot.childCount;
		for (int i = 0; i < count; i++)
		{
			Transform t = m_savePointRoot.GetChild(i);
			savePoint.CopyFrom(t);
			mapInfo.savepoint.Add(savePoint);
		}

		MapInfo.Teleporter mapTel = new MapInfo.Teleporter();
		count = m_teleporterRoot.childCount;
		for (int i = 0; i < count; i++)
		{
			Transform t = m_teleporterRoot.GetChild(i);
			Teleporter tel = t.GetComponent<Teleporter>();
			if (tel != null)
			{
				mapTel.pos.CopyFrom(t);
				mapTel.scaling = tel.transform.localScale;
				mapTel.toMap = tel.m_toMap;
				mapInfo.teleporter.Add(mapTel);
			}
		}

		Game.Database.SaveMapInfo(mapInfo, m_name);
	}

	public void CreateSavePoint(MapInfo.Position data)
	{
		GameObject go = Game.Database.CreateEffect("guanghuan");
		go.transform.SetParent(m_savePointRoot);
		data.CopyTo(go.transform);
		go.AddComponent<SavePoint>();
	}

	public void CreateTeleporter(MapInfo.Teleporter data)
	{
		GameObject go = Game.Database.LoadResource("Map/Teleporter");
		go.name = "to " + data.toMap;
		go.transform.SetParent(m_teleporterRoot);

		go.transform.localScale = data.scaling;
		data.pos.CopyTo(go.transform);

		Teleporter tel = go.AddComponent<Teleporter>();
		tel.m_toMap = data.toMap;
	}

	public void CreateNpc(MapInfo.Npc data)
	{
		Npc.Info info = Game.Database.GetNpcInfo(data.id);

		GameObject go = Game.Database.LoadResource("Roles/" + info.model);
		go.name = info.name.ToString();
		go.transform.SetParent(m_npcRoot);

		CharacterController cc = go.AddComponent<CharacterController>();
		cc.center = new Vector3(0, 1, 0);

		float scale = info.scaling;
		go.transform.localScale = new Vector3(scale, scale, scale);
		data.pos.CopyTo(go.transform);

		Role role = go.AddComponent<Role>();
		role.team = Role.Team.Yellow;
		role.hp = info.hp;
		role.atk = info.atk;
		role.def = info.def;

		Npc npc = go.AddComponent<Npc>();
		npc.m_id = data.id;

		m_roles.Add(role);
	}

	public void CreateMonster(MapInfo.Monster data)
	{
		Monster.Info info = Game.Database.GetMonsterInfo(data.id);

		GameObject go = Game.Database.LoadResource("Roles/" + info.model);
		go.name = info.name.ToString();
		go.transform.SetParent(m_npcRoot);

		CharacterController cc = go.AddComponent<CharacterController>();
		cc.center = new Vector3(0, 1, 0);

		float scale = info.scaling;
		go.transform.localScale = new Vector3(scale, scale, scale);
		data.pos.CopyTo(go.transform);

		Role role = go.AddComponent<Role>();
		role.team = Role.Team.Red;
		role.m_MoveSpeed = 0.5f;
		role.hp = info.hp;
		role.atk = info.atk;
		role.def = info.def;
		role.exp = info.exp;

		go.AddComponent<ZakoAI>();
		Monster monster = go.AddComponent<Monster>();
		monster.m_id = data.id;

		m_roles.Add(role);
	}

	public void Born(MapInfo.Position position)
	{
		SaveData.Record record = Game.Record;

		GameObject go = Game.Database.LoadResource("Roles/" + record._class.ToString());
		go.name = record.name;
		go.transform.SetParent(m_playerRoot);

		CharacterController cc = go.AddComponent<CharacterController>();
		cc.center = new Vector3(0, 1, 0);

		const float scale = 0.6f;
		go.transform.localScale = new Vector3(scale, scale, scale);
		position.CopyTo(go.transform);
		
		Game.camera.target = go.transform;

		Role role = go.AddComponent<Role>();
		role.team = Role.Team.Blue;
		role.m_MoveSpeed = 5;

		LevelInfo levelInfo = Game.Database.GetLevelInfo(record.level);
		role.hp = levelInfo.hp;
		role.atk = levelInfo.atk;
		role.def = levelInfo.def;

		go.AddComponent<Player>();

		m_roles.Add(role);
		m_player = role;
	}
	
	public Role FindNearsetEnemy(Role self, float range)
	{
		Role target = null;
		float min = float.MaxValue;

		foreach (var role in m_roles)
		{
			if (role.team == self.team || !role.IsAlive())
				continue;

			Vector3 delta = role.transform.position - self.transform.position;
			delta.y = 0;
			float distance = delta.magnitude;

			if (distance > range)
				continue;

			if (distance < min)
			{
				target = role;
				min = distance;
			}
		}

		return target;
	}

	public List<Role> FindNearbyEnemy(Role self, float range)
	{
		List<Role> targets = new List<Role>();

		foreach (var role in m_roles)
		{
			if (role.team == self.team || !role.IsAlive())
				continue;

			Vector3 delta = role.transform.position - self.transform.position;
			delta.y = 0;
			float distance = delta.magnitude;

			if (distance > range)
				continue;

			targets.Add(role);
		}

		return targets;
	}
}
