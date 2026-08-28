using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        ServiceLocator.Get<ISceneService>().Load("SelectCar");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Has Quit The Game");
    }
}
