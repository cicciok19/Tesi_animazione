﻿using UnityEngine.Animations;
using System;

namespace MxM
{
    //============================================================================================
    /**
    *  @brief Data structure used to define an idle animation set
    *         
    *********************************************************************************************/
    [System.Serializable]
    public struct IdleSetData : IComplexAnimData
    {
        public ETags Tags;

        public int PrimaryClipId;
        public int[] TransitionClipIds;
        public int[] SecondaryClipIds;

        public int[] PrimaryPoseIds;
        public int[] TransitionPoseIds;
        public int[] SecondaryPoseIds;

        public int MinLoops;
        public int MaxLoops;

        public EComplexAnimType ComplexAnimType { get { return EComplexAnimType.IdleSet; } }

        public IdleSetData(ETags a_tags, int a_primaryClipId, int a_primaryPoseCount,
            int a_transitionClipCount, int a_secondaryClipCount, int a_minLoops, int a_maxLoops)
        {
            Tags = a_tags;
            MinLoops = a_minLoops;
            MaxLoops = a_maxLoops;

            PrimaryClipId = a_primaryClipId;
            PrimaryPoseIds = new int[a_primaryPoseCount];

            TransitionClipIds = new int[a_transitionClipCount];
            TransitionPoseIds = new int[a_transitionClipCount];

            SecondaryClipIds = new int[a_secondaryClipCount];
            SecondaryPoseIds = new int[a_secondaryClipCount];
        }

        public IdleSetData(ETags a_tags, int a_primaryClipId, int[] a_primaryPoseIds,
            int[] a_transitionClipsIds, int[] a_transitionPoseIds, int[] a_secondaryClipIds,
            int[] a_secondaryPoseIds, int a_minLoops, int a_maxLoops)
        {
            Tags = a_tags;
            MinLoops = a_minLoops;
            MaxLoops = a_maxLoops;

            PrimaryClipId = a_primaryClipId;

            PrimaryPoseIds = new int[a_primaryPoseIds.Length];
            Array.Copy(a_primaryPoseIds, PrimaryPoseIds, a_primaryPoseIds.Length);

            TransitionClipIds = new int[a_transitionClipsIds.Length];
            Array.Copy(a_transitionClipsIds, TransitionClipIds, a_transitionClipsIds.Length);

            TransitionPoseIds = new int[a_transitionPoseIds.Length];
            Array.Copy(a_transitionPoseIds, TransitionPoseIds, a_transitionPoseIds.Length);

            SecondaryClipIds = new int[a_secondaryClipIds.Length];
            Array.Copy(a_secondaryClipIds, SecondaryClipIds, a_secondaryClipIds.Length);

            SecondaryPoseIds = new int[a_secondaryPoseIds.Length];
            Array.Copy(a_secondaryPoseIds, SecondaryPoseIds, a_secondaryPoseIds.Length);

        }

        //public void Setup(ref MxMPlayableState a_state, MxMAnimator m_mxmAnimator)
        //{
        //    ref AnimationMixerPlayable mixer = ref m_mxmAnimator.MixerPlayable;
        //}

        //public void Update(ref MxMPlayableState a_state, MxMAnimator m_mxmAnimator)
        //{

        //}

        //public void Destroy(ref MxMPlayableState a_state, MxMAnimator m_mxmAnimator)
        //{

        //}

    }//End of class: IdleSet
}//End of namespace: MxM