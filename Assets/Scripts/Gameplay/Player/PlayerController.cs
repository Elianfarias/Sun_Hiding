using Assets.Scripts.Gameplay.Player;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool onJump;
    private HealthSystem healthSystem;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    [SerializeField] private PlayerDataSO playerData;

    [Header("VFX")]
    [SerializeField] private PlayerDissolveEffect dissolveEffect;
    [SerializeField] private DamageVignette damageVignette;
    [SerializeField] private SpriteRenderer spriteOnJump;
    [SerializeField] private GhostTrail trail;
    [Header("Sound clips")]
    [SerializeField] private AudioClip clipHurt;
    [SerializeField] private AudioClip clipDie;

    private Animator animator;
    private List<State> states = new();
    [SerializeField] private State currentState;
    [SerializeField] private State previousState;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponentInChildren<Animator>();
        playerMovement.OnJump += PlayerMovement_onJump;
        healthSystem.OnDie += HealthSystem_onDie;
        healthSystem.OnLifeUpdated += HealthSystem_onLifeUpdated;
        healthSystem.OnHealing += HealthSystem_onHealing;
    }

    private void Start()
    {
        states.Add(new StateIdle(playerMovement, this));
        states.Add(new StateWalk(playerMovement, this));
        states.Add(new StateJump(playerMovement, this));
        onJump = false;
        SwapStateTo(PlayerAnimatorEnum.Idle);
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentGameState == GameState.TUTORIAL)
            return;

        currentState.Update();
    }

    private void OnDestroy()
    {
        playerMovement.OnJump -= PlayerMovement_onJump;
        healthSystem.OnDie -= HealthSystem_onDie;
        healthSystem.OnLifeUpdated -= HealthSystem_onLifeUpdated;
        healthSystem.OnHealing -= HealthSystem_onHealing;
    }

    private void PlayerMovement_onJump(bool jump)
    {
        onJump = jump;
        spriteRenderer.enabled = !jump;
        spriteOnJump.enabled = jump;

        if (jump)
        {
            capsuleCollider.size = playerData.sizeJumpCollider;
            capsuleCollider.offset = playerData.offsetJumpCollider;
            dissolveEffect.PlayDissolve();
            trail.Enable();
        }
        else
        {
            capsuleCollider.size = playerData.sizeCollider;
            capsuleCollider.offset = playerData.offsetCollider;
            dissolveEffect.PlayRestore();
            trail.Disable();
        }
    }

    private void HealthSystem_onLifeUpdated(int life, int maxLife, bool takeDmgMyselft)
    {
        if (life < maxLife && !takeDmgMyselft)
        {
            damageVignette.ShowDamage();
            CombatEvents.RaiseCameraShake(2f, 0.12f, transform.position);
            AudioController.Instance.PlaySoundEffect(clipHurt, priority: 2);
        }
    }

    private void HealthSystem_onHealing(int life, int maxLife, bool takeDmgMyselft)
    {
    }

    private void HealthSystem_onDie()
    {
        AudioController.Instance.PlaySoundEffect(clipDie, priority: 3);
        GameStateManager.Instance.SetGameState(GameState.GAME_OVER);
    }

    public void SwapStateTo(PlayerAnimatorEnum nextState)
    {
        foreach (State state in states)
        {
            if (state.state == nextState)
            {
                currentState?.OnExit();

                currentState = state;
                currentState.OnEnter();
                break;
            }
        }
    }

    public void ChangeAnimatorState(int state)
    {
        animator.SetInteger("State", state);
    }
}