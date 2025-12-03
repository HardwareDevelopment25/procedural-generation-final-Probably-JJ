using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventLogic : MonoBehaviour
{
    [SerializeField] private TerrainMesh _terrainMesh;
    public TextMeshProUGUI lodText;
    public TextMeshProUGUI mapSizeText;
    public TextMeshProUGUI heightMultText;
    public Slider heightMultSlider;
    private int mapCase = 3;

    private void Awake()
    {
        _terrainMesh = GameObject.FindWithTag("LogicHolder").GetComponent<TerrainMesh>();
        lodText.text = ("Smoothing Level: " + _terrainMesh.levelOfDetail);
        mapSizeText.text = ("Map Size: " + _terrainMesh.size);
        heightMultText.text = ("Mountain Height: " + _terrainMesh.heightMult);

    }
    public void UpdateSeed(string input)
    {
        _terrainMesh.seed = int.Parse(input);

        _terrainMesh.seed = Mathf.Abs(_terrainMesh.seed);

        if (_terrainMesh.seed > 999999999)
        {
            _terrainMesh.seed = 999999999;
        }

    }

    #region Level Of Detail

    public void IncreaseLOD()
    {
        //level of detail is inverse to variable
        //higher value means more optimisation
        _terrainMesh.levelOfDetail += 1;
        if (_terrainMesh.levelOfDetail > 6)
        {
            _terrainMesh.levelOfDetail = 6;
        }
        lodText.text = ("Smoothing Level: " + _terrainMesh.levelOfDetail);
    }
    public void DecreaseLOD()
    {
        _terrainMesh.levelOfDetail -= 1;
        if(_terrainMesh.levelOfDetail < 1)
        {
            _terrainMesh.levelOfDetail = 1;
        }
        lodText.text = ("Smoothing Level: " + _terrainMesh.levelOfDetail);
    }

    #endregion

    #region Map Size

    public void IncreaseMapSize()
    {
        mapCase++;
        if(mapCase > 4)
        {
            mapCase = 4;
        }
        UpdateMapSize(mapCase);
    }

    public void DecreaseMapSize()
    {
        mapCase--;
        if (mapCase < 0)
        {
            mapCase = 0;
        }
        UpdateMapSize(mapCase);
    }

    private void UpdateMapSize(int _case)
    {
        switch (_case)
        {
            case 1:
                _terrainMesh.size = 64;
                break;

            case 2:
                _terrainMesh.size = 128;
                break;

            case 3:
                _terrainMesh.size = 256;
                break;

            case 4:
                _terrainMesh.size = 512;
                break;
                
            default:
                break;

        }
        mapSizeText.text = ("Map Size: " + _terrainMesh.size);
    }

    #endregion

    public void UpdateHeightMult()
    {
        _terrainMesh.heightMult = (int)heightMultSlider.value;
        heightMultText.text = ("Mountain Height: " + _terrainMesh.heightMult);
    }

}
