using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using MxM;

public class AnimationRiggingManager : MonoBehaviour
{
    private AI_mover actionManager;
    private MxMAnimator m_animator;

    //[SerializeField] private GameObject aimRig;
    [SerializeField] private GameObject hipsRig;

    private TwoBoneIKConstraint L_legConstraint;
    private TwoBoneIKConstraint R_legConstraint;

    void Start()
    {
        actionManager = GetComponent<AI_mover>();
        m_animator = GetComponent<MxMAnimator>();

        L_legConstraint = hipsRig.transform.Find("L_legConstraint").GetComponent<TwoBoneIKConstraint>();
        R_legConstraint = hipsRig.transform.Find("R_legConstraint").GetComponent<TwoBoneIKConstraint>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_animator.BeginEvent("Sit");
            m_animator.SetRequiredTag("Sitting");
        }

    }
}
