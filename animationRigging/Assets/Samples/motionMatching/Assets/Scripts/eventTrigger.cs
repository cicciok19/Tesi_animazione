﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;
using System;

public class eventTrigger : MonoBehaviour
{
    private MxMAnimator m_animator;
    private MxMTrajectoryGenerator_BasicAI trajectoryGenerator;

    [SerializeField] private MxMEventDefinition sitDefinition;
    [SerializeField] private MxMEventDefinition standUpDefinition;
    [SerializeField] private MxMEventDefinition pickUpDefinition;

    [SerializeField] private Transform sitPoint_1;
    [SerializeField] private Transform sitPoint_2;
    [SerializeField] private Transform objToPick;

    [SerializeField] private Transform handEmpty;

    public bool sit;
    public bool strafe;
    public bool picked;

    private bool oneTime;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
        trajectoryGenerator = GetComponent<MxMTrajectoryGenerator_BasicAI>();
        //m_animator.SetRequiredTag("Locomotion");

        sit = false;
        strafe = false;
        picked = false;

        oneTime = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            toggleSit();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleStrafe();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            togglePickUp();
        }

        if (sit && !oneTime)
        {
            setSittingTag();
        }
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

            trajectoryGenerator.StrafeDirection = -sitPoint_1.right;

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
            sitDefinition.AddEventContact(sitPoint_1.position, this.transform.rotation.y);
            sitPoint_1 = sitPoint_2;
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

    protected void togglePickUp()
    {
        if (!picked)
        {
            pickUpDefinition.ClearContacts();
            pickUpDefinition.AddEventContact(objToPick.position, this.transform.rotation.y);
            m_animator.BeginEvent(pickUpDefinition);

            //picked = true;
        }
        else
        {

        }
    }
}
