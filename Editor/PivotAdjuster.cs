using UnityEngine;
using UnityEditor;

namespace Vahaponur.PivotAdjuster
{
    public enum PivotPreset
    {
        Center,
        TopCenter,
        TopFront,
        TopBack,
        TopLeft,
        TopRight,
        BottomCenter,
        BottomFront,
        BottomBack,
        BottomLeft,
        BottomRight,
        Custom
    }

    public static class PivotAdjuster
    {
        public static void AdjustPivot(GameObject gameObject, Vector3 newPivotPosition)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("GameObject must have a MeshFilter with a valid mesh!");
                return;
            }

            // Record undo for all components that will change
            Undo.RecordObject(gameObject.transform, "Adjust Pivot");
            Undo.RecordObject(meshFilter, "Adjust Pivot");
            
            // Record colliders for undo
            Collider[] colliders = gameObject.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                Undo.RecordObject(collider, "Adjust Pivot");
            }
            
            Mesh originalMesh = meshFilter.sharedMesh;
            Mesh newMesh = Object.Instantiate(originalMesh);
            newMesh.name = originalMesh.name + "_pivot_adjusted";

            // Calculate offset - we want to move vertices so that newPivotPosition becomes (0,0,0)
            Vector3[] vertices = newMesh.vertices;
            Vector3 offset = -newPivotPosition;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += offset;
            }

            newMesh.vertices = vertices;
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();

            string path = AssetDatabase.GetAssetPath(originalMesh);
            if (string.IsNullOrEmpty(path) || path.StartsWith("Library/"))
            {
                path = "Assets/AdjustedMeshes/";
                if (!AssetDatabase.IsValidFolder("Assets/AdjustedMeshes"))
                {
                    try
                    {
                        AssetDatabase.CreateFolder("Assets", "AdjustedMeshes");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to create AdjustedMeshes folder: {e.Message}");
                        return;
                    }
                }
                path += newMesh.name + ".asset";
            }
            else
            {
                string directory = System.IO.Path.GetDirectoryName(path);
                string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                path = System.IO.Path.Combine(directory, filename + "_pivot_adjusted.asset");
            }

            try
            {
                AssetDatabase.CreateAsset(newMesh, path);
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create mesh asset at path: {path}\nError: {e.Message}");
                return;
            }

            // Apply the new mesh
            meshFilter.sharedMesh = newMesh;

            // Adjust transform to maintain world position
            // Convert the offset from local space to world space
            gameObject.transform.position += gameObject.transform.TransformVector(newPivotPosition);

            // Update colliders
            UpdateColliders(gameObject, offset);

            EditorUtility.SetDirty(gameObject);
        }

        private static void UpdateColliders(GameObject gameObject, Vector3 offset)
        {
            // Update MeshCollider
            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            }

            // Update BoxCollider
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.center += offset;
            }

            // Update SphereCollider
            SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.center += offset;
            }

            // Update CapsuleCollider
            CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                capsuleCollider.center += offset;
            }
        }

        public static Vector3 GetPresetPivotPosition(GameObject gameObject, PivotPreset preset)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("GameObject must have a MeshFilter with a valid mesh!");
                return Vector3.zero;
            }

            Bounds bounds = meshFilter.sharedMesh.bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 center = bounds.center;

            switch (preset)
            {
                case PivotPreset.Center:
                    return center;
                
                case PivotPreset.TopCenter:
                    return new Vector3(center.x, max.y, center.z);
                
                case PivotPreset.TopFront:
                    return new Vector3(center.x, max.y, min.z);
                
                case PivotPreset.TopBack:
                    return new Vector3(center.x, max.y, max.z);
                
                case PivotPreset.TopLeft:
                    return new Vector3(min.x, max.y, center.z);
                
                case PivotPreset.TopRight:
                    return new Vector3(max.x, max.y, center.z);
                
                case PivotPreset.BottomCenter:
                    return new Vector3(center.x, min.y, center.z);
                
                case PivotPreset.BottomFront:
                    return new Vector3(center.x, min.y, min.z);
                
                case PivotPreset.BottomBack:
                    return new Vector3(center.x, min.y, max.z);
                
                case PivotPreset.BottomLeft:
                    return new Vector3(min.x, min.y, center.z);
                
                case PivotPreset.BottomRight:
                    return new Vector3(max.x, min.y, center.z);
                
                default:
                    return center;
            }
        }
    }
}