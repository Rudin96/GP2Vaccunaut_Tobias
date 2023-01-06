using UnityEngine;

namespace Team14
{
    public interface ICharacterMovement
    {
        public Vector3 InputDir { get; set; }
        public void AddMovementInput();
        public void Jump();
        public void StopJump();
        public void Crouch();
        public void UnCrouch();
        void StartSprint();
        void StopSprint();
    }
}