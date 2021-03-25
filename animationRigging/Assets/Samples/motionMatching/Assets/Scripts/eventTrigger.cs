using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;
using System;
using UnityEngine.Animations.Rigging;

public class eventTrigger : MonoBehaviour
{
    private MxMAnimator m_animator;
    private MxMTrajectoryGenerator_BasicAI trajectoryGenerator;

    [SerializeField] private MxMEventDefinition sitDefinition;
    [SerializeField] private MxMEventDefinition standUpDefinition;
    [SerializeField] private MxMEventDefinition pointingDefinition;
    [SerializeField] private MxMEventDefinition pointingUpDefinition;

    [SerializeField] private MxMEventLayers eventLayer;

    [SerializeField] private Transform sitPoint_1;
    [SerializeField] private Transform sitPoint_2;

    [SerializeField] AvatarMask fullBody;
    [SerializeField] AvatarMask upperBody;

    private EventData currentEvent;

    private GameObject rig;
    private GameObject handAim;

    public bool sit;
    public bool strafe;
    public bool point;

    public bool endPoint;

    public bool oneTime;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
        trajectoryGenerator = GetComponent<MxMTrajectoryGenerator_BasicAI>();
        //m_animator.SetRequiredTag("Locomotion");

        eventLayer = GetComponent<MxMEventLayers>();

        rig = this.transform.Find("Rig").gameObject;
        handAim = rig.transform.Find("HandAim").gameObject;

        sit = false;
        strafe = false;
        point = false;
        endPoint = false;

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

        if (sit && !oneTime)
        {
            setSittingTag();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            togglePoint();
        }

        if (m_animator.GetLayer(10) != null)
        {
            endPoint = m_animator.GetLayer(10).IsDone;
            if (endPoint)
            {
                print("sono dentro");
            }
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

    protected void togglePoint()
    {
        if (!point)
        {
            eventLayer.BeginEvent(pointingUpDefinition, upperBody, .8f);
            /*var data_h = handAim.GetComponent<TwoBoneIKConstraint>().data;
            data_h.targetPositionWeight = 1;
            data_h.targetRotationWeight = 1;
            handAim.GetComponent<TwoBoneIKConstraint>().data = data_h;
            this.GetComponent<RigBuilder>().Build();*/
            point = true;
        }
    }

    public void pointing(string nameEvent)
    {
        currentEvent = m_animator.CurrentEvent;
        print(currentEvent.getID());
    }
}
