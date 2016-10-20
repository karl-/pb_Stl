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

	[Test]
	public void TestExportMultiple()
	{
		GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
		GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);

		a.transform.position = Vector3.right;
		b.transform.position = new Vector3(3f, 5f, 2.4f);
		b.transform.localRotation = Quaternion.Euler( new Vector3(45f, 45f, 10f) );

		if(!Directory.Exists(TEMP_FILE_DIR))
			Directory.CreateDirectory(TEMP_FILE_DIR);

		string temp_model_path = string.Format("{0}/multiple.stl", TEMP_FILE_DIR);
		pb_Stl_Exporter.Export(temp_model_path, new GameObject[] {a, b}, FileType.Binary );

		Assert.IsTrue(CompareFiles(string.Format("{0}/CompositeCubes_BINARY.stl", TEST_MODELS), temp_model_path));

		GameObject.DestroyImmediate(a);
		GameObject.DestroyImmediate(b);

		Directory.Delete(TEMP_FILE_DIR, true);
	}

	[Test]
	public void TestImportAscii()
	{
		Mesh[] meshes = pb_Stl_Importer.Import(string.Format("{0}/Cylinder_ASCII_RH.stl", TEST_MODELS));
		Assert.IsTrue(meshes != null);
		Assert.AreEqual(1, meshes.Length);
		Assert.AreEqual(240, meshes[0].triangles.Length);
		Assert.AreEqual(240, meshes[0].vertexCount);
	}

	[Test]
	public void TestImportBinary()
	{
		Mesh[] meshes = pb_Stl_Importer.Import(string.Format("{0}/Cylinder_BINARY_RH.stl", TEST_MODELS));
		Assert.IsTrue(meshes != null);
		Assert.AreEqual(1, meshes.Length);
		Assert.AreEqual(240, meshes[0].triangles.Length);
		Assert.AreEqual(240, meshes[0].vertexCount);
	}

    [Test]
    public void TestImportBinaryWithHeaders()
    {
        Mesh[] meshes = pb_Stl_Importer.Import(string.Format("{0}/CubedShape_BINARY_H.stl", TEST_MODELS));
        Assert.IsTrue(meshes != null);
        Assert.AreEqual(1, meshes.Length);
        Assert.AreEqual(204, meshes[0].triangles.Length);
        Assert.AreEqual(204, meshes[0].vertexCount);
    }

    private void DoVerifyWriteBinary(string expected_path, GameObject go)
	{
		string temp_model_path = string.Format("{0}/binary_file.stl", TEMP_FILE_DIR);

		Assert.IsTrue(pb_Stl.WriteFile(temp_model_path, go.GetComponent<MeshFilter>().sharedMesh, FileType.Binary));
		Assert.IsTrue(CompareFiles(temp_model_path, expected_path));

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

	private bool CompareFiles(string left, string right)
	{
		if(left == null || right == null)
			return false;

		// http://stackoverflow.com/questions/968935/compare-binary-files-in-c-sharp
		FileInfo a = new FileInfo(left);
		FileInfo b = new FileInfo(right);

		if(a.Length != b.Length)
			return false;

		using(FileStream f0 = a.OpenRead())
		using(FileStream f1 = b.OpenRead())
		using(BufferedStream bs0 = new BufferedStream(f0))
		using(BufferedStream bs1 = new BufferedStream(f1))
		{
			for(long i = 0; i < a.Length; i++)
			{
				if(bs0.ReadByte() != bs1.ReadByte())
				{
					return false;
				}
			}
		}

		return true;
	}
}
