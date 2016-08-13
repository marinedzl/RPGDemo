using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class Tools
{
	public static T LoadXmlFile<T>(string path)
	{
		if (File.Exists(path))
		{
			try
			{
				StreamReader reader = File.OpenText(path);
				string text = reader.ReadToEnd();
				reader.Close();
				return XmlFile.DeserializeObject<T>(text);
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
			}
		}
		return default(T);
	}

	public static void WriteXmlFile<T>(T data, string path)
	{
		try
		{
			string text = XmlFile.SerializeObject(data);
			FileInfo file = new FileInfo(path);
			StreamWriter writer = file.CreateText();
			writer.Write(text);
			writer.Close();
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex);
		}
	}

	public static List<T> LoadCsvFile<T>(string path)
	{
		CsvFileReader reader = new CsvFileReader(path);
		List<T> list = new List<T>();
		Type type = typeof(T);

		CsvRow names = null;
		CsvRow descriptions = null;

		while (!reader.EndOfStream)
		{
			CsvRow row = new CsvRow();
			if (reader.ReadRow(row))
			{
				if (names == null)
				{
					names = row;
				}
				else if (descriptions == null)
				{
					descriptions = row;
				}
				else
				{
					if (row.Count == names.Count)
					{
						T t = Activator.CreateInstance<T>();
						for (int i = 0; i < row.Count; i++)
						{
							FieldInfo field = type.GetField(names[i], BindingFlags.Public | BindingFlags.Instance);
							object value = null;
							if (field.FieldType == typeof(string))
								value = row[i];
							else if (field.FieldType == typeof(int))
								value = int.Parse(row[i]);
							else if (field.FieldType == typeof(float))
								value = float.Parse(row[i]);
							else if (field.FieldType.IsEnum)
								value = Enum.Parse(field.FieldType, row[i]);
							if (value != null)
								field.SetValue(t, value);
						}
						list.Add(t);
					}
				}
			}
		}
		return list;
	}
}
