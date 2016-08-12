using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExporter
{
	static void CheckDataPath()
	{
		string path = Application.dataPath + "/StreamingAssets";
		if (!Directory.Exists(path))
		{
			path += "/Scenes";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
	}

	[MenuItem("Marine/ExportScene")]
	public static void ExportScene()
	{
		Scene scene = SceneManager.GetActiveScene();
		if (string.IsNullOrEmpty(scene.name))
		{
			Debug.LogError("Save the scene first!");
			return;
		}

		CheckDataPath();

		string path = string.Format("{0}/StreamingAssets/Scenes/{1}.assetbundle", Application.dataPath, scene.name);
		BuildPipeline.BuildPlayer(null, path, BuildTarget.WebPlayer, BuildOptions.BuildAdditionalStreamedScenes);
	}

	static GameObject CreateDummy(string name)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.name = name;
		go.transform.SetParent(m_root);
		return go;
	}

	static Transform m_root = null;

	[MenuItem("Marine/LoadMapInfo")]
	public static void LoadMapInfo()
	{
		Scene scene = SceneManager.GetActiveScene();
		if (string.IsNullOrEmpty(scene.name))
		{
			Debug.LogError("Save the scene first!");
			return;
		}

		CheckDataPath();

		string path = string.Format("{0}/StreamingAssets/Scenes/{1}.xml", Application.dataPath, scene.name);
		if (!File.Exists(path))
		{
			Debug.LogError("No map info for current scene");
			return;
		}

		MapInfo mapInfo = Tools.Load<MapInfo>(path);

		Transform child = null;
		GameObject go = null;

		go = GameObject.Find("MapInfo");
		if (go == null)
			go = new GameObject("MapInfo");

		m_root = go.transform;

		child = m_root.FindChild("Born");
		if (child == null)
			go = CreateDummy("Born");
		mapInfo.born.CopyTo(go.transform);

		m_root = null;
	}

	[MenuItem("Marine/SaveMapInfo")]
	public static void SaveMapInfo()
	{
		Scene scene = SceneManager.GetActiveScene();
		if (string.IsNullOrEmpty(scene.name))
		{
			Debug.LogError("Save the scene first!");
			return;
		}

		MapInfo mapInfo = new MapInfo();

		Transform root = null;
		Transform child = null;

		root = GameObject.Find("MapInfo").transform;

		child = root.FindChild("Born");
		if (child != null)
		{
			mapInfo.born.CopyFrom(child);
		}
		
		CheckDataPath();

		string path = string.Format("{0}/StreamingAssets/Scenes/{1}.xml", Application.dataPath, scene.name);
		Tools.Save(mapInfo, path);
	}
}
