using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayInfo : MonoBehaviour
{
    public GameObject infoPanel; // Assign a UI panel in the Inspector
    public TextMeshProUGUI infoText; // Assign a Text component in the Inspector

    public void ShowInfo(string data)
    {
        infoText.text = data;
        infoPanel.SetActive(true);
    }
}
