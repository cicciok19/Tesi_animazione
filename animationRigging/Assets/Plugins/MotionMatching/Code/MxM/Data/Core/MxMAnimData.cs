// ============================================================================================
// File: AnimationData.cs
// 
// Authors:  Kenneth Claassen
// Date:     2017-09-16: Created this file.
// 
//     Contains a part of the 'MxM' namespace for 'Unity Engine 5'.
// 
// Copyright (c) 2017 Kenneth Claassen. All rights reserved.
// ============================================================================================
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MxM
{
    public class MxMAnimData : ScriptableObject
    {
        public int StartPoseId = 0;
        public float PoseInterval = 0.1f;
        public float[] PosePredictionTimes;
        public AnimationClip[] Clips;
        public HumanBodyBones[] MatchBones;
        public string[] MatchBonesGeneric;
        
        //Primary Data
        public PoseData[] Poses;
        public EventData[] Events;
        public CalibrationData[] CalibrationSets;

        //MxMAnims Anim Data
        public IdleSetData[] IdleSets;
        public BlendSpaceData[] BlendSpaces;
        public ClipData[] ClipsData;
        public CompositeData[] Composites;
        public BlendClipData[] BlendClips;
        //public SequenceData[] Sequences;

        //Events and tags
        public string[] EventNames;
        public string[] TagNames;
        public string[] FavourTagNames;
        public string[] UserTagNames;

        //Tracks
        public FootstepTagTrackData[] LeftFootSteps;
        public FootstepTagTrackData[] RightFootSteps;

        public PoseMask poseMask;
        public int PoseUtilisationLevel = 1;

        public bool GetBonesByName = false;

        public Dictionary<ETags, MxMNativeAnimData> NativeAnimData;

        private int m_refCount;
        private bool m_isInitialized;

        public int MaxPoseUseCount
        {
            get
            {
                if (NativeAnimData == null || NativeAnimData.Count == 0)
                    return 0;

                int maxCount = 0;
                foreach (KeyValuePair<ETags, MxMNativeAnimData> data in NativeAnimData)
                {
                    if (data.Value.UsedPoseIds.Length > maxCount)
                        maxCount = data.Value.UsedPoseIds.Length;
                }

                return maxCount;
            }
        }

        public void ResetAllData()
        {
            PosePredictionTimes = null;
            Clips = null;
            LeftFootSteps = null;
            RightFootSteps = null;
            MatchBones = null;
        }

        public void InitializeNativeData()
        {
            ++m_refCount;

            if (m_isInitialized)
                return;
            
            m_isInitialized = true;

            int count = 0;

            //Create all NativeAnimData for each tag combo
            NativeAnimData = new Dictionary<ETags, MxMNativeAnimData>();
            for(int i = 0; i < Poses.Length; ++i)
            {
                ref readonly PoseData poseData = ref Poses[i];

                if ((poseData.Tags & ETags.DoNotUse) == ETags.DoNotUse)
                {
                    ++count;
                    continue;
                }

                if(!NativeAnimData.ContainsKey(poseData.Tags))
                {
                    NativeAnimData.Add(poseData.Tags, new MxMNativeAnimData(poseData.Tags));
                }
            }

            //Initialize each NativeAnimData
            foreach(KeyValuePair<ETags, MxMNativeAnimData> data in NativeAnimData)
            {
                data.Value.Initialize(this);
            }   
        }
        
        public void ReleaseNativeData()
        {
            if (!m_isInitialized)
            {
                return;
            }

            m_refCount--;

            if (m_refCount > 0)
            {
                return;
            }

            DisposeAll();

            m_isInitialized = false;
        }

        public void OnEnable()
        {
            if (m_isInitialized)
            {
                DisposeAll();

                m_isInitialized = false;
                m_refCount = 0;
            }

            //if (CalibrationSets == null)
            //    CalibrationSets = new List<CalibrationData>();
        }

        public void OnDisable()
        {
            if (m_isInitialized)
            {
                DisposeAll();

                m_isInitialized = false;
                m_refCount = 0;
            }
        }

        public void DisposeAll()
        {
            if (NativeAnimData != null)
            {
                foreach (KeyValuePair<ETags, MxMNativeAnimData> data in NativeAnimData)
                {
                    if (data.Value != null)
                    {
                        data.Value.DisposeAll();
                    }
                }
            }
        }

        public void InitializeCalibration(List<CalibrationData> a_sourceCalibration = null)
        {
            if(a_sourceCalibration == null || a_sourceCalibration.Count == 0)
            {
                CalibrationData newCalibData = new CalibrationData();
                newCalibData.Initialize("Calibration 0", this);

                CalibrationSets = new CalibrationData[1];
                CalibrationSets[0] = newCalibData;
            }
            else
            {
                CalibrationSets = new CalibrationData[a_sourceCalibration.Count];

                
                for(int i = 0; i < a_sourceCalibration.Count; ++i)
                {
                    CalibrationData newCalibData = new CalibrationData(a_sourceCalibration[i]);
                    newCalibData.Validate(this);

                    CalibrationSets[i] = newCalibData;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private void OnDestroy()
        {
            DisposeAll();
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public int EventIdFromName(string a_eventName)
        {
            for (int i = 0; i < EventNames.Length; ++i)
            {
                if (EventNames[i] == a_eventName)
                    return i;
            }

            return 0;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public ETags TagFromName(string a_tagName)
        {
            for(int i = 0; i < TagNames.Length; ++i)
            {
                if(TagNames[i] == a_tagName)
                {
                    return (ETags)(1 << i);
                }
            }

            return ETags.None;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public ETags FavourTagFromName(string a_favourTagName)
        {
            for (int i = 0; i < FavourTagNames.Length; ++i)
            {
                if (FavourTagNames[i] == a_favourTagName)
                {
                    return (ETags)(1 << i);
                }
            }

            return ETags.None;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public EUserTags UserTagFromName(string a_userTagName)
        {
            for (int i = 0; i < UserTagNames.Length; ++i)
            {
                if (UserTagNames[i] == a_userTagName)
                {
                    return (EUserTags)(1 << i);
                }
            }

            return EUserTags.None;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        //public int GetPoseClipId(ref PoseData a_poseData)
        //{
        //    switch (a_poseData.AnimType)
        //    {
        //        case EMxMAnimtype.Composite: { return Composites[a_poseData.AnimId].ClipIdA; }
        //        case EMxMAnimtype.IdleSet: { return a_poseData.AnimId; }
        //        case EMxMAnimtype.BlendSpace: { return BlendSpaces[a_poseData.AnimId].ClipIds[0]; }
        //        case EMxMAnimtype.Clip: { return ClipsData[a_poseData.AnimId].ClipId; }
        //        case EMxMAnimtype.BlendClip: { return BlendClips[a_poseData.AnimId].ClipIds[0]; } 
        //        default: { return 0; }
        //    }
        //}

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public IComplexAnimData GetComplexAnim(ref PoseData a_poseData)
        {
            switch (a_poseData.AnimType)
            {
                case EMxMAnimtype.Composite: { return Composites[a_poseData.AnimId]; }
                case EMxMAnimtype.BlendSpace: { return BlendSpaces[a_poseData.AnimId]; }
                case EMxMAnimtype.Clip: { return ClipsData[a_poseData.AnimId]; }
                case EMxMAnimtype.BlendClip: { return BlendClips[a_poseData.AnimId]; }
                default: { return null; }
            }
        }

#if UNITY_EDITOR
            public void StripPoseMask()
        {
            if(poseMask != null)
            {
                AssetDatabase.RemoveObjectFromAsset(poseMask);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
                poseMask = null;
            }
        }

        public void BindPoseMask(PoseMask a_poseMask)
        {
            poseMask = a_poseMask;
            AssetDatabase.AddObjectToAsset(a_poseMask, this);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
        }
#endif

    }//End of class: AnimationData
}//End of namespace: MxM