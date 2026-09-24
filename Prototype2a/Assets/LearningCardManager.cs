using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class LearningCardManager : MonoBehaviour
{
    public TMP_Text chineseText;
    public TMP_Text pinyinText;
    public TMP_Text englishText;
    public TMP_Text progressText;

    public UIFlowManager uiFlowManager;

    private int currentIndex = 0;

    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;

    private string[] chineseWords =
    {
        "书",
        "桌子",
        "电脑",
        "找到",
        "拿起"
    };

    private string[] pinyinWords =
    {
        "shū",
        "zhuō zi",
        "diàn nǎo",
        "zhǎo dào",
        "ná qǐ"
    };

    private string[] englishWords =
    {
        "book",
        "table",
        "computer",
        "find",
        "pick up"
    };

    void Start()
    {
        ShowCard();
    }

    void Update()
    {
        CheckControllerInput();
    }

    void CheckControllerInput()
    {
        InputDevice leftController =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        InputDevice rightController =
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (leftController.TryGetFeatureValue(
            CommonUsages.triggerButton, out bool leftPressed))
        {
            if (leftPressed && !leftTriggerPressed)
            {
                PreviousCard();
            }

            leftTriggerPressed = leftPressed;
        }

        if (rightController.TryGetFeatureValue(
            CommonUsages.triggerButton, out bool rightPressed))
        {
            if (rightPressed && !rightTriggerPressed)
            {
                NextCard();
            }

            rightTriggerPressed = rightPressed;
        }
    }

    public void NextCard()
    {
        if (currentIndex < chineseWords.Length - 1)
        {
            currentIndex++;
            ShowCard();
        }
        else
        {
            uiFlowManager.ShowReady();
        }
    }

    public void PreviousCard()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCard();
        }
    }

    void ShowCard()
    {
        chineseText.text = chineseWords[currentIndex];
        pinyinText.text = pinyinWords[currentIndex];
        englishText.text = englishWords[currentIndex];

        progressText.text =
            (currentIndex + 1) + " / " + chineseWords.Length;
    }
}