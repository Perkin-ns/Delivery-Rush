using UnityEngine;
using System;
using VContainer.Unity;

public class HelloWorldPresenter : IStartable, IDisposable
{
    private readonly HelloWorldService _helloWorldService;
    private readonly HelloWorldModel _helloWorldModel;

    public HelloWorldPresenter(HelloWorldService helloWorldService, HelloWorldModel helloWorldModel)
    {
        _helloWorldService = helloWorldService;
        _helloWorldModel = helloWorldModel;
    }

    public void Start()
    {
        _helloWorldModel.HelloWorldButton.onClick.AddListener(Print);
    }

    public void Dispose()
    {
        _helloWorldModel.HelloWorldButton.onClick.RemoveListener(Print);
    }

    private void Print() => _helloWorldService.Print();
}
