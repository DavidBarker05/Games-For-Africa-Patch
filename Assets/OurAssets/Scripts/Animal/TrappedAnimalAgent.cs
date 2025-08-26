using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TrappedAnimalAgent : Interactable
{
    [SerializeField]
    GameObject cage;
    [Header("Movement")]
    [SerializeField, Min(1f)]
    [Tooltip("The base speed the animal flees at (Note: Is randomised between 0.875-1.125x)")]
    float baseFleeSpeed = 8f;
    [SerializeField, Min(1f)]
    [Tooltip("The base acceleration for the animal (Note: Is randomised between 0.875-1.125x)")]
    float baseFleeAcceleration = 4f;
    [SerializeField, Min(1f)]
    [Tooltip("The base angular speed the animal flees at (Note: Is randomised between 0.8-1.2x)")]
    float baseFleeAngularSpeed = 120f;
    [Header("Target")]
    [SerializeField, Min(0f)]
    [Tooltip("How far the animal must be from the target to be considered at its target")]
    float targetDistance = 2f;

    NavMeshAgent agent;
    bool isTrapped = true;
    Vector3 target;
    Renderer _renderer;
    bool isVisible;
    Collider _collider;

    const float MIN_FLEE_SPEED_SCALE = 0.875f;
    const float MAX_FLEE_SPEED_SCALE = 1.125f;
    const float MIN_FLEE_ACCELERATION_SCALE = 0.875f;
    const float MAX_FLEE_ACCELERATION_SCALE = 1.125f;
    const float MIN_FLEE_ANGULAR_SPEED_SCALE = 0.8f;
    const float MAX_FLEE_ANGULAR_SPEED_SCALE = 1.2f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.speed = baseFleeSpeed * Random.Range(MIN_FLEE_SPEED_SCALE, MAX_FLEE_SPEED_SCALE);
        agent.acceleration = baseFleeAcceleration * Random.Range(MIN_FLEE_ACCELERATION_SCALE, MAX_FLEE_ACCELERATION_SCALE);
        agent.angularSpeed = baseFleeAngularSpeed * Random.Range(MIN_FLEE_ANGULAR_SPEED_SCALE, MAX_FLEE_ANGULAR_SPEED_SCALE);
        _collider = GetComponent<Collider>();
    }

    void Start() => _renderer = GetComponentInChildren<Renderer>();

    void Update()
    {
        if (isVisible != _renderer.isVisible) isVisible = _renderer.isVisible;
        if (agent.updateRotation != isVisible) agent.updateRotation = isVisible;
        if (agent.updateUpAxis != isVisible) agent.updateUpAxis = isVisible;
        if (!isTrapped && (transform.position - target).sqrMagnitude <= targetDistance * targetDistance) Destroy(gameObject);
    }

    public override bool Interact(params object[] parameters)
    {
        if (parameters.Length != 0)
        {
            #if UNITY_EDITOR
                Debug.LogWarning($"WARNING: TrappedAnimalAgent objects need 0 parameters. Received {parameters.Length} parameter(s)");
            #endif
        }
        else if (isTrapped)
        {
            cage.SetActive(false);
            _collider.enabled = false;
            isTrapped = false;
            agent.isStopped = false;
            if (PointsController.instance != null) PointsController.instance.addCertainPoints(5);
            if (NavMesh.SamplePosition(TrappedAnimalManager.Instance.GetFleeTarget(), out NavMeshHit hit, 5f, 1 << NavMesh.GetAreaFromName("Walkable"))) target = hit.position;
            else target = transform.position;
            agent.SetDestination(target);
        }
        return true;
    }
}
