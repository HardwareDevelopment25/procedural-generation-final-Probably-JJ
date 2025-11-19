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
        Stack<TransformInfo> _transformInfo = new Stack<TransformInfo>();
        List<Vector3> positions = new List<Vector3>();

        foreach(char c in currentString)
        {
            switch (c)
            {
                case 'F':
                case 'G':
                    transform.Translate(Vector3.forward * length);
                    break;

                case '+':
                    transform.Rotate(Vector3.forward * angle);
                    break;
                
                case '-':
                    transform.Rotate(Vector3.forward * -angle);
                    break;

                case '[':
                    TI.SetValues(transform.position, transform.rotation);
                    _transformInfo.Push(TI);
                    break;

                case ']':
                    TI = _transformInfo.Pop();

                    transform.position = TI.position;
                    transform.rotation = TI.rotation;
                    break;

                default:
                    break;
            
            }
            positions.Add(transform.position);
        }

        transform.position = Vector3.zero; //we start at 0,0,0 so resets pos back to start point
        lineRendererObject.GetComponent<LineRenderer>().positionCount = positions.Count;
        lineRendererObject.GetComponent<LineRenderer>().SetPositions(positions.ToArray());
    }
   

}



