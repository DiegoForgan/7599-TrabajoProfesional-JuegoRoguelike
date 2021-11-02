using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new RandomWalkParameters",menuName = "PCG/RandomWalkData")]
public class RandomWalkData : ScriptableObject
{
    public int iterations = 10;
    public int walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
