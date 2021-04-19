using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class IKECA : IKSetter
{
    public AimIK HeadIK;
    public AimIK RightHandIK;
    public AimIK LeftHandIK;
    public FullBodyBipedIK FullBodyBipedIK;

    [SerializeField] private Transform HeadBone;
    [SerializeField] private Transform NeckBone;
    [SerializeField] private Transform RightHandBone;
    [SerializeField] private Transform RightForeArm;
    [SerializeField] private Transform RightArm;
    [SerializeField] private Transform RightShoulder;
    [SerializeField] private Transform LeftHandBone;
    [SerializeField] private Transform LeftForeArm;
    [SerializeField] private Transform LeftArm;
    [SerializeField] private Transform LeftShoulder;
    [SerializeField] private Transform RootNode;

    // Start is called before the first frame update
    void Start()
    {
        //create the IKs
        HeadIK = SetIKHead(HeadBone, NeckBone);
        RightHandIK = SetIKRightHand(RightHandBone, RightForeArm, RightArm, RightShoulder);
        LeftHandIK = SetIKLeftHand(LeftHandBone, LeftForeArm, LeftArm, LeftShoulder);
        FullBodyBipedIK = SetFullBodyIK(RootNode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
