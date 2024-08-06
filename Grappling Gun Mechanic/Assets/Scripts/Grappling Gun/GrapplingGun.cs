using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Rigidbody _playerRigidbody;

    [SerializeField] private float _grappleDistance;

    private bool _isApplyingGrappleForces;

    private float _ropeLength;
    private bool _isInTension;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartGrapple();
        }

        if (Input.GetMouseButton(0)) {
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

            if (Vector3.Dot(_playerRigidbody.velocity, GrapplePoint - _playerRigidbody.position) <= 0 && _ropeLength * _ropeLength < (_playerRigidbody.position - GrapplePoint).sqrMagnitude) {
                TugPlayer();
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
            _ropeLength = hit.distance;
        }
    }

    private void ApplyGrappleForces() {
        Vector3 direction = (GrapplePoint - _playerRigidbody.position).normalized;
        float theta = Vector3.Angle(-direction, Vector3.down) * Mathf.Deg2Rad;

        float centripetalAcceleration = _playerRigidbody.velocity.sqrMagnitude / _ropeLength;
        Vector3 tension = _playerRigidbody.mass * (centripetalAcceleration + Physics.gravity.magnitude * Mathf.Cos(theta)) * direction;
        
        if (Mathf.Cos(theta) < -centripetalAcceleration / Physics.gravity.magnitude) {
            _isInTension = false;
        }

        if (_isInTension) {
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
