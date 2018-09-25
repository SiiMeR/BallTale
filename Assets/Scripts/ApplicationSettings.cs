using System;
using UnityEngine;

/// <summary>
/// A general class to set various settings related to the application
/// </summary>
public class ApplicationSettings : MonoBehaviour
{
    private void Awake()
    {
        
        QualitySettings.vSyncCount = 0; // TODO : Make it possible to change vSync settings
        Application.targetFrameRate = (int) FramerateTarget.MAX_60; // TODO : Make it so you can set this in settings

    }

    private void Update() // hidden options
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var enumValues = (int[]) Enum.GetValues(typeof(FramerateTarget));

            var targetFrameRate = Application.targetFrameRate;
            var currentFrameRate = Array.IndexOf(enumValues, targetFrameRate) + 1;

            Application.targetFrameRate = (enumValues.Length == currentFrameRate) ? enumValues[0] : enumValues[currentFrameRate];
        }
    }
}

public enum FramerateTarget
{
    MAX_30 = 30,
    MAX_60 = 60,
    MAX_120 = 120,
    MAX_144 = 144,
    UNLIMITED = 1000, // not really unlimited but basically
}