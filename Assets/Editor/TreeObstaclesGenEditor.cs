using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

[CustomEditor(typeof(TreeObstacleGen))]
public class TreeObstacleGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TreeObstacleGen generator = (TreeObstacleGen)target;
        if (GUILayout.Button("Generate Tree Obstacles & Bake NavMesh")) GenerateTrees(generator);
    }

    private void GenerateTrees(TreeObstacleGen generator)
    {
        Terrain terrain = generator.GetComponent<Terrain>();
        NavMeshSurface surface = generator.GetComponent<NavMeshSurface>();
        if (terrain == null || surface == null) return;
        TerrainData terrainData = terrain.terrainData;
        TreeInstance[] instances = terrainData.treeInstances;
        TreePrototype[] prototypes = terrainData.treePrototypes;
        GameObject parent = new GameObject("GeneratedTreeObstacles");
        parent.transform.SetParent(generator.transform);
        foreach (TreeInstance tree in instances)
        {
            Vector3 worldPos = Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;
            GameObject prefab = prototypes[tree.prototypeIndex].prefab;
            if (prefab == null) continue;
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            obj.transform.position = worldPos;
            obj.transform.rotation = Quaternion.Euler(0f, tree.rotation * Mathf.Rad2Deg, 0f); ;
            obj.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            obj.transform.SetParent(parent.transform);
        }
        surface.BuildNavMesh();
        DestroyImmediate(parent);
    }
}
