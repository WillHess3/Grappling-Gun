using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private float _speed;
    private Vector3 _moveInputDirection;

    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private Transform _groundCheckTransform;

    private Rigidbody _rigidbody;
    private bool _isGrounded;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        _moveInputDirection = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
        _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, .05f, 3);

        Jump();
        Move();

    }

    private void Move() {
        if (!_isGrounded) {
            return;
        }

        if (_moveInputDirection == Vector3.zero) { 
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        } else {
            _moveInputDirection.Normalize();
            _rigidbody.velocity = _speed * _moveInputDirection;
        }
    }

    private void Jump() {
        if (Input.GetButtonDown("Jump") && _isGrounded) {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}
