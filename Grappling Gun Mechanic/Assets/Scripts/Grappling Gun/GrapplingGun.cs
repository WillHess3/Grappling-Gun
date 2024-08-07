using UnityEngine;
using System;

public class GrapplingGun : MonoBehaviour {

    public event Action<GrapplePhase> GrapplePhaseChanged;

    public Vector3 GrapplePoint { get; private set; }

    public enum GrapplePhase {
        Waiting,
        Launching,
        Grappling,
        Retracting
    }

    public GrapplePhase CurrentGrapplePhase { get; private set; }

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Rigidbody _playerRigidbody;

    [SerializeField] private float _grappleDistance;
    [SerializeField] private float _reelInAcceleration;

    [SerializeField] private float _retractionTime;
    [SerializeField] private Transform _launcherTransform;

    private Vector3 _launcherOffset;
    private float _retractionTimer;

    private float _reelInSpeed;
    private bool _isReelingIn;

    private bool _isApplyingGrappleForces;

    private float _ropeLength;
    private bool _isInTension;

    public void LaunchFinished(bool isLaunchSuccessful, Vector3 grapplePoint) {
        if (CurrentGrapplePhase != GrapplePhase.Launching) {
            return;
        }

        GrapplePoint = grapplePoint;

        if (isLaunchSuccessful) {
            StartGrapple();
        } else {
            CurrentGrapplePhase = GrapplePhase.Retracting;
            GrapplePhaseChanged?.Invoke(GrapplePhase.Launching);
        }
    }

    private void Start() {
        CurrentGrapplePhase = GrapplePhase.Waiting;
        _launcherOffset = _launcherTransform.localPosition;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && CurrentGrapplePhase == GrapplePhase.Waiting) {
            CurrentGrapplePhase = GrapplePhase.Launching;
            GrapplePhaseChanged?.Invoke(GrapplePhase.Waiting);
        }

        if (CurrentGrapplePhase == GrapplePhase.Grappling) {
            if (Input.GetMouseButton(0)) {
                _isApplyingGrappleForces = true;

                _isInTension = _ropeLength * _ropeLength < (_playerRigidbody.position - GrapplePoint).sqrMagnitude;
            } else {
                CurrentGrapplePhase = GrapplePhase.Retracting;
                GrapplePhaseChanged?.Invoke(GrapplePhase.Grappling);

                _isApplyingGrappleForces = false;

                _playerMovement.enabled = true;
            }
        }

        if (CurrentGrapplePhase == GrapplePhase.Retracting) {
            if (_retractionTimer < _retractionTime) {
                _retractionTimer += Time.deltaTime;

                float t = _retractionTimer / _retractionTime;
                Vector3 finalPosition = _playerRigidbody.position + _launcherOffset;
                _launcherTransform.position = Vector3.Lerp(GrapplePoint, finalPosition, t);
            } else {
                ResetGrapplingGun();
            }
        }
    }

    private void ResetGrapplingGun() {
        _launcherTransform.parent = transform;
        _launcherTransform.localPosition = _launcherOffset;
        _launcherTransform.localRotation = Quaternion.Euler(0, 0, 90);

        _retractionTimer = 0;

        CurrentGrapplePhase = GrapplePhase.Waiting;
        GrapplePhaseChanged?.Invoke(GrapplePhase.Retracting);
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
        _playerMovement.enabled = false;
        _ropeLength = (GrapplePoint - _playerRigidbody.position).magnitude;
        _isReelingIn = true;
        _reelInSpeed = 0;

        CurrentGrapplePhase = GrapplePhase.Grappling;
        GrapplePhaseChanged?.Invoke(GrapplePhase.Launching);
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
