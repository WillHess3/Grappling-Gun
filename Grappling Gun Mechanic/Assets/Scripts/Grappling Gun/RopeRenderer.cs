using UnityEngine;

public class RopeRenderer : MonoBehaviour {
    
    private LineRenderer _lineRenderer;
    private GrapplingGun _grapplingGun;
    private GrappleLauncher _launcher;

    private void Start() {
        _lineRenderer = GetComponent<LineRenderer>();
        _grapplingGun = GetComponentInParent<GrapplingGun>();
        _launcher = _grapplingGun.GetComponentInChildren<GrappleLauncher>();
    }

    private void Update() {
        if (_grapplingGun.CurrentGrapplePhase != GrapplingGun.GrapplePhase.Waiting) {
            if (!_lineRenderer.enabled) {
                _lineRenderer.enabled = true;
            }

            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _launcher.transform.position);
        } else {
            _lineRenderer.enabled = false;
        }
    }

}
