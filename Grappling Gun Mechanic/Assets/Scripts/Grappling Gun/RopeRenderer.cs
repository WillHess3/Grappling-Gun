using UnityEngine;

public class RopeRenderer : MonoBehaviour {

    private GrapplingGun _grapplingGun;
    private LineRenderer _lineRenderer;
    private GrappleLauncher _launcher;

    private void Start () {
        _grapplingGun = GetComponentInParent<GrapplingGun>();
        _lineRenderer = GetComponent<LineRenderer>();
        _launcher = _grapplingGun.GetComponentInChildren<GrappleLauncher>();
    }

    private void Update () {
        if (_grapplingGun.CurrentGrapplePhase != GrapplingGun.GrapplePhase.Waiting) {
            if (!_lineRenderer.enabled) {
                _lineRenderer.enabled = true;
            }

            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _launcher.transform.position);
        } else {
            if (_lineRenderer.enabled) {
                _lineRenderer.enabled = false;
            }
        }
    }

}
