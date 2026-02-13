using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    [SerializeField] private Volume volume;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.1f;
    [SerializeField] private float holdDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Effect")]
    [SerializeField] private float maxIntensity = 0.6f;
    [SerializeField] private Color damageColor = Color.red;

    private Vignette vignette;

    private void Awake()
    {
        if (!volume.profile.TryGet(out vignette))
        {
            Debug.LogError("Vignette not found in Volume profile!");
        }
    }

    public void ShowDamage()
    {
        StopAllCoroutines();
        StartCoroutine(DamageEffectRoutine());
    }

    private IEnumerator DamageEffectRoutine()
    {
        vignette.color.Override(damageColor);

        yield return AnimateIntensity(0f, maxIntensity, fadeInDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return AnimateIntensity(maxIntensity, 0f, fadeOutDuration);
    }

    private IEnumerator AnimateIntensity(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.Override(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }

        vignette.intensity.Override(to);
    }
}