using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectDistribution : MonoBehaviour
{
    [SerializeField] private GameObject _treePrefabA;
    [SerializeField] private GameObject _treePrefabB;
    [SerializeField] private GameObject _shrubPrefabA;
    [SerializeField] private GameObject _shrubPrefabB;
    [SerializeField] private GameObject _rockPrefabA;
    [SerializeField] private GameObject _rockPrefabB;
    [SerializeField] private GameObject _shipPrefab;


    System.Random rand;


    /// <summary>
    /// Algorithm For Spawning Trees | Rocks | Shrubs | Ships 
    /// <br> Range Should Be The Same As Map Size </br>
    /// </summary>
    /// <param name="range"></param>
    public void GenerateTrees(int range, int seed, int totalTrees, int uniformity)
    {
        //reset values when recalled;
        rand = new System.Random(seed);
        int currentAmount = 0;
        int amount = totalTrees;
        int numberOfCandidates = uniformity;
    List<Vector3> points = new List<Vector3>();

        //makes the first point set where ever
        float minDist = float.MaxValue;
        float currentDist = 0;

        //stores all distances between candidates
        float[] distances = new float[numberOfCandidates];

        while (currentAmount < amount)
        {
            currentAmount++;
            Vector3[] candidates = new Vector3[numberOfCandidates];

            //creating candidates
            for (int i = 0; i < numberOfCandidates; i++)
            {
                candidates[i] = new Vector3((float)rand.Next(-(range/2), (range/2) + 1), 0, (float)rand.Next(-(range/2), (range/2) + 1));
            }

            //iterates through for each candidate made, storing the one that has the most distance from the closest placed point until we have the lowest one
            for (int Candidate = 0; Candidate < numberOfCandidates; Candidate++)
            {
                minDist = float.MaxValue;
                foreach (Vector3 point in points)
                {
                    currentDist = Mathf.Abs(Vector2.Distance(candidates[Candidate], point));
                    if (currentDist < minDist)
                    {
                        minDist = currentDist;
                    }
                    distances[Candidate] = minDist;
                }
            }

            //checks for the best candidate and stores it ready for placement
            int correctCandidate = distances.ToList().IndexOf(distances.Max());
            points.Add(candidates[correctCandidate]);


            //convert world pos to grid[x,y]
                    //world pos is float, cast to int(truncates decimal) then add (range/2)
                    //grab value at that pos and check it against height of regions only place if wihtin region bounds (pass in regions)
                    //if within bounds, make the y pos height otherwise skip



            //places the saved candidates
            Instantiate(_treePrefabA, candidates[correctCandidate], Quaternion.identity, this.transform);
        }
    }
}
