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
        public PanelColorChanger panelColorChanger; // PanelColorChangerのインスタンス

        private Text m_Text = null;


        private float leftPupilSum = 0f;
        private float rightPupilSum = 0f;
        private int leftPupilCount = 0;
        private int rightPupilCount = 0;
        private float averageInterval = 9f; // 9秒
        private float timeElapsed = 0f;

        // 平均値表示用の変数
        private string leftAverageText = "";
        private string rightAverageText = "";

        // 最小値を保持する変数
        private float leftPupilMin = float.MaxValue;
        private float rightPupilMin = float.MaxValue;

        // 平均値が計算されたかどうかのフラグ
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

            // 瞳孔の最小値を更新
            UpdatePupilMinValues();

            // 10秒ごとの平均値を計算（未計算の場合のみ）
            CalculateAveragePupilValues();

            // 平均値を表示
            DisplayAveragePupilValues();

            // 最小値を表示
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
            m_Text.text += "左目の状態: " + (leftGaze.isValid ? "開いている" : "閉じている") + "\n";

            if (leftGaze.isValid)
            {
                leftGazeTransform.position = leftGaze.gazePose.position.ToUnityVector();
                leftGazeTransform.rotation = leftGaze.gazePose.orientation.ToUnityQuaternion();
            }

            XrSingleEyeGazeDataHTC rightGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            m_Text.text += "右目の状態: " + (rightGaze.isValid ? "開いている" : "閉じている") + "\n";

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
                m_Text.text += "左目の瞳孔半径: " + leftDiameter.ToString("F4") + " mm\n";
            }
            else
            {
                m_Text.text += "左目の瞳孔半径: 無効\n";
            }

            XrSingleEyePupilDataHTC rightPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            if (rightPupil.isDiameterValid)
            {
                rightDiameter = rightPupil.pupilDiameter * 1000; // mm
                rightPupilSum += rightDiameter;
                rightPupilCount++;
                m_Text.text += "右目の瞳孔半径: " + rightDiameter.ToString("F4") + " mm\n";
            }
            else
            {
                m_Text.text += "右目の瞳孔半径: 無効\n";
            }
        }

        private void UpdatePupilMinValues()
        {
            // 左目の最小値を更新
            if (leftPupilCount > 0)
            {
                leftPupilMin = Mathf.Min(leftPupilMin, leftDiameter);
            }

            // 右目の最小値を更新
            if (rightPupilCount > 0)
            {
                rightPupilMin = Mathf.Min(rightPupilMin, rightDiameter);
            }

            // 収縮率を算出
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

                    leftAverageText = "左目の平均瞳孔半径: " + leftAverage.ToString("F4") + " mm\n";
                    rightAverageText = "右目の平均瞳孔半径: " + rightAverage.ToString("F4") + " mm\n";

                    // 平均値を計算済みとしてフラグを設定
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
            // 最小値を表示
            if (leftPupilCount > 0)
            {
                m_Text.text += "左目の最小瞳孔半径: " + leftPupilMin.ToString("F4") + " mm\n";
                m_Text.text += "左目の収縮率: " + leftContractionRate.ToString("F4") + " %\n";

            }

            if (rightPupilCount > 0)
            {
                m_Text.text += "右目の最小瞳孔半径: " + rightPupilMin.ToString("F4") + " mm\n";
                m_Text.text += "右目の収縮率: " + rightContractionRate.ToString("F4") + " %\n";
            }


        }
    }
}
