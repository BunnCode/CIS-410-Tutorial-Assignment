using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour {
    public Transform PursuitTarget;

    public NavMeshAgent navMeshAgent;
    public Transform[] Waypoints;

    private float initialSpeed;
    public float PursuitSpeedMultiplier = 2;

    int m_CurrentWaypointIndex;

    void Start()
    {
        navMeshAgent.SetDestination(Waypoints[0].position);
        initialSpeed = navMeshAgent.speed;
    }

    void Update()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % Waypoints.Length;
            navMeshAgent.SetDestination(Waypoints[m_CurrentWaypointIndex].position);
        }

        Vector3 heading = PursuitTarget.position - transform.position;
        float dot = Vector3.Dot(heading, transform.forward);
        if (dot > 0) {
            navMeshAgent.speed = initialSpeed * PursuitSpeedMultiplier;
        }
        else {
            navMeshAgent.speed = initialSpeed;
        }
    }
}