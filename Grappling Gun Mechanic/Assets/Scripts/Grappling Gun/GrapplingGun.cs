using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Rigidbody _playerRigidbody;

    [SerializeField] private float _grappleDistance;

    private bool _isApplyingGrappleForces;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartGrapple();
        }

        if (Input.GetMouseButton(0)) {
            _isApplyingGrappleForces = true;
        } else {
            IsGrappling = false;
            _isApplyingGrappleForces = false;

            _playerMovement.enabled = true;
        }
    }

    private void FixedUpdate() {
        if (_isApplyingGrappleForces) {
            ApplyGrappleForces();
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

            _playerRigidbody.velocity = Vector3.zero;
        }
    }

    private void ApplyGrappleForces() {
        Vector3 displacement = GrapplePoint - _playerRigidbody.position;
        float theta = Vector3.Angle(-displacement, Vector3.down) * Mathf.Deg2Rad;

        float centripetalAcceleration = _playerRigidbody.velocity.sqrMagnitude / displacement.magnitude;
        Vector3 tension = _playerRigidbody.mass * (centripetalAcceleration + Physics.gravity.magnitude * Mathf.Cos(theta)) * displacement.normalized;
        
        _playerRigidbody.AddForce(tension, ForceMode.Force);
    }
}
