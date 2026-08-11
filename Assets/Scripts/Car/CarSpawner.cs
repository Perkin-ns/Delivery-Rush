using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private CarData[] cars;

    private void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, cars.Length - 1);

        CarData data = cars[selectedIndex];
        if (data.carPrefab == null)
        {
            Debug.LogError("Car prefab is null for selected car index: " + selectedIndex);
            return;
        }

        GameObject spawnedCar = Instantiate(data.carPrefab, transform.position, transform.rotation);
        spawnedCar.name = data.displayName;
        spawnedCar.tag = "Player";
    }
}
