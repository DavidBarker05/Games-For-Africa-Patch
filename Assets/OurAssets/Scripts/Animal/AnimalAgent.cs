using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalAgent : MonoBehaviour
{
    public enum AnimalAgentState
    {
        Roaming,
        Idling
    }

    [Header("Movement")]
    [SerializeField, Min(0f)]
    [Tooltip("The base speed the animal moves at (Note: Is randomised between 0.85-1.15x)")]
    float baseSpeed = 3f;
    [SerializeField, Min(0f)]
    [Tooltip("The base acceleration for the animal (Note: Is randomised between 0.8-1.2x)")]
    float baseAcceleration = 1.5f;
    [SerializeField, Min(0f)]
    [Tooltip("The base angular speed the animal moves at (Note: Is randomised between 0.8-1.2x)")]
    float baseAngularSpeed = 90f;
    [Header("Spawning")]
    [SerializeField, Min(0f)]
    float spawnRadius;
    [Header("Roam Targeting")]
    [SerializeField, Min(0f)]
    [Tooltip("How far the animal can be from its target destination to be considered at its target destination")]
    float destinationDistance = 2f;
    [SerializeField, Min(0.1f)]
    [Tooltip("How often the animal will check if it's at its destination (Note: the next check time is randomised between 0.8-1.2x after the check)")]
    float targetDistanceCheckInterval = 0.5f;
    [SerializeField, Min(0f)]
    [Tooltip("How far away the animal's current target can be from the manager's predefined targets (adds randomness to movement)")]
    float targetRandomOffsetRadius = 2.5f;
    [Header("Begin Idling Timing and Chances")]
    [SerializeField, Min(1f)]
    [Tooltip("The minimum time an idle can last for (Note: the duration is randomised from 1-1.5x)")]
    float minIdleDuration = 10f;
    [SerializeField, Min(0.1f)]
    [Tooltip("How often the animal will check if it can idle if it's not idling (Note: the next check time is randomised between 0.8-1.2x after the check)")]
    float idleCheckInterval = 2f;
    [SerializeField, Range(0f, 1f)]
    [Tooltip("The chance the animal idles after spawning")]
    float spawningIdleChance = 0.02f;
    [SerializeField, Range(0f, 1f)]
    [Tooltip("The chance the animal idles while roaming")]
    float roamingIdleChance = 0.05f;
    [SerializeField, Range(0f, 1f)]
    [Tooltip("The chance the animal idles after reaching its target destination")]
    float targetReachedIdleChance = 0.45f;
    [Header("Stop Idling Timing and Chances")]
    [SerializeField, Min(0.1f)]
    [Tooltip("How often the animal will check if it can stop idling if it's idling (Note: the next check time is randomised between 0.8-1.2x after the check)")]
    float idleStopCheckInterval = 0.5f;
    [SerializeField, Range(0f, 1f)]
    float baseStopIdleChance = 0.2f;
    [SerializeField, Range(1f, 2f)]
    float failedStopChanceScale = 1.1f;

    NavMeshAgent agent;
    Vector3 target;
    float nextTargetDistanceCheckTime;
    float targetDistanceCheckTimer;
    bool isIdle = false;
    float currentIdleDuration;
    float idleTimer;
    float nextIdleCheckTime;
    float idleCheckTimer;
    float currentStopIdleChance;
    float nextIdleStopCheckTime;
    float idleStopCheckTimer;
    Renderer _renderer;
    bool isVisible;

    const float MIN_TIMER_SCALE = 0.8f;
    const float MAX_TIMER_SCALE = 1.2f;
    const float MIN_IDLE_TIME_SCALE = 1f;
    const float MAX_IDLE_TIME_SCALE = 1.5f;
    const float MIN_SPEED_SCALE = 0.85f;
    const float MAX_SPEED_SCALE = 1.15f;
    const float MIN_ACCELERATION_SCALE = 0.8f;
    const float MAX_ACCELERATION_SCALE = 1.2f;
    const float MIN_ANGULAR_SPEED_SCALE = 0.8f;
    const float MAX_ANGULAR_SPEED_SCALE = 1.2f;

    AnimalAgentState _state;
    AnimalAgentState CurrentAnimalAgentState
    {
        get => _state;
        set
        {
            if (_state == value) return;
            agent.isStopped = value == AnimalAgentState.Idling;
            _state = value;
        }
    }

    public float SpawnRadius => spawnRadius;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        nextTargetDistanceCheckTime = targetDistanceCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
        currentIdleDuration = minIdleDuration * Random.Range(MIN_IDLE_TIME_SCALE, MAX_IDLE_TIME_SCALE);
        nextIdleCheckTime = idleCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
        nextIdleStopCheckTime = idleStopCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
        currentStopIdleChance = baseStopIdleChance;
        agent.speed = baseSpeed * Random.Range(MIN_SPEED_SCALE, MAX_SPEED_SCALE);
        agent.acceleration = baseAcceleration * Random.Range(MIN_ACCELERATION_SCALE, MAX_ACCELERATION_SCALE);
        agent.angularSpeed = baseAngularSpeed * Random.Range(MIN_ANGULAR_SPEED_SCALE, MAX_ANGULAR_SPEED_SCALE);
    }

    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        AttemptToIdle(idleChance: spawningIdleChance);
        UpdateTarget();
    }

    void Update()
    {
        if (isVisible != _renderer.isVisible) isVisible = _renderer.isVisible;
        if (agent.updateRotation != isVisible) agent.updateRotation = isVisible;
        if (agent.updateUpAxis != isVisible) agent.updateUpAxis = isVisible;
        targetDistanceCheckTimer += Time.deltaTime;
        if (isIdle)
        {
            if (idleCheckTimer != 0f)
            {
                idleCheckTimer = 0f;
                nextIdleCheckTime = idleCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
            }
            idleTimer += Time.deltaTime;
            if (idleTimer >= currentIdleDuration)
            {
                idleStopCheckTimer += Time.deltaTime;
                if (idleStopCheckTimer >= nextIdleStopCheckTime)
                {
                    idleStopCheckTimer = 0;
                    nextIdleStopCheckTime = idleStopCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
                    AttemptToStopIdling();
                    if (!isIdle)
                    {
                        idleTimer = 0f;
                        currentIdleDuration = minIdleDuration * Random.Range(MIN_IDLE_TIME_SCALE, MAX_IDLE_TIME_SCALE);
                    }
                }
            }
        }
        else
        {
            if (idleStopCheckTimer != 0f)
            {
                idleStopCheckTimer = 0;
                nextIdleStopCheckTime = idleStopCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
            }
            idleCheckTimer += Time.deltaTime;
            if (idleCheckTimer >= nextIdleCheckTime)
            {
                idleCheckTimer = 0;
                nextIdleCheckTime = idleCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
                AttemptToIdle(idleChance: roamingIdleChance);
            }
        }
        if (targetDistanceCheckTimer < nextTargetDistanceCheckTime) return;
        targetDistanceCheckTimer = 0f;
        nextTargetDistanceCheckTime = targetDistanceCheckInterval * Random.Range(MIN_TIMER_SCALE, MAX_TIMER_SCALE);
        if (isIdle) return; // Putting this here allows for more randomness with roam timings
        if ((transform.position - target).sqrMagnitude > destinationDistance * destinationDistance) return;
        AttemptToIdle(idleChance: targetReachedIdleChance);
        UpdateTarget();
    }

    void AttemptToIdle(float idleChance)
    {
        idleChance = Mathf.Clamp01(idleChance);
        if (idleChance == 0f) { isIdle = false; goto end; } // Don't do other checks to save performance because random number needs more computation, more likely than 100% so done first
        if (idleChance == 1f) { isIdle = true; goto end; } // Don't do other checks to save performance because random number needs more computation, less likely than 0% so done next
        isIdle = Random.Range(0f, 1f) < idleChance; // Least performant so checked last even though most likely
        end: // Label used for early jump to save performance
            CurrentAnimalAgentState = isIdle ? AnimalAgentState.Idling : AnimalAgentState.Roaming;
    }

    void AttemptToStopIdling()
    {
        currentStopIdleChance = Mathf.Clamp01(currentStopIdleChance);
        if (currentStopIdleChance == 0f) { isIdle = true; goto end; } // Don't do other checks to save performance because random number needs more computation, more likely than 100% so done first
        if (currentStopIdleChance == 1f) { isIdle = false; goto end; } // Don't do other checks to save performance because random number needs more computation, less likely than 0% so done next
        isIdle = Random.Range(0f, 1f) >= currentStopIdleChance; // Least performant so checked last even though most likely
        end: // Label used for early jump to save performance
            if (isIdle) currentStopIdleChance *= failedStopChanceScale;
            else currentStopIdleChance = baseStopIdleChance;
            CurrentAnimalAgentState = isIdle ? AnimalAgentState.Idling : AnimalAgentState.Roaming;
    }

    void UpdateTarget()
    {
        target = AnimalManager.Instance.GetNewTarget();
        if (target == Vector3.positiveInfinity) return;
        Vector3 randomTarget = target + Random.insideUnitSphere * targetRandomOffsetRadius;
        randomTarget.y = target.y;
        int depth = 10;
        int iterations = 0;
        int maxDistance = 2;
        NavMeshHit hit = new NavMeshHit();
        bool found = false;
        while (!found && iterations < depth)
        {
            if (NavMesh.SamplePosition(randomTarget, out hit, maxDistance, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                found = true;
                break;
            }
            maxDistance = Mathf.Clamp(maxDistance <<= 1, 0, 64);
            ++iterations;
        }
        if (found)
        {
            target = hit.position;
            agent.SetDestination(hit.position);
        }
    }
}
