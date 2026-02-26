using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InfectionSystem : MonoBehaviour
{
    public event Action<float, float> OnInfectionChanged;
    public static InfectionSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float maxInfection = 100f;
    [SerializeField] private float infectionRate = 5f;
    [SerializeField] private float healRate = 20f;
    [SerializeField] private InfectionVignette infectionVignette;

    [Header("Events")]
    public UnityEvent OnInfectionMaxReached;
    private Coroutine AddInfectionCoroutine;
    public float CurrentInfection { get; private set; } = 0f;
    public float MaxInfection => maxInfection;

    private bool _isHealing = false;
    private bool _inFog = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void EnterToxicFog()
    {
        if (_inFog) return;
        _inFog = true;

        if (AddInfectionCoroutine != null)
            StopCoroutine(AddInfectionCoroutine);

        AddInfectionCoroutine = StartCoroutine(AddInfectionLoop());
    }

    public void ExitToxicFog()
    {
        _inFog = false;

        if (AddInfectionCoroutine != null)
        {
            StopCoroutine(AddInfectionCoroutine);
            AddInfectionCoroutine = null;
        }
    }

    public void StartHealing() => _isHealing = true;
    public void StopHealing() => _isHealing = false;

    public void Heal(float amount)
    {
        CurrentInfection = Mathf.Max(0f, CurrentInfection - amount);
        OnInfectionChanged?.Invoke(CurrentInfection, maxInfection);
    }

    public void HealOverTime()
    {
        Heal(healRate * Time.deltaTime);
    }

    private IEnumerator AddInfectionLoop()
    {
        while (_inFog)
        {
            if (CurrentInfection < maxInfection)
            {
                CurrentInfection = Mathf.Min(maxInfection, CurrentInfection + 5f);
                infectionVignette.ShowDamage();
                OnInfectionChanged?.Invoke(CurrentInfection, maxInfection);

                if (CurrentInfection >= maxInfection)
                    OnInfectionMaxReached?.Invoke();
            }

            yield return new WaitForSeconds(1f / infectionRate);
        }
    }
}