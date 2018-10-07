using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System.Linq;

namespace Parabox.Stl
{
    [ScriptedImporter(1, "stl")]
    public class StlImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var name = Path.GetFileNameWithoutExtension(ctx.assetPath);
            var meshes = Importer.Import(ctx.assetPath);

            if(meshes.Length < 1)
                return;

            if(meshes.Length < 2)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = name;
                meshes[0].name = "Mesh-" + name;
                go.GetComponent<MeshFilter>().sharedMesh = meshes[0];

                ctx.AddObjectToAsset(go.name, go);
                ctx.AddObjectToAsset(meshes[0].name, meshes[0]);
                ctx.SetMainObject(go);
            }
            else
            {
                var parent = new GameObject();
                parent.name = name;

                for(int i = 0, c = meshes.Length; i < c; i++)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.SetParent(parent.transform, false);
                    go.name = name + "(" + i + ")";

                    var mesh = meshes[i];
                    mesh.name = "Mesh-" + name + "(" + i + ")";
                    go.GetComponent<MeshFilter>().sharedMesh = mesh;

                    // ctx.AddObjectToAsset(go.name, go);
                    ctx.AddObjectToAsset(mesh.name, mesh);
                }

                ctx.AddObjectToAsset(parent.name, parent);
                ctx.SetMainObject(parent);
            }
        }
    }
}
