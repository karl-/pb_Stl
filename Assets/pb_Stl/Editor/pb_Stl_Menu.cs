using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace Parabox.STL
{
	/**
	 * Menu items for STL import/export.
	 */
	public class pb_Stl_Menu : Editor
	{
		[MenuItem("Assets/Export/STL (Ascii)", true)]
		[MenuItem("Assets/Export/STL (Binary)", true)]
		static bool VerifyExport()
		{
			return Selection.transforms.SelectMany(x => x.GetComponentsInChildren<MeshFilter>()).FirstOrDefault(y => y.sharedMesh != null) != null;
		}

		[MenuItem("Assets/Export Model/STL (Ascii)", false, 30)]
		static void MenuExportAscii()
		{
			ExportWithFileDialog(Selection.gameObjects, FileType.Ascii);
		}

		[MenuItem("Assets/Export Model/STL (Binary)", false, 30)]
		static void MenuExportBinary()
		{
			ExportWithFileDialog(Selection.gameObjects, FileType.Binary);
		}

		private static void ExportWithFileDialog(GameObject[] gameObjects, FileType type)
		{
			string path = EditorUtility.SaveFilePanel("Save Mesh to STL", "", "Mesh", "stl");

			if( pb_Stl_Exporter.Export(path, gameObjects, type) )
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
	}
}
