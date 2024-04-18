using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CourseController : MonoBehaviour
{
    [Header("Course Data")]
    public CourseData currentCourse;
    public LevelNames currentLevelName;
    public int numHoles;
    public GameObject[] holes;
    public int currentHole = 0;

    [Header("Debug Values")]
    public bool showDebugMessages;

    private void Start()
    {
        // Load the course data from the scriptable object
        LoadCourseFile();
    }

    /*
     *  The following method loads the course data from the scriptable object
     */
    private void LoadCourseFile()
    {
        OutputDebugMessage("Loading the current course scriptable object...", "green", false);

        if (holes == null || holes.Length == 0)
        {
            OutputDebugMessage("The current course has either no holes or is missing its references", "red", true);
        }
    }

    /*
     * The following method starts the course
     */
    private void StartCourse()
    {

    }

    /*
     *  The following method moves to the next hole
     */
    private void GoToNextHole()
    {
        currentHole++;

        if (currentHole > numHoles)
        {
            Debug.Log("Course Over");
        }
    }

    /*
     * The following method is used to determine if debugging is currently activated and then
     * outputs the given message
     */
    private void OutputDebugMessage(string message, string color, bool isError)
    {
        if (showDebugMessages)
        {
            if (isError)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.Log($"<color={color}>{message}</color>");
            }
        }

    }
}
