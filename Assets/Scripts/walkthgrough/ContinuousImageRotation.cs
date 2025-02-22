using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ContinuousImageRotation : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The Image to rotate.")]
    public Image targetImage;
    [Tooltip("Rotation speed in degrees per second.")]
    public float rotateSpeed = 90f;

    private Tween rotationTween;
    private bool isPaused;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        // Calculate how long it takes to do a full 360° rotation
        float duration = 360f / rotateSpeed;

        // Create a DOTween rotation that loops indefinitely
        rotationTween = targetImage.rectTransform
            .DORotate(new Vector3(0, 0, -360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    /// <summary>
    /// Pauses the rotation tween if it's currently playing.
    /// </summary>
    public void PauseRotation()
    {
        if (rotationTween != null && rotationTween.IsPlaying())
        {
            rotationTween.Pause();
            isPaused = true;
        }
    }

    /// <summary>
    /// Resumes the rotation tween if it was paused.
    /// </summary>
    public void ResumeRotation()
    {
        if (rotationTween != null && isPaused)
        {
            rotationTween.Play();
            isPaused = false;
        }
    }
}