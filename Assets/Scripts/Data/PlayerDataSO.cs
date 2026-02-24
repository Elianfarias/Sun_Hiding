using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
public class PlayerDataSO : ScriptableObject
{
    public KeyCode keyCodeJump = KeyCode.Space;
    public KeyCode keyCodeDown = KeyCode.S;
    public KeyCode keyCodeLeft = KeyCode.A;
    public KeyCode keyCodeRight = KeyCode.D;
    public KeyCode keyCodeDash = KeyCode.LeftShift;
    public KeyCode slowMotionKey = KeyCode.E;
    public KeyCode reloadKey = KeyCode.R;
    public Bullet bulletPrefab;
    public int speed;
    public int jumpForce;
    public string playerName;
    public float volumeMusic;
    public float volumeSFX;
    public float volumeUI;
    // Bullets
    public int chargerSize = 4;
    public float extraReloadDelay = 2f;
    // dash
    public float dashSpeed = 7f;
    public float inmortalDuration = 0.8f;
    public float dashDuration = 2f;
    public float dashCD = 2f;
    // Jump
    public float timeToFullCharge = 1f;
    public float tapThreshold = 0.3f;
    public float minChargeToRelease = 0.05f;
    public float maxImpulseForce = 8f;
    public bool canChargeWhileDashing = false;
    public float distanceOffset = 0.5f;
    public float radiusCircleRaycast = 5f;
    // slow motion
    public float slowMotionCooldown = 2f;
    public float slowMotionScale = 0.3f;
    public float slowMotionDuration = 2f;
    public int takeDamageSlowMotion = 1;
    // squash and stretch
    public float squashAmount = 0.3f;      // Cuánto se aplasta (30%)
    public float stretchAmount = 0.2f;     // Cuánto se estira (20%)
    public float dashSquashY = 0.4f;      // Dash se aplasta verticalmente
    public float dashStretchX = 0.3f;     // Dash se estira horizontalmente
    public float returnSpeed = 12f;        // Qué tan rápido vuelve a su estado normal
}