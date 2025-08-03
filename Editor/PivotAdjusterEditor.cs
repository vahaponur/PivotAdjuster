using UnityEngine;
using UnityEditor;

namespace Vahaponur.PivotAdjuster
{
    public class PivotAdjusterWindow : EditorWindow
    {
        private GameObject selectedObject;
        private PivotPreset selectedPreset = PivotPreset.Center;
        private Vector3 customPivotPosition = Vector3.zero;
        private bool showPreview = false;
        private Vector3 previewPivotPosition = Vector3.zero;

        [MenuItem("Tools/Pivot Adjuster")]
        public static void ShowWindow()
        {
            GetWindow<PivotAdjusterWindow>("Pivot Adjuster");
        }

        private void OnGUI()
        {
            GUILayout.Label("Pivot Adjuster", EditorStyles.boldLabel);
            GUILayout.Space(10);

            selectedObject = EditorGUILayout.ObjectField("Target GameObject", selectedObject, typeof(GameObject), true) as GameObject;

            if (selectedObject == null)
            {
                EditorGUILayout.HelpBox("Please select a GameObject with a MeshFilter component.", MessageType.Info);
                return;
            }

            MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                EditorGUILayout.HelpBox("Selected GameObject must have a MeshFilter with a valid mesh!", MessageType.Error);
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Pivot Preset", EditorStyles.boldLabel);
            selectedPreset = (PivotPreset)EditorGUILayout.EnumPopup("Preset", selectedPreset);

            if (selectedPreset == PivotPreset.Custom)
            {
                customPivotPosition = EditorGUILayout.Vector3Field("Custom Position", customPivotPosition);
            }
            else
            {
                Vector3 presetPosition = PivotAdjuster.GetPresetPivotPosition(selectedObject, selectedPreset);
                EditorGUILayout.Vector3Field("Preset Position", presetPosition);
            }

            GUILayout.Space(10);

            showPreview = EditorGUILayout.Toggle("Show Preview", showPreview);

            GUILayout.Space(20);

            if (GUILayout.Button("Apply Pivot Adjustment", GUILayout.Height(30)))
            {
                ApplyPivotAdjustment();
            }

            if (showPreview)
            {
                UpdatePreview();
            }
        }

        private void ApplyPivotAdjustment()
        {
            if (selectedObject == null) return;

            Vector3 newPivotPosition = selectedPreset == PivotPreset.Custom 
                ? customPivotPosition 
                : PivotAdjuster.GetPresetPivotPosition(selectedObject, selectedPreset);

            PivotAdjuster.AdjustPivot(selectedObject, newPivotPosition);
            
            EditorUtility.DisplayDialog("Success", "Pivot adjusted successfully!", "OK");
        }

        private void UpdatePreview()
        {
            if (selectedObject == null) return;

            previewPivotPosition = selectedPreset == PivotPreset.Custom 
                ? customPivotPosition 
                : PivotAdjuster.GetPresetPivotPosition(selectedObject, selectedPreset);

            SceneView.RepaintAll();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!showPreview || selectedObject == null) return;

            Handles.color = Color.red;
            Handles.SphereHandleCap(0, selectedObject.transform.TransformPoint(previewPivotPosition), Quaternion.identity, 0.1f, EventType.Repaint);
            
            Handles.color = Color.green;
            Handles.DrawWireCube(selectedObject.transform.TransformPoint(previewPivotPosition), Vector3.one * 0.05f);
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }

    [CustomEditor(typeof(MeshFilter))]
    public class MeshFilterPivotEditor : Editor
    {
        private bool showPivotAdjuster = false;
        private PivotPreset selectedPreset = PivotPreset.Center;
        private Vector3 customPivotPosition = Vector3.zero;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MeshFilter meshFilter = (MeshFilter)target;
            if (meshFilter.sharedMesh == null) return;

            GUILayout.Space(10);
            showPivotAdjuster = EditorGUILayout.Foldout(showPivotAdjuster, "Pivot Adjuster", true);

            if (showPivotAdjuster)
            {
                EditorGUI.indentLevel++;
                
                selectedPreset = (PivotPreset)EditorGUILayout.EnumPopup("Preset", selectedPreset);

                if (selectedPreset == PivotPreset.Custom)
                {
                    customPivotPosition = EditorGUILayout.Vector3Field("Custom Position", customPivotPosition);
                }

                if (GUILayout.Button("Apply Pivot"))
                {
                    Vector3 newPivotPosition = selectedPreset == PivotPreset.Custom 
                        ? customPivotPosition 
                        : PivotAdjuster.GetPresetPivotPosition(meshFilter.gameObject, selectedPreset);

                    PivotAdjuster.AdjustPivot(meshFilter.gameObject, newPivotPosition);
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}