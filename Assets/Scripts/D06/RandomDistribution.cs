//using JetBrains.Annotations;
//using NUnit.Framework;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
//using UnityEngine.VFX;

public class RandomDistribution : MonoBehaviour
{
    public int range = 100;
    public int amount = 3;
    public int numberOfCandidates = 4;

    List<Vector3> points = new List<Vector3>();

    [SerializeField] GameObject mSphere;

    private int randx;
    private int randz;
    private int currentAmount = 0;

    System.Random rand;



    void Start()
    {
        rand = new System.Random();

        Vector3 spherepos = new Vector3(rand.Next(0, range + 1), rand.Next(0, range + 1), 0);
        Instantiate(mSphere, spherepos, Quaternion.identity);
        points.Add(spherepos);

        //StartCoroutine(GeneratePoints());

        //spawnSphere(range, numberOfCandidates);
    }


    public void spawnSphere()
    {

        randx = rand.Next(0, range + 1);
        randz = rand.Next(0, range + 1);

        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);


        Sphere.transform.position = new Vector3(randx, 0, randz);


    }





    public void GeneratePoints()
    {

        float minDist = float.MaxValue;
        float currentDist = 0;
        float[] distances = new float[numberOfCandidates];

        while (currentAmount < amount)
        {
            currentAmount++;
            Vector3[] candidates = new Vector3[numberOfCandidates];

            for (int i = 0; i < numberOfCandidates; i++)
            {
                candidates[i] = new Vector3((float)rand.Next(0, range + 1), (float)rand.Next(0, range + 1), 0);
            }
            for (int Candidate = 0; Candidate < candidates.Length; Candidate++)
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

            int correctCandidate = distances.ToList().IndexOf(distances.Max());
            points.Add(candidates[correctCandidate]);
            Instantiate(mSphere, candidates[correctCandidate], Quaternion.identity, this.transform);
            Debug.Log(currentAmount + " " + amount);
        }
    }

}