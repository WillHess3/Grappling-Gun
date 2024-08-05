using UnityEngine;

public class RopeRenderer : MonoBehaviour {

    private GrapplingGun _grapplingGun;
    private LineRenderer _lineRenderer;

    private void Start () {
        _grapplingGun = GetComponentInParent<GrapplingGun>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update () {
        if (_grapplingGun.IsGrappling) {
            if (!_lineRenderer.enabled) {
                _lineRenderer.enabled = true;
            }

            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _grapplingGun.GrapplePoint);
        } else {
            if (_lineRenderer.enabled) {
                _lineRenderer.enabled = false;
            }
        }
    }

}
