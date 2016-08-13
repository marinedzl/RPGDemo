using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class XmlFile
{
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

	public static string SerializeObject<T>(T obj)
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

	public static T DeserializeObject<T>(string pXmlizedString)
	{
		XmlSerializer xs = new XmlSerializer(typeof(T));
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
		return (T)xs.Deserialize(memoryStream);
	}
}
