using UnityEngine;

public class Deadzone : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out HealthSystem healthSystem)
            && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            healthSystem.DoDamage(99999);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out HealthSystem healthSystem)
            && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            healthSystem.DoDamage(99999);
    }
}
