using UnityEngine;
using TMPro;

public class CarSelectionManager : MonoBehaviour
{
    [SerializeField] private CarData[] cars;
    [SerializeField] private Transform previewParent;
    [SerializeField] private TMP_Text carNameText;
    [SerializeField] private string gameSceneName = "Game";

    private int currentIndex;
    private GameObject currentPreview;

    private void Start()
    {
        if (cars.Length == 0) return;

        currentIndex = ServiceLocator.Get<IPersistenceService>().GetInt("SelectedCar", 0);
        currentIndex = Mathf.Clamp(currentIndex, 0, cars.Length - 1);
        ShowCar(currentIndex);
    }

    private void Update()
    {
        if (currentPreview != null)
        {
            currentPreview.transform.Rotate(Vector3.up, cars[currentIndex].previewRotationSpeed * Time.deltaTime);
        }
    }

    public void OnNext()
    {
        currentIndex = (currentIndex + 1) % cars.Length;
        ShowCar(currentIndex);
    }

    public void OnPrevious()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = cars.Length - 1;
        ShowCar(currentIndex);
    }

    public void OnConfirm()
    {
        IPersistenceService save = ServiceLocator.Get<IPersistenceService>();
        save.SetInt("SelectedCar", currentIndex);
        save.Save();
        ServiceLocator.Get<ISceneService>().Load(gameSceneName);
    }

    private void ShowCar(int index)
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        CarData data = cars[index];
        if (data.carPrefab == null) return;

        currentPreview = Instantiate(data.carPrefab, previewParent);
        currentPreview.transform.localPosition = data.previewOffset;
        currentPreview.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        if (currentPreview.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }

        if (carNameText != null)
            carNameText.text = data.displayName;
    }
}
