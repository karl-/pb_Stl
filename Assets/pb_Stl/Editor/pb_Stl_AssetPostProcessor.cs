using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.IO;

namespace Parabox.STL
{
	public class pb_Stl_AssetPostProcessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			foreach(string path in importedAssets.Where(x => x.EndsWith(".stl")))
			{
				string dir = Path.GetDirectoryName(path).Replace("\\", "/");
				string name = Path.GetFileNameWithoutExtension(path);

				Mesh[] meshes = pb_Stl_Importer.Import(path);

				if(meshes == null)
					continue;

				for(int i = 0; i < meshes.Length; i++)
					AssetDatabase.CreateAsset(meshes[i], string.Format("{0}/{1}{2}.asset", dir, name, i));
			}
		}

		public static void CreateMeshAssetWithPath(string path)
		{
			string dir = Path.GetDirectoryName(path).Replace("\\", "/");
			string name = Path.GetFileNameWithoutExtension(path);

			Mesh[] meshes = pb_Stl_Importer.Import(path);

			if(meshes == null)
				return;

			for(int i = 0; i < meshes.Length; i++)
				AssetDatabase.CreateAsset(meshes[i], string.Format("{0}/{1}{2}.asset", dir, name, i));
		}

		[MenuItem("Tools/Force Import &d")]
		static void ditos()
		{
			foreach(Object o in Selection.objects)
			{
				CreateMeshAssetWithPath(AssetDatabase.GetAssetPath(o));
			}
		}
	}
}
