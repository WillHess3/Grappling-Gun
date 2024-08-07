using UnityEngine;

public class GrappleSoundManager : MonoBehaviour {

    [SerializeField] private AudioClip _launchSound;
    [SerializeField] private AudioClip _stickSound;
    [SerializeField] private AudioClip _reelInSound;

    private GrapplingGun _grapplingGun;
    private AudioSource _audioSource;

    private bool _isWaitingForStickSoundToFinish;

    private void Start() {
        _grapplingGun = GetComponent<GrapplingGun>();
        _audioSource = GetComponent<AudioSource>();

        _grapplingGun.GrapplePhaseChanged += OnGrapplePhaseChanged;
    }

    private void OnGrapplePhaseChanged(GrapplingGun.GrapplePhase previousPhase) {
        if (_grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Launching && previousPhase == GrapplingGun.GrapplePhase.Waiting) {
            PlayLaunchSound();
        } else if (_grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Grappling && previousPhase == GrapplingGun.GrapplePhase.Launching) {
            PlayStickSound();
        } else if (_grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Retracting && previousPhase == GrapplingGun.GrapplePhase.Launching) {
            PlayReelInSound();
        } else if (_grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Waiting) {
            _audioSource.Stop();
        }
    }

    private void PlayLaunchSound() {
        _audioSource.Stop();

        _audioSource.clip = _launchSound;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    private void PlayStickSound() {
        _audioSource.Stop();

        _audioSource.clip = _stickSound;
        _audioSource.loop = false;
        _audioSource.Play();

        _isWaitingForStickSoundToFinish = true;
    }

    private void PlayReelInSound() {
        _audioSource.Stop();

        _audioSource.clip = _reelInSound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void Update() {
        if (_isWaitingForStickSoundToFinish && !_audioSource.isPlaying) {
            _isWaitingForStickSoundToFinish = false;
            PlayReelInSound();
        }
    }

    private void OnDestroy() {
        _grapplingGun.GrapplePhaseChanged -= OnGrapplePhaseChanged;
    }

}
