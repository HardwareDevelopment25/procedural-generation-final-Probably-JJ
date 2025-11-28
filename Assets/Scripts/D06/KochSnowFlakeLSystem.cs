using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*
 * THIS SCRIP WAS GENERATED WITH COPILOT AI TO HELP ASSIST WITH DEBUGGING WHY MY ACTUAL SCRIPT WAS NOT WORKING
 * 
 * IT HELPED ME REALISE THAT I WAS ROTATING AND MOVING ALONG THE SAME AXIS
 * 
 * I CHANGED MY CODE ACCORDINGLY AND IT FIXED IT 
 * 
 * ALL COMMENTS WERE ADDED BY ME AS I WAS LEARNING HOW THE SCRIPT WORKS
 */


public class AIKochSnowFlake : MonoBehaviour
{
    public class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public string axiom; //F--F--F <- Koch Snowflake Axiom
    public string[] laws; //F->F+F--F+F <- Koch Snowflake Law
    public int iterations = 3;

    [SerializeField] private float length = 5f;
    [SerializeField] private float angle = 60f; //needs to be 60 for SnowFlake
    public LineRenderer lineRenderer;

    private string currentString;
    private Dictionary<char, string> rules = new Dictionary<char, string>();

    private void Awake()
    {
        foreach (string law in laws)
        {
            string[] l = law.Split("->");
            rules.Add(l[0][0], l[1]);
        }

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
        var positions = new List<Vector3>();

        // Start at 0,0,0
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        //store start point
        positions.Add(pos);

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                case 'G':
                    //Move forward
                    pos += rot * Vector3.right * length;
                    positions.Add(pos);
                    break;

                case '+':
                    //Turn left
                    rot *= Quaternion.AngleAxis(angle, Vector3.forward);
                    break;

                case '-':
                    //Turn right
                    rot *= Quaternion.AngleAxis(-angle, Vector3.forward);
                    break;
            }
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}