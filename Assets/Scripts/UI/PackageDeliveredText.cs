using System.Collections;
using UnityEngine;
using TMPro;

public class PackageDeliveredText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float displayDuration = 1f;

    private Coroutine hideCoroutine;

    private void OnEnable() => DeliveryManager.OnDeliveryCompleted += Show;
    private void OnDisable() => DeliveryManager.OnDeliveryCompleted -= Show;

    private void Show()
    {
        text.gameObject.SetActive(true);
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        text.gameObject.SetActive(false);
        hideCoroutine = null;
    }
}
