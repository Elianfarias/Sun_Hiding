using UnityEngine;

public class MinimapEnemyIcon : MonoBehaviour
{
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float blinkDuration = 0.6f;
    [SerializeField] private float maxScale = 1.3f;

    private SpriteRenderer _sr;
    private Vector3 _originalScale;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        MinimapBlinkManager.Register(this);
    }

    // Llamado desde el manager central
    public void Tick(float normalizedTime)
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, normalizedTime);
        float scale = Mathf.Lerp(1f, maxScale, normalizedTime);

        Color color = _sr.color;
        color.a = alpha;
        _sr.color = color;

        transform.localScale = _originalScale * scale;
    }

    private void OnDestroy()
    {
        MinimapBlinkManager.Unregister(this);
    }
}