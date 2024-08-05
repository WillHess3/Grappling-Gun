using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    private Vector3 _moveInputDirection;

    [SerializeField] private float _jumpForce = 5f;

    private Rigidbody _rigidbody;
    private bool _isGrounded;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        _moveInputDirection = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
        _isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 1.2f, .05f, 3);

        Jump();
    }

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        if (!_isGrounded) {
            return;
        }

        if (_moveInputDirection == Vector3.zero) { 
            //decelerate becasue no input
            if (_rigidbody.velocity.sqrMagnitude < .5f) {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            } else {
                Vector3 movementDirection = _rigidbody.velocity.normalized;
                _rigidbody.velocity -= _deceleration * Time.fixedDeltaTime * movementDirection;
            }
        } else {
            _moveInputDirection.Normalize();

            if (_rigidbody.velocity.sqrMagnitude < _maxSpeed * _maxSpeed) {
                //accelerate
                _rigidbody.velocity += _acceleration * Time.fixedDeltaTime * _moveInputDirection;
            } else {
                //decelerate becuase going too fast
                _rigidbody.velocity -= _deceleration * Time.fixedDeltaTime * _moveInputDirection;
            }
        }
    }

    private void Jump() {
        if (Input.GetButtonDown("Jump") && _isGrounded) {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}
