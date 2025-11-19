using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RandomDistribution : MonoBehaviour
{
    public int totalCandidates = 5; //how many points are generated to be compaired to existing ones
    public int totalPoints = 50; //final amount of placed objects
    public int range = 100;

    List<Vector2> points = new List<Vector2>();
    Vector3[] candidates;

    void GeneratePoints()
    {
        
    }
}

/*
 *  start with a point and store in array
 *  generate totalCandidates amount of candidates 
 *  store pos in array
 *  compair distances to stored distances
 *  store the one with highest distance
 * 
 *  iterate through each candidate compairing against each placed point, overriding its distance with the smallest one it comes across each time
 *  iterate though each candidate compairing against each candidate, the one with the greatest distance is the one to be stored
 * 
 * 
 */