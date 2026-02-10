using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Player
{
    public class StateJump : State
    {
        protected bool onGround = false;
        protected float currentCharge = 0f;
        protected float chargeStartTime = 0f;
        private float lastJumpInitiatedTime = -Mathf.Infinity;
        private readonly float jumpGrace = 0.12f;

        public StateJump(PlayerMovement playerMovement, PlayerController playerController)
        {
            this.playerMovement = playerMovement;
            this.playerController = playerController;
            state = PlayerAnimatorEnum.Jump;
        }

        public override void OnEnter()
        {
            playerController.ChangeAnimatorState((int)state);
            chargeStartTime = Time.time;
            lastJumpInitiatedTime = 0;
        }

        public override void Update()
        {
            onGround = playerMovement.IsOnGround();
            bool justJumpedRecently = (Time.time - lastJumpInitiatedTime) < jumpGrace;

            if (System.Math.Abs(playerMovement.GetVelocityX()) > 0.1f)
                playerController.ChangeAnimatorState((int)PlayerAnimatorEnum.Run);
            else
                playerController.ChangeAnimatorState((int)PlayerAnimatorEnum.Idle);

            if (onGround && chargeStartTime < 0.1f && !justJumpedRecently)
            {
                if (System.Math.Abs(playerMovement.GetVelocityX()) > 0.1f)
                    playerController.SwapStateTo(PlayerAnimatorEnum.Run);
                else
                    playerController.SwapStateTo(PlayerAnimatorEnum.Idle);
            }
            else if (Input.GetKey(playerMovement.data.keyCodeLeft))
                playerMovement.MoveX(-1f);
            else if (Input.GetKey(playerMovement.data.keyCodeRight))
                playerMovement.MoveX(1f);

            if (!onGround)
                return;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (currentCharge < playerMovement.data.tapThreshold)
                    playerMovement.Jump();
                else
                    playerMovement.ReleaseCharge(ref currentCharge);

                lastJumpInitiatedTime = Time.time;
                currentCharge = 0f;
                chargeStartTime = 0f;
            }

            if (Input.GetKey(KeyCode.Space))
                playerMovement.Charging(ref currentCharge);
        }

        public override void OnExit()
        {
            playerMovement.OnGroundInvoke();
        }
    }
}