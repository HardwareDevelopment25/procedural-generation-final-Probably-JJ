using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectDistribution : MonoBehaviour
{
    [SerializeField] private GameObject _treePrefabA;
    [SerializeField] private GameObject _treePrefabB;
    [SerializeField] private GameObject _rockPrefabA;
    [SerializeField] private GameObject _rockPrefabB;
    [SerializeField] private GameObject _shipPrefab;


    System.Random rand;


    /// <summary>
    /// Algorithm For Spawning Trees
    /// <br> Range Should Be The Same As Map Size </br>
    /// <br> Tree Range Should Have A Region Height Passed In </br>
    /// </summary>
    /// <param name="range"></param>
    public void GenerateTrees(GameObject parent, int range, int seed, int totalTrees, int uniformity, float[,] grid, float treeRangeUpper, float treeRangeLower)
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
                candidates[i] = new Vector3( (float)rand.Next(-(range/2), (range/2)), 0, (float)rand.Next(-(range/2), (range/2)) );
            }

            //iterates through for each candidate made, storing the one that has the most distance from the closest placed point until we have the lowest one
            for (int Candidate = 0; Candidate < numberOfCandidates; Candidate++)
            {
                minDist = float.MaxValue;
                foreach (Vector3 point in points)
                {
                    currentDist = Mathf.Abs(Vector2.Distance(new Vector2 (candidates[Candidate].x, candidates[Candidate].z), new Vector2 (point.x, point.z) ));
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


            //convert world pos to grid[x,y]:
            //world pos is float, cast to int(truncates decimal) then add (range/2)
            float mapHeight = grid[(int)candidates[correctCandidate].x + (range / 2), (int)candidates[correctCandidate].z + (range / 2)];

            //sets the y pos extremely high up to be appended later
            Vector3 treeGridPos = new Vector3((int)candidates[correctCandidate].x, 50, (int)candidates[correctCandidate].z);


            //grab value at that pos and check it against height of regions only place if wihtin region bounds
            //if within bounds, make the y pos height otherwise skip
            if (mapHeight > treeRangeLower && mapHeight < treeRangeUpper)
            {
                //generate random number then check if odd or even
                int randomNum = rand.Next(1, 101);
                int rem = randomNum % 2;

                //places tree a or b based on random number outcome
                if (rem == 0)
                {
                    //places the saved candidate
                    GameObject tree = Instantiate(_treePrefabA, treeGridPos, Quaternion.identity, parent.transform);

                    //shoots a ray into the mesh, sets the posistion onto the mesh
                    RaycastHit hit;
                    if (Physics.Raycast(treeGridPos, Vector3.down, out hit, 100))
                    {
                        tree.transform.position = hit.point;
                    }
                    else
                    {
                        Debug.Log("Shit's fucked mate");
                    }
                }
                else
                {
                    //places the saved candidate
                    GameObject tree = Instantiate(_treePrefabB, treeGridPos, Quaternion.identity, parent.transform);

                    //shoots a ray into the mesh, sets the posistion onto the mesh
                    RaycastHit hit;
                    if (Physics.Raycast(treeGridPos, Vector3.down, out hit, 100))
                    {
                        tree.transform.position = hit.point;
                    }
                    else
                    {
                        Debug.Log("Shit's fucked mate");
                    }
                }



            }
            else
            {
                currentAmount--;
            }
        }
    }

    /// <summary>
    /// Algorithm For Spawning Rocks
    /// <br> Range Should Be The Same As Map Size </br>
    /// <br> Rock Range Should Have A Region Height Passed In </br>
    /// </summary>
    /// <param name="range"></param>
    public void GenerateRocks(GameObject parent, int range, int seed, int totalRocks, int uniformity, float[,] grid, float rockRangeUpper, float rockRangeLower)
    {
        //reset values when recalled;
        rand = new System.Random(seed);
        int currentAmount = 0;
        int amount = totalRocks;
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
                candidates[i] = new Vector3((float)rand.Next(-(range / 2), (range / 2)), 0, (float)rand.Next(-(range / 2), (range / 2)));
            }

            //iterates through for each candidate made, storing the one that has the most distance from the closest placed point until we have the lowest one
            for (int Candidate = 0; Candidate < numberOfCandidates; Candidate++)
            {
                minDist = float.MaxValue;
                foreach (Vector3 point in points)
                {
                    currentDist = Mathf.Abs(Vector2.Distance(new Vector2(candidates[Candidate].x, candidates[Candidate].z), new Vector2(point.x, point.z)));
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


            //convert world pos to grid[x,y]:
            //world pos is float, cast to int(truncates decimal) then add (range/2)
            float mapHeight = grid[(int)candidates[correctCandidate].x + (range / 2), (int)candidates[correctCandidate].z + (range / 2)];

            //sets the y pos extremely high up to be appended later
            Vector3 rockGridPos = new Vector3((int)candidates[correctCandidate].x, 50, (int)candidates[correctCandidate].z);


            //grab value at that pos and check it against height of regions only place if wihtin region bounds
            //if within bounds, make the y pos height otherwise skip
            if (mapHeight > rockRangeLower && mapHeight < rockRangeUpper)
            {
                //generate random number then check if odd or even
                int randomNum = rand.Next(1, 101);
                int rem = randomNum % 2;

                //places tree a or b based on random number outcome
                if (rem == 0)
                {
                    //places the saved candidate
                    GameObject rock = Instantiate(_rockPrefabA, rockGridPos, Quaternion.identity, parent.transform);

                    //shoots a ray into the mesh, sets the posistion onto the mesh
                    RaycastHit hit;
                    if (Physics.Raycast(rockGridPos, Vector3.down, out hit, 100))
                    {
                        rock.transform.position = hit.point;
                    }
                    else
                    {
                        Debug.Log("Shit's fucked mate");
                    }
                }
                else
                {
                    //places the saved candidate
                    GameObject rock = Instantiate(_rockPrefabB, rockGridPos, Quaternion.identity, parent.transform);

                    //shoots a ray into the mesh, sets the posistion onto the mesh
                    RaycastHit hit;
                    if (Physics.Raycast(rockGridPos, Vector3.down, out hit, 100))
                    {
                        rock.transform.position = hit.point;
                    }
                    else
                    {
                        Debug.Log("Shit's fucked mate");
                    }
                }



            }
            else
            {
                currentAmount--;
            }
        }
    }

    /// <summary>
    /// Algorithm For Spawning Ships
    /// <br> Range Should Be The Same As Map Size </br>
    /// <br> Ship Range Should Have A Region Height Passed In </br>
    /// </summary>
    /// <param name="range"></param>
    public void GenerateShips(GameObject parent, int range, int seed, int totalShips, int uniformity, float[,] grid, float shipRangeUpper, float shipRangeLower)
    {
        //reset values when recalled;
        rand = new System.Random(seed);
        int currentAmount = 0;
        int amount = totalShips;
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
                candidates[i] = new Vector3((float)rand.Next(-(range / 2), (range / 2)), 0, (float)rand.Next(-(range / 2), (range / 2)));
            }

            //iterates through for each candidate made, storing the one that has the most distance from the closest placed point until we have the lowest one
            for (int Candidate = 0; Candidate < numberOfCandidates; Candidate++)
            {
                minDist = float.MaxValue;
                foreach (Vector3 point in points)
                {
                    currentDist = Mathf.Abs(Vector2.Distance(new Vector2(candidates[Candidate].x, candidates[Candidate].z), new Vector2(point.x, point.z)));
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


            //convert world pos to grid[x,y]:
            //world pos is float, cast to int(truncates decimal) then add (range/2)
            float mapHeight = grid[(int)candidates[correctCandidate].x + (range / 2), (int)candidates[correctCandidate].z + (range / 2)];

            //sets the y pos extremely high up to be appended later
            Vector3 shipGridPos = new Vector3((int)candidates[correctCandidate].x, 50, (int)candidates[correctCandidate].z);


            //grab value at that pos and check it against height of regions only place if wihtin region bounds
            //if within bounds, make the y pos height otherwise skip
            if (mapHeight > shipRangeLower && mapHeight < shipRangeUpper)
            {
                //places the saved candidate
                GameObject ship = Instantiate(_shipPrefab, shipGridPos, Quaternion.identity, parent.transform);

                //shoots a ray into the mesh, sets the posistion onto the mesh
                RaycastHit hit;
                if (Physics.Raycast(shipGridPos, Vector3.down, out hit, 100))
                {
                    ship.transform.position = hit.point;
                }
                else
                {
                    Debug.Log("Shit's fucked mate");
                }
                
            }
            else
            {
                currentAmount--;
            }
        }
    }

}
