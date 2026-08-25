using UnityEngine;
using UnityEngine.UI;

public class HelloWorldModel : MonoBehaviour
{
    [SerializeField] private Button _helloWorldButton;

    public Button HelloWorldButton => _helloWorldButton;
}
