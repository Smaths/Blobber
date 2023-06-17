using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Editor fields
    [BoxGroup("Dependencies")]
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 0.15f;
    [SerializeField] [DisplayAsString] private bool _isGrounded;

    // Private fields
    private Vector2 _moveDirection;

    #region Lifecycle
    private void OnValidate()
    {
        _characterController ??= GetComponentInChildren<CharacterController>();
    }

    private void Update()
    {
        _isGrounded = _characterController.isGrounded;

        MovePlayer();
    }
    #endregion

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        if (_moveDirection == Vector2.zero) return;

        // Movement
        // Vector3 movement = new Vector3(_moveDirection.x, _characterController.isGrounded ? 0 : -1.0f, _moveDirection.y) * _moveSpeed * Time.deltaTime ;
        Vector3 movement = new Vector3(_moveDirection.x, 0f, _moveDirection.y) * _moveSpeed * Time.deltaTime ;

        // Rotate character to face movement direction
        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        // Rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), _rotationSpeed);

        _characterController.Move(movement);
    }
}
