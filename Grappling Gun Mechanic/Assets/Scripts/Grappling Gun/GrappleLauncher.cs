using UnityEngine;

public class GrappleLauncher : MonoBehaviour {

    [SerializeField] private float _launchSpeed;
    [SerializeField] private float _maxLaunchTime;

    private bool _isLaunched;
    private float _launchTimer;

    private SphereCollider _sphereCollider;
    private Rigidbody _rigidbody;
    private GrapplingGun _grapplingGun;

    private void Start() {
        _sphereCollider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _grapplingGun = GetComponentInParent<GrapplingGun>();

        _grapplingGun.GrapplePhaseChanged += OnGrapplePhaseChanged;
    }

    private void OnGrapplePhaseChanged(GrapplingGun.GrapplePhase previousGrapplePhase) {
        if (_grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Launching && previousGrapplePhase == GrapplingGun.GrapplePhase.Waiting) {
            Launch();
        }
    }

    private void Launch() {
        _isLaunched = true;
        _launchTimer = _maxLaunchTime;
        _sphereCollider.enabled = true;
        _rigidbody.isKinematic = false;

        _rigidbody.AddForce(_rigidbody.mass * _launchSpeed * Camera.main.transform.forward, ForceMode.Impulse);

        transform.parent = null;
    }

    private void EndLaunch(bool isLaunchSuccessful) {
        _isLaunched = false;
        _sphereCollider.enabled = false;
        _rigidbody.isKinematic = true;

        _grapplingGun.LaunchFinished(isLaunchSuccessful, transform.position);
    }

    private void Update() {
        if (_isLaunched) {
            _launchTimer -= Time.deltaTime;

            if (_launchTimer < 0) {
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
