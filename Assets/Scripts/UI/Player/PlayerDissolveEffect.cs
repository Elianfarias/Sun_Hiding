using UnityEngine;

public class PlayerDissolveEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer spriteJumpRenderer;
    [SerializeField] private PlayerController playerController;
    public float dissolveDuration = 0.4f;
    public float dissolveJumpDuration = 0.9f;

    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private Material _material;
    private float _duration;

    public void PlayDissolve(float? duration = null)
    {
        if (duration == null)
        {
            if (playerController.onJump)
            {
                _duration = dissolveJumpDuration;
                _material = spriteJumpRenderer.material;
            }
            else
            {
                _duration = dissolveDuration;
                _material = spriteRenderer.material;
            }
        }
        else
            _duration = duration.Value;

        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 0f, 1f, _duration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate(v => _material.SetFloat(DissolveAmount, v));
    }

    public void PlayRestore(float? duration = null)
    {
        if (duration == null)
        {
            if (playerController.onJump)
            {
                _duration = dissolveJumpDuration;
                _material = spriteJumpRenderer.material;
            }
            else
            {
                _duration = dissolveDuration;
                _material = spriteRenderer.material;
            }
        }
        else
            _duration = duration.Value;

        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 1f, 0f, _duration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate(v => _material.SetFloat(DissolveAmount, v));
    }
}