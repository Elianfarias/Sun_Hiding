using UnityEngine;

namespace Assets.Scripts.Gameplay.Player
{
    public class JumpInputFacade
    {
        private readonly PlayerDataSO _data;

        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;
        private float _lastJumpInitiatedTime = -Mathf.Infinity;

        private const float JumpGrace = 0.12f;

        public bool JumpBuffered => _jumpBufferCounter > 0f;
        public bool CoyoteAvailable => _coyoteTimeCounter > 0f;
        public bool JustJumped => (Time.time - _lastJumpInitiatedTime) < JumpGrace;

        public JumpInputFacade(PlayerDataSO data)
        {
            _data = data;
        }

        public void Tick(bool isOnGround)
        {
            TickCoyote(isOnGround);
            TickJumpBuffer();
        }

        private void TickCoyote(bool isOnGround)
        {
            if (isOnGround)
                _coyoteTimeCounter = _data.coyoteTime;
            else
                _coyoteTimeCounter -= Time.deltaTime;
        }

        private void TickJumpBuffer()
        {
            if (Input.GetKeyDown(_data.keyCodeJump))
                _jumpBufferCounter = _data.jumpBufferTime;
            else
                _jumpBufferCounter -= Time.deltaTime;
        }

        public void ConsumeJumpBuffer() => _jumpBufferCounter = 0f;
        public void ConsumeCoyote() => _coyoteTimeCounter = 0f;
        public void RegisterJump() => _lastJumpInitiatedTime = Time.time;

        public bool IsHoldingJump() => Input.GetKey(_data.keyCodeJump);
        public bool IsMovingLeft() => Input.GetKey(_data.keyCodeLeft);
        public bool IsMovingRight() => Input.GetKey(_data.keyCodeRight);
    }
}