using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class OptimisedAnimator : MonoBehaviour
{
    public enum AnimatorMode
    {
        Full,
        Slow,
        Skip
    }

    [SerializeField, Min(0f)]
    [Tooltip("The animator will slow down the animation after this distance to save performance")]
    protected float slowDownDistance = 10f;
    [SerializeField, Range(0f, 1f)]
    [Tooltip("The speed the animation will play at when past the slowDownDistance")]
    protected float slowDownSpeed = 0.5f;
    [SerializeField, Min(0f)]
    [Tooltip("The animator will start skipping frames after this distance to save performance")]
    protected float frameSkipDistance = 30f;
    [SerializeField, Range(1, 30)]
    [Tooltip("The amount of frames the animator will update after when past frameSkipDistance")]
    protected int updateAnimationEveryNFrames = 5;

    protected Animator animator;
    protected Camera mainCam;
    protected float sqrDistance;
    protected int frameCounter;
    protected AnimatorMode currentAnimatorMode;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        mainCam = Camera.main;
    }

    protected virtual void Update()
    {
        sqrDistance = (transform.position - mainCam.transform.position).sqrMagnitude;
        if (sqrDistance < slowDownDistance * slowDownDistance)
        {
            if (currentAnimatorMode != AnimatorMode.Full)
            {
                animator.speed = 1f;
                if (frameCounter != 0) frameCounter = 0;
                currentAnimatorMode = AnimatorMode.Full;
            }
        }
        else if (sqrDistance < frameSkipDistance * frameSkipDistance)
        {
            if (currentAnimatorMode != AnimatorMode.Slow)
            {
                animator.speed = slowDownSpeed;
                if (frameCounter != 0) frameCounter = 0;
                currentAnimatorMode = AnimatorMode.Slow;
            }
        }
        else
        {
            if (currentAnimatorMode != AnimatorMode.Skip)
            {
                animator.speed = 0f;
                currentAnimatorMode = AnimatorMode.Skip;
            }
            UpdateEveryNthFrame(n: updateAnimationEveryNFrames);
        }
    }

    protected void UpdateEveryNthFrame(int n)
    {
        ++frameCounter;
        if (frameCounter < n) return;
        frameCounter = 0;
        animator.Update(Time.deltaTime * n);
    }
}
