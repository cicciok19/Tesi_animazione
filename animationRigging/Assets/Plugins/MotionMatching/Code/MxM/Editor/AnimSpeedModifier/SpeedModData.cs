using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MxM;

namespace MxMEditor
{
    [System.Serializable]
    public class MotionModifyData
    {
        [SerializeField]
        private IMxMAnim m_targetMxMAnim;

        [SerializeField]
        public List<MotionSection> MotionSections = new List<MotionSection>(); //Name of the variable for a section

        public MotionModifyData() { }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public MotionModifyData(MotionModifyData a_copy, IMxMAnim a_targetMxMAnim)
        {
            m_targetMxMAnim = a_targetMxMAnim;

            MotionSections = new List<MotionSection>();
            foreach(MotionSection motionSection in a_copy.MotionSections)
            {
                MotionSections.Add(new MotionSection(motionSection));
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void OnEnable(IMxMAnim m_targetMxMAnim)
        {
            if (MotionSections == null)
                MotionSections = new List<MotionSection>();

            if (m_targetMxMAnim == null)
                return;

            AnimationClip primaryClip = m_targetMxMAnim.TargetClip;

            if (MotionSections.Count == 0 && m_targetMxMAnim != null && primaryClip != null)
            {
                MotionSections.Add(new MotionSection(0, 0, 0, primaryClip.length));
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void NewClipSet()
        {
            if (MotionSections == null)
                MotionSections = new List<MotionSection>();

            MotionSections.Clear();

            if (m_targetMxMAnim != null)
            {
                AnimationClip primaryClip = m_targetMxMAnim.TargetClip;

                if (primaryClip != null)
                {
                    MotionSections.Add(new MotionSection(0, 0, EMotionModSmooth.Linear, primaryClip.length));
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void AddPOI(float a_time)
        {
            Undo.RecordObject(m_targetMxMAnim as ScriptableObject, "Section added");

            for (int i=0; i < MotionSections.Count; ++i)
            {
                if(a_time < MotionSections[i].EndTime)
                {
                    MotionSection newSection = new MotionSection(i, MotionSections[i], a_time);

                    //MotionSection newSection = new MotionSection(i, 0, EMotionModSmooth.Linear, a_time);
                    MotionSections.Insert(i, newSection);

                    for(int n=i+1; n < MotionSections.Count; ++n)
                    {
                        MotionSections[n].MotionSectionId = n;
                    }

                    break;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void DrawSections(ref Rect a_trackRect, ref Rect a_timelineRect, float a_zoom, MxMTaggingWindow a_taggingWindow)
        {
            Texture markerIcon = EditorGUIUtility.IconContent("Animation.EventMarker").image;

            Rect markerRect;

            Event evt = Event.current;

            float lastTime = 0f;

            MotionTimingPresets motionTimingPresets = null;

            if (a_taggingWindow != null)
                m_targetMxMAnim = a_taggingWindow.TargetMxMAnim;

            var targetPreProcess = m_targetMxMAnim.TargetPreProcess;

            if (m_targetMxMAnim != null && targetPreProcess != null)
            {
                motionTimingPresets = targetPreProcess.MotionTimingPresets;
            }

            string[] defenitionNames = null;
            if(motionTimingPresets != null)
            {
                defenitionNames = motionTimingPresets.GetDefenitionNames();
            }

           float maxSpeedVariance = 0.5f;

            //Determine max variance
            for (int i = 0; i < MotionSections.Count; ++i)
            {
                MotionSection curSection = MotionSections[i];

                float speedMod = 1f;
                speedMod = curSection.GetSpeedMod(lastTime, motionTimingPresets, m_targetMxMAnim);

                float speedVariance = 0f;
                if (speedMod > 1f)
                    speedVariance = (speedMod / 1f) - 1f;
                else if (speedMod < 1f)
                    speedVariance = (1f / speedMod) - 1f;

                if (speedVariance > maxSpeedVariance)
                    maxSpeedVariance = speedVariance;

                lastTime = curSection.EndTime;
            }

            lastTime = 0f;
            for (int i = 0; i < MotionSections.Count; ++i)
            {
                MotionSection curSection = MotionSections[i];

                markerRect = new Rect(curSection.EndTime * 75f * a_zoom - (markerIcon.width / 2f), a_trackRect.y,
                     markerIcon.width, markerIcon.height);

                Vector3 start = new Vector3(markerRect.x + markerRect.width / 2f,
                                                markerRect.y + markerRect.height);
                Vector3 end = new Vector3(start.x, a_timelineRect.height);

                if (i != MotionSections.Count - 1)
                {
                    GUI.DrawTexture(markerRect, markerIcon);

                    Handles.color = Color.black;
                    Handles.DrawLine(start, end);

                    if (curSection.Selected)
                        GUI.DrawTexture(markerRect, EditorUtil.EditorFunctions.GetHighlightTex());


                    markerRect.x -= 3f;
                    markerRect.height = a_trackRect.height;
                    markerRect.width += 6f;

                    if (evt.isMouse && evt.button == 0)
                    {
                        switch (evt.type)
                        {
                            case EventType.MouseDown:
                                {
                                    if (markerRect.Contains(evt.mousePosition))
                                    {
                                        curSection.Selected = true;
                                        curSection.Dragging = true;
                                        a_taggingWindow.SelectSection(curSection, curSection.EndTime);
                                    }

                                }
                                break;
                            case EventType.MouseUp:
                                {
                                    curSection.Dragging = false;
                                }
                                break;
                            case EventType.MouseDrag:
                                {
                                    if (curSection.Dragging && curSection.Selected)
                                    {
                                        float desiredValueDelta = ((evt.delta.x / a_zoom)) / 75f;

                                        curSection.EndTime += desiredValueDelta;

                                        curSection.EndTime = Mathf.Clamp(curSection.EndTime, 0f,
                                            a_taggingWindow.TargetClip.length);
                                        a_taggingWindow.Modified(curSection.EndTime);
                                    }
                                }
                                break;
                        }
                    }
                }


                float speedMod = 1f;
                speedMod = curSection.GetSpeedMod(lastTime, motionTimingPresets, m_targetMxMAnim);

                float speedVariance = 0f;
                if (speedMod > 1f)
                {
                    speedVariance = (speedMod / 1f) - 1f;
                }
                else if (speedMod < 1f)
                {
                    speedVariance = (1f / speedMod) - 1f;
                }

                float heightRatio = speedVariance / maxSpeedVariance;

                if (speedMod < 1f)
                    heightRatio *= -1;

                if (maxSpeedVariance < 0.001f)
                    heightRatio = 0f;

                //Draw Baseline
                end = new Vector3(curSection.EndTime * 75f * a_zoom, a_timelineRect.height / 2f + 9f);
                start = new Vector3(lastTime * 75f * a_zoom, end.y);

                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawLine(start, end);

                //Draw limit line
                Handles.color = new Color(0f, 0f, 0f, 0.3f);
                if (speedMod > 1f + Mathf.Epsilon)
                {
                    end.y = a_timelineRect.height / 2f + (a_timelineRect.height / 2f - 44f) + 9f;
                    start.y = end.y;

                    Handles.DrawLine(start, end);
                }
                else
                {

                    //Draw limit line
                    end.y = a_timelineRect.height / 2f - (a_timelineRect.height / 2f - 44f) + 9f;
                    start.y = end.y;

                    Handles.DrawLine(start, end);
                }

                //Draw Green Line
                end.y = a_timelineRect.height / 2f + heightRatio * (a_timelineRect.height / 2f - 44f) + 9f;
                start.y = end.y;

                Handles.color = Color.green;
                Handles.DrawLine(start, end);

                float invertedSpeedMod = 1f / speedMod;
                string speedString = invertedSpeedMod.ToString("F2");

                speedString += "x";

                float width = GUI.skin.label.CalcSize(new GUIContent(speedString)).x;
                GUI.Label(new Rect(start.x + (end.x - start.x) / 2f - width / 2f, end.y - 18f, width, 18f), speedString);


                Rect dataRect = new Rect(lastTime * 75f * a_zoom + 4, a_timelineRect.height / 1.75f,
                    end.x - start.x - 8, a_timelineRect.height - (a_timelineRect.height / 1.75f) - 18f);

                if (speedMod > 1f + Mathf.Epsilon)
                {
                    dataRect.y = 40f;
                }

                //Draw Drop Down Boxes

                GUI.Box(dataRect, "Section " + i);

                dataRect.x += 3f;
                dataRect.width -= 6f;

                GUILayout.BeginArea(dataRect);
                GUILayout.Space(18f);

                if (motionTimingPresets != null)
                {
                    curSection.UsePresets = GUILayout.Toggle(curSection.UsePresets,
                        new GUIContent("Use Preset"));
                }

                if (curSection.UsePresets && motionTimingPresets != null)
                {
                    curSection.MotionPresetId = EditorGUILayout.Popup(curSection.MotionPresetId,
                        defenitionNames);
                }
                else
                {
                    float defaultLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 40f;
                    curSection.ModType = (EMotionModType)EditorGUILayout.EnumPopup(new GUIContent("Type"), curSection.ModType);
                    curSection.RawModValue = EditorGUILayout.FloatField(new GUIContent("Value"), curSection.RawModValue);


                    if (curSection.RawModValue < 0.01f)
                        curSection.RawModValue = 0.01f;


                    EditorGUIUtility.labelWidth = defaultLabelWidth;
                }

                GUILayout.FlexibleSpace();

                float originalDurationF = (curSection.EndTime - lastTime);
                float finalDurationF = ((curSection.EndTime - lastTime) * speedMod);

                string originalDuration = originalDurationF.ToString("F2");
                string finalDuration = finalDurationF.ToString("F2");

                float originalSpeedF = m_targetMxMAnim.GetAverageRootSpeed(lastTime, curSection.EndTime);
                float finalSpeedF = originalSpeedF * (originalDurationF / finalDurationF);

                string originalSpeed = originalSpeedF.ToString("F2");
                string finalSpeed = finalSpeedF.ToString("F2");



                EditorGUILayout.LabelField("Original: " + originalDuration + " sec | " + originalSpeed + "m/s");
                EditorGUILayout.LabelField("Final: " + finalDuration + " sec | " + finalSpeed + "m/s");

                GUILayout.EndArea();

                lastTime = curSection.EndTime;
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public float GetSectionSpeedAtTime(float a_time, MotionTimingPresets a_presets)
        {
            float speed = 1f;
            float lastTime = 0f;
            for(int i=0; i < MotionSections.Count; ++i)
            {
                if(a_time > lastTime && a_time < MotionSections[i].EndTime)
                {
                    speed = 1f / MotionSections[i].GetSpeedMod(lastTime, a_presets, m_targetMxMAnim);
                    break;
                }

                lastTime = MotionSections[i].EndTime;
            }

            return speed;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void VerifyData()
        {

            if (m_targetMxMAnim == null)
                return;

            AnimationClip primaryClip = m_targetMxMAnim.TargetClip;

            if (primaryClip == null)
                return;

            for (int i = MotionSections.Count - 1; i >= 0; --i)
            {
                MotionSection section = MotionSections[i];

                if (i - 1 >= 0)
                {
                    if (MotionSections[i - 1].EndTime > primaryClip.length)
                    {
                        MotionSections.RemoveAt(i);
                        continue;
                    }
                }
                else if (section.EndTime > primaryClip.length)
                {
                    section.EndTime = primaryClip.length;
                }
            }

            if (MotionSections.Count > 0)
            {
                MotionSections[MotionSections.Count - 1].EndTime = primaryClip.length;
            }

        }

    }//End of class: SpeedModData
}//End of namepsace: MxM