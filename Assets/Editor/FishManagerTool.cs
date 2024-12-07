using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.EditorTools;

public class FishManagerTool : EditorWindow
{
    private string fishManagerName = "New GameObject";
    private float spawnInterval = 1f;
    private int maxFish = 1000;
    public bool isMainBoss;
    public bool isHorizontal;
    public bool isVertical;
    public bool isGoFromMiddle;
    public GameObject fishPrefab;

    [MenuItem("THZ_FishManagerCreator/FishManager Creator")]
    public static void ShowWindow()
    {
        GetWindow<FishManagerTool>("Fish Manager Creator");
    }

    private void OnEnable()
    {

    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Fish Manager", EditorStyles.boldLabel);

        // Input for GameObject name
        fishManagerName = EditorGUILayout.TextField("Manager Name", fishManagerName);

        GUILayout.Space(10);
        GUILayout.Label("Fish Manager Settings", EditorStyles.boldLabel);

        // Input for spawn interval
        spawnInterval = EditorGUILayout.FloatField("Spawn Interval", spawnInterval);

        // Input for maximum fish
        maxFish = EditorGUILayout.IntField("Max Fish", maxFish);

        // Toggle for boolean fields
        isMainBoss = EditorGUILayout.Toggle("Is Main Boss", isMainBoss);
        isHorizontal = EditorGUILayout.Toggle("Is Horizontal", isHorizontal);
        isVertical = EditorGUILayout.Toggle("Is Vertical", isVertical);
        isGoFromMiddle = EditorGUILayout.Toggle("Is Go From Middle", isGoFromMiddle);
        fishPrefab = (GameObject)EditorGUILayout.ObjectField("Fish Prefab", fishPrefab, typeof(GameObject), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create GameObject"))
        {
            CreateGameObject();
        }
    }

    private void CreateGameObject()
    {
        GameObject newObject = new GameObject(fishManagerName);
        newObject.transform.SetParent(GameObject.Find("Managers").transform);
        FishManager attached = newObject.AddComponent<FishManager>();
        attached.parentTF = GameObject.Find("FishSpawn").transform;

        attached.fishPrefab = fishPrefab;
        attached.spawnInterval = spawnInterval;
        attached.maxFish = maxFish;
        attached._isMainBoss = isMainBoss;
        attached._isHorizontal = isHorizontal;
        attached._isVertical = isVertical;
        attached._isGoFromMiddle = isGoFromMiddle;

        // Select the created GameObject in the Hierarchy
        Selection.activeGameObject = newObject;

        Debug.Log($"Created GameObject: {fishManagerName} at {position}");
    }
}
