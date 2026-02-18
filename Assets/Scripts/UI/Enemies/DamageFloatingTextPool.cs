using System.Collections.Generic;
using UnityEngine;

public class DamageFloatingTextPool : MonoBehaviour
{
    public static DamageFloatingTextPool Instance { get; private set; }

    [SerializeField] private DamageFloatingText prefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<DamageFloatingText> pool = new();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
            CreateInstance();
    }

    private DamageFloatingText CreateInstance()
    {
        DamageFloatingText instance = Instantiate(prefab, transform);
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
        return instance;
    }

    public DamageFloatingText Get(Vector3 position)
    {
        if (pool.Count == 0)
            CreateInstance();

        DamageFloatingText instance = pool.Dequeue();
        instance.transform.position = position;
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void Return(DamageFloatingText instance)
    {
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}