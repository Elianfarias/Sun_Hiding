using UnityEngine;

public class ToxicFogTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            InfectionSystem.Instance.EnterToxicFog();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            InfectionSystem.Instance.ExitToxicFog();
    }
}