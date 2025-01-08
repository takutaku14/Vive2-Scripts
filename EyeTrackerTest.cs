using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VIVE.OpenXR.EyeTracker;

namespace VIVE.OpenXR.Samples.EyeTracker
{
    public class EyeTrackerTest : MonoBehaviour
    {
        public Transform leftGazeTransform = null;
        public Transform rightGazeTransform = null;
        public PanelColorChanger panelColorChanger; // PanelColorChanger�̃C���X�^���X

        private Text m_Text = null;


        private float leftPupilSum = 0f;
        private float rightPupilSum = 0f;
        private int leftPupilCount = 0;
        private int rightPupilCount = 0;
        private float averageInterval = 9f; // 9�b
        private float timeElapsed = 0f;

        // ���ϒl�\���p�̕ϐ�
        private string leftAverageText = "";
        private string rightAverageText = "";

        // �ŏ��l��ێ�����ϐ�
        private float leftPupilMin = float.MaxValue;
        private float rightPupilMin = float.MaxValue;

        // ���ϒl���v�Z���ꂽ���ǂ����̃t���O
        private bool hasCalculatedAverage = false;

        private void Awake()
        {
            m_Text = GetComponent<Text>();
            if (m_Text == null)
            {
                Debug.LogError("Text component is not found!");
            }
        }

        bool isStart = false;
        void Update()
        {
            if (m_Text == null || leftGazeTransform == null || rightGazeTransform == null || panelColorChanger == null)
            {
                return;
            }

            m_Text.text = "[Eye Tracker]\n";

            GetEyeGazeData();
            GetPupilData();

            // ���E�̍ŏ��l���X�V
            UpdatePupilMinValues();

            // 10�b���Ƃ̕��ϒl���v�Z�i���v�Z�̏ꍇ�̂݁j
            CalculateAveragePupilValues();

            // ���ϒl��\��
            DisplayAveragePupilValues();

            // �ŏ��l��\��
            DisplayPupilMinValues();

            if (isStart)
            {
                CalculateContractionSpped();
            }
        }

        private void CalculateContractionSpped()
        {

        }

        private void GetEyeGazeData()
        {
            XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes);
            if (out_gazes.Length >= 2)
            {
                UpdateGazeTransform(out_gazes);
            }
        }

        private void UpdateGazeTransform(XrSingleEyeGazeDataHTC[] out_gazes)
        {
            XrSingleEyeGazeDataHTC leftGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            m_Text.text += "���ڂ̏��: " + (leftGaze.isValid ? "�J���Ă���" : "���Ă���") + "\n";

            if (leftGaze.isValid)
            {
                leftGazeTransform.position = leftGaze.gazePose.position.ToUnityVector();
                leftGazeTransform.rotation = leftGaze.gazePose.orientation.ToUnityQuaternion();
            }

            XrSingleEyeGazeDataHTC rightGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            m_Text.text += "�E�ڂ̏��: " + (rightGaze.isValid ? "�J���Ă���" : "���Ă���") + "\n";

            if (rightGaze.isValid)
            {
                rightGazeTransform.position = rightGaze.gazePose.position.ToUnityVector();
                rightGazeTransform.rotation = rightGaze.gazePose.orientation.ToUnityQuaternion();
            }
        }

        private void GetPupilData()
        {
            XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] out_pupils);
            if (out_pupils.Length >= 2)
            {
                UpdatePupilValues(out_pupils);
            }
        }

        float leftDiameter = 0;

        float rightDiameter = 0;

        private void UpdatePupilValues(XrSingleEyePupilDataHTC[] out_pupils)
        {
            XrSingleEyePupilDataHTC leftPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            if (leftPupil.isDiameterValid)
            {
                leftDiameter = leftPupil.pupilDiameter * 1000; // mm
                leftPupilSum += leftDiameter;
                leftPupilCount++;
                m_Text.text += "���ڂ̓��E���a: " + leftDiameter.ToString("F4") + " mm\n";
            }
            else
            {
                m_Text.text += "���ڂ̓��E���a: ����\n";
            }

            XrSingleEyePupilDataHTC rightPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            if (rightPupil.isDiameterValid)
            {
                rightDiameter = rightPupil.pupilDiameter * 1000; // mm
                rightPupilSum += rightDiameter;
                rightPupilCount++;
                m_Text.text += "�E�ڂ̓��E���a: " + rightDiameter.ToString("F4") + " mm\n";
            }
            else
            {
                m_Text.text += "�E�ڂ̓��E���a: ����\n";
            }
        }

        private void UpdatePupilMinValues()
        {
            // ���ڂ̍ŏ��l���X�V
            if (leftPupilCount > 0)
            {
                leftPupilMin = Mathf.Min(leftPupilMin, leftDiameter);
            }

            // �E�ڂ̍ŏ��l���X�V
            if (rightPupilCount > 0)
            {
                rightPupilMin = Mathf.Min(rightPupilMin, rightDiameter);
            }

            // ���k�����Z�o
            if (leftAverage != 0)
            {
                leftContractionRate = (1 - leftPupilMin / leftAverage) * 100;
            }

            if (rightAverage != 0)
            {
                rightContractionRate = (1 - rightPupilMin / rightAverage) * 100;
            }
        }

        float leftAverage = 0;
        float rightAverage = 0;

        private void CalculateAveragePupilValues()
        {
            if (!hasCalculatedAverage)
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= averageInterval)
                {
                    leftAverage = leftPupilCount > 0 ? leftPupilSum / leftPupilCount : 0f;
                    rightAverage = rightPupilCount > 0 ? rightPupilSum / rightPupilCount : 0f;

                    leftAverageText = "���ڂ̕��ϓ��E���a: " + leftAverage.ToString("F4") + " mm\n";
                    rightAverageText = "�E�ڂ̕��ϓ��E���a: " + rightAverage.ToString("F4") + " mm\n";

                    // ���ϒl���v�Z�ς݂Ƃ��ăt���O��ݒ�
                    hasCalculatedAverage = true;
                }
            }
        }

        private void DisplayAveragePupilValues()
        {
            m_Text.text += leftAverageText;
            m_Text.text += rightAverageText;
        }

        bool isMinAdded = false;

        float leftContractionRate = 0;
        float rightContractionRate = 0;

        private void DisplayPupilMinValues()
        {
            // �ŏ��l��\��
            if (leftPupilCount > 0)
            {
                m_Text.text += "���ڂ̍ŏ����E���a: " + leftPupilMin.ToString("F4") + " mm\n";
                m_Text.text += "���ڂ̎��k��: " + leftContractionRate.ToString("F4") + " %\n";

            }

            if (rightPupilCount > 0)
            {
                m_Text.text += "�E�ڂ̍ŏ����E���a: " + rightPupilMin.ToString("F4") + " mm\n";
                m_Text.text += "�E�ڂ̎��k��: " + rightContractionRate.ToString("F4") + " %\n";
            }


        }
    }
}
