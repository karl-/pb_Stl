using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Parabox.STL
{
	/**
	 *	Provides menu items for writing STL files from a scene selection.
	 */
	public class pb_Stl_Menu : Editor
	{
		[MenuItem("Assets/Export/STL (Ascii)", true)]
		[MenuItem("Assets/Export/STL (Binary)", true)]
		static bool VerifyExport()
		{
			return Selection.transforms.SelectMany(x => x.GetComponentsInChildren<MeshFilter>()).FirstOrDefault(y => y.sharedMesh != null) != null;
		}

		[MenuItem("Assets/Export/STL (Ascii)", false)]
		static void MenuExportAscii()
		{
			ExportStl(FileType.Ascii);
		}
		
		[MenuItem("Assets/Export/STL (Binary)", false)]
		static void MenuExportBinary()
		{
			ExportStl(FileType.Binary);
		}

		public static void ExportStl(FileType type)
		{
			MeshFilter mf = Selection.transforms.SelectMany(x => x.GetComponentsInChildren<MeshFilter>()).FirstOrDefault(y => y.sharedMesh != null);

			if(mf != null)
			{
				string path = EditorUtility.SaveFilePanel("Save Mesh to STL", "", mf.sharedMesh.name, "stl");

				if(!string.IsNullOrEmpty(path))
				{
					if( pb_Stl.WriteFile(path, mf.sharedMesh, type) )
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
				else
				{
					UnityEngine.Debug.LogWarning("Invalid file path, aborting STL export.");
				}
			}
		}
	}
}
