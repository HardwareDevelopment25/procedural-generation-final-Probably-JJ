using UnityEditor;
using UnityEngine;

//was causing build errors
/*
[CustomEditor(typeof(MazeSpawner2D3D))]

public class MazeSpawner2D3DEditor : Editor
{
    //override the deaful inspector GUI Rendering 

    public override void OnInspectorGUI()
    {
        //cast target ibj to MazeSpawner2D3D to access fields
        MazeSpawner2D3D mazeSpawner = (MazeSpawner2D3D)target;
        GUILayout.Label("--Configure Maze--", EditorStyles.largeLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Draw deafult inspector UI for all serialised fields

        if (DrawDefaultInspector())
        {

            if (mazeSpawner.size < 2)
                mazeSpawner.size = 2;
            else if (mazeSpawner.size > 256)
                mazeSpawner.size = 256;

            mazeSpawner.GenImageOfNewMaze();
        }

        //adds a button to regen maze
        if (GUILayout.Button("Generate New Maze"))
            mazeSpawner.GenImageOfNewMaze();
    }
}

//define a custom editor window for complete mazes

public class MazeSpawnerWindow : EditorWindow
{
    public int initialMazeSize = 32;

    //generates menu item inside unity project wide
    //[MenuItem("Tools/Generate Maze By Size")]

    public static void ShowWindow()
    {
        GetWindow<MazeSpawnerWindow>();
    }

    //decorate window and create new game object with maze

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("Maze Generator, will Create a Maze within your scene");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("Configure Maze:");

        //create input field to choose default size
        initialMazeSize = EditorGUILayout.IntField("Size: ", initialMazeSize);

        //if button pressed, run thread to generate maze in editor
        if(GUILayout.Button("Generate"))
        {
            GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newGameObject.name = initialMazeSize + "x" + initialMazeSize + "Generated Maze";
            Undo.RegisterCreatedObjectUndo(newGameObject, "Undo Maze");

            //create new material and add shader
            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.color = Color.white;

            //get renderer and put material onto obj
            newGameObject.GetComponent<Renderer>().material = mat;

            //add script to generate Maz
            //adding component returns a reference too
            MazeSpawner2D3D mazeSpawner = newGameObject.AddComponent<MazeSpawner2D3D>();


            //run generation of maze
            mazeSpawner.size = initialMazeSize;
            mazeSpawner.GenImageOfNewMaze();
        }
    }
}
*/