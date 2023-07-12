using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 origin;
    private Vector3 target;
    private Vector3 currentVelocity;

    public float offsetVelocity = 5;
    public float maxDelta = 1;

    private void ChangeDirection() 
    {
        var local = Random.insideUnitCircle * maxDelta;
        target = transform.TransformPoint(new Vector3(local.x, 0f, local.y));
        currentVelocity = (target - transform.position) * offsetVelocity;
    }

    void Awake()
    {
        origin = transform.position;
        ChangeDirection();
    }

float time = 0;
    // Update is called once per frame
    void Update()
    {
        var currentSqrtOffset = (transform.position - target).sqrMagnitude;
        if (Mathf.Approximately(currentSqrtOffset, 0f))
        {
            ChangeDirection();
        }

        transform.position += transform.forward * Mathf.Sin(time) * offsetVelocity;//currentVelocity * Time.deltaTime;
        time += Time.deltaTime;
    }
}
