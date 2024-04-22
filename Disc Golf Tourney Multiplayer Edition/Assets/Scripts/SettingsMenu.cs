using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 *  This script controls the settings menu accessible from the main menu
 * 
 */


public class SettingsMenu : MonoBehaviour
{
    [Header("Resolution Settings")]
    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        FindResolutionsAndSetDropDown();
    }

    /*
     *  This method grabs the eligible resolutions and setting them in the drop down
     */
    private void FindResolutionsAndSetDropDown()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        int counter = 0;

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);

            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = counter;
            }

            counter++;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }


    /*
     *  This method is for the full screen toggle
     */
    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    /*
     *  This method is for the resolution drop down
     */
    public void ChangeResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /*
     *  This method is for the vsync toggle
     */
    public void SetVsync(bool value)
    {
        if (value)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }
}
