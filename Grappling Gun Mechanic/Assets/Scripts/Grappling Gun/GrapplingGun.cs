using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private float _grappleDistance;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartGrapple();
        }

        if (Input.GetMouseButton(0)) {
            //Grapple
        } else {
            IsGrappling = false;
            _playerMovement.enabled = true;
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
        }
    }
}
