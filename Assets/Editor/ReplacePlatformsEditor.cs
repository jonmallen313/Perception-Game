using UnityEngine;
using UnityEditor;

public class ReplacePlatformsEditor : EditorWindow
{
    public GameObject prefabToReplaceWith;
    public string nameContains = "Platform"; // Filter by name (change if needed)

    [MenuItem("Tools/Replace Platforms")]
    public static void ShowWindow()
    {
        GetWindow<ReplacePlatformsEditor>("Replace Platforms");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace all platforms (cubes) with prefab", EditorStyles.boldLabel);
        prefabToReplaceWith = (GameObject)EditorGUILayout.ObjectField("Prefab To Use", prefabToReplaceWith, typeof(GameObject), false);
        nameContains = EditorGUILayout.TextField("Name Contains (filter)", nameContains);

        if (GUILayout.Button("Replace Platforms"))
        {
            if (prefabToReplaceWith == null)
            {
                Debug.LogError("Assign a prefab first!");
                return;
            }
            ReplaceAllPlatforms();
        }
    }

    void ReplaceAllPlatforms()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            if (go.name.Contains(nameContains) && go.GetComponent<BoxCollider>() != null)
            {
                Vector3 pos = go.transform.position;
                Quaternion rot = go.transform.rotation;
                Vector3 scale = go.transform.localScale;

                // Instantiate prefab in the same scene
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToReplaceWith, go.scene);
                newObj.transform.position = pos;
                newObj.transform.rotation = rot;
                newObj.transform.localScale = scale;

                Undo.RegisterCreatedObjectUndo(newObj, "Replace Platform");
                Undo.DestroyObjectImmediate(go);
                count++;
            }
        }
        Debug.Log($"Replaced {count} platforms!");
    }
}
