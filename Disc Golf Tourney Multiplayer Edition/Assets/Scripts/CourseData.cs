using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CourseData", menuName = "Course Related Scriptable Objects/CourseData", order = 1)]
public class CourseData : ScriptableObject
{
    [Header("Course Details")]
    public LevelNames courseName;
    public int numHoles;
    public int parTotal;
    public HoleData[] holes;
}

