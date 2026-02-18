using System.Collections;
using TMPro;
using UnityEngine;

public class DamageFloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private AnimationCurve fadeCurve;

    private Coroutine animationCoroutine;

    public void Initialize(int damage)
    {
        text.SetText("-" + damage.ToString());

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Color originalColor = text.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / lifetime;

            transform.position += floatSpeed * Time.unscaledDeltaTime * Vector3.up;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadeCurve.Evaluate(t));

            yield return null;
        }

        DamageFloatingTextPool.Instance.Return(this);
    }
}