using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystems : MonoBehaviour 
{

    public string axiom = "F";

    private string currentString;

    public float angle = 45.0f;

    public int iterations = 3;

    public string[] laws; 

    [SerializeField] private Dictionary<char, string> rules = new Dictionary<char, string>();

    private void Awake()
    {
        foreach(string law in laws)
        {
            string[] l = law.Split("->");
            rules.Add(l[0][0], l[1]); //grabs what is first in a string then what ever is after

        }



        currentString = axiom;
        GenerateLSystem();
    }

    private void GenerateLSystem()
    {
        for(int i = 0; i < iterations; i++)
        {
            StringBuilder sb = new StringBuilder();

            foreach(char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }
            currentString = sb.ToString();
        }
        Debug.Log(currentString);
    }
}
