using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer playerJumpSprite;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float ghostInterval = 0.05f;
    [SerializeField] private float ghostLifetime = 0.2f;
    [SerializeField] private Color ghostColor = new(0.5f, 0f, 1f, 0.6f);


    private SpriteRenderer _sprite;
    private bool _active;
    private float _timer;

    public void Enable() => _active = true;
    public void Disable() => _active = false;

    private void Update()
    {
        if (!_active) return;

        _timer += Time.deltaTime;
        if (_timer < ghostInterval) return;

        _timer = 0f;
        SpawnGhost();
    }

    private void SpawnGhost()
    {
        if (playerController.onJump)
            _sprite = playerJumpSprite;
        else
            _sprite = playerSprite;

        GameObject ghost = new("Ghost");
        ghost.transform.position = _sprite.transform.position;
        ghost.transform.rotation = _sprite.transform.rotation;
        ghost.transform.localScale = _sprite.transform.localScale;

        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = _sprite.sprite;
        sr.color = ghostColor;
        sr.sortingLayerName = _sprite.sortingLayerName;
        sr.sortingOrder = _sprite.sortingOrder - 1;

        LeanTween.value(ghost, ghostColor.a, 0f, ghostLifetime)
            .setOnUpdate(a =>
            {
                if (sr == null) return;
                Color c = sr.color;
                c.a = a;
                sr.color = c;
            })
            .setOnComplete(() => Destroy(ghost));
    }
}