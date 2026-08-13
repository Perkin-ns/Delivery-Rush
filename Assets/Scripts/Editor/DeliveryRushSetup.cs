using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class DeliveryRushSetup : EditorWindow
{
    [MenuItem("Tools/Delivery Rush/Setup Car Prefabs")]
    public static void BuildCarPrefabs()
    {
        Material car1Mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cars/Car1.mat");
        Material car2Mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cars/Car2.mat");

        if (car1Mat == null || car2Mat == null)
        {
            Debug.LogError("Car materials not found. Ensure Car1.mat and Car2.mat exist in Assets/Materials/Cars/");
            return;
        }

        string prefabPath = "Assets/Prefabs/Cars/";

        CreateCarPrefab("Car1", car1Mat, prefabPath);
        CreateCarPrefab("Car2", car2Mat, prefabPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Car prefabs created successfully in " + prefabPath);
    }

    private static void CreateCarPrefab(string name, Material material, string path)
    {
        GameObject root = new GameObject(name);
        root.layer = LayerMask.NameToLayer("Default");

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        body.transform.localScale = new Vector3(1.8f, 0.5f, 3.8f);
        body.transform.localRotation = Quaternion.identity;
        body.GetComponent<MeshRenderer>().material = material;
        DestroyImmediate(body.GetComponent<Collider>());

        GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabin.name = "Cabin";
        cabin.transform.SetParent(body.transform);
        cabin.transform.localPosition = new Vector3(0f, 0.35f, -0.3f);
        cabin.transform.localScale = new Vector3(0.9f, 0.6f, 0.7f);
        cabin.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
        cabin.GetComponent<MeshRenderer>().material = material;
        DestroyImmediate(cabin.GetComponent<Collider>());

        CreateWheel(root, "Wheel_FL", new Vector3(-0.9f, -0.1f, 1.1f), material);
        CreateWheel(root, "Wheel_FR", new Vector3(0.9f, -0.1f, 1.1f), material);
        CreateWheel(root, "Wheel_RL", new Vector3(-0.9f, -0.1f, -1.1f), material);
        CreateWheel(root, "Wheel_RR", new Vector3(0.9f, -0.1f, -1.1f), material);

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 1000f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 2f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = new Vector3(0f, -0.3f, 0f);

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.4f, 0f);
        collider.size = new Vector3(1.9f, 0.8f, 4f);

        root.AddComponent<PlayerMovement>();
        root.AddComponent<WaypointArrow>();

        string fullPath = path + name + ".prefab";
        bool success;
        PrefabUtility.SaveAsPrefabAsset(root, fullPath, out success);
        if (success)
            Debug.Log("Created prefab: " + fullPath);
        else
            Debug.LogError("Failed to create prefab: " + fullPath);

        Object.DestroyImmediate(root);
    }

    private static void CreateWheel(GameObject parent, string name, Vector3 position, Material material)
    {
        GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheel.name = name;
        wheel.transform.SetParent(parent.transform);
        wheel.transform.localPosition = position;
        wheel.transform.localScale = new Vector3(0.45f, 0.15f, 0.45f);
        wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        DestroyImmediate(wheel.GetComponent<Collider>());

        MeshRenderer renderer = wheel.GetComponent<MeshRenderer>();
        Material wheelMat = new Material(material);
        Color col = material.color;
        wheelMat.color = new Color(col.r * 0.3f, col.g * 0.3f, col.b * 0.3f, 1f);
        renderer.material = wheelMat;
    }

    [MenuItem("Tools/Delivery Rush/Setup Speed Boost Prefab")]
    public static void BuildSpeedBoostPrefab()
    {
        Material boostMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PowerUps/SpeedBoost.mat");
        if (boostMat == null)
        {
            Debug.LogError("SpeedBoost material not found. Ensure SpeedBoost.mat exists in Assets/Materials/PowerUps/");
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/PowerUps"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder("Assets/Prefabs", "PowerUps");
        }

        GameObject boost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boost.name = "SpeedBoost";
        boost.transform.localScale = Vector3.one;
        boost.GetComponent<MeshRenderer>().material = boostMat;

        Collider col = boost.GetComponent<Collider>();
        col.isTrigger = true;

        boost.AddComponent<SpeedBoost>();

        string fullPath = "Assets/Prefabs/PowerUps/SpeedBoost.prefab";
        bool success;
        PrefabUtility.SaveAsPrefabAsset(boost, fullPath, out success);
        if (success)
            Debug.Log("Created prefab: " + fullPath);
        else
            Debug.LogError("Failed to create prefab: " + fullPath);

        Object.DestroyImmediate(boost);
    }

    [MenuItem("Tools/Delivery Rush/Create Car Data Assets")]
    public static void CreateCarDataAssets()
    {
        string carDataPath = "Assets/ScriptableObjects/Cars/";
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder(carDataPath))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Cars");

        foreach (string guid in AssetDatabase.FindAssets("t:CarData", new[] { carDataPath }))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
        }

        GameObject car1Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cars/Car1.prefab");
        GameObject car2Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cars/Car2.prefab");

        CreateCarDataAsset(carDataPath + "Car1Data.asset", "Red Racer", car1Prefab);
        CreateCarDataAsset(carDataPath + "Car2Data.asset", "Blue Streak", car2Prefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Car Data assets created in " + carDataPath);
    }

    private static void CreateCarDataAsset(string path, string displayName, GameObject prefab)
    {
        CarData data = ScriptableObject.CreateInstance<CarData>();
        data.displayName = displayName;
        data.carPrefab = prefab;

        AssetDatabase.CreateAsset(data, path);
    }

    [MenuItem("Tools/Delivery Rush/Setup All")]
    public static void SetupAll()
    {
        BuildCarPrefabs();
        BuildSpeedBoostPrefab();
        CreateCarDataAssets();
        SetupSelectCarScene();
        SetupGameScene();
        SetupGameOverScene();
        AddScenesToBuildSettings();
        Debug.Log("Delivery Rush setup complete!");
    }

    [MenuItem("Tools/Delivery Rush/Add Scenes to Build Settings")]
    public static void AddScenesToBuildSettings()
    {
        EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;
        var sceneList = new System.Collections.Generic.List<EditorBuildSettingsScene>(currentScenes);

        AddSceneIfMissing(sceneList, "Assets/Scenes/MainMenu.unity");
        AddSceneIfMissing(sceneList, "Assets/Scenes/SelectCar.unity");
        AddSceneIfMissing(sceneList, "Assets/Scenes/Game.unity");
        AddSceneIfMissing(sceneList, "Assets/Scenes/GameOver.unity");

        EditorBuildSettings.scenes = sceneList.ToArray();
        Debug.Log("Scenes added to Build Settings.");
    }

    private static void AddSceneIfMissing(System.Collections.Generic.List<EditorBuildSettingsScene> list, string path)
    {
        foreach (var s in list)
        {
            if (s.path == path) return;
        }
        list.Add(new EditorBuildSettingsScene(path, true));
    }

    [MenuItem("Tools/Delivery Rush/Setup SelectCar Scene")]
    public static void SetupSelectCarScene()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/SelectCar.unity", OpenSceneMode.Single);

        GameObject[] roots = scene.GetRootGameObjects();
        foreach (GameObject go in roots)
        {
            if (go.name == "SceneSetup") Object.DestroyImmediate(go);
        }

        GameObject setupRoot = new GameObject("SceneSetup");

        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.transform.SetParent(setupRoot.transform);
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        GameObject previewPlatform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        previewPlatform.name = "PreviewPlatform";
        previewPlatform.transform.SetParent(setupRoot.transform);
        previewPlatform.transform.position = new Vector3(0f, -0.1f, 0f);
        previewPlatform.transform.localScale = new Vector3(3f, 0.1f, 3f);
        previewPlatform.GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        DestroyImmediate(previewPlatform.GetComponent<Collider>());

        GameObject previewParent = new GameObject("PreviewParent");
        previewParent.transform.SetParent(setupRoot.transform);
        previewParent.transform.position = new Vector3(0f, 0.15f, 0f);

        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        CarSelectionManager manager = canvasGO.AddComponent<CarSelectionManager>();
        manager.transform.SetParent(setupRoot.transform);

        CarData[] carDatas = new CarData[2];
        carDatas[0] = AssetDatabase.LoadAssetAtPath<CarData>("Assets/ScriptableObjects/Cars/Car1Data.asset");
        carDatas[1] = AssetDatabase.LoadAssetAtPath<CarData>("Assets/ScriptableObjects/Cars/Car2Data.asset");

        SerializedObject so = new SerializedObject(manager);
        so.FindProperty("cars").arraySize = carDatas.Length;
        for (int i = 0; i < carDatas.Length; i++)
        {
            so.FindProperty($"cars.Array.data[{i}]").objectReferenceValue = carDatas[i];
        }
        so.FindProperty("previewParent").objectReferenceValue = previewParent.transform;
        so.FindProperty("gameSceneName").stringValue = "Game";
        so.ApplyModifiedProperties();

        GameObject carNameTextGO = CreateUIElement(canvasGO, "CarNameText", new Vector2(0f, 200f), 24);
        TMPro.TMP_Text carNameTMP = carNameTextGO.GetComponent<TMPro.TMP_Text>();
        carNameTMP.text = "Car Name";
        carNameTMP.alignment = TMPro.TextAlignmentOptions.Center;
        so.FindProperty("carNameText").objectReferenceValue = carNameTMP;
        so.ApplyModifiedProperties();

        CreateTextButton(canvasGO, "PreviousButton", "Previous", new Vector2(-200f, -80f), manager, "OnPrevious");
        CreateTextButton(canvasGO, "NextButton", "Next", new Vector2(200f, -80f), manager, "OnNext");
        CreateTextButton(canvasGO, "ConfirmButton", "Confirm", new Vector2(0f, -320f), manager, "OnConfirm");

        CanvasRenderer[] renderers = canvasGO.GetComponentsInChildren<CanvasRenderer>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("SelectCar scene setup complete.");
    }

    private static GameObject CreateUIElement(GameObject parent, string name, Vector2 position, int fontSize)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300f, 50f);

        TMPro.TMP_Text text = go.AddComponent<TMPro.TMP_Text>();
        text.fontSize = fontSize;
        text.color = Color.white;

        return go;
    }

    private static void CreateTextButton(GameObject parent, string name, string label, Vector2 position, CarSelectionManager manager, string methodName)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200f, 60f);

        UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.2f, 0.6f, 1f, 1f);

        UnityEngine.UI.Button button = go.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = image;

        System.Reflection.MethodInfo method = typeof(CarSelectionManager).GetMethod(methodName);
        if (method != null)
        {
            var action = (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(
                typeof(UnityEngine.Events.UnityAction), manager, method);
            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(button.onClick, action);
        }

        GameObject labelGO = new GameObject("Text");
        labelGO.transform.SetParent(go.transform);

        RectTransform labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;

        TMPro.TMP_Text labelText = labelGO.AddComponent<TMPro.TMP_Text>();
        labelText.text = label;
        labelText.fontSize = 24;
        labelText.color = Color.white;
        labelText.alignment = TMPro.TextAlignmentOptions.Center;
    }

    [MenuItem("Tools/Delivery Rush/Setup Game Scene")]
    public static void SetupGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject sceneRoot = new GameObject("SceneSetup");

        CreateMainCamera(sceneRoot);
        CreateDirectionalLight(sceneRoot);
        CreateGround(sceneRoot);
        CreateRoads(sceneRoot);
        CreateObstacles(sceneRoot);
        CreateSpeedBoosts(sceneRoot);
        PickupPoint[] pickups = CreatePickupDeliveryPairs(sceneRoot);
        CreateSpawnPoint(sceneRoot);
        CreateHUDCanvas(sceneRoot);
        CreateGameManager(sceneRoot);
        WirePickupsToDeliveryManager(sceneRoot, pickups);

        string path = "Assets/Scenes/Game.unity";
        bool success = EditorSceneManager.SaveScene(scene, path);
        if (success)
            Debug.Log("Game scene saved to " + path);
        else
            Debug.LogError("Failed to save Game scene.");
    }

    private static void CreateMainCamera(GameObject root)
    {
        GameObject camGO = new GameObject("Main Camera");
        camGO.transform.SetParent(root.transform);
        camGO.tag = "MainCamera";
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.fieldOfView = 60f;
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 1000f;
        camGO.transform.position = new Vector3(0f, 8f, -12f);
        camGO.transform.rotation = Quaternion.Euler(30f, 0f, 0f);
        camGO.AddComponent<AudioListener>();
        camGO.AddComponent<CameraController>();
    }

    private static void CreateDirectionalLight(GameObject root)
    {
        GameObject lightGO = new GameObject("Directional Light");
        lightGO.transform.SetParent(root.transform);
        Light l = lightGO.AddComponent<Light>();
        l.type = LightType.Directional;
        l.intensity = 1f;
        l.color = new Color(1f, 0.95686275f, 0.8392157f);
        l.shadows = LightShadows.Soft;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static void CreateGround(GameObject root)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.SetParent(root.transform);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(8f, 1f, 8f);
        ground.GetComponent<MeshRenderer>().material.color = new Color(0.25f, 0.45f, 0.15f, 1f);
    }

    private static void CreateRoads(GameObject root)
    {
        GameObject roadsParent = new GameObject("Roads");
        roadsParent.transform.SetParent(root.transform);

        CreateRoad(roadsParent, "Road_Horizontal", new Vector3(0f, 0.02f, -3f), Quaternion.identity, 40f, 3f);
        CreateRoad(roadsParent, "Road_Vertical", new Vector3(0f, 0.02f, 10f), Quaternion.identity, 3f, 26f);
        CreateRoad(roadsParent, "Road_BranchLeft", new Vector3(-10f, 0.02f, -3f), Quaternion.identity, 3f, 14f);
    }

    private static void CreateRoad(GameObject parent, string name, Vector3 position, Quaternion rotation, float width, float length)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = name;
        road.transform.SetParent(parent.transform);
        road.transform.position = position;
        road.transform.rotation = rotation;
        road.transform.localScale = new Vector3(width, 0.05f, length);
        road.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.2f, 0.22f, 1f);

        road.layer = LayerMask.NameToLayer("Default");
    }

    private static void CreateObstacles(GameObject root)
    {
        GameObject obstaclesParent = new GameObject("Obstacles");
        obstaclesParent.transform.SetParent(root.transform);

        CreateObstacle(obstaclesParent, new Vector3(-6f, 0.5f, -3f));
        CreateObstacle(obstaclesParent, new Vector3(6f, 0.5f, -3f));
        CreateObstacle(obstaclesParent, new Vector3(0f, 0.5f, 15f));
        CreateObstacle(obstaclesParent, new Vector3(0f, 0.5f, -10f));
        CreateObstacle(obstaclesParent, new Vector3(4f, 0.5f, 3f));
        CreateObstacle(obstaclesParent, new Vector3(-4f, 0.5f, 8f));
        CreateObstacle(obstaclesParent, new Vector3(-10f, 0.5f, -10f));
        CreateObstacle(obstaclesParent, new Vector3(8f, 0.5f, -8f));
    }

    private static void CreateObstacle(GameObject parent, Vector3 position)
    {
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = "Obstacle";
        obstacle.transform.SetParent(parent.transform);
        obstacle.transform.position = position;
        obstacle.transform.localScale = new Vector3(1.2f, 1f, 1.2f);
        obstacle.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.2f, 0.1f, 1f);
    }

    private static PickupPoint[] CreatePickupDeliveryPairs(GameObject root)
    {
        GameObject pointsParent = new GameObject("PickupDeliveryPoints");
        pointsParent.transform.SetParent(root.transform);

        System.Collections.Generic.List<PickupPoint> pickupList = new System.Collections.Generic.List<PickupPoint>();

        CreatePair(pointsParent, "Pair1",
            new Vector3(-12f, 0.5f, -3f), "West Pickup",
            new Vector3(0f, 0.5f, 20f), "North Delivery", pickupList);

        CreatePair(pointsParent, "Pair2",
            new Vector3(0f, 0.5f, 22f), "North Pickup",
            new Vector3(12f, 0.5f, -3f), "East Delivery", pickupList);

        CreatePair(pointsParent, "Pair3",
            new Vector3(-12f, 0.5f, -14f), "Southwest Pickup",
            new Vector3(6f, 0.5f, 10f), "Central Delivery", pickupList);

        return pickupList.ToArray();
    }

    private static void CreatePair(GameObject parent, string pairName,
        Vector3 pickupPos, string pickupDisplayName,
        Vector3 deliveryPos, string deliveryDisplayName,
        System.Collections.Generic.List<PickupPoint> pickupList)
    {
        GameObject pickupObj = CreateMarker(parent, pairName + "_Pickup", pickupPos, Color.green);
        PickupPoint pickup = pickupObj.AddComponent<PickupPoint>();

        GameObject deliveryObj = CreateMarker(parent, pairName + "_Delivery", deliveryPos, Color.blue);
        DeliveryPoint delivery = deliveryObj.AddComponent<DeliveryPoint>();
        delivery.displayName = deliveryDisplayName;

        SerializedObject pickupSO = new SerializedObject(pickup);
        pickupSO.FindProperty("pairedDeliveryPoint").objectReferenceValue = delivery;
        pickupSO.FindProperty("pickupName").stringValue = pickupDisplayName;
        pickupSO.ApplyModifiedProperties();

        delivery.SetVisualActive(false);
        pickup.SetVisualActive(true);

        pickupList.Add(pickup);
    }

    private static GameObject CreateMarker(GameObject parent, string name, Vector3 position, Color color)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = name;
        marker.transform.SetParent(parent.transform);
        marker.transform.position = position;
        marker.transform.localScale = new Vector3(2f, 2f, 2f);
        marker.GetComponent<MeshRenderer>().material.color = color;

        BoxCollider col = marker.GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.5f, 1.5f, 1.5f);

        return marker;
    }

    private static void CreateSpawnPoint(GameObject root)
    {
        GameObject spawnPoint = new GameObject("SpawnPoint");
        spawnPoint.transform.SetParent(root.transform);
        spawnPoint.transform.position = new Vector3(0f, 0.5f, -15f);
        spawnPoint.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        CarSpawner spawner = spawnPoint.AddComponent<CarSpawner>();
        CarData[] carDatas = new CarData[2];
        carDatas[0] = AssetDatabase.LoadAssetAtPath<CarData>("Assets/ScriptableObjects/Cars/Car1Data.asset");
        carDatas[1] = AssetDatabase.LoadAssetAtPath<CarData>("Assets/ScriptableObjects/Cars/Car2Data.asset");

        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("cars").arraySize = carDatas.Length;
        for (int i = 0; i < carDatas.Length; i++)
            so.FindProperty($"cars.Array.data[{i}]").objectReferenceValue = carDatas[i];
        so.ApplyModifiedProperties();
    }

    private static void CreateHUDCanvas(GameObject root)
    {
        GameObject canvasGO = new GameObject("HUDCanvas");
        canvasGO.transform.SetParent(root.transform);
        canvasGO.AddComponent<ScoreUI>();
        canvasGO.AddComponent<Minimap>();

        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.transform.SetParent(root.transform);
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
    }

    private static void CreateGameManager(GameObject root)
    {
        GameObject gmGO = new GameObject("GameManager");
        gmGO.transform.SetParent(root.transform);
        gmGO.AddComponent<GameManager>();
    }

    private static void CreateSpeedBoosts(GameObject root)
    {
        GameObject boostsParent = new GameObject("SpeedBoosts");
        boostsParent.transform.SetParent(root.transform);

        CreateSpeedBoost(boostsParent, new Vector3(-8f, 0.5f, -3f));
        CreateSpeedBoost(boostsParent, new Vector3(4f, 0.5f, 14f));
        CreateSpeedBoost(boostsParent, new Vector3(-6f, 0.5f, -12f));
    }

    private static void CreateSpeedBoost(GameObject parent, Vector3 position)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PowerUps/SpeedBoost.prefab");
        if (prefab == null)
        {
            Debug.LogError("SpeedBoost prefab not found. Run 'Setup Speed Boost Prefab' first.");
            return;
        }

        GameObject boost = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        boost.transform.SetParent(parent.transform, false);
        boost.transform.localPosition = position;
    }

    private static void WirePickupsToDeliveryManager(GameObject root, PickupPoint[] pickups)
    {
        DeliveryManager dm = root.GetComponentInChildren<DeliveryManager>();
        if (dm == null || pickups == null) return;

        SerializedObject so = new SerializedObject(dm);
        so.FindProperty("pickupPoints").arraySize = pickups.Length;
        for (int i = 0; i < pickups.Length; i++)
            so.FindProperty($"pickupPoints.Array.data[{i}]").objectReferenceValue = pickups[i];
        so.ApplyModifiedProperties();
    }

    [MenuItem("Tools/Delivery Rush/Setup GameOver Scene")]
    public static void SetupGameOverScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject sceneRoot = new GameObject("SceneSetup");
        sceneRoot.AddComponent<GameOverUI>();

        string path = "Assets/Scenes/GameOver.unity";
        bool success = EditorSceneManager.SaveScene(scene, path);
        if (success)
            Debug.Log("GameOver scene saved to " + path);
        else
            Debug.LogError("Failed to save GameOver scene.");
    }
}
