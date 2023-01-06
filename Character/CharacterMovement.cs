using UnityEngine;
using UnityEngine.Events;

namespace Team14
{
    public enum MovementState
    {
        Walking,
        Falling
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour, ICharacterMovement
    {
        public MovementState MovementState { get; set; }
        public Vector3 InputDir { get; set; }

        public float Velocity { get => rb.velocity.magnitude; }

        public AudioManager AudioManager;

        private Rigidbody rb;
        private CapsuleCollider capsuleCollider;
        private float _currentWalkSpeed;
        private int jumpCount;
        private float gravityDownForce;
        private float gravityMultiplier = 3f;
        private bool isCrouching;
        private bool isSprinting;
        private float fallTime;

        private float footStepTimer;

        [Header("Walking")]
        public float Acceleration = 100f;
        public float Deceleration = 50f;
        public float MaxWalkSpeed = 10f;
        public float BaseEyeHeight = 2f;
        [Range(0f, 1f)] public float FootStepVolume = .7f;
        [Range(0f, 5f)] public float FootStepSoundDelay = 3.5f;

        [Header("Sprinting")]
        public float MaxSprintSpeed;

        [Header("Jumping")]
        public float JumpForce;
        [Range(0, 10)] public int MaxJumpCount = 1;
        public UnityEvent OnJumpResponse;

        [Header("Falling")]
        public float AirControl = 1f;
        public float MaxFallSpeed = 20f;
        public float FallThreshold = .1f;

        [Header("Crouching")]
        public float MaxCrouchSpeed;
        public float CrouchedEyeHeight;

        [Header("GroundCheck")]
        [Range(0f, 1f)] private float groundCheckRadius = .45f;
        [Range(0f, 1f)] private float groundCheckThreshhold = .6f;
        public UnityEvent OnGroundedResponse;
        private bool hasJumped;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            _currentWalkSpeed = MaxWalkSpeed;
            gravityDownForce = Mathf.Abs(Physics.gravity.y);
        }

        private void FixedUpdate()
        {
            AddMovementInput();
        }

        private void OnCollisionExit(Collision collision)
        {
            UpdateFallingState();
        }

        private void OnCollisionEnter(Collision collision)
        {
            UpdateFallingState();
        }

        private void UpdateFallingState()
        {
            if (CheckIfGrounded())
            {
                if (MovementState != MovementState.Falling) return;
                MovementState = MovementState.Walking;
                if(hasJumped)
                {
                    jumpCount = 0;
                    hasJumped = false;
                }
                OnGroundedResponse.Invoke();
                if (AudioManager.GameSounds == null) return;
                if (AudioManager.GameSounds.CharacterLandingSounds != null)
                {
                    int rng = Random.Range(0, AudioManager.GameSounds.CharacterLandingSounds.Count);
                    AudioManager.PlayAudio(AudioManager.GameSounds.CharacterLandingSounds[rng], AudioType.SFX, transform.position);
                }
            }
            else
            {
                MovementState = MovementState.Falling;
            }
        }

        private bool CheckIfGrounded()
        {
            return Physics.CheckSphere(transform.position + -transform.up * groundCheckThreshhold, groundCheckRadius);
        }

        private bool HeadingIntoWall()
        {
            Vector3 wallCheckPoint1 = transform.position + transform.up * .5f + transform.forward * (InputDir.y / 2f) + transform.right * InputDir.x;
            Vector3 wallCheckPoint2 = transform.position + -transform.up * .5f + transform.forward * (InputDir.y / 2f) + transform.right * InputDir.x;
            return Physics.CheckCapsule(wallCheckPoint1, wallCheckPoint2, groundCheckRadius);
        }

        public void AddMovementInput()
        {
            switch (MovementState)
            {
                case MovementState.Walking:
                    if (InputDir != Vector3.zero)
                    {
                        PlayFootStepSound();
                        rb.AddForce(transform.forward * InputDir.y * Acceleration + transform.right * InputDir.x * Acceleration + Vector3.down * gravityDownForce * gravityMultiplier, ForceMode.Force);
                    }

                    ClampVelocity(_currentWalkSpeed);


                    if (InputDir == Vector3.zero)
                        Decelerate();
                    if (InputDir == Vector3.down || InputDir == Vector3.left || InputDir == Vector3.right)
                        StopSprint();

                    break;
                case MovementState.Falling:
                    if (!HeadingIntoWall())
                    {
                        rb.AddForce(transform.forward * InputDir.y * AirControl + transform.right * InputDir.x * AirControl, ForceMode.Force);
                    }

                    ClampVelocity(MaxFallSpeed);

                    break;
            }
        }

        public void PlayFootStepSound()
        {
            footStepTimer += (1f * Time.deltaTime) * rb.velocity.magnitude;
            if(footStepTimer >= FootStepSoundDelay)
            {
                if (AudioManager.GameSounds == null) return;
                if (AudioManager.GameSounds.FootstepSounds != null)
                {
                    int rng = Random.Range(0, AudioManager.GameSounds.FootstepSounds.Count);
                    AudioManager.PlayAudio(AudioManager.GameSounds.FootstepSounds[rng], AudioType.SFX, transform.position, FootStepVolume);
                    footStepTimer = 0f;
                }
            }
        }

        private void ClampVelocity(float maxVel)
        {
            if (rb.velocity.magnitude > 0f)
            {
                Vector3 clampedVel = Vector3.ClampMagnitude(rb.velocity, maxVel);
                rb.velocity = new Vector3(clampedVel.x, rb.velocity.y, clampedVel.z);
            }

        }

        private void Decelerate()
        {
            Vector3 normalizedVelocity = Vector3.Normalize(rb.velocity);
            if (rb.velocity.magnitude > .3f)
                rb.AddForce(-new Vector3(normalizedVelocity.x, 0f, normalizedVelocity.z) * Deceleration + Vector3.down * gravityDownForce * 2f, ForceMode.Force);
        }

        public void Jump()
        {
            if (isCrouching) return;
            if (MovementState == MovementState.Falling && MaxJumpCount < 2) return;
            if (jumpCount < MaxJumpCount)
            {
                jumpCount++;
                rb.AddForce(new Vector3(0f, JumpForce, 0f));
                MovementState = MovementState.Falling;
                OnJumpResponse.Invoke();
                hasJumped = true;
            }
        }

        public void StopJump()
        {
            if(hasJumped)
            {
                if(MaxJumpCount < 2 && jumpCount > 0)
                jumpCount = 0;
                hasJumped = false;
            }
        }

        public void Crouch()
        {
            _currentWalkSpeed = MaxCrouchSpeed;
            capsuleCollider.height = CrouchedEyeHeight;
            isCrouching = true;
        }

        public void UnCrouch()
        {
            _currentWalkSpeed = MaxWalkSpeed;
            capsuleCollider.height = BaseEyeHeight;
            isCrouching = false;
        }

        public void StartSprint()
        {
            if (!isCrouching)
            {
                _currentWalkSpeed = MaxSprintSpeed;
                isSprinting = true;
            }
        }

        public void StopSprint()
        {
            if (isCrouching)
            {
                _currentWalkSpeed = MaxCrouchSpeed;
            }
            else
            {
                _currentWalkSpeed = MaxWalkSpeed;
            }
            isSprinting = false;
        }

        //private void OnDrawGizmos()
        //{
        //    capsuleCollider = GetComponent<CapsuleCollider>();
        //    if (CheckIfGrounded())
        //        Gizmos.color = Color.green;
        //    else
        //        Gizmos.color = Color.red;
        //    if (HeadingIntoWall())
        //        Gizmos.color = Color.yellow;
        //    else
        //        Gizmos.color = Color.red;
        //    Gizmos.DrawSphere(transform.position + -transform.up * groundCheckThreshhold, groundCheckRadius);
        //    Gizmos.DrawSphere(transform.position + transform.up * .5f + transform.forward * InputDir.y + transform.right * InputDir.x, groundCheckRadius);
        //    Gizmos.DrawSphere(transform.position + -transform.up * .5f + transform.forward * InputDir.y + transform.right * InputDir.x, groundCheckRadius);
        //}
    }
}