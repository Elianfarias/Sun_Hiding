using UnityEngine;

public class BonfireHealZone : MonoBehaviour
{
    private bool _playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        InfectionSystem.Instance.StartHealing();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
        InfectionSystem.Instance.StopHealing();
    }

    private void Update()
    {
        if (_playerInside)
            InfectionSystem.Instance.HealOverTime();
    }
}