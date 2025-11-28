using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KochLSystem : MonoBehaviour
{
    public class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;

        public void SetValues(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    private TransformInfo TI;

    private string currentString;
    public string axiom;
    public string[] laws;
    public int iterations;
    [SerializeField] private float length;
    [SerializeField] private float angle;

    [SerializeField] private Dictionary<char, string> rules = new Dictionary<char, string>();

    public GameObject lineRendererObject;

    private void Awake()
    {
        foreach (string law in laws)
        {
            string[] l = law.Split("->");
            rules.Add(l[0][0], l[1]);
        }
        Debug.Log(rules);

        currentString = axiom;

        GenerateLSystem();

        DrawLSystem();
    }

    private void GenerateLSystem()
    {
        for (int i = 0; i < iterations; i++)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }
            currentString = sb.ToString();
        }
        Debug.Log(currentString);
    }

    
    private void DrawLSystem()
    {
        List<Vector3> positions = new List<Vector3>();

        //protective programming
        //resets pos and rot before moving

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        positions.Add(transform.position); //adding the starting position

        foreach(char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    this.gameObject.transform.Translate(Vector3.forward * length);
                    positions.Add(transform.position); //only need to add a pos when we go to a new one
                    break;

                case '+':
                    transform.Rotate(Vector3.up * angle);
                    break;
                
                case '-':
                    transform.Rotate(Vector3.up * -angle);
                    break;

                default:
                    break;
            
            }
        }

        //transform.position = Vector3.zero; //we start at 0,0,0 so resets pos back to start point
        lineRendererObject.GetComponent<LineRenderer>().positionCount = positions.Count;
        lineRendererObject.GetComponent<LineRenderer>().SetPositions(positions.ToArray());

        //reset so it goes back to origin
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
   
}



