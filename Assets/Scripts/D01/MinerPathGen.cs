using UnityEngine;

public class MinerPathGen : MonoBehaviour
{
    [SerializeField] private Transform minerTransform;
    [SerializeField] private Transform minerPathTransform;
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private int movesAvailable;

    private int randomChoice;

    private float upperZBound = 5.0f;
    private float lowerZBound = -5.0f;
    private float upperXBound = 5.0f;
    private float lowerXBound = -5.0f;
    private Vector3 minerVector;
    private bool hasMoved = false;
    private void Awake()
    {
        minerVector = minerTransform.position;
    }
    private void Start()
    {
        for (int i = 0; i < movesAvailable; i++)
        {
            hasMoved = false;
            do
            {
                randomChoice = Random.Range(0, 4);
                if (randomChoice == 0 && minerVector.z < upperZBound)
                {
                    minerVector.z += 1.0f;
                    hasMoved = true;
                }
                else if (randomChoice == 1 && minerVector.z > lowerZBound)
                {
                    minerVector.z -= 1.0f;
                    hasMoved = true;
                }
                else if (randomChoice == 2 && minerVector.x < upperXBound)
                {
                    minerVector.x += 1.0f;
                    hasMoved = true;
                }
                else if (randomChoice == 3 && minerVector.x > lowerXBound)
                {
                    minerVector.z -= 1.0f;
                    hasMoved = true;
                }
            } while(!hasMoved);

            minerTransform.position = minerVector;
            minerPathTransform.position = new Vector3(minerVector.x, minerVector.y - 1, minerVector.z);
            Debug.Log("Miner location: " + minerTransform.position);
            Debug.Log("Path location: " + minerPathTransform.position);
            Instantiate(pathPrefab, minerPathTransform);
        }
    }
}
