using UnityEngine;
using UnityEngine.UI;

public class UIInfectionBar : MonoBehaviour
{
    [SerializeField] private Image barLife;

    private void Start()
    {
        // Esperar un frame para que todos los TutorialTarget se registren
        LeanTween.delayedCall(0.2f, () =>
        {
            InfectionSystem.Instance.OnInfectionChanged += InfectionSystem_onInfectionUpdated;
        });
    }

    private void OnDestroy()
    {
        InfectionSystem.Instance.OnInfectionChanged -= InfectionSystem_onInfectionUpdated;
    }

    public void InfectionSystem_onInfectionUpdated(float current, float max)
    {
        float lerp = current / max;
        barLife.fillAmount = lerp;
    }
}