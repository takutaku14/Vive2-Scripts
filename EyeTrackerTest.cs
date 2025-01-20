using System.IO;
using System.Text;
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
        private float averageInterval = 60f; // ���ϒl���擾

        private float responceInterval = 30f; // �Ό������J�n����v���I���܂ł̎���

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

        bool isStart = false;
        bool isStop = false;

        bool isExport = false;

        // ����
        float leftLatency = 0f;
        float rightLatency = 0f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isStart)
            {
                isStart = true;
            }
            else if (!isStart)
            {
                return;
            }

            if (isStart)
            {
                if (m_Text == null || leftGazeTransform == null || rightGazeTransform == null || panelColorChanger == null)
                {
                    return;
                }



                if (isStop)
                {

                    if (!isExport)
                    {
                        ExportToCsv();
                        Debug.LogError("Export to CSV!");

                        // ���ϒl��\��
                        DisplayAveragePupilValues();

                        // �ŏ��l��\��
                        DisplayPupilMinValues();

                        // ���k���x��\��
                        DisplayContractionSpeed();

                        // �Ċg�����x��\��
                        DisplayExpantionSpeed();

                        // ������\��
                        DisplayLatency();
                    }



                    return;
                }

                // ���ϒl�����܂�����Ό������𒲂ׂ�
                if (hasCalculatedAverage)
                {
                    LightResponce();
                }

                //m_Text.text = "[Eye Tracker]\n";
                m_Text.text = "";


                GetEyeGazeData();
                GetPupilData();

                // ���E�̍ŏ��l�Ǝ��k�����X�V
                UpdatePupilMinValues();

                // ���ϒl���v�Z�i���v�Z�̏ꍇ�̂݁j
                CalculateAveragePupilValues();



            }



        }

        private void ExportToCsv()
        {
            ExportListToCsv(leftContractionList, "LeftContractionList.csv");
            ExportListToCsv(leftExpantionList, "LeftExpantionList.csv");

            ExportListToCsv(rightContractionList, "RightContractionList.csv");
            ExportListToCsv(rightExpantionList, "RightExpantionList.csv");


            ExportPupilDataToCsv("PupilData.csv");

            ExportAverageListsToCsv(leftAverageList, "LeftAverageList.csv");
            ExportAverageListsToCsv(rightAverageList, "RightAverageList.csv");

            isExport = true;
        }


        private void ExportPupilDataToCsv(string fileName)
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Eye,Average Diameter,Minimum Diameter,Contraction Rate,Contraction Speed,Expansion Speed,Latency"); // ���o���s�̒ǉ�

            // ���ڂ̃f�[�^
            csvContent.AppendLine($"Left,{leftAverage.ToString("F4")},{leftPupilMin.ToString("F4")},{leftContractionRate.ToString("F4")},{leftContractionSpeed.ToString("F4")},{leftExpantionSpeed.ToString("F4")},{leftLatency.ToString("F4")}");

            // �E�ڂ̃f�[�^
            csvContent.AppendLine($"Right,{rightAverage.ToString("F4")},{rightPupilMin.ToString("F4")},{rightContractionRate.ToString("F4")},{rightContractionSpeed.ToString("F4")},{rightExpantionSpeed.ToString("F4")},{rightLatency.ToString("F4")}");

            // �J�����g�f�B���N�g���Ƀt�@�C�����o��
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(filePath, csvContent.ToString()); // CSV�t�@�C���ɏ�������
            Debug.Log($"{fileName} has been saved to {filePath}");
        }


        private void ExportListToCsv(List<float[]> dataList, string fileName)
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Diameter,Time"); // ���o���s�̒ǉ�

            foreach (var entry in dataList)
            {
                csvContent.AppendLine($"{entry[0]},{entry[1]}"); // Diameter �� Time ���J���}��؂�Œǉ�
            }

            // �J�����g�f�B���N�g���Ƀt�@�C�����o��
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(filePath, csvContent.ToString()); // CSV�t�@�C���ɏ�������
            Debug.Log($"{fileName} has been saved to {filePath}");
        }

        private void ExportAverageListsToCsv(List<float[]> averageList, string fileName)
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Average Diameter,Time"); // ���o���s�̒ǉ�

            foreach (var entry in averageList)
            {
                csvContent.AppendLine($"{entry[0]},{entry[1]}"); // Average Diameter �� Time ���J���}��؂�Œǉ�
            }

            // �J�����g�f�B���N�g���Ƀt�@�C�����o��
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(filePath, csvContent.ToString()); // CSV�t�@�C���ɏ�������
            Debug.Log($"{fileName} has been saved to {filePath}");
        }

        List<float[]> leftDiameterTimeList = new List<float[]>(); // [Diameter,sec]���i�[
        List<float[]> rightDiameterTimeList = new List<float[]>();

        List<float[]> leftContractionList = new List<float[]>();
        List<float[]> leftExpantionList = new List<float[]>();

        List<float[]> rightContractionList = new List<float[]>();
        List<float[]> rightExpantionList = new List<float[]>();

        // ���ϒl���Z�o���Ă���Ԃ̓��E���a�A���Ԃ��W�߂����X�g
        List<float[]> leftAverageList = new List<float[]>(); // [leftAverage,sec]���i�[
        List<float[]> rightAverageList = new List<float[]>(); // [rightAverage,sec]���i�[


        private void LightResponce()
        {

            if (!isWhiteStart)
            {
                panelColorChanger.ChangeColor();
                isWhiteStart = true;
                Debug.LogError($"Start LightResponce!");

            }



            if (sec < responceInterval)
            {
                // Debug.LogError($"Adding data - leftDiameter: {leftDiameter}, sec: {sec}");
                leftDiameterTimeList.Add(new float[] { leftDiameter, sec });
                rightDiameterTimeList.Add(new float[] { rightDiameter, sec });
                sec += Time.deltaTime;
                // Debug.LogError($"List count after adding: {leftDiameterTimeList.Count}");
            }
            else if (!isStop)
            {
                //���X�g���ŏ��l�O��ŕ���(���k�A�g���ɕ�����)
                var leftList = SplitDiameterTimeList(leftDiameterTimeList, leftPupilMin);
                var rightList = SplitDiameterTimeList(rightDiameterTimeList, rightPupilMin);



                leftContractionList = leftList.before;
                leftExpantionList = leftList.after;
                //// Debug.LogError($"leftContractionListCount: {leftContractionList.Count}");
                //// Debug.LogError($"leftExpantionListCount: {leftExpantionList.Count}");

                rightContractionList = rightList.before;
                rightExpantionList = rightList.after;
                //// Debug.LogError($"rightContractionListCount: {rightContractionList.Count}");
                //// Debug.LogError($"rightExpantionListCount: {rightExpantionList.Count}");

                CalculateContractionSpeed();
                CalculateExpantionSpeed();

                isStop = true;
            }

        }

        private void DisplayContractionSpeed()
        {
            m_Text.text += "���ڎ��k���x: " + leftContractionSpeed.ToString("F4") + " mm/s\n";
            m_Text.text += "�E�ڎ��k���x: " + rightContractionSpeed.ToString("F4") + " mm/s\n";
        }

        private void DisplayExpantionSpeed()
        {
            m_Text.text += "���ڍĊg�����x: " + leftExpantionSpeed.ToString("F4") + " mm/s\n";
            m_Text.text += "�E�ڍĊg�����x: " + rightExpantionSpeed.ToString("F4") + " mm/s\n";
        }

        float leftContractionSpeed = 0f;
        float rightContractionSpeed = 0f;


        float leftExpantionSpeed = 0f;
        float rightExpantionSpeed = 0f;


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

        private float GetFirstTimeAboveDiameter(List<float[]> diameterTimeList, float diameter)
        {
            foreach (var entry in diameterTimeList)
            {
                if (entry[0] > diameter) // entry[0] �� Diameter
                {
                    return entry[1]; // entry[1] �� Time
                }
            }

            return -1f; // �w�肳�ꂽDiameter������v�f��������Ȃ������ꍇ
        }



        private void CalculateExpantionSpeed()
        {
            // ����
            float deltaLeftMaxToMin = Mathf.Abs(leftAverage - leftPupilMin);
            float a = leftAverage - deltaLeftMaxToMin * 0.9f; // 10%�Ċg��
            float b = leftAverage - deltaLeftMaxToMin * 0.5f; // 50%�Ċg��
            float a_time = GetFirstTimeAboveDiameter(leftExpantionList, a);
            float b_time = GetFirstTimeAboveDiameter(leftExpantionList, b);
            float leftTime = b_time - a_time;

            leftExpantionSpeed = Mathf.Abs((a - b) / leftTime);

            // �E��
            float deltaRightMaxToMin = Mathf.Abs(rightAverage - rightPupilMin);
            float c = rightAverage - deltaRightMaxToMin * 0.9f; // 10%�Ċg��
            float d = rightAverage - deltaRightMaxToMin * 0.5f; // 50%�Ċg��
            float c_time = GetFirstTimeAboveDiameter(rightExpantionList, c);
            float d_time = GetFirstTimeAboveDiameter(rightExpantionList, d);
            float rightTime = d_time - c_time;

            rightExpantionSpeed = Mathf.Abs((c - d) / rightTime);
        }

        private void CalculateContractionSpeed()
        {
            // ����
            float deltaLeftMaxToMin = Mathf.Abs(leftAverage - leftPupilMin);
            float a = leftAverage - deltaLeftMaxToMin * 0.1f; // 10%���k
            float b = leftAverage - deltaLeftMaxToMin * 0.9f; // 90%���k
            float a_time = GetFirstTimeBelowDiameter(leftContractionList, a);
            float b_time = GetFirstTimeBelowDiameter(leftContractionList, b);
            float leftTime = b_time - a_time;

            leftContractionSpeed = Mathf.Abs((a - b) / leftTime);

            // �E��
            float deltaRightMaxToMin = Mathf.Abs(rightAverage - rightPupilMin);
            float c = rightAverage - deltaRightMaxToMin * 0.1f; // 10%���k
            float d = rightAverage - deltaRightMaxToMin * 0.9f; // 90%���k
            float c_time = GetFirstTimeBelowDiameter(rightContractionList, c);
            float d_time = GetFirstTimeBelowDiameter(rightContractionList, d);
            float rightTime = d_time - c_time;

            rightContractionSpeed = Mathf.Abs((c - d) / rightTime);
        }


        public static bool AreApproximatelyEqual(float a, float b, float epsilon = 0.0001f)
        {
            return Mathf.Abs(a - b) < epsilon;
        }

        private (List<float[]> before, List<float[]> after) SplitDiameterTimeList(List<float[]> diameterTimeList, float diameter)
        {
            List<float[]> beforeList = new List<float[]>();
            List<float[]> afterList = new List<float[]>();
            bool isContraction = true;

            // Debug.LogError($"-----------start------------");

            foreach (var entry in diameterTimeList)
            {
                bool result = AreApproximatelyEqual(entry[0], diameter);
                // Debug.LogError($"{entry[0]} vs {diameter}, isContraction: {isContraction}");
                if (isContraction && !result)
                {
                    beforeList.Add(entry);
                    // Debug.LogError($"before");
                }
                else if (result)
                {
                    // Debug.LogError($"Min!!!!!!");
                    isContraction = false;
                }
                else
                {
                    afterList.Add(entry);
                    // Debug.LogError($"after");
                }
            }

            // Debug.LogError($"before: {beforeList.Count}");
            // Debug.LogError($"after: {afterList.Count}");

            // before:���k�� after:�Ċg����
            return (before: beforeList, after: afterList);
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
            //m_Text.text += "���ڂ̏��: " + (leftGaze.isValid ? "�J���Ă���" : "���Ă���") + "\n";

            if (leftGaze.isValid)
            {
                leftGazeTransform.position = leftGaze.gazePose.position.ToUnityVector();
                leftGazeTransform.rotation = leftGaze.gazePose.orientation.ToUnityQuaternion();
            }

            XrSingleEyeGazeDataHTC rightGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            //m_Text.text += "�E�ڂ̏��: " + (rightGaze.isValid ? "�J���Ă���" : "���Ă���") + "\n";

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

        float leftDiameter = 0f;

        float rightDiameter = 0f;

        private void UpdatePupilValues(XrSingleEyePupilDataHTC[] out_pupils)
        {
            XrSingleEyePupilDataHTC leftPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            if (leftPupil.isDiameterValid)
            {
                leftDiameter = leftPupil.pupilDiameter * 1000; // mm
                leftPupilSum += leftDiameter;
                leftPupilCount++;
                //m_Text.text += "���ڂ̓��E���a: " + leftDiameter.ToString("F4") + " mm\n";
            }
            else
            {
                //m_Text.text += "���ڂ̓��E���F������Ă��܂���\n";
            }

            XrSingleEyePupilDataHTC rightPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            if (rightPupil.isDiameterValid)
            {
                rightDiameter = rightPupil.pupilDiameter * 1000; // mm
                rightPupilSum += rightDiameter;
                rightPupilCount++;
                //m_Text.text += "�E�ڂ̓��E���a: " + rightDiameter.ToString("F4") + " mm\n";
            }
            else
            {
                //m_Text.text += "�E�ڂ̓��E���F������Ă��܂���\n";
            }
        }

        // ���k��
        float leftContractionRate = 0f;
        float rightContractionRate = 0f;

        bool isLeftContractionStart = false;
        bool isRightContractionStart = false;


        private void UpdatePupilMinValues()
        {

            if (isWhiteStart) {
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

            }

            // ���k�����X�V
            if (leftAverage != 0)
            {
                leftContractionRate = (1 - leftPupilMin / leftAverage) * 100;
                Debug.LogError($"leftContractionRate: {leftContractionRate}, {timeElapsed}");

                if (leftContractionRate >= 10f)
                {

                    if (!isLeftContractionStart && isWhiteStart)
                    {
                        isLeftContractionStart = true;
                    }
                }

                if (!isLeftContractionStart) {
                    if (isWhiteStart) { 
                        leftLatency += Time.deltaTime;
                        Debug.LogError($"leftLatecncy: {leftLatency}");
                    }
                }

            }

            if (rightAverage != 0)
            {
                rightContractionRate = (1 - rightPupilMin / rightAverage) * 100;
                Debug.LogError($"rightContractionRate: {rightContractionRate}, {timeElapsed}");

                if (rightContractionRate >= 10f)
                {

                    if (!isRightContractionStart && isWhiteStart)
                    {
                        isRightContractionStart = true;
                    }
                }

                if (!isRightContractionStart)
                {
                    if (isWhiteStart)
                    {
                        rightLatency += Time.deltaTime;
                        Debug.LogError($"rightLatecncy: {rightLatency}");
                    }
                }
            }
        }

        float leftAverage = 0f;
        float rightAverage = 0f;

        private void CalculateAveragePupilValues()
        {
            if (!hasCalculatedAverage)
            {
                timeElapsed += Time.deltaTime;

                leftAverage = leftPupilCount > 0 ? leftPupilSum / leftPupilCount : 0f;
                rightAverage = rightPupilCount > 0 ? rightPupilSum / rightPupilCount : 0f;

                // ���ϒl���X�g�ɒǉ�
                leftAverageList.Add(new float[] { leftAverage, timeElapsed });
                rightAverageList.Add(new float[] { rightAverage, timeElapsed });


                if (timeElapsed >= averageInterval)
                {
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

        private void DisplayLatency()
        {
            // ������\��
            m_Text.text += "���ڂ̐���: " + leftLatency.ToString("F4") + " �b\n";
            m_Text.text += "�E�ڂ̐���: " + rightLatency.ToString("F4") + " �b\n";

        }
    }
}
