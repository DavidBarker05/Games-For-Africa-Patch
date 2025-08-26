using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TreeColliderLOD : MonoBehaviour
{
    [SerializeField]
    float baseCheckTime = 5f;
    [SerializeField]
    float disableDistance = 50f;

    float thisTreeCheckTime;

    Collider c;
    Camera mC;

    const float MIN_MULT = 0.5f;
    const float MAX_MULT = 2f;

    void Awake()
    {
        c = GetComponent<Collider>();
        mC = Camera.main;
        thisTreeCheckTime = baseCheckTime * Random.Range(MIN_MULT, MAX_MULT); // Offset so not every tree checks at the same time
        StartCoroutine(CheckDistance());
    }

    void Start() => mC = mC ?? Camera.main;

    IEnumerator CheckDistance()
    {
        while (true)
        {
            yield return new WaitForSeconds(thisTreeCheckTime);
            if ((transform.position - mC.transform.position).sqrMagnitude >= disableDistance * disableDistance && c.enabled) c.enabled = false;
            else if (!c.enabled) c.enabled = true;
        }
    }
}
