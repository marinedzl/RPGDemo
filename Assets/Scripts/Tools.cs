using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

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

	public Position born;
}

public static class Tools
{
	public static T Load<T>(string path)
	{
		if (File.Exists(path))
		{
			try
			{
				StreamReader reader = File.OpenText(path);
				string text = reader.ReadToEnd();
				reader.Close();
				return DeserializeObject<T>(text);
			}
			catch (System.Exception ex)
			{
				Debug.Log(ex);
			}
		}
		return default(T);
	}

	public static void Save<T>(T data, string path)
	{
		try
		{
			string text = SerializeObject(data);
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

	static string UTF8ByteArrayToString(byte[] characters)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		string constructedString = encoding.GetString(characters);
		return constructedString;
	}

	static byte[] StringToUTF8ByteArray(string xmlText)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		byte[] byteArray = encoding.GetBytes(xmlText);
		return byteArray;
	}

	static string SerializeObject<T>(T obj)
	{
		string xmlText = null;
		MemoryStream memoryStream = new MemoryStream();
		XmlSerializer xs = new XmlSerializer(typeof(T));
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
		xs.Serialize(xmlTextWriter, obj);
		memoryStream = xmlTextWriter.BaseStream as MemoryStream;
		xmlText = UTF8ByteArrayToString(memoryStream.ToArray());
		return xmlText;
	}

	static T DeserializeObject<T>(string pXmlizedString)
	{
		XmlSerializer xs = new XmlSerializer(typeof(T));
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
		return (T)xs.Deserialize(memoryStream);
	}
}
