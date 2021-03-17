using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class objTarget : MonoBehaviour
{
    public GameObject target;

    private GameObject rig;
    private GameObject headAim;

    float angle;
    float a;

    bool checkAngle;
    bool zeroValue;


    // Start is called before the first frame update
    void Start()
    {
        rig = this.transform.Find("Rig").gameObject;
        headAim = rig.transform.Find("HeadAim").gameObject;
        var data = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
        data.SetWeight(0,1f);
        headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data;
        this.GetComponent<RigBuilder>().Build();

        checkAngle = true;
        zeroValue = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkAngle)
        {
            checkAngle = false;
            angle = Vector3.Angle(this.transform.forward, target.transform.position - this.transform.position);
            if (Mathf.Abs(angle) > 110 && !zeroValue)
            {
                //this.GetComponentInChildren<MultiAimConstraintData> ();
                var data = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
                data.SetWeight(0, 0f);
                headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data;
                this.GetComponent<RigBuilder>().Build();
                print(angle);
                zeroValue = true;
            }
            else if(Mathf.Abs(angle) < 110 && zeroValue)
            {
                var data = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
                data.SetWeight(0, 1f);
                headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data;
                this.GetComponent<RigBuilder>().Build();
                zeroValue = false;
            }
            checkAngle = true;
        }

    }
}
