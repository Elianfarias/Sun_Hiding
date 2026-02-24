using System.Collections.Generic;
using UnityEngine;

public class MinimapBlinkManager : MonoBehaviour
{
    [SerializeField] private float blinkDuration = 0.6f;

    private static readonly List<MinimapEnemyIcon> _icons = new();
    private float _timer;

    public static void Register(MinimapEnemyIcon icon) => _icons.Add(icon);
    public static void Unregister(MinimapEnemyIcon icon) => _icons.Remove(icon);

    private void Update()
    {
        if (_icons.Count == 0) return;

        _timer += Time.deltaTime / blinkDuration;
        if (_timer > 1f) _timer -= 1f;

        // PingPong para que suba y baje suavemente
        float normalizedTime = Mathf.Sin(_timer * Mathf.PI);

        foreach (MinimapEnemyIcon icon in _icons)
            icon.Tick(normalizedTime);
    }
}