// ================================================================================================
// File: MxMAnimatorInspector.cs
// 
// Authors:  Kenneth Claassen
// Date:     2018-05-31: Created this file.
// 
//     Contains a part of the 'MxMEditor' namespace for 'Unity Engine 2018'.
// 
// Copyright (c) 2018 Kenneth Claassen. All rights reserved.
// ================================================================================================
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using MxM;

namespace MxMEditor
{
    //============================================================================================
    /**
    *  @brief Inspector class for drawing and managing the MxMAnimator component in the Unity Editor
    *         
    *********************************************************************************************/
    [CustomEditor(typeof(MxMAnimator))]
    public class MxMAnimatorInspector : Editor
    {
        private SerializedProperty m_spDIYPlayableGraph;
        private SerializedProperty m_spAutoCreateAnimatorController;
        private SerializedProperty m_spRootMotionMode;
        private SerializedProperty m_spTransitionMethod;
        private SerializedProperty m_spUpdateInterval;
        private SerializedProperty m_spPlaybackSpeed;
        private SerializedProperty m_spPlaybackSpeedSmoothRate;
        private SerializedProperty m_spMatchBlendTime;
        private SerializedProperty m_spTurnInPlaceThreshold;
        private SerializedProperty m_spAnimationRootOverride;
        private SerializedProperty m_spAnimData;
        private SerializedProperty m_spMaxMixCount;

        private SerializedProperty m_spDebugGoal;
        private SerializedProperty m_spDebugChosenTrajectory;
        private SerializedProperty m_spDebugPoses;
        private SerializedProperty m_spDebugCurrentPose;
        private SerializedProperty m_spDebugAnimDataId;
        private SerializedProperty m_spDebugPoseId;
        private SerializedProperty m_spAngularWarpType;
        private SerializedProperty m_spAngularWarpMethod;
        private SerializedProperty m_spLongErrorWarpType;
        private SerializedProperty m_spSpeedWarpLimits;

        private SerializedProperty m_spPoseMatchMethod;
        private SerializedProperty m_spFavourTagMethod;
        private SerializedProperty m_spPastTrajectoryMode;
        private SerializedProperty m_spAngularErrorWarpRate;
        private SerializedProperty m_spAngularErrorWarpThreshold;
        private SerializedProperty m_spAngularErrorWarpAngleThreshold;
        private SerializedProperty m_spAngularErrorWarpMinAngleThreshold;
        private SerializedProperty m_spAngularErrorWarpSmoothing;
        private SerializedProperty m_spApplyTrajectoryBlending;
        private SerializedProperty m_spTrajectoryBlendingWeight;
        private SerializedProperty m_spFavourCurrentPose;
        private SerializedProperty m_spTransformGoal;
        private SerializedProperty m_spPoseFavourFactor;
        private SerializedProperty m_spBlendSpaceSmoothing;
        private SerializedProperty m_spBlendSpaceSmoothRate;
        private SerializedProperty m_spAnimatorControllerMask;
        private SerializedProperty m_spNextPoseToleranceTest;
        private SerializedProperty m_spNextPoseToleranceDist;
        private SerializedProperty m_spNextPoseToleranceAngle;
        private SerializedProperty m_spBlendOutEarly;

        private SerializedProperty m_spOnSetupCompleteCallback;
        private SerializedProperty m_spOnIdleTriggeredCallback;
        private SerializedProperty m_spOnLeftFootStepStartCallback;
        private SerializedProperty m_spOnRightFootStepStartCallback;
        private SerializedProperty m_spOnEventCompleteCallback;
        private SerializedProperty m_spOnEventContactCallback;
        private SerializedProperty m_spOnEventChangeStateCallback;

        private MxMAnimData m_animData;
        private MxMAnimator m_animator;

        private SerializedProperty m_spGeneralFoldout;
        private SerializedProperty m_spAnimDataFoldout;
        private SerializedProperty m_spOptionsFoldout;
        private SerializedProperty m_spWarpingFoldout;
        private SerializedProperty m_spCallbackFoldout;
        private SerializedProperty m_spDebugFoldout;

        private ReorderableList m_animDataReorderableList;

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private void OnEnable()
        {
            m_animator = target as MxMAnimator;

            m_spDIYPlayableGraph = serializedObject.FindProperty("p_DIYPlayableGraph");
            m_spAutoCreateAnimatorController = serializedObject.FindProperty("m_autoCreateAnimatorController");
            m_spAnimatorControllerMask = serializedObject.FindProperty("m_animatorControllerMask");
            m_spRootMotionMode = serializedObject.FindProperty("m_rootMotionMode");
            m_spTransitionMethod = serializedObject.FindProperty("m_transitionMethod");
            m_spUpdateInterval = serializedObject.FindProperty("m_updateInterval");
            m_spPlaybackSpeed = serializedObject.FindProperty("m_playbackSpeed");
            m_spPlaybackSpeedSmoothRate = serializedObject.FindProperty("m_playbackSpeedSmoothRate");
            m_spMatchBlendTime = serializedObject.FindProperty("m_matchBlendTime");
            m_spTurnInPlaceThreshold = serializedObject.FindProperty("m_turnInPlaceThreshold");
            m_spAnimData = serializedObject.FindProperty("m_animData");
            m_spAnimationRootOverride = serializedObject.FindProperty("m_animationRoot");
            m_spMaxMixCount = serializedObject.FindProperty("m_maxMixCount");

            m_spDebugGoal = serializedObject.FindProperty("m_debugGoal");
            m_spDebugChosenTrajectory = serializedObject.FindProperty("m_debugChosenTrajectory");
            m_spDebugPoses = serializedObject.FindProperty("m_debugPoses");
            m_spDebugCurrentPose = serializedObject.FindProperty("m_debugCurrentPose");
            m_spDebugAnimDataId = serializedObject.FindProperty("m_debugAnimDataId");
            m_spDebugPoseId = serializedObject.FindProperty("m_debugPoseId");
            m_spAngularWarpType = serializedObject.FindProperty("m_angularWarpType");
            m_spAngularWarpMethod = serializedObject.FindProperty("m_angularWarpMethod");
            m_spLongErrorWarpType = serializedObject.FindProperty("m_longErrorWarpType");
            m_spSpeedWarpLimits = serializedObject.FindProperty("m_speedWarpLimits");

            m_spBlendOutEarly = serializedObject.FindProperty("m_blendOutEarly");
            m_spPoseMatchMethod = serializedObject.FindProperty("m_poseMatchMethod");
            m_spFavourTagMethod = serializedObject.FindProperty("m_favourTagMethod");
            m_spPastTrajectoryMode = serializedObject.FindProperty("m_pastTrajectoryMode");
            m_spAngularErrorWarpRate = serializedObject.FindProperty("m_angularErrorWarpRate");
            m_spAngularErrorWarpThreshold = serializedObject.FindProperty("m_angularErrorWarpThreshold");
            m_spAngularErrorWarpAngleThreshold = serializedObject.FindProperty("m_angularErrorWarpAngleThreshold");
            m_spAngularErrorWarpMinAngleThreshold = serializedObject.FindProperty("m_angularErrorWarpMinAngleThreshold");
            m_spApplyTrajectoryBlending = serializedObject.FindProperty("m_applyTrajectoryBlending");
            m_spTrajectoryBlendingWeight = serializedObject.FindProperty("m_trajectoryBlendingWeight");
            m_spFavourCurrentPose = serializedObject.FindProperty("m_favourCurrentPose");
            m_spTransformGoal = serializedObject.FindProperty("m_transformGoal");
            m_spPoseFavourFactor = serializedObject.FindProperty("m_currentPoseFavour");
            m_spBlendSpaceSmoothing = serializedObject.FindProperty("m_blendSpaceSmoothing");
            m_spBlendSpaceSmoothRate = serializedObject.FindProperty("m_blendSpaceSmoothRate");
            m_spNextPoseToleranceTest = serializedObject.FindProperty("m_nextPoseToleranceTest");
            m_spNextPoseToleranceDist = serializedObject.FindProperty("m_nextPoseToleranceDist");
            m_spNextPoseToleranceAngle = serializedObject.FindProperty("m_nextPoseToleranceAngle");
            

            m_spOnSetupCompleteCallback = serializedObject.FindProperty("m_onSetupComplete");
            m_spOnIdleTriggeredCallback = serializedObject.FindProperty("m_onIdleTriggered");
            m_spOnLeftFootStepStartCallback = serializedObject.FindProperty("m_onLeftFootStepStart");
            m_spOnRightFootStepStartCallback = serializedObject.FindProperty("m_onRightFootStepStart");
            m_spOnEventCompleteCallback = serializedObject.FindProperty("m_onEventComplete");
            m_spOnEventContactCallback = serializedObject.FindProperty("m_onEventContactReached");
            m_spOnEventChangeStateCallback = serializedObject.FindProperty("m_onEventStateChanged");

            m_spGeneralFoldout = serializedObject.FindProperty("m_generalFoldout");
            m_spAnimDataFoldout = serializedObject.FindProperty("m_animDataFoldout");
            m_spOptionsFoldout = serializedObject.FindProperty("m_optionsFoldout");
            m_spWarpingFoldout = serializedObject.FindProperty("m_warpingFoldout");
            m_spCallbackFoldout = serializedObject.FindProperty("m_debugFoldout");
            m_spDebugFoldout = serializedObject.FindProperty("m_callbackFoldout");

            if (m_spAnimData.arraySize == 0)
                m_spAnimData.InsertArrayElementAtIndex(0);

            if (m_spAnimData.arraySize > 0)
            {
                SerializedProperty spAnimData = m_spAnimData.GetArrayElementAtIndex(0);
                if (spAnimData != null)
                {
                    m_animData = m_spAnimData.GetArrayElementAtIndex(0).objectReferenceValue as MxMAnimData;
                }
            }

            if (m_spDebugPoses.boolValue)
                m_animator.StartPoseDebug(m_spDebugAnimDataId.intValue);

            m_animDataReorderableList = new ReorderableList(serializedObject, m_spAnimData,
                true, true, true, true);

            m_animDataReorderableList.drawElementCallback =
                (Rect a_rect, int a_index, bool a_isActive, bool a_isFocused) =>
                {
                    var element = m_animDataReorderableList.serializedProperty.GetArrayElementAtIndex(a_index);

                    EditorGUI.LabelField(new Rect(a_rect.x, a_rect.y, 100f, EditorGUIUtility.singleLineHeight),
                        "Anim Data " + (a_index + 1).ToString());
                    EditorGUI.ObjectField(new Rect(a_rect.x + 100f, a_rect.y, EditorGUIUtility.currentViewWidth - 170f,
                        EditorGUIUtility.singleLineHeight), element, new GUIContent(""));
                };

            m_animDataReorderableList.drawHeaderCallback =
                (Rect a_rect) =>
                {
                    EditorGUI.LabelField(a_rect, "Anim Data");
                };

            m_animDataReorderableList.onRemoveCallback =
                (ReorderableList a_list) =>
                {
                    if(a_list.index >= 0 && a_list.index < a_list.serializedProperty.arraySize)
                    {
                        SerializedProperty spObject = a_list.serializedProperty.GetArrayElementAtIndex(a_list.index);

                        if(spObject.objectReferenceValue != null)
                        {
                            spObject.objectReferenceValue = null;
                        }
                    }

                    ReorderableList.defaultBehaviours.DoRemoveButton(a_list);
                };

            serializedObject.ApplyModifiedProperties();

            if(EditorApplication.isPlaying)
                SetDebuggerTarget();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private void OnDisable()
        {
            if (m_spDebugPoses.boolValue)
            {
                m_animator.StopPoseDebug();
                m_spDebugPoses.boolValue = false;

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredPlayMode)
                SetDebuggerTarget();
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private void SetDebuggerTarget()
        {
            if (m_animator != null && MxMDebuggerWindow.Instance != null)
                MxMDebuggerWindow.SetTarget(m_animator, false);
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public override void OnInspectorGUI()
        {
            if (m_animData == null)
            {
                if (m_spAnimData.arraySize > 0)
                {
                    SerializedProperty spAnimData = m_spAnimData.GetArrayElementAtIndex(0);
                    if (spAnimData != null)
                    {
                        m_animData = m_spAnimData.GetArrayElementAtIndex(0).objectReferenceValue as MxMAnimData;
                    }
                }
            }

            EditorGUILayout.LabelField("");
            Rect lastRect = GUILayoutUtility.GetLastRect();

            float curHeight = lastRect.y + 9f;

            curHeight = EditorUtil.EditorFunctions.DrawTitle("MxM Animator", curHeight);

            m_spGeneralFoldout.boolValue = EditorUtil.EditorFunctions.DrawFoldout("General", curHeight, EditorGUIUtility.currentViewWidth, m_spGeneralFoldout.boolValue);
            if (m_spGeneralFoldout.boolValue)
            {
                EditorGUILayout.Slider(m_spPlaybackSpeed, 0f, 3f, new GUIContent("PlaybackSpeed"));
                EditorGUI.BeginChangeCheck();
                m_spPlaybackSpeedSmoothRate.floatValue = EditorGUILayout.FloatField(new GUIContent("Playback Smooth Rate"), m_spPlaybackSpeedSmoothRate.floatValue);
                if(EditorGUI.EndChangeCheck())
                {
                    if (m_spPlaybackSpeedSmoothRate.floatValue <= 0.01f)
                        m_spPlaybackSpeedSmoothRate.floatValue = 0.01f;
                }

                m_spMaxMixCount.intValue = EditorGUILayout.IntField("Max Blend Channels", m_spMaxMixCount.intValue);

                EditorGUI.BeginChangeCheck();
                float updateRate = EditorGUILayout.Slider(new GUIContent("Update Rate (Hz)", "The rate at which motion matching searches take place (not the FPS!)"), 1f / m_spUpdateInterval.floatValue, 1f, 144f);
                if(EditorGUI.EndChangeCheck())
                {
                    m_spUpdateInterval.floatValue = 1f / updateRate;
                }
                EditorGUILayout.Slider(m_spMatchBlendTime, 0f, 2f, new GUIContent("Blend Time"));

                EditorGUILayout.Slider(m_spTurnInPlaceThreshold, 0f, 180f, new GUIContent("Turn in place threshold"));

                curHeight += 18f * 10f;
            }

            curHeight += 30f;
            GUILayout.Space(3f);

            m_spAnimDataFoldout.boolValue = EditorUtil.EditorFunctions.DrawFoldout("Animation Data", curHeight, EditorGUIUtility.currentViewWidth, m_spAnimDataFoldout.boolValue);

            bool greyOutData = false;
            if (m_spAnimDataFoldout.boolValue)
            {

                m_animDataReorderableList.DoLayoutList();

                GUIStyle redBoldStyle = new GUIStyle(GUI.skin.label);
                redBoldStyle.fontStyle = FontStyle.Bold;
                redBoldStyle.normal.textColor = Color.red;

                for (int i=0; i < m_spAnimData.arraySize; ++i)
                {
                    SerializedProperty spAnimData = m_spAnimData.GetArrayElementAtIndex(i);

                    if(spAnimData.objectReferenceValue == null)
                    {
                        EditorGUILayout.LabelField("You have NULL anim data in the list. Please remove or fill the null entry in the list.", redBoldStyle);
                        curHeight += 18f;
                        greyOutData = true;
                    }
                }

                if (m_spAnimData.arraySize == 0)
                {
                    EditorGUILayout.LabelField("Please add anim data into the list above.", redBoldStyle);
                    curHeight += 18f;
                    greyOutData = false;
                }
            }

            curHeight += 31f;
            GUILayout.Space(3f);

            if (greyOutData)
            {
                GUI.enabled = false;
            }

            m_spOptionsFoldout.boolValue = EditorUtil.EditorFunctions.DrawFoldout("Options",
                curHeight, EditorGUIUtility.currentViewWidth, m_spOptionsFoldout.boolValue);

            if (m_spOptionsFoldout.boolValue)
            {
                EditorGUILayout.ObjectField(m_spAnimationRootOverride, typeof(Transform), new GUIContent("Animation Root Override",
                    "The root transform for animation. Leaveing this blank will use the transform of the MxMAnimator."));

                EditorGUILayout.ObjectField(m_spAnimatorControllerMask, typeof(AvatarMask), new GUIContent("Controller Mask",
                    "The mask that is placed here will be applied to the mecanim animator controller if you are using one."));

                GUILayout.Space(10f);
                curHeight += 10f;

                m_spPoseMatchMethod.enumValueIndex = (int)(EPoseMatchMethod)EditorGUILayout.EnumPopup(new GUIContent(
                    "Pose Match Method", "The method to use for pose matching"), (EPoseMatchMethod)m_spPoseMatchMethod.enumValueIndex);

                m_spFavourTagMethod.enumValueIndex = (int)(EFavourTagMethod)EditorGUILayout.EnumPopup(new GUIContent(
                    "Favour Tag Method", "The method to process favour tags with"), (EFavourTagMethod)m_spFavourTagMethod.enumValueIndex);
                

                m_spRootMotionMode.enumValueIndex = (int)(EMxMRootMotion)EditorGUILayout.EnumPopup(new GUIContent(
                    "Root Motion", "How to handle root motion"), (EMxMRootMotion)m_spRootMotionMode.enumValueIndex);

                m_spPastTrajectoryMode.intValue = (int)(EPastTrajectoryMode)EditorGUILayout.EnumPopup(
                    "Past Trajectory Mode", (EPastTrajectoryMode)m_spPastTrajectoryMode.intValue);


                m_spBlendSpaceSmoothing.enumValueIndex = (int)(EBlendSpaceSmoothing)EditorGUILayout.EnumPopup(
                    new GUIContent("Blend Space Smoothing", "How blend space smoothing should operate"),
                    (EBlendSpaceSmoothing)m_spBlendSpaceSmoothing.enumValueIndex);

                switch (m_spBlendSpaceSmoothing.intValue)
                {
                    case (int)EBlendSpaceSmoothing.Lerp:
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(15f);

                            EditorGUI.BeginChangeCheck();
                            float smoothRate = EditorGUILayout.FloatField(new GUIContent("Smooth Rate"),
                                m_spBlendSpaceSmoothRate.vector2Value.x);

                            if(EditorGUI.EndChangeCheck())
                            {
                                m_spBlendSpaceSmoothRate.vector2Value = new Vector2(smoothRate, smoothRate);
                            }
                            EditorGUILayout.EndHorizontal();

                            curHeight += 18f;
                        }
                        break;
                    case (int)EBlendSpaceSmoothing.Lerp2D:
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(15f);
                            m_spBlendSpaceSmoothRate.vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Smooth Rate"),
                                m_spBlendSpaceSmoothRate.vector2Value);
                            EditorGUILayout.EndHorizontal();

                            
                        }
                        break;
                }

                EditorGUI.BeginChangeCheck();
                m_spTransitionMethod.intValue = (int)(ETransitionMethod)EditorGUILayout.EnumPopup(
                    "Transition Method", (ETransitionMethod)m_spTransitionMethod.intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_spTransitionMethod.intValue == (int)ETransitionMethod.None)
                        m_spBlendOutEarly.boolValue = false;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (m_spTransitionMethod.intValue == (int)ETransitionMethod.Blend)
                {
                    m_spBlendOutEarly.boolValue = EditorGUILayout.Toggle(new GUIContent("Blend Out Early", "If true, animations will be " +
                        "forced to change and blend out before they reach their end."), m_spBlendOutEarly.boolValue);

                    curHeight += 18f;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);
                curHeight += 10f;

                m_spTransformGoal.boolValue = EditorGUILayout.Toggle(new GUIContent("Transorm Goal"),
                    m_spTransformGoal.boolValue);

                //m_spApplyBlending.boolValue = EditorGUILayout.Toggle(new GUIContent("Apply Blending"), m_spApplyBlending.boolValue);
                m_spApplyTrajectoryBlending.boolValue = EditorGUILayout.Toggle(new
                    GUIContent("Apply Trajectory Blending"), m_spApplyTrajectoryBlending.boolValue);

                if(m_spApplyTrajectoryBlending.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);

                    m_spTrajectoryBlendingWeight.floatValue = EditorGUILayout.FloatField("Trajectory Blend Weight", 
                        m_spTrajectoryBlendingWeight.floatValue);

                    EditorGUILayout.EndHorizontal();

                    curHeight += 18f;
                }

                m_spFavourCurrentPose.boolValue = EditorGUILayout.Toggle(
                    new GUIContent("Favour Current Pose"), m_spFavourCurrentPose.boolValue);

                if (m_spFavourCurrentPose.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    m_spPoseFavourFactor.floatValue = EditorGUILayout.FloatField(
                        new GUIContent("Pose Favour Factor"), m_spPoseFavourFactor.floatValue);
                    EditorGUILayout.EndHorizontal();

                    curHeight += 18f;
                }

                m_spNextPoseToleranceTest.boolValue = EditorGUILayout.Toggle(new GUIContent("Next Pose Tolerance Test"), m_spNextPoseToleranceTest.boolValue);

                if (m_spNextPoseToleranceTest.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    EditorGUILayout.BeginVertical();

                    m_spNextPoseToleranceDist.floatValue = EditorGUILayout.FloatField("Distance Tolerance", m_spNextPoseToleranceDist.floatValue);
                    m_spNextPoseToleranceAngle.floatValue = EditorGUILayout.FloatField("Angular Tolerance", m_spNextPoseToleranceAngle.floatValue);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    curHeight += 18f * 2f;
                }

                m_spDIYPlayableGraph.boolValue = EditorGUILayout.Toggle(new GUIContent("DIY Playable Graph"), m_spDIYPlayableGraph.boolValue);

                if (m_spDIYPlayableGraph.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    m_spAutoCreateAnimatorController.boolValue = EditorGUILayout.Toggle(
                        new GUIContent("Auto Create Controller"), m_spAutoCreateAnimatorController.boolValue);
                    EditorGUILayout.EndHorizontal();

                    curHeight += 18f;
                }

                GUILayout.Space(5f);
                curHeight += 18 * 13f + 5f;
            }

            curHeight += 30f;
            GUILayout.Space(2f);

            m_spWarpingFoldout.boolValue = EditorUtil.EditorFunctions.DrawFoldout("Warping",
                curHeight, EditorGUIUtility.currentViewWidth, m_spWarpingFoldout.boolValue);

            if (m_spWarpingFoldout.boolValue)
            {
                m_spAngularWarpType.intValue = (int)(EAngularErrorWarp)EditorGUILayout.EnumPopup(
                    "Angular Error Warping", (EAngularErrorWarp)m_spAngularWarpType.intValue);

                if (m_spAngularWarpType.intValue > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    m_spAngularWarpMethod.intValue = (int)(EAngularErrorWarpMethod)EditorGUILayout.EnumPopup(
                        "Warp Method", (EAngularErrorWarpMethod)m_spAngularWarpMethod.intValue);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    m_spAngularErrorWarpRate.floatValue = EditorGUILayout.FloatField(new GUIContent("Warp Rate",
                        "How fast the error will be warped for in 'degrees per second'."),
                        m_spAngularErrorWarpRate.floatValue);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    m_spAngularErrorWarpThreshold.floatValue = EditorGUILayout.FloatField(new GUIContent("Distance Threshold",
                        "Discrepancy compensation will not activate unless the final trajectory point is greater than this value"),
                        m_spAngularErrorWarpThreshold.floatValue);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);

                    Vector2 angularErrorWarpRange = new Vector2(m_spAngularErrorWarpMinAngleThreshold.floatValue, m_spAngularErrorWarpAngleThreshold.floatValue);

                    EditorGUI.BeginChangeCheck();
                    angularErrorWarpRange = EditorGUILayout.Vector2Field(new GUIContent("Angle Range",
                        "The minimum and maximum angle underwhich angular error warping will be activated (degrees)."),
                        angularErrorWarpRange);
                    if(EditorGUI.EndChangeCheck())
                    {
                        m_spAngularErrorWarpAngleThreshold.floatValue = angularErrorWarpRange.y;
                        m_spAngularErrorWarpMinAngleThreshold.floatValue = angularErrorWarpRange.x;
                    }

                    EditorGUILayout.EndHorizontal();
                    curHeight += 18f * 3f;
                }

                GUILayout.Space(10f);
                curHeight += 10f;

                m_spLongErrorWarpType.intValue = (int)(ELongitudinalErrorWarp)EditorGUILayout.EnumPopup(
                    "Longitudinal Error Warping", (ELongitudinalErrorWarp)m_spLongErrorWarpType.intValue);

                if (m_spLongErrorWarpType.intValue > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    EditorGUI.BeginChangeCheck();

                    if (m_spLongErrorWarpType.intValue == 1)
                    {

                        m_spSpeedWarpLimits.vector2Value = EditorGUILayout.Vector2Field(
                            "Speed Warp Limits", m_spSpeedWarpLimits.vector2Value);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (m_spSpeedWarpLimits.vector2Value.x > m_spSpeedWarpLimits.vector2Value.y)
                            {
                                m_spSpeedWarpLimits.vector2Value = new Vector2(m_spSpeedWarpLimits.vector2Value.y,
                                    m_spSpeedWarpLimits.vector2Value.y);
                            }
                        }
                    }
                    else
                    {
                        GUIStyle redBoldStyle = new GUIStyle(GUI.skin.label);
                        redBoldStyle.fontStyle = FontStyle.Bold;
                        redBoldStyle.normal.textColor = Color.red;

                        EditorGUILayout.LabelField("Check Stride Warper Component Attached", redBoldStyle);
                    }

                    EditorGUILayout.EndHorizontal();
                }


                GUILayout.Space(5f);
                curHeight += 5f;
            }

            curHeight += 30f;
            GUILayout.Space(2f);

            m_spDebugFoldout.boolValue = EditorUtil.EditorFunctions.DrawFoldout("Debug",
                curHeight, EditorGUIUtility.currentViewWidth, m_spDebugFoldout.boolValue);

            if (m_spDebugFoldout.boolValue)
            {
                m_spDebugGoal.boolValue = EditorGUILayout.Toggle(new GUIContent("Debug Goal"), m_spDebugGoal.boolValue);
                m_spDebugChosenTrajectory.boolValue = EditorGUILayout.Toggle(new GUIContent("Debug AnimTrajectory"), m_spDebugChosenTrajectory.boolValue);
                m_spDebugCurrentPose.boolValue = EditorGUILayout.Toggle(new GUIContent("Debug Current Pose"), m_spDebugCurrentPose.boolValue);

                if (!Application.IsPlaying(m_animator))
                {

                    EditorGUI.BeginChangeCheck();
                    m_spDebugPoses.boolValue = EditorGUILayout.Toggle(new GUIContent("Debug Poses In Editor"), m_spDebugPoses.boolValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (m_spDebugPoses.boolValue)
                            m_animator.StartPoseDebug(m_spDebugAnimDataId.intValue);
                        else
                            m_animator.StopPoseDebug();
                    }
                }

                if (m_spDebugPoses.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    EditorGUI.BeginChangeCheck();
                    m_spDebugAnimDataId.intValue = EditorGUILayout.IntField(new GUIContent("AnimData Id"), m_spDebugAnimDataId.intValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (m_spDebugAnimDataId.intValue < 0)
                            m_spDebugAnimDataId.intValue = 0;

                        if (m_spDebugAnimDataId.intValue >= m_spAnimData.arraySize - 1)
                            m_spDebugAnimDataId.intValue = m_spAnimData.arraySize - 2;

                        m_animator.StopPoseDebug();
                        m_animator.StartPoseDebug(m_spDebugAnimDataId.intValue);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (m_spDebugAnimDataId.intValue < m_spAnimData.arraySize - 1)
                    {
                        m_animData = m_spAnimData.GetArrayElementAtIndex(m_spDebugAnimDataId.intValue).objectReferenceValue as MxMAnimData;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    m_spDebugPoseId.intValue = EditorGUILayout.IntField(new GUIContent("Pose Id"), m_spDebugPoseId.intValue);
                    EditorGUILayout.EndHorizontal();

                    if (m_spDebugPoseId.intValue < 0)
                        m_spDebugPoseId.intValue = 0;

                    if (m_spDebugPoseId.intValue >= m_animData.Poses.Length)
                        m_spDebugPoseId.intValue = m_animData.Poses.Length - 1;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);

                    PoseData curPose = m_animData.Poses[m_spDebugPoseId.intValue];

                    EditorGUILayout.LabelField(m_animData.Clips[curPose.PrimaryClipId].name);
                    if (GUILayout.Button("Back"))
                    {
                        m_spDebugPoseId.intValue -= 1;

                        if (m_spDebugPoseId.intValue < 0)
                            m_spDebugPoseId.intValue = 0;
                    }

                    if (GUILayout.Button("Next"))
                    {
                        m_spDebugPoseId.intValue += 1;

                        if (m_spDebugPoseId.intValue >= m_animData.Poses.Length)
                            m_spDebugPoseId.intValue = m_animData.Poses.Length - 1;
                    }
                    EditorGUILayout.EndHorizontal();


                    curHeight += 36f;
                }

                if(GUILayout.Button("OpenDebugWindow"))
                {
                    MxMDebuggerWindow.SetTarget(m_animator);
                    MxMDebuggerWindow.ShowWindow();
                }

                if (Application.IsPlaying(m_animator))
                {

                    string pauseString = "Pause";

                    if (m_animator.IsPaused)
                        pauseString = "UnPause";


                    if (GUILayout.Button(new GUIContent(pauseString)))
                    {
                        m_animator.TogglePause();
                    }
                }

                if (GUILayout.Button(new GUIContent("Strip Pose Masks")))
                {
                    if (EditorUtility.DisplayDialog("Strip Pose Masks", "Are you sure you want to delete all pose masks attached " +
                         "to animData in this Animator? This is irreversible.", "Yes", "No"))
                    {
                        m_animator.ClearPoseMask();
                    }
                }
            }

            lastRect = GUILayoutUtility.GetLastRect();

            curHeight = lastRect.y + lastRect.height + 5f;
            GUILayout.Space(5f);

            m_spCallbackFoldout.boolValue = EditorUtil.EditorFunctions.DrawFoldout("Callbacks",
                curHeight, EditorGUIUtility.currentViewWidth, m_spCallbackFoldout.boolValue);

            if (m_spCallbackFoldout.boolValue)
            {
                EditorGUILayout.PropertyField(m_spOnSetupCompleteCallback);
                EditorGUILayout.PropertyField(m_spOnIdleTriggeredCallback);
                EditorGUILayout.PropertyField(m_spOnLeftFootStepStartCallback);
                EditorGUILayout.PropertyField(m_spOnRightFootStepStartCallback);
                EditorGUILayout.PropertyField(m_spOnEventCompleteCallback);
                EditorGUILayout.PropertyField(m_spOnEventContactCallback);
                EditorGUILayout.PropertyField(m_spOnEventChangeStateCallback);
            }

            if (m_spAnimData.arraySize <= 1 || m_spAnimData.GetArrayElementAtIndex(0).objectReferenceValue == null)
            {
                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }//End of class: MxMAnimatorInspector
}//End of namespace: MxM