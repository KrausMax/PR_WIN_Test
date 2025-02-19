using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;

public class DisplayInfo : MonoBehaviour
{
    public GameObject infoPanel; // Assign a UI panel in the Inspector
    public TextMeshPro infoText; // Assign a Text component in the Inspector
    public bool isActive;

    void Start()
    {
        InvokeRepeating("ToggleText", 1.0f, 0.1f);
    }

    public void ShowInfo(string data)
    {
        infoText.text = data;
        infoPanel.SetActive(true);
        isActive = true;
    }

    void ToggleText()
    {
        /*
        if (CheckForRightTriggerInput())
        {
            infoText.text = "input pressed";
        }
        */
        if (CheckForRightPrimaryInput())
        {
            if(isActive){
                infoPanel.SetActive(false);
                isActive = false;
            }
            else{
                infoPanel.SetActive(true);
                isActive = true;
            }
            
        }
    }


    bool CheckForRightTriggerInput()
    {
        // Get the right-hand controller device
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check if the trigger button is pressed
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            return true;
        }

        return false;
    }

    bool CheckForRightPrimaryInput()
    {
        // Get the right-hand controller device
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check if the trigger button is pressed
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressed) && buttonPressed)
        {
            return true;
        }

        return false;
    }
}
