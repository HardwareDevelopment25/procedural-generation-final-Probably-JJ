using UnityEngine;

public class CameraControls : MonoBehaviour
{
    Transform camTransform;
    GameObject mapCentre;
    [SerializeField]private TerrainMesh _terrainMesh;
    public float speed = 1.0f;

    float camY;

    private void Start()
    {
        _terrainMesh = GameObject.FindWithTag("LogicHolder").GetComponent<TerrainMesh>();
        mapCentre = GameObject.FindWithTag("LogicHolder");
        camTransform = this.transform;
    }

    void FixedUpdate()
    {
        camY = camTransform.position.y;

        if (Input.GetKey(KeyCode.A))
        {
            camTransform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            camTransform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        




        if(camY <= 100 && camY > 1)
        {
            if (Input.GetKey(KeyCode.W))
            {
                camTransform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                camTransform.Translate(Vector3.back * speed * Time.deltaTime);
            }
        }
        else if (camY < 30)
        {
            camY = 31;
        }
        else if (camY > 100)
        {
            camY = 99;
        }



        camTransform.position =new Vector3(camTransform.position.x, camY, camTransform.position.z);

        camTransform.LookAt(mapCentre.transform);
    }
}
