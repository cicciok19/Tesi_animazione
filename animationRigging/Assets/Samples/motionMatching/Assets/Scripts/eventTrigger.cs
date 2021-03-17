using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;

public class eventTrigger : MonoBehaviour
{
    private MxMAnimator m_animator;
    private MxMTrajectoryGenerator m_trajectoryGenerator;

    public bool sit;

    [SerializeField]
    private MxMEventDefinition m_attackDefinition;
    [SerializeField]
    private MxMEventDefinition m_greetingsDefinition;
    [SerializeField]
    private MxMEventDefinition m_sitDefinition;
    [SerializeField]
    private MxMEventDefinition m_standUpDefinition;
    [SerializeField]
    private Transform sitPoint;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
        m_trajectoryGenerator = GetComponent<MxMTrajectoryGenerator>();
        m_animator.SetRequiredTag("Locomotion");
        
        sit = false;
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

        if (Input.GetKeyDown(KeyCode.K) && !sit)
        {
            m_sitDefinition.ClearContacts();
            m_sitDefinition.AddEventContact(sitPoint.position, this.transform.rotation.y);
            m_animator.BeginEvent(m_sitDefinition);
            sit = true;
            m_animator.RemoveRequiredTag("Locomotion");
            m_animator.SetRequiredTag("Sitting");
        }

        else if(Input.GetKeyDown(KeyCode.K) && sit)
        {
            m_animator.BeginEvent(m_standUpDefinition);
            sit = false;
            m_animator.SetRequiredTag("Locomotion");
        }
    }
}
