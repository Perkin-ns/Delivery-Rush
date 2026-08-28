using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;

    public void Play()
    {
        IPersistenceService save = ServiceLocator.Get<IPersistenceService>();
        string name = (nameField != null && !string.IsNullOrWhiteSpace(nameField.text)) ? nameField.text.Trim() : "Player";
        save.SetString("PlayerName", name);
        save.Save();

        ServiceLocator.Get<ISceneService>().Load("SelectCar");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Has Quit The Game");
    }
}
