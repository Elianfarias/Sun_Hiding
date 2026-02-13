using Assets.Scripts.Gameplay.Player;
using System.Collections.Generic;
using UnityEngine;

public class UIChargerJump : MonoBehaviour
{
    [SerializeField] private PlayerMovement target;

    [Header("Chevron Settings")]
    [SerializeField] private GameObject chevronPrefab;
    [SerializeField] private int maxChevrons = 8;
    [SerializeField] private float chevronSpacing = 0.4f;
    [SerializeField] private float distanceFromPlayer = 1f;

    [Header("Visual Settings")]
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseAmount = 0.15f;
    [SerializeField] private float pulseMultiplier = 2f;
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.3f);

    private List<GameObject> activeChevrons = new();
    private List<Vector3> originalScales = new();
    private Camera mainCamera;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        mainCamera = Camera.main;
        target.OnChargerJump += OnChargerJump;
        target.OnJump += OnJump;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void OnDestroy()
    {
        target.OnChargerJump -= OnChargerJump;
        target.OnJump -= OnJump;
        ClearChevrons();
    }

    public void OnChargerJump(float currentCharge, float maxCharge)
    {
        if (currentCharge < 0.2f)
        {
            ClearChevrons();
            return;
        }

        gameObject.SetActive(true);

        float chargePercent = currentCharge / maxCharge;
        int chevronsToShow = Mathf.CeilToInt(chargePercent * maxChevrons);

        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3 playerPos = target.transform.position;
        Vector3 direction = (mouseWorldPos - playerPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        while (activeChevrons.Count < chevronsToShow)
        {
            GameObject chevron = Instantiate(chevronPrefab, transform);
            activeChevrons.Add(chevron);

            originalScales.Add(chevron.transform.localScale);
        }

        while (activeChevrons.Count > chevronsToShow)
        {
            GameObject toRemove = activeChevrons[activeChevrons.Count - 1];
            activeChevrons.RemoveAt(activeChevrons.Count - 1);
            originalScales.RemoveAt(originalScales.Count - 1);
            Destroy(toRemove);
        }

        for (int i = 0; i < activeChevrons.Count; i++)
        {
            GameObject chevron = activeChevrons[i];

            float distance = distanceFromPlayer + (i * chevronSpacing);
            Vector3 position = playerPos + direction * distance;

            chevron.transform.position = position;
            chevron.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (chevron.TryGetComponent<SpriteRenderer>(out var sr))
            {
                float normalizedIndex = (float)i / maxChevrons;
                float alpha = fadeOutCurve.Evaluate(normalizedIndex);
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;

                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed + i * 0.2f) * pulseAmount;
                Vector3 baseScale = originalScales[i];
                float finalScale = Mathf.Lerp(1f, pulseMultiplier, (pulse - 1f) / pulseAmount);

                if (direction.y > 0f)
                    chevron.transform.localScale = mousePos.y > mouseWorldPos.y ? (baseScale * finalScale) : -(baseScale * finalScale);
                else
                    chevron.transform.localScale = mousePos.x > mouseWorldPos.x ? (baseScale * finalScale) : -(baseScale * finalScale);


            }
        }
    }

    public void OnJump(bool jump)
    {
        if (jump)
            OnJump();
        else
            OnLand();

        gameObject.SetActive(false);
        ClearChevrons();
    }

    public void OnJump()
    {
        targetScale = new Vector3(
            originalScale.x * (1f - target.data.stretchAmount),  // Más delgado (80% ancho)
            originalScale.y * (1f + target.data.stretchAmount),  // Más alto (120% alto)
            originalScale.z
        );
    }

    public void OnLand()
    {
        targetScale = new Vector3(
            originalScale.x * (1f + target.data.squashAmount),   // Más ancho (130%)
            originalScale.y * (1f - target.data.squashAmount),   // Más bajo (70%)
            originalScale.z
        );
    }

    private void ClearChevrons()
    {
        foreach (GameObject chevron in activeChevrons)
        {
            if (chevron != null)
                Destroy(chevron);
        }
        activeChevrons.Clear();
        originalScales.Clear();
    }
}