using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Parabox.STL
{
	/**
	 *	Provides menu items for writing STL files from a scene selection.
	 */
	public class pb_Stl_Exporter : Editor
	{
		[MenuItem("Assets/Export/STL (Ascii)", true)]
		[MenuItem("Assets/Export/STL (Binary)", true)]
		static bool VerifyExport()
		{
			return Selection.transforms.SelectMany(x => x.GetComponentsInChildren<MeshFilter>()).FirstOrDefault(y => y.sharedMesh != null) != null;
		}

		[MenuItem("Assets/Export/STL (Ascii) &d", false)]
		static void MenuExportAscii()
		{
			ExportWithFileDialog(Selection.gameObjects, FileType.Ascii);
		}

		[MenuItem("Assets/Export/STL (Binary)", false)]
		static void MenuExportBinary()
		{
			ExportWithFileDialog(Selection.gameObjects, FileType.Binary);
		}

		public static void ExportWithFileDialog(GameObject[] gameObjects, FileType type)
		{
			string path = EditorUtility.SaveFilePanel("Save Mesh to STL", "", "Mesh", "stl");

			if( Export(path, gameObjects, type) )
			{
				string full = path.Replace("\\", "/");

				// if the file was saved in project, ping it
				if(full.Contains(Application.dataPath))
				{
					string relative = full.Replace(Application.dataPath, "Assets");
					Object o = AssetDatabase.LoadAssetAtPath<Object>(relative);
					if(o != null)
						EditorGUIUtility.PingObject(o);
					AssetDatabase.Refresh();
				}
			}
		}

		public static bool Export(string path, GameObject[] gameObjects, FileType type)
		{
			Mesh[] meshes = CreateWorldSpaceMeshesWithTransforms(gameObjects.Select(x => x.transform).ToArray());

			if(meshes != null && meshes.Length > 0)
			{
				if(!string.IsNullOrEmpty(path))
					return pb_Stl.WriteFile(path, meshes, type);
				else
					UnityEngine.Debug.LogWarning("Invalid file path, aborting STL export.");
			}
			else
			{
				UnityEngine.Debug.LogWarning("No meshes selected.");
			}
			return false;
		}

		/**
		 * Extracts a list of mesh values with their relative transformations intact.
		 */
		private static Mesh[] CreateWorldSpaceMeshesWithTransforms(IList<Transform> transforms)
		{
			if(transforms == null)
				return null;

			// move root node to center of selection
			Vector3 p = Vector3.zero;

			for(int i = 0; i < transforms.Count; i++)
				p += transforms[i].position;
			Vector3 mesh_center = p / (float) transforms.Count;

			GameObject root = new GameObject();
			root.name = "ROOT";
			root.transform.position = mesh_center;

			// copy all transforms to new root gameobject
			foreach(Transform t in transforms)
			{
				GameObject go = GameObject.Instantiate(t.gameObject);
				go.transform.SetParent(t.parent, false);
				go.transform.SetParent(root.transform, true);
			}

			// move root to 0,0,0 so mesh transformations are relative to origin
			root.transform.position = Vector3.zero;

			// create new meshes by iterating the root node and transforming vertex & normal
			// values (ignoring all other mesh attributes since STL doesn't care about them)
			List<MeshFilter> mfs = root.GetComponentsInChildren<MeshFilter>().Where(x => x.sharedMesh != null).ToList();
			int meshCount = mfs.Count;
			Mesh[] meshes = new Mesh[meshCount];

			for(int i = 0; i < meshCount; i++)
			{
				Transform t = mfs[i].transform;

				Vector3[] v = mfs[i].sharedMesh.vertices;
				Vector3[] n = mfs[i].sharedMesh.normals;

				for(int it = 0; it < v.Length; it++)
				{
					v[it] = t.TransformPoint(v[it]);
					n[it] = t.TransformDirection(n[it]);
				}

				Mesh m = new Mesh();

				m.name = mfs[i].name;
				m.vertices = v;
				m.normals = n;
				m.triangles = mfs[i].sharedMesh.triangles;

				meshes[i] = m;
			}

			// Cleanup
			GameObject.DestroyImmediate(root);

			return meshes;
		}
	}
}
