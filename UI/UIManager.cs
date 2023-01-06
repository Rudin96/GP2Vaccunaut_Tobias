using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform MessageObject;

    public Text TextObject;

    private string cleanInputName;
    private string cleanBindingName;
    private TutorialMessage tempMessage;
    private InputActionReference prevInputAction;

    private int buttonsPressedCount;

    private void Awake()
    {
        InputUser.onChange += InputUser_onChange;
    }

    private void OnDestroy()
    {
        InputUser.onChange -= InputUser_onChange;
    }

    private void InputUser_onChange(InputUser inputUser, InputUserChange userChange, InputDevice device)
    {
        cleanInputName = inputUser.controlScheme.Value.ToString().Split('(')[0];
    }

    public void ShowTutorialUI(TutorialMessage tutorialMessage)
    {
        if(gameObject.activeSelf)
        {
            HideTutorialUI();
        }
        MessageObject.gameObject.SetActive(true);
        tempMessage = tutorialMessage;
        TextObject.text = FormatUI(tutorialMessage.Message, tutorialMessage.Binding.Count);
        tutorialMessage.Binding.ForEach(e => e.action.performed += Action_performedMultiple);
        tutorialMessage.Binding.ForEach(e => e.action.canceled += Action_canceledMultiple);
    }

    public void HideTutorialUI()
    {
        MessageObject.gameObject.SetActive(false);
        if(tempMessage != null)
        {
            tempMessage.Binding.ForEach(e => e.action.performed -= Action_performedMultiple);
            tempMessage.Binding.ForEach(e => e.action.canceled -= Action_canceledMultiple);
            buttonsPressedCount = 0;
            tempMessage = null;
        }
    }

    private void Action_canceledMultiple(InputAction.CallbackContext obj)
    {
        buttonsPressedCount--;
    }

    private void Action_performedMultiple(InputAction.CallbackContext context)
    {
        HandleMultiplePress();
    }

    private void HandleMultiplePress()
    {
        buttonsPressedCount++;
        if (buttonsPressedCount == tempMessage.Binding.Count)
        {
            HideTutorialUI();
        }
    }

    private string FormatUI(string message, int length)
    {
        string tempString = message;
        for (int i = 0; i < length; i++)
        {
            cleanBindingName = tempMessage.Binding[i].action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions, group: cleanInputName);
            tempString = tempString.Replace($"\"Input{i}\"", cleanBindingName);
        }
        message = tempString;
        return message;
    }
}
