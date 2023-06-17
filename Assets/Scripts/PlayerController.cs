using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 0.15f;
    
    private Vector2 _move;

    private void Update()
    {
        MovePlayer();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        if (_move == Vector2.zero) return;

        Vector3 movement = new Vector3(_move.x, 0, _move.y);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), _rotateSpeed);
        transform.Translate(movement * _moveSpeed * Time.deltaTime, Space.World);
    }
}
