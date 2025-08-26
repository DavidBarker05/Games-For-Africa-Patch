using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Holdable : Interactable
{
    [SerializeField]
    float maxReleaseVelocity = 10f;
    [SerializeField]
    float groundDistance = 0.1f;
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField, Range(2, 10)]
    [Tooltip("The maximum number of \"ground\" colliders checked and stored in memory.")]
    int maxGroundColliders = 2;
    [SerializeField, Range(5, 15)]
    [Tooltip("The maximum number of colliders checked and stored in memory when handling clipping.")]
    int maxClippingColliders = 10;

    Rigidbody rb;
    int startLayer;
    Vector3 startPos;
    Quaternion startRot;
    Collider _collider;
    bool held = false;
    Vector3 lastPos;
    Vector3 releaseVel;
    bool isClipping;
    Collider[] groundColliders;
    Collider[] clippingColliders;
    Transform[] childTransforms;
    LayerMask playerMask;
    bool isStartLayerAlsoGround;

    public Vector3 StartPos => startPos;
    public Quaternion StartRot => startRot;
    public bool IsGrounded => !isClipping && Physics.OverlapBoxNonAlloc(_collider.bounds.center + Vector3.down * groundDistance, _collider.bounds.extents, groundColliders, transform.rotation, groundLayer) > (isStartLayerAlsoGround ? 1 : 0);

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startLayer = gameObject.layer;
        _collider = GetComponent<Collider>();
        startPos = transform.position;
        startRot = transform.rotation;
        groundColliders = new Collider[maxGroundColliders];
        clippingColliders = new Collider[maxClippingColliders];
        childTransforms = GetComponentsInChildren<Transform>();
        playerMask = LayerMask.GetMask("Player");
        isStartLayerAlsoGround = ((1 << startLayer) & groundLayer) != 0;
    }

    void FixedUpdate()
    {
        if (!held) return; // Don't do calculations if not being held
        if (lastPos == transform.position) return; // Don't do calculations if hasn't moved
        releaseVel = Vector3.ClampMagnitude((transform.position - lastPos) / Time.fixedDeltaTime, maxReleaseVelocity);
        lastPos = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("OutOfBounds")) return;
        if (rb.linearVelocity != Vector3.zero) rb.linearVelocity = Vector3.zero;
        if (rb.angularVelocity != Vector3.zero) rb.angularVelocity = Vector3.zero;
        if (transform.position != startPos) transform.position = startPos;
        if (transform.rotation != startRot) transform.rotation = startRot;
    }

    public override bool Interact(params object[] parameters)
    {
        if (parameters.Length != 4)
        {
            #if UNITY_EDITOR
                Debug.LogWarning($"WARNING: Holdable objects needs 4 parameters. Received {parameters.Length} parameters");
            #endif
            return true; // Drop object
        }
        if (parameters[0] is Transform holdPos && parameters[1] is LayerMask holdLayer && parameters[2] is Collider playerCollider && parameters[3] is Camera cam)
        {
            held = !held;
            transform.parent = held ? holdPos : null;
            Physics.IgnoreCollision(_collider, playerCollider, held);
            if (held)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false; // Make sure we can't push other interactables through floor
                if (transform.localPosition != Vector3.zero) transform.localPosition = Vector3.zero;
                if (lastPos != transform.position) lastPos = transform.position;
            }
            else
            {
                rb.detectCollisions = true; // Re-enable collisions so that we can unclip the object
                float rayDistance = Vector3.Distance(cam.transform.position, transform.position);
                int bitMask = ~(holdLayer | playerMask);
                Vector3 placePos = transform.position;
                isClipping = false;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, rayDistance, bitMask))
                {
                    isClipping = true;
                    if (Physics.Linecast(cam.transform.position, transform.position, out RaycastHit boundHit, holdLayer))
                    {
                        float boundsLength = Vector3.Distance(transform.position, boundHit.point);
                        placePos = cam.transform.position + cam.transform.forward * (hit.distance - boundsLength);
                    }
                }
                Vector3 totalOffset = Vector3.zero;
                int hitCount = 0;
                int clipCount = Physics.OverlapBoxNonAlloc(transform.position, _collider.bounds.extents, clippingColliders, transform.rotation, bitMask);
                for (int i = 0; i < clipCount; ++i)
                {
                    isClipping = true;
                    Vector3 closest = clippingColliders[i].ClosestPoint(transform.position);
                    if (closest == transform.position) continue; // If the centre of the box is inside the collider skip it because the camera raycast will correct it
                    if (Physics.Linecast(transform.position, closest, out RaycastHit closeHit, bitMask)) // Guarantee the point is on the surface and is the normal we should use
                    {
                        Vector3 outerPoint = (closeHit.point - transform.position).normalized * _collider.bounds.size.sqrMagnitude; // Point guaranteed to be outside the box's bounds
                        if (Physics.Linecast(outerPoint, transform.position, out RaycastHit boundHit, holdLayer)) // Go from outside back inside to figure out what part of the box should touch the closest point
                        {
                            float boundsLength = Vector3.Distance(transform.position, boundHit.point);
                            Vector3 offset = closeHit.normal.normalized * boundsLength;
                            totalOffset += offset;
                            hitCount++;
                        }
                    }
                }
                if (hitCount > 0) placePos += totalOffset / hitCount; // Average the displacement
                if (placePos != transform.position) transform.position = placePos;
                rb.isKinematic = false;
                if (!rb.useGravity) rb.useGravity = true;
                if (isClipping) // Try to prevent object from flying
                {
                    if (rb.linearVelocity.sqrMagnitude > 0f) rb.linearVelocity = Vector3.zero;
                    if (rb.angularVelocity.sqrMagnitude > 0f) rb.angularVelocity = Vector3.zero;
                }
                else if (rb.linearVelocity != releaseVel) rb.linearVelocity = releaseVel;
            }
            int holdLayerIndex = ConvertLayerToIndex(holdLayer);
            gameObject.layer = held ? holdLayerIndex : startLayer;
            foreach (Transform child in childTransforms) child.gameObject.layer = gameObject.layer;
            if (isClipping) isClipping = false;
            return !held; // Return true when drop, false when pick up
        }
        else
        {
            #if UNITY_EDITOR
                if (parameters[0] is not Transform) Debug.LogWarning($"WARNING: Parameter 0 needs to be the hold position transform. Received {parameters[0]} type {parameters[0].GetType()} as parameter 0");
                if (parameters[1] is not LayerMask) Debug.LogWarning($"WARNING: Parameter 1 needs to be the hold layer to render on. Received {parameters[1]} type {parameters[1].GetType()} as parameter 1");
                if (parameters[2] is not Collider) Debug.LogWarning($"WARNING: Parameter 2 needs to be the player collider. Received {parameters[2]} type {parameters[2].GetType()} as parameter 2");
                if (parameters[3] is not Camera) Debug.LogWarning($"WARNING: Parameter 3 needs to be the player camera. Received {parameters[3]} type {parameters[3].GetType()} as parameter 3");
            #endif
            return true; // Drop object
        }
    }

    public void LookAtPlayer(Vector3 playerPos) => transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));

    // Custom ConvertLayerToIndex is faster than using log to calculate
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    int ConvertLayerToIndex(LayerMask layer) // Will only do a single layer
    {
        int value = layer.value;
        int index = 0;
        while (value != 1)
        {
            value >>= 1; // Shift bits right by one (divide by 2)
            ++index;
        }
        return index;
    }
}