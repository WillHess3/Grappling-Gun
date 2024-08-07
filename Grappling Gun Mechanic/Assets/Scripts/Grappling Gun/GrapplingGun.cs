using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Rigidbody _playerRigidbody;

    [SerializeField] private float _grappleDistance;
    [SerializeField] private float _reelInAcceleration;
    private float _reelInSpeed;
    private bool _isReelingIn;

    private bool _isApplyingGrappleForces;

    private float _ropeLength;
    private bool _isInTension;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartGrapple();
        }

        if (Input.GetMouseButton(0) && IsGrappling) {
            _isApplyingGrappleForces = true;

            _isInTension = _ropeLength * _ropeLength < (_playerRigidbody.position - GrapplePoint).sqrMagnitude;
        } else {
            IsGrappling = false;
            _isApplyingGrappleForces = false;

            _playerMovement.enabled = true;
        }
    }

    private void FixedUpdate() {
        if (_isApplyingGrappleForces) {
            ApplyGrappleForces();

            if (Vector3.Dot(_playerRigidbody.velocity, GrapplePoint - _playerRigidbody.position) <= 0 && _isInTension) {
                TugPlayer();
            }

            if (_ropeLength > 1.5f && _isReelingIn) {
                _reelInSpeed += _reelInAcceleration * Time.fixedDeltaTime;
                _ropeLength -= _reelInSpeed * Time.fixedDeltaTime;
            } else {
                _ropeLength = 1.5f;
                _isReelingIn = false;
            }
        }
    }

    private void StartGrapple() {
        if (IsGrappling) {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, _grappleDistance)) {
            GrapplePoint = hit.point;
            IsGrappling = true;
            _playerMovement.enabled = false;
            _ropeLength = (GrapplePoint - _playerRigidbody.position).magnitude;
            _isReelingIn = true;
            _reelInSpeed = 0;
        }
    }

    private void ApplyGrappleForces() {
        Vector3 direction = (GrapplePoint - _playerRigidbody.position).normalized;
        float theta = Vector3.Angle(-direction, Vector3.down) * Mathf.Deg2Rad;

        float centripetalAcceleration = _playerRigidbody.velocity.sqrMagnitude / _ropeLength;
        Vector3 tension = _playerRigidbody.mass * (centripetalAcceleration + Physics.gravity.magnitude * Mathf.Cos(theta)) * direction;

        if (_isInTension) {
            if (_isReelingIn) {
                Vector3 reelInForce = _playerRigidbody.mass * _reelInAcceleration * direction;
                _playerRigidbody.AddForce(reelInForce, ForceMode.Force);
            }

            _playerRigidbody.AddForce(tension, ForceMode.Force);
        }
    }

    private void TugPlayer() {
        Vector3 tangentialVelocity = Vector3.ProjectOnPlane(_playerRigidbody.velocity, (GrapplePoint - _playerRigidbody.position).normalized);
        _playerRigidbody.velocity = tangentialVelocity;
        _isInTension = true;

        _playerRigidbody.position = GrapplePoint - (GrapplePoint - _playerRigidbody.position).normalized * _ropeLength;
    }
}
