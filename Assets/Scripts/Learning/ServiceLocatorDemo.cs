using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class ServiceLocatorDemo : MonoBehaviour
{
    private TMP_Text statusText;

    private void Awake()
    {
        IUIFactory ui = ServiceLocator.Get<IUIFactory>();
        ui.EnsureCanvas(gameObject);
        TMP_FontAsset font = ui.LoadFont();

        statusText = ui.CreateText(transform, "StatusText", new Vector2(0f, 0f), new Vector2(1100f, 800f), 22, font);
        statusText.alignment = TextAlignmentOptions.TopLeft;
        statusText.text = "Booting...";
    }

    private void Start()
    {
        StartCoroutine(RunDemo());
    }

    private IEnumerator RunDemo()
    {
        yield return Step(
            "=== SERVICE LOCATOR ===\n\n" +
            "A global registry mapping Type -> instance.\n" +
            "Code asks for a service by INTERFACE,\n" +
            "without knowing the concrete class.\n\n" +
            "Files:\n" +
            "  Core/ServiceLocator.cs  -> static Dictionary<Type, object>\n" +
            "  Core/IGameServices.cs   -> the interfaces (contracts)\n\n" +
            "Press SPACE to continue.",
            "Intro");

        var persistence = ServiceLocator.Get<IPersistenceService>();
        var scenes = ServiceLocator.Get<ISceneService>();
        var ui = ServiceLocator.Get<IUIFactory>();
        yield return Step(
            "=== 1. Pre-registered services ===\n\n" +
            "Registered at startup by CoreServiceRegistration\n" +
            "(RuntimeInitializeOnLoadMethod.BeforeSceneLoad):\n\n" +
            $"  IPersistenceService -> {persistence.GetType().Name}\n" +
            $"  ISceneService       -> {scenes.GetType().Name}\n" +
            $"  IUIFactory          -> {ui.GetType().Name}\n\n" +
            "This very text was drawn using that IUIFactory!",
            "Step 1: pre-registered services resolved");

        string error;
        try { ServiceLocator.Get<IDeliveryService>(); error = "(no exception)"; }
        catch (System.Exception e) { error = e.Message; }
        yield return Step(
            "=== 2. Get<T>() THROWS when missing ===\n\n" +
            "No DeliveryManager in this scene, so\n" +
            "IDeliveryService was never registered.\n\n" +
            "Get<IDeliveryService>() threw:\n\n" +
            $"  {error}",
            "Step 2: Get threw - " + error);

        bool found = ServiceLocator.TryGet<IDeliveryService>(out var missingSvc);
        yield return Step(
            "=== 3. TryGet<T>() is safe ===\n\n" +
            "Returns a bool instead of throwing.\n\n" +
            "TryGet<IDeliveryService>(out svc)\n" +
            $"  -> found = {found}, svc = {(missingSvc == null ? "null" : missingSvc.ToString())}\n\n" +
            "Use TryGet when a service might not exist yet\n" +
            "(e.g. IPlayerService before the car spawns).",
            "Step 3: TryGet returned false");

        ServiceLocator.Register<IDeliveryService>(new MockDelivery());
        var delivery = ServiceLocator.Get<IDeliveryService>();
        yield return Step(
            "=== 4. Register + consume ===\n\n" +
            "Register<IDeliveryService>(new MockDelivery());\n\n" +
            $"Get<IDeliveryService>().Score -> {delivery.Score}\n" +
            $"Get<IDeliveryService>().CurrentDeliveryName -> {delivery.CurrentDeliveryName}\n\n" +
            "We registered our own implementation and\n" +
            "read it back through the interface.",
            "Step 4: registered and consumed a mock");

        ServiceLocator.Register<IPlayerService, MockPlayer>(new MockPlayer());
        bool playerFound = ServiceLocator.TryGet<IPlayerService>(out var player);
        yield return Step(
            "=== 5. Two Register overloads ===\n\n" +
            "Register<T>(instance)\n" +
            "  -> stores under typeof(T)\n\n" +
            "Register<TInterface, TImpl>(instance)\n" +
            "  -> stores TImpl under typeof(TInterface)\n\n" +
            "Both fill the same dictionary.\n\n" +
            "Register<IPlayerService, MockPlayer>(...)\n" +
            $"TryGet<IPlayerService> -> {playerFound}\n" +
            $"  IsBoosted = {player.IsBoosted}",
            "Step 5: overloads demonstrated");

        delivery.TryGetCurrentTarget(out var pos);
        yield return Step(
            "=== 6. Mocking == testability ===\n\n" +
            "Consumers depend on the INTERFACE, not on\n" +
            "DeliveryManager, so a fake can stand in.\n\n" +
            $"MockDelivery.TryGetCurrentTarget -> ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})\n\n" +
            "No GameObject, no scene needed.\n" +
            "You can test logic with a mock in isolation.",
            "Step 6: mock returned a fake position");

        ServiceLocator.Reset();
        string resetError;
        try { ServiceLocator.Get<IPersistenceService>(); resetError = "(no exception)"; }
        catch (System.Exception e) { resetError = e.Message; }

        ServiceLocator.Register<IPersistenceService>(new PlayerPrefsService());
        ServiceLocator.Register<ISceneService>(new SceneService());
        ServiceLocator.Register<IUIFactory>(new UIFactoryService());
        var rebuilt = ServiceLocator.Get<IPersistenceService>();
        yield return Step(
            "=== 7. Reset() clears everything ===\n\n" +
            "ServiceLocator.Reset() wipes the registry.\n" +
            "Right after Reset, startup services are gone:\n\n" +
            $"  Get<IPersistenceService>() -> {resetError}\n\n" +
            "...but you can rebuild manually:\n\n" +
            "  Register<IPersistenceService>(new PlayerPrefsService());\n" +
            $"  Get<IPersistenceService>() -> {rebuilt.GetType().Name}\n\n" +
            "(RuntimeInitializeOnLoadMethod only re-runs\n" +
            "on the next Play session.)",
            "Step 7: Reset cleared, then rebuilt");

        yield return Step(
            "=== DONE ===\n\n" +
            "Recap:\n" +
            "  Register<T> / Register<TI,TImpl> -> add a service\n" +
            "  Get<T>      -> required service (throws)\n" +
            "  TryGet<T>   -> optional service (safe)\n" +
            "  Reset()     -> clear everything\n\n" +
            "Consumers depend on interfaces (IGameServices.cs),\n" +
            "not on DeliveryManager / PlayerMovement concretes.\n\n" +
            "Press SPACE to reload this scene.",
            "Done");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator Step(string body, string log)
    {
        statusText.text = body;
        Debug.Log("[ServiceLocatorDemo] " + log);

        yield return null;
        while (true)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                break;
            yield return null;
        }
    }

    private sealed class MockDelivery : IDeliveryService
    {
        public int Score => 999;
        public int DeliveriesCompleted => 3;
        public float TimeRemaining => 42f;
        public bool IsGameOver => false;
        public bool HasActiveDelivery => true;
        public string CurrentPickupName => "Mock Pickup";
        public string CurrentDeliveryName => "Mock Delivery";

        public bool TryGetCurrentTarget(out Vector3 position)
        {
            position = new Vector3(1f, 2f, 3f);
            return true;
        }

        public void StartDelivery(DeliveryPoint target, string pickupName) { }
        public bool TryCompleteDelivery(DeliveryPoint point) => true;
    }

    private sealed class MockPlayer : IPlayerService
    {
        public Transform Transform => null;
        public bool IsBoosted => true;
        public float BoostTimeRemaining => 9.5f;
        public void ActivateBoost(float multiplier, float duration) { }
    }
}
