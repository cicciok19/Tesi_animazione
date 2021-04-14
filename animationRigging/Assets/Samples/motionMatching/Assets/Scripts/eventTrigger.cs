using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;
using RootMotion.FinalIK;
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

    public GameObject targetShoulder;
    public GameObject targetHead;

    private AimIK[] aimIKs = new AimIK[3];
    private AimIK aimIKShoulder;
    private AimIK aimIKHead;
    private AimIK aimIKHand;

    public bool sit;
    public bool strafe;
    public bool point;

    public bool endPoint;

    public bool oneTime;

    private bool checkAngleHead;
    private bool zeroValueHead;
    private bool coroutineHead;
    private float angleHead;
    private int nAngleZeroHead;
    private int nAngleOneHead;

    private bool checkAngleShoulder;
    private bool zeroValueShoulder;
    private bool coroutineShoulder;
    private float angleShoulder;
    private int nAngleZeroShoulder;
    private int nAngleOneShoulder;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
        trajectoryGenerator = GetComponent<MxMTrajectoryGenerator_BasicAI>();
        //m_animator.SetRequiredTag("Locomotion");

        eventLayer = GetComponent<MxMEventLayers>();

        aimIKs = GetComponents <AimIK>();
        aimIKShoulder = aimIKs[0];
        aimIKHead = aimIKs[1];
        aimIKHand = aimIKs[2];

        targetShoulder = aimIKShoulder.solver.target.gameObject;
        targetHead = aimIKHead.solver.target.gameObject;

        rig = this.transform.Find("Rig").gameObject;
        

        sit = false;
        strafe = false;
        point = false;

        endPoint = false;

        oneTime = false;

        checkAngleHead = true;
        zeroValueHead = false;
        coroutineHead = false;

        checkAngleShoulder = true;
        zeroValueHead = false;
        coroutineShoulder = false;
    }

    void Update()
    {
        if (aimIKShoulder.solver.target.gameObject != targetShoulder)
            targetShoulder = aimIKShoulder.solver.target.gameObject;

        if (aimIKHead.solver.target.gameObject != targetHead)
            targetHead = aimIKHead.solver.target.gameObject;

        //controllo se l'oggetto è all'interno di un angolo tale da non fare IK strane
        //come testa che si gira di 360 gradi

        if (checkAngleHead)
        {
            checkAngleHead = false;
            angleHead = Vector3.Angle(this.transform.forward, targetHead.transform.position - this.transform.position);
            //print(Mathf.Abs(angle));
            if (Mathf.Abs(angleHead) > 80 && !zeroValueHead)
            {
                nAngleZeroHead += 1;
                if (nAngleZeroHead == 25)
                {
                    nAngleZeroHead = 0;
                    nAngleOneHead = 0;

                    if (!coroutineHead)
                    {
                        coroutineHead = true;
                        StartCoroutine(notWatchTarget());
                    }

                    zeroValueHead = true;
                }
            }
            else if (Mathf.Abs(angleHead) < 80 && zeroValueHead)
            {
                nAngleOneHead += 1;
                if (nAngleOneHead == 25)
                {
                    nAngleOneHead = 0;
                    nAngleZeroHead = 0;

                    if (!coroutineHead)
                    {
                        coroutineHead = true;
                        StartCoroutine(watchTarget());
                    }

                    zeroValueHead = false;
                }
            }
            checkAngleHead = true;
        }

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
            StartCoroutine(pointHand());
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

    IEnumerator pointHand()
    {
        float var = 0f;

        while(var < 1f)
        {
            aimIKShoulder.solver.SetIKPositionWeight(var);
            yield return new WaitForSeconds(.01f);
            var += .01f;
        }
    }

    IEnumerator watchTarget()
    {
        float var = aimIKHead.solver.IKPositionWeight;

        while (var < 1f)
        {
            aimIKHead.solver.SetIKPositionWeight(var);
            yield return new WaitForSeconds(.01f);
            var += .01f;
        }

        coroutineHead = false;
    }

    IEnumerator notWatchTarget()
    {
        float var = aimIKHead.solver.IKPositionWeight;

        while (var > 0f)
        {
            aimIKHead.solver.SetIKPositionWeight(var);
            yield return new WaitForSeconds(.01f);
            var -= .01f;
        }

        coroutineHead = false;
    }
}
