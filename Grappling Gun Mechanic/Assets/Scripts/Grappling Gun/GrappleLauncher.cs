using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLauncher : MonoBehaviour {

    [SerializeField] private GrapplingGun _grapplingGun;

    [SerializeField] private float _launchSpeed;
    [SerializeField] private float _maxLaunchTime;

    private bool _isLaunched;
    private float _launchTime;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();

        _grapplingGun.GrapplePhaseChanged += OnGrapplePhaseChanged;
    }

    private void OnGrapplePhaseChanged(GrapplingGun.GrapplePhase previousPhase) {
        if (previousPhase == GrapplingGun.GrapplePhase.Waiting && _grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Launching) {
            Launch();
        }
    }

    private void Launch() {
        _isLaunched = true;
        _launchTime = _maxLaunchTime;

        _collider.enabled = true;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(_rigidbody.mass * _launchSpeed * Camera.main.transform.forward, ForceMode.Impulse);

        transform.parent = null;

    }

    private void EndLaunch(bool isLaunchSuccessful) {
        _isLaunched = false;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;

        _grapplingGun.LaunchFinished(isLaunchSuccessful, transform.position);
    }

    private void Update() {
        if (_isLaunched) {
            _launchTime -= Time.deltaTime;

            if (_launchTime < 0) {
                EndLaunch(false);
            }

            if (Input.GetMouseButtonUp(0)) {
                EndLaunch(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (_isLaunched) {
            transform.position = collision.GetContact(0).point;
            transform.right = -collision.GetContact(0).normal;

            EndLaunch(true);
        }
    }

    private void OnDestroy() {
        _grapplingGun.GrapplePhaseChanged -= OnGrapplePhaseChanged;
    }
}
