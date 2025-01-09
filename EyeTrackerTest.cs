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
        private float averageInterval = 10f; // 10�b�ŕ��ϒl���擾
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

        bool isWhiteStart = false;
        float sec = 0f;

        bool isStop = false;


        void Update()
        {
            if (m_Text == null || leftGazeTransform == null || rightGazeTransform == null || panelColorChanger == null)
            {
                return;
            }

            // ���ϒl�����܂�����Ό������𒲂ׂ�
            if (hasCalculatedAverage)
            {
                LightResponce();
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

            // ���k���x��\��
            DisplayContractionSpeed();






        }

        List<float[]> leftDiameterTimeList = new List<float[]>(); // [Diameter,sec]���i�[
        List<float[]> rightDiameterTimeList = new List<float[]>();

        List<float[]> leftContractionList = new List<float[]>();
        List<float[]> leftExpantionList = new List<float[]>();

        List<float[]> rightContractionList = new List<float[]>();
        List<float[]> rightExpantionList = new List<float[]>();


        private void LightResponce()
        {

            if (!isWhiteStart)
            {
                panelColorChanger.ChangeColor();
                isWhiteStart = true;
            }

            if (sec < 10f)
            {
                leftDiameterTimeList.Add([leftDiameter, sec]);
                rightDiameterTimeList.Add([rightDiameter, sec]);
                sec += Time.deltaTime;
            }
            else if (!isStop)
            {
                //���X�g���ŏ��l�O��ŕ���(���k�A�g���ɕ�����)
                var leftList = SplitDiameterTimeList(leftDiameterTimeList, leftPupilMin);
                var rightList = SplitDiameterTimeList(rightDiameterTimeList, rightPupilMin);

                leftContractionList = leftList.before;
                leftExpantionList = leftList.after;

                rightContractionList = rightList.before;
                rightExpantionList = rightList.after;

                CalculateContractionSpped();

                isStop = true;
            }

        }
        private void DisplayContractionSpeed()
        {
            m_Text.text += "���ڎ��k���x: " + leftContractionSpeed.ToString("F4") + " mm/s\n";
            m_Text.text += "�E�ڎ��k���x: " + rightContractionSpeed.ToString("F4") + " mm/s\n";
        }

        float leftContractionSpeed = 0f;
        float rightContractionSpeed = 0f;



        private float GetFirstTimeBelowDiameter(List<float[]> diameterTimeList, float diameter)
        {
            foreach (var entry in diameterTimeList)
            {
                if (entry[0] < diameter) // entry[0] �� Diameter
                {
                    return entry[1]; // entry[1] �� Time
                }
            }

            return -1f; // �w�肳�ꂽDiameter�������v�f��������Ȃ������ꍇ
        }



        private void CalculateContractionSpped()
        {
            // ����
            float deltaLeftMaxToMin = leftAverage - leftPupilMin;
            float a = leftAverage - deltaLeftMaxToMin * 0.1; // 10%���k
            float b = leftAverage - deltaLeftMaxToMin * 0.9; // 90%���k
            float a_time = GetFirstTimeBelowDiameter(leftContractionList, a);
            float b_time = GetFirstTimeBelowDiameter(leftContractionList, b);
            float leftTime = b_time - a_time;

            leftContractionSpeed = Math.abs((a - b) / leftTime);

            // �E��
            float deltaRightMaxToMin = RightAverage - RightPupilMin;
            float c = rightAverage - deltaRightMaxToMin * 0.1; // 10%���k
            float d = rightAverage - deltaRightMaxToMin * 0.9; // 90%���k
            float c_time = GetFirstTimeBelowDiameter(rightContractionList, a);
            float d_time = GetFirstTimeBelowDiameter(rightContractionList, b);
            float rightTime = d_time - c_time;

            rightContractionSpeed = Math.abs((c - d) / rightTime);

        }


        private (List<float[]> before, List<float[]> after) SplitDiameterTimeList(List<float[] DiameterTimeList>, float diameter)
        {
            List<float[]> beforeList = new List<float[]>();
            List<float[]> afterList = new List<float[]>();

            // ���̒��a���X�g�𕪊�
            foreach (var entry in DiameterTimeList)
            {
                if (entry[0] < diameter)
                {
                    beforeList.Add(entry);
                }
                else
                {
                    afterList.Add(entry);
                }
            }

            return (beforeList, afterList);
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

        // ���k��
        float leftContractionRate = 0;
        float rightContractionRate = 0;

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
