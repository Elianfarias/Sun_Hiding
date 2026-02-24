using UnityEngine;

namespace Assets.Scripts.Gameplay.Player
{
    public class JumpChargeFacade
    {
        private readonly PlayerDataSO _data;
        private readonly PlayerMovement _playerMovement;

        private float _currentCharge;
        private float _chargeStartTime;

        public float ChargeStartTime => _chargeStartTime;

        public JumpChargeFacade(PlayerDataSO data, PlayerMovement playerMovement)
        {
            _data = data;
            _playerMovement = playerMovement;
        }

        public void Begin()
        {
            _currentCharge = 0f;
            _chargeStartTime = Time.time;
        }

        public void Tick()
        {
            if (_playerMovement.IsOnGround())
                _playerMovement.Charging(ref _currentCharge);
        }

        public void Release()
        {
            if (_currentCharge < _data.tapThreshold)
                _playerMovement.Jump();
            else
                _playerMovement.ReleaseCharge(ref _currentCharge);

            Reset();
        }

        public void Reset()
        {
            _currentCharge = 0f;
            _chargeStartTime = 0f;
        }
    }
}