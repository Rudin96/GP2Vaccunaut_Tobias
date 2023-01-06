using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Collections;

namespace Team14
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterController : MonoBehaviour
    {
        public Vector3 ControllerRotation { get => controllerRotation; set => controllerRotation = value * sensitivity; }

        private ICharacterMovement characterMovement;
        private Vector3 controllerRotation;
        private Camera playerCamera;
        private PlayerInput playerInput;
        private float sensitivity;

        [Header("Crouching")]
        public bool toggleCrouch;
        public bool canCrouch = true;

        [Header("Jumping")]
        public bool canJump = true;

        [Header("Sprinting")]
        public bool canSprint = true;

        void Start()
        {
            characterMovement = GetComponent<ICharacterMovement>();
            playerCamera = Camera.main;
            playerInput = GetComponent<PlayerInput>();
            sensitivity = Settings.GetSettings().Sensitivity;
        }

        private void LateUpdate()
        {
            transform.Rotate(new Vector3(0f, ControllerRotation.x, 0f));
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if(canJump)
                if(context.performed)
                    characterMovement.Jump();
            if (context.canceled)
                characterMovement.StopJump();
        }

        private void OnApplicationFocus(bool focus)
        {
            Settings.LockCursorToGame();
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RaycastHit hitInfo;
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * 15f);
                if (Physics.Raycast(ray, out hitInfo, 15f))
                {
                    var interactable = hitInfo.collider.gameObject.GetComponent<IpickupIntereact>();
                    if (interactable != null)
                    {
                        interactable.Interiact();
                    }
                }
            }
        }

        public void HandleSprint(InputAction.CallbackContext context)
        {
            if (context.performed && canSprint)
                characterMovement.StartSprint();
            if (context.canceled && canSprint)
                characterMovement.StopSprint();
        }

        public void HandleCrouch(InputAction.CallbackContext context)
        {
            if (canCrouch)
            {
                if (context.performed)
                    characterMovement.Crouch();
                if (context.canceled)
                    characterMovement.UnCrouch();
            }
        }

        public void SetControllerRotation(InputAction.CallbackContext context) => ControllerRotation = context.ReadValue<Vector2>();

        public void ExitGame(InputAction.CallbackContext context) => Cursor.lockState = CursorLockMode.None;

        public void SetMoveDir(InputAction.CallbackContext context) => characterMovement.InputDir = context.ReadValue<Vector2>();
    }
}