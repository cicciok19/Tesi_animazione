using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MxM;
using System;

public class AI_mover : MonoBehaviour
{
    [SerializeField] private Transform destinationTransform_1;
    [SerializeField] private Transform destinationTransform_2;
    [SerializeField] private Transform destinationTransform_3;
    [SerializeField] private Transform sitPoint;

    private MxMAnimator m_animator;
    private MxMTrajectoryGenerator_BasicAI trajectoryGenerator;

    [SerializeField] private MxMEventDefinition sitDefinition;
    [SerializeField] private MxMEventDefinition standUpDefinition;

    private bool[] destinationReached= new bool[3];

    public bool chair;
    public bool sit;
    public bool strafe;

    private bool oneTime;

    private int counterDestination;

    private NavMeshAgent navAgent;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
        trajectoryGenerator = GetComponent<MxMTrajectoryGenerator_BasicAI>();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.autoBraking = true;
        navAgent.SetDestination(destinationTransform_1.position);

        counterDestination = 0;

        destinationReached[counterDestination] = false;

        chair = false;
        sit = false;
        strafe = false;
        oneTime = false;
    }
    private void Update()
    {
        if(navAgent.remainingDistance <= .35f && destinationReached[counterDestination])
            destinationReached[counterDestination] = true;
        
        if (Input.GetKeyDown(KeyCode.O))
            navAgent.SetDestination(destinationTransform_2.position);

        if (chair) {
            chair = false;
            StartCoroutine(Sitting());
            StartCoroutine(StandUp());
        }

        if (sit && !oneTime)
        {
            setSittingTag();
        }
    }

    IEnumerator Sitting()
    {
        toggleStrafe();

        yield return new WaitForSeconds(2f);

        toggleSit();
    }

    IEnumerator StandUp()
    {
        yield return new WaitForSeconds(10f);
        toggleSit();

        counterDestination += 1;
        destinationReached[counterDestination] = false;
        navAgent.SetDestination(destinationTransform_2.position);

    }

    protected void setSittingTag()
    {
        if (m_animator.IsEventComplete)
        {
            oneTime = true;
            m_animator.ClearRequiredTags();
            m_animator.SetRequiredTag("Sitting");
        }
    }

    protected void toggleStrafe()
    {
        if (!strafe)
        {
            //setup strafing
            m_animator.ClearRequiredTags();
            m_animator.SetRequiredTag("Strafe");
            trajectoryGenerator.TrajectoryMode = ETrajectoryMoveMode.Strafe;
            m_animator.AngularErrorWarpMethod = EAngularErrorWarpMethod.TrajectoryHeading;
            m_animator.AngularErrorWarpRate = 360f;

            trajectoryGenerator.StrafeDirection = -sitPoint.right;

            strafe = true;
        }
        else
        {
            //back to normal locomotion
            m_animator.ClearRequiredTags();
            //m_animator.RemoveRequiredTag("Strafe");
            trajectoryGenerator.TrajectoryMode = ETrajectoryMoveMode.Normal;
            m_animator.AngularErrorWarpMethod = EAngularErrorWarpMethod.CurrentHeading;
            m_animator.AngularErrorWarpRate = 45f;

            strafe = false;
        }
    }

    protected void toggleSit()
    {
        if (!sit)   //SIT
        {
            sitDefinition.ClearContacts();
            sitDefinition.AddEventContact(sitPoint.position, this.transform.rotation.y);
            m_animator.BeginEvent(sitDefinition);

            //m_animator.ClearRequiredTags();
            //m_animator.SetRequiredTag("Sitting");

            sit = true;
        }
        else        //STAND-UP
        {
            m_animator.BeginEvent(standUpDefinition);

            m_animator.ClearRequiredTags();
            //m_animator.RemoveRequiredTag("Sitting");

            sit = false;
            oneTime = false;
        }
    }

}
