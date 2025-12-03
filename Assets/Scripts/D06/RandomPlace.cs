using UnityEngine;

public class RandomPlace : MonoBehaviour
{
    public int range;
    public int iterations;
    System.Random rand;

    void Start()
    {
        rand = new System.Random();

        for (int i = 0; i < iterations; i++)
        {
            SpawnSphere(range);
        }
    }

    public void SpawnSphere(int range)
    {
        int posX = range, posZ = range;
        posX = rand.Next(0, range + 1);
        posZ = rand.Next(0, range + 1);

        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.position = new Vector3(posX, 0, posZ);
    }
}
