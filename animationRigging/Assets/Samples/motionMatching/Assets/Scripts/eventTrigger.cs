using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;

public class eventTrigger : MonoBehaviour
{
    private MxMAnimator m_animator;

    [SerializeField]
    private MxMEventDefinition m_attackDefinition;

    [SerializeField]
    private MxMEventDefinition m_greetingsDefinition;


    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            m_animator.BeginEvent(m_attackDefinition);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            m_animator.BeginEvent(m_greetingsDefinition);
        }

        if (m_animator.IsEventComplete)
        {
            m_animator.ForceExitEvent();
        }
    }
}
