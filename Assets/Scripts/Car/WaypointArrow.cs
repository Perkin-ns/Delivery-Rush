using UnityEngine;

public class WaypointArrow : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 2.2f;
    [SerializeField] private Color pickupColor = Color.green;
    [SerializeField] private Color deliveryColor = Color.blue;

    private GameObject arrowRoot;
    private Material arrowMaterial;
    private Color lastColor;

    private void Awake()
    {
        arrowRoot = new GameObject("WaypointArrow");
        arrowRoot.transform.SetParent(transform, false);
        arrowRoot.transform.localPosition = new Vector3(0f, hoverHeight, 0f);

        MeshRenderer[] renderers =
        {
            BuildPiece("Shaft", new Vector3(0f, 0f, 0.1f), new Vector3(0.08f, 0.04f, 0.6f), Quaternion.identity),
            BuildPiece("Wing_L", new Vector3(0f, 0f, 0.42f), new Vector3(0.24f, 0.04f, 0.06f), Quaternion.Euler(0f, 45f, 0f)),
            BuildPiece("Wing_R", new Vector3(0f, 0f, 0.42f), new Vector3(0.24f, 0.04f, 0.06f), Quaternion.Euler(0f, -45f, 0f))
        };

        arrowMaterial = renderers[0].material;
        for (int i = 1; i < renderers.Length; i++)
            renderers[i].sharedMaterial = arrowMaterial;

        arrowRoot.SetActive(false);
    }

    private MeshRenderer BuildPiece(string name, Vector3 localPos, Vector3 scale, Quaternion localRot)
    {
        GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piece.name = name;
        piece.transform.SetParent(arrowRoot.transform, false);
        piece.transform.localPosition = localPos;
        piece.transform.localScale = scale;
        piece.transform.localRotation = localRot;

        Collider col = piece.GetComponent<Collider>();
        if (col != null) Destroy(col);

        return piece.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!ServiceLocator.TryGet<IDeliveryService>(out var dm) || dm.IsGameOver)
        {
            arrowRoot.SetActive(false);
            return;
        }

        if (!dm.TryGetCurrentTarget(out Vector3 targetPos))
        {
            arrowRoot.SetActive(false);
            return;
        }

        Vector3 flatDir = targetPos - transform.position;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude < 0.01f)
        {
            arrowRoot.SetActive(false);
            return;
        }

        arrowRoot.SetActive(true);

        Color color = dm.HasActiveDelivery ? deliveryColor : pickupColor;
        if (color != lastColor)
        {
            lastColor = color;
            arrowMaterial.color = color;
        }

        arrowRoot.transform.rotation = Quaternion.LookRotation(flatDir.normalized);
    }
}
