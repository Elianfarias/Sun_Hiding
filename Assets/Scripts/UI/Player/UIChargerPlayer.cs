using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class UIChargerPlayer : MonoBehaviour
{
    private static readonly int OnReloadAnim = Animator.StringToHash("OnReload");
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private GameObject[] fireballsCharger;
    private Vector3 _originalScale;


    private void Awake()
    {
        _originalScale = fireballsCharger[0].transform.localScale;
        playerAttack.OnChargerUpdate += OnChargerUpdate;
        playerAttack.OnReload += OnReload;
    }

    private void OnDestroy()
    {
        playerAttack.OnChargerUpdate -= OnChargerUpdate;
        playerAttack.OnReload -= OnReload;
    }

    private void OnChargerUpdate(int current)
    {
        for (int i = current - 1; i < fireballsCharger.Length; i++)
        {
            fireballsCharger[i].SetActive(false);
        }
    }

    private void OnReload()
    {
        StartCoroutine(nameof(Reload));
    }

    private IEnumerator Reload()
    {
        for (int i = 0; i < fireballsCharger.Length; i++)
        {
            fireballsCharger[i].SetActive(true);
            AnimateBullet(fireballsCharger[i].GetComponent<RectTransform>(), i * 0.1f);
            yield return new WaitForSeconds(playerAttack.data.extraReloadDelay / fireballsCharger.Length);
        }
    }

    private void AnimateBullet(RectTransform bullet, float delay)
    {
        LeanTween.cancel(bullet.gameObject);
        bullet.localScale = _originalScale;

        LeanTween.scale(bullet.gameObject, _originalScale * 1.4f, 0.1f)
            .setDelay(delay)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(bullet.gameObject, _originalScale, 0.15f)
                    .setEase(LeanTweenType.easeOutBounce);
            });
    }
}