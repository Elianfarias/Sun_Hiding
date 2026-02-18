using UnityEngine;

public class DamageFloatingTextSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset = new(0f, 1.5f, 0f);

    private HealthSystem healthSystem;
    private int lastLife;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        healthSystem.OnLifeUpdated += OnLifeUpdated;
    }

    private void OnDisable()
    {
        healthSystem.OnLifeUpdated -= OnLifeUpdated;
    }

    private void OnLifeUpdated(int life, int maxLife, bool tookDamageMyself)
    {
        if (tookDamageMyself) return;

        int damage = lastLife - life;
        lastLife = life;

        if (damage <= 0) return;

        DamageFloatingText instance = DamageFloatingTextPool.Instance.Get(transform.position + spawnOffset);
        instance.Initialize(damage);
    }

    public void SetInitialLife(int life) => lastLife = life;
}