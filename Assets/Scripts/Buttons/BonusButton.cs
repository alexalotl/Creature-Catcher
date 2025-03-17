using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    // Declares variable
    public int gemsNeeded;

    private Text[] buttonTextList;

    // Lists how many gems are needed for the button to be available
    void Start()
    {
        buttonTextList = GetComponentsInChildren<Text>();
        foreach (Text buttonText in buttonTextList)
        {
            if (buttonText.name == "GemsNeeded")
            {
                buttonText.text = gemsNeeded.ToString();
            }
        }
    }
}
