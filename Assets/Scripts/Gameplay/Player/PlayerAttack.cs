using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public event Action<int> OnChargerUpdate;
    public event Action OnReload;

    [Header("PlayerData")]
    public PlayerDataSO data;
    [SerializeField] private Transform firePoint;
    [SerializeField] private HealthSystem healthSystem;
    [Header("Bullets Pooling")]
    [SerializeField] private Bullet[] bulletsPool;
    [Header("Sound clips")]
    [SerializeField] private AudioClip clipFireball;

    private bool canFire = true;
    private int shotsSinceExtraDelay = 0;
    private int nextIndex = 0;
    private bool isSlowMotionActive = false;
    private Coroutine slowMotionCoroutine;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryFire();

        if (Input.GetKeyDown(data.slowMotionKey))
            ToggleSlowMotion();

        if (Input.GetKeyDown(data.reloadKey) && shotsSinceExtraDelay > 0)
        {
            Reload();
        }
    }
    private void Reload()
    {
        float cd = bulletsPool[0].data.baseCooldown;
        OnReload.Invoke();
        cd += data.extraReloadDelay;
        shotsSinceExtraDelay = 0;
        nextIndex = 0;
        StartCoroutine(Cooldown(cd));
    }

    private void TryFire()
    {
        if (!canFire) return;

        Bullet bullet = GetBulletFromPool();
        if (bullet == null) return;

        AudioController.Instance.PlaySoundEffect(clipFireball, priority: 2);
        bullet.gameObject.SetActive(true);
        bullet.transform.SetPositionAndRotation(firePoint.position, Quaternion.identity);
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (targetPos - transform.position).normalized;

        bullet.Shoot(direction);

        // Cooldown
        shotsSinceExtraDelay++;
        float cd = bullet.data.baseCooldown;

        if (shotsSinceExtraDelay >= data.chargerSize)
        {
            OnReload.Invoke();
            cd += data.extraReloadDelay;
            shotsSinceExtraDelay = 0;
        }

        StartCoroutine(Cooldown(cd));
    }

    private IEnumerator Cooldown(float seconds)
    {
        canFire = false;
        yield return new WaitForSeconds(seconds);
        canFire = true;
    }

    private Bullet GetBulletFromPool()
    {
        int count = bulletsPool.Length;

        for (int i = 0; i < count; i++)
        {
            var idx = (nextIndex + i) % count;
            if (!bulletsPool[idx].gameObject.activeSelf)
            {
                nextIndex = (idx + 1) % count;
                OnChargerUpdate?.Invoke(bulletsPool.Length - nextIndex + 1);
                return bulletsPool[idx];
            }
        }

        return null;
    }


    private void ToggleSlowMotion()
    {
        if (isSlowMotionActive)
        {
            // Desactivar slow motion
            StopCoroutine(slowMotionCoroutine);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            isSlowMotionActive = false;
        }
        else
            // Activar slow motion
            slowMotionCoroutine = StartCoroutine(SlowMotionRoutine());
    }

    private IEnumerator SlowMotionRoutine()
    {
        isSlowMotionActive = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = data.slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        while (isSlowMotionActive)
        {
            healthSystem.DoDamage(data.takeDamageSlowMotion, takeDmgMyself: true);
            yield return new WaitForSecondsRealtime(1f);
        }

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;
    }
}