using UnityEngine;

[CreateAssetMenu(fileName = "NewCarData", menuName = "Delivery Rush/Car Data")]
public class CarData : ScriptableObject
{
    public string displayName = "Car";
    public GameObject carPrefab;
    public Vector3 previewOffset = new Vector3(0f, 0.5f, 0f);
    public float previewRotationSpeed = 30f;
}
