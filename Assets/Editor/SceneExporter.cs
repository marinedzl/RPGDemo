﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExporter
{
	[MenuItem("Marine/ExportScene")]
	public static void ExportScene()
	{
		Scene scene = SceneManager.GetActiveScene();
		if (string.IsNullOrEmpty(scene.name))
		{
			Debug.LogError("Save the scene first!");
			return;
		}

		string path = string.Format("{0}/StreamingAssets/Scenes/{1}.assetbundle", Application.dataPath, scene.name);
		BuildPipeline.BuildPlayer(null, path, BuildTarget.WebPlayer, BuildOptions.BuildAdditionalStreamedScenes);
	}

	[MenuItem("Marine/LoadMapInfo")]
	public static void LoadMapInfo()
	{
		Game.Instance.LoadMapInfo(SceneManager.GetActiveScene().name);
	}

	[MenuItem("Marine/SaveMapInfo")]
	public static void SaveMapInfo()
	{
		Game.Instance.SaveMapInfo(SceneManager.GetActiveScene().name);
	}
}
