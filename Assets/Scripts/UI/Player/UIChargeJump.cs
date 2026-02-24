using Assets.Scripts.Gameplay.Player;
using System.Collections.Generic;
using UnityEngine;

public class UIChargerJump : MonoBehaviour
{
    [SerializeField] private PlayerMovement target;

    [Header("Chevron Settings")]
    [SerializeField] private GameObject chevronPrefab;
    [SerializeField] private int maxChevrons = 8;
    [SerializeField] private float chevronSpacing = 0.15f; // tiempo entre puntos en segundos
    [SerializeField] private float distanceFromPlayer = 0.3f; // delay inicial en segundos

    [Header("Visual Settings")]
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseAmount = 0.15f;
    [SerializeField] private float pulseMultiplier = 2f;
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.3f);

    private List<GameObject> activeChevrons = new();
    private List<Vector3> originalScales = new();
    private Camera mainCamera;
    private Rigidbody2D playerRb;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        mainCamera = Camera.main;
        playerRb = target.GetComponent<Rigidbody2D>();

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

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 playerPos = target.transform.position;
        Vector3 direction = (mouseWorldPos - playerPos).normalized;

        // Velocidad inicial estimada del salto cargado
        // Ajustá "jumpForce" al nombre real del campo en tu PlayerMovement.data
        float jumpForce = Mathf.Lerp(
            target.data.jumpForce,
            target.data.maxImpulseForce,
            chargePercent
        );
        Vector2 initialVelocity = direction * jumpForce;

        // Gravedad real del rigidbody
        float gravity = Physics2D.gravity.y * playerRb.gravityScale;

        SyncChevronCount(chevronsToShow);
        PlaceChevronAlongParabola(chevronsToShow, playerPos, initialVelocity, gravity, direction);
    }

    private void SyncChevronCount(int targetCount)
    {
        while (activeChevrons.Count < targetCount)
        {
            GameObject chevron = Instantiate(chevronPrefab, transform);
            activeChevrons.Add(chevron);
            originalScales.Add(chevron.transform.localScale);
        }

        while (activeChevrons.Count > targetCount)
        {
            int last = activeChevrons.Count - 1;
            Destroy(activeChevrons[last]);
            activeChevrons.RemoveAt(last);
            originalScales.RemoveAt(last);
        }
    }

    private void PlaceChevronAlongParabola(int count, Vector3 origin, Vector2 initialVelocity, float gravity, Vector3 direction)
    {
        for (int i = 0; i < count; i++)
        {
            float t = distanceFromPlayer + i * chevronSpacing;

            // Posición parabólica: x = vx*t, y = vy*t + 0.5*g*t²
            float x = origin.x + initialVelocity.x * t;
            float y = origin.y + initialVelocity.y * t + 0.5f * gravity * t * t;
            Vector3 worldPos = new(x, y, 0f);

            // Tangente de la parábola para rotar el chevron correctamente
            float vx = initialVelocity.x;
            float vy = initialVelocity.y + gravity * t;
            float angle = Mathf.Atan2(vy, vx) * Mathf.Rad2Deg;

            GameObject chevron = activeChevrons[i];
            chevron.transform.position = worldPos;
            chevron.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            if (!chevron.TryGetComponent<SpriteRenderer>(out var sr)) continue;

            float normalizedIndex = (float)i / maxChevrons;
            Color color = sr.color;
            color.a = fadeOutCurve.Evaluate(normalizedIndex);
            sr.color = color;

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed + i * 0.2f) * pulseAmount;
            float finalScale = Mathf.Lerp(1f, pulseMultiplier, (pulse - 1f) / pulseAmount);
            chevron.transform.localScale = originalScales[i] * finalScale;
        }
    }

    public void OnJump(bool jump)
    {
        if (jump) OnJump();
        else OnLand();

        gameObject.SetActive(false);
        ClearChevrons();
    }

    public void OnJump()
    {
        targetScale = new Vector3(
            originalScale.x * (1f - target.data.stretchAmount),
            originalScale.y * (1f + target.data.stretchAmount),
            originalScale.z
        );
    }

    public void OnLand()
    {
        targetScale = new Vector3(
            originalScale.x * (1f + target.data.squashAmount),
            originalScale.y * (1f - target.data.squashAmount),
            originalScale.z
        );
    }

    private void ClearChevrons()
    {
        foreach (GameObject chevron in activeChevrons)
            if (chevron != null) Destroy(chevron);

        activeChevrons.Clear();
        originalScales.Clear();
    }
}