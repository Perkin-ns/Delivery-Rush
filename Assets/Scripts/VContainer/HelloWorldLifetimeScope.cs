using VContainer;
using VContainer.Unity;
using UnityEngine;


[RequireComponent(typeof(HelloWorldModel))]

public class HelloWorldLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<HelloWorldService>(Lifetime.Singleton);
        builder.RegisterEntryPoint<HelloWorldPresenter>();
        builder.RegisterComponent(GetComponent<HelloWorldModel>());
    }
}
