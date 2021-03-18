using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_mover : MonoBehaviour
{
    [SerializeField] private Transform destinationTransform_1;
    [SerializeField] private Transform destinationTransform_2;
    [SerializeField] private Transform destinationTransform_3;

    private NavMeshAgent navAgent;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.autoBraking = true;
        navAgent.SetDestination(destinationTransform_1.position);
    }

}
