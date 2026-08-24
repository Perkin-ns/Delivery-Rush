using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Vector2 minimapSize = new Vector2(200f, 200f);
    [SerializeField] private float cameraHeight = 50f;
    [SerializeField] private float orthographicSize = 40f;
    [SerializeField] private int renderTextureSize = 256;

    private Camera minimapCamera;
    private RenderTexture rt;

    private void Awake()
    {
        rt = new RenderTexture(renderTextureSize, renderTextureSize, 16);

        GameObject camGO = new GameObject("MinimapCamera");
        minimapCamera = camGO.AddComponent<Camera>();
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = orthographicSize;
        minimapCamera.targetTexture = rt;
        minimapCamera.depth = 10f;
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        minimapCamera.backgroundColor = new Color(0.15f, 0.25f, 0.1f, 1f);

        UIFactory.EnsureCanvas(gameObject);

        GameObject rawImageGO = new GameObject("MinimapImage");
        rawImageGO.transform.SetParent(transform, false);

        RectTransform rect = rawImageGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = minimapSize;

        RawImage rawImage = rawImageGO.AddComponent<RawImage>();
        rawImage.texture = rt;
        rawImage.color = Color.white;
    }

    private void LateUpdate()
    {
        if (PlayerMovement.Instance == null) return;

        Transform car = PlayerMovement.Instance.transform;
        minimapCamera.transform.position = car.position + Vector3.up * cameraHeight;
        minimapCamera.transform.rotation = Quaternion.Euler(90f, car.eulerAngles.y, 0f);
    }

    private void OnDestroy()
    {
        if (rt != null)
            rt.Release();
    }
}
