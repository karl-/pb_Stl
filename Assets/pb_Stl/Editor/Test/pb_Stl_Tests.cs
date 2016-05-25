using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using Parabox.STL;
using System.IO;

/**
 *    Editor tests for pb_Stl lib.
 */
public class pb_Stl_Tests
{
	const string TEMP_FILE_DIR = "Assets/pb_Stl/Editor/Test/Temp";
	const string TEST_MODELS = "Assets/pb_Stl/Editor/Test/Models/";

	[Test]
	public void VerifyWriteASCII()
	{
		DoVerifyWriteString(TEST_MODELS + "Cylinder_ASCII_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Cylinder));
		DoVerifyWriteString(TEST_MODELS + "Sphere_ASCII_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Sphere));
	}

	[Test]
	public void VerifyWriteBinary()
	{
		if(!Directory.Exists(TEMP_FILE_DIR))
			Directory.CreateDirectory(TEMP_FILE_DIR);

		DoVerifyWriteBinary(TEST_MODELS + "Cylinder_BINARY_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Cylinder));
		DoVerifyWriteBinary(TEST_MODELS + "Sphere_BINARY_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Sphere));

		Directory.Delete(TEMP_FILE_DIR, true);
	}

	private void DoVerifyWriteBinary(string expected_path, GameObject go)
	{
		string temp_model_path = string.Format("{0}/binary_file.stl", TEMP_FILE_DIR);
		Assert.AreEqual(true, pb_Stl.WriteFile(temp_model_path, go.GetComponent<MeshFilter>().sharedMesh, FileType.Binary));

		// http://stackoverflow.com/questions/968935/compare-binary-files-in-c-sharp
		FileInfo a = new FileInfo(temp_model_path);
		FileInfo b = new FileInfo(expected_path);

		Assert.AreEqual(a.Length, b.Length);

		bool match = true;

		using(FileStream f0 = a.OpenRead())
		using(FileStream f1 = b.OpenRead())
		using(BufferedStream bs0 = new BufferedStream(f0))
		using(BufferedStream bs1 = new BufferedStream(f1))
		{
			for(long i = 0; i < a.Length; i++)
			{
				if(bs0.ReadByte() != bs1.ReadByte())
				{
					match = false;
					break;
				}
			}
		}

		Assert.AreEqual(true, match);

		GameObject.DestroyImmediate(go);
	}

	private void DoVerifyWriteString(string path, GameObject go)
	{
		string ascii = pb_Stl.WriteString(go.GetComponent<MeshFilter>().sharedMesh, true);
		Assert.AreNotEqual(ascii, null);
		Assert.AreNotEqual(ascii, "");
		string expected = File.ReadAllText(path);
		Assert.AreNotEqual(expected, null);
		Assert.AreNotEqual(expected, "");
		Assert.AreEqual(ascii, expected);
		GameObject.DestroyImmediate(go);
	}
}
