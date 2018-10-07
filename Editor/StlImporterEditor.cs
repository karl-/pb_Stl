using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System.Linq;

namespace Parabox.Stl
{
    [CustomEditor(typeof(StlImporter))]
    class StlImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var m_CoordinateSpace = serializedObject.FindProperty("m_CoordinateSpace");
            var m_UpAxis = serializedObject.FindProperty("m_UpAxis");

            EditorGUILayout.PropertyField(m_CoordinateSpace);
            EditorGUILayout.PropertyField(m_UpAxis);

            base.ApplyRevertGUI();
        }
    }
}
