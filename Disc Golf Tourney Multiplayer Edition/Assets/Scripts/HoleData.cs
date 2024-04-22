using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoleData", menuName = "Course Related Scriptable Objects/HoleData", order = 2)]
public class HoleData : ScriptableObject
{
    public Vector3 startingPosition;
    public string holeName;
    public LevelNames associatedLevel;
    public int parScore;
}
