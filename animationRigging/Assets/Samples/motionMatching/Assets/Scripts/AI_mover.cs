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
    private NavMeshPath path;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.autoBraking = true;nav
        navAgent.SetDestination(destinationTransform_1.position);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(navAgent.pathStatus);
        if(navAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {

        }
    }
}
