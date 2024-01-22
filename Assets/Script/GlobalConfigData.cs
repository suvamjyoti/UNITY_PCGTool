
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.IO;

public  class GlobalConfigData : MonoBehaviour 
{
    private string LogChannel = "GlobalCOnfigData";

    public int globalEvaluationIteration { get; private set; }              //this will be increased after each tiles propogation is complete


    [SerializeField] private List<TileObjectController> _tileObjectList;

    [SerializeField] private GameObject modelTile;
    [SerializeField] private GameObject TileObjectHolder;

    public List<TileObjectController> tileObjectList => _tileObjectList;

    private Dictionary<GameEnums.TileObjectName, TileObjectController> _tileObjectDict;
    public  Dictionary<GameEnums.TileObjectName, TileObjectController> tileObjectDict => _tileObjectDict;

    private  int[,] _levelMatrix;
    public  int[,] levelMatrix;

    public Action Vanilla_GenerationEvent;
    public Action Vanilla_ResetEvent;

    public Action Edit_StartEvent;
    public Action Edit_GenerationEvent;
    public Action Edit_ResetEvent;



    public GameEnums.ToolMode currentToolMode { get; private set; }

    [SerializeField] private MainUIController uiController;


    #region UserInterface

    public int mapLength { get; private set; }
    public int mapBreadth { get; private set; }
    public bool isBorderTilesEnabled { get; private set; }
    public bool isVisualisationOn { get; private set; }
    public int visualisationSpeed { get; private set;}

    public int waterProbability { get; private set; }
    public int dirtProbability { get; private set; }
    public int grassProbability { get; private set; }


    #endregion

    #region SINGELTON

    private static GlobalConfigData _instance = null;

    public static GlobalConfigData GetInstance() 
    {
        if(_instance == null) 
        {
            _instance = FindObjectOfType<GlobalConfigData>();
            if(_instance is null) 
            {
               GameObject obj = Utils.InstantiateEmptyObject("GlobalConfigData");
                _instance = obj.AddComponent<GlobalConfigData>();
                DontDestroyOnLoad(obj);
                return _instance;
            }
        }

        return _instance;
    }

    #endregion


    private void Start()
    {
        currentToolMode = GameEnums.ToolMode.VanillaMode;
        _tileObjectList = new List<TileObjectController>();

        LoadDataModel();

        _tileObjectDict = new Dictionary<GameEnums.TileObjectName, TileObjectController>();

        foreach (TileObjectController tile in _tileObjectList)
        {
            tile.metaData.SetAttachmentValue();
            _tileObjectDict.Add(tile.metaData.name, tile);
        }
        
    }

    private void InitialiseGlobalValues()
    {
        GetValuesFromUI();

        globalEvaluationIteration = 0;



        //??
        //TODO: here read from Data file



        _levelMatrix = new int[mapLength, mapBreadth];
    }

    private void GetValuesFromUI()
    {
        if(currentToolMode == GameEnums.ToolMode.VanillaMode)
        {
            mapLength = int.Parse(uiController.V_lengthFeild.text);
            mapBreadth = int.Parse(uiController.V_breadthFeild.text);
            isBorderTilesEnabled = uiController.V_BorderToggle.isOn;
            isVisualisationOn = uiController.V_VisualisationToggle.isOn;
            visualisationSpeed = (int) ( (uiController.V_VisualisationDelaySlider.value + 0.001) * 1000 );
            waterProbability = (int)((uiController.V_WaterPriority.value)*10 + 1);
            dirtProbability = (int)((uiController.V_DirtPriority.value)*10 + 1);
            grassProbability = (int)((uiController.V_LandPriority.value)*10 + 1);
        }
        else
        {

           mapLength = int.Parse(uiController.E_lengthFeild.text);
           mapBreadth = int.Parse(uiController.E_breadthFeild.text);
           isBorderTilesEnabled = uiController.E_BorderToggle.isOn;
            waterProbability = (int)((uiController.V_WaterPriority.value)*10 + 1);
            dirtProbability = (int)((uiController.V_DirtPriority.value)*10 + 1);
            grassProbability = (int)((uiController.V_LandPriority.value)*10 + 1);
            visualisationSpeed = 1;
        }
    }

    public void IncreaseGlobalEvaluationIteration(int value = 1)
    {
        globalEvaluationIteration += value;
    }
    

    public void StartVanillaGeneration()
    {
        InitialiseGlobalValues();

        Vanilla_GenerationEvent?.Invoke();
    }

    public void StartEditGeneration()
    {
        Edit_GenerationEvent?.Invoke();
    }

    public void StartEditMode()
    {
        InitialiseGlobalValues();

        Edit_StartEvent?.Invoke();
    }

    public void ResetEditMode()
    {
        ResetUI();
        Edit_ResetEvent?.Invoke();
    }

    public void ResetVanillaMode()
    {
        ResetUI();

        Vanilla_ResetEvent?.Invoke();
    }




    public void SetToolMode(GameEnums.ToolMode mode)
    {
        currentToolMode = mode;
    }

    private void ResetUI()
    {
        //TODO: change UI element to UimainController

        if(currentToolMode == GameEnums.ToolMode.VanillaMode)
        {

            uiController.V_lengthFeild.text = mapLength.ToString();
            mapLength = 7;

            uiController.V_breadthFeild.text = mapBreadth.ToString();
            mapBreadth = 7;

            visualisationSpeed = 1;
            uiController.V_VisualisationDelaySlider.value = 0f;

        }
        else
        {
            uiController.E_lengthFeild.text = mapLength.ToString();
            mapLength = 7;

            uiController.E_breadthFeild.text = mapBreadth.ToString();
            mapBreadth = 7;

            visualisationSpeed = 1;
        }

        
    }

    private void ReadDataFile()
    {

    }

    private void LoadDataModel()
    {
        // Open a file dialog to select the text file
        string filePath = EditorUtility.OpenFilePanel("Load Tile Data", "", "txt");

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("File open operation cancelled.");
            return;
        }

        // Read the file and load tile data
        using (StreamReader reader = new StreamReader(filePath))
        {

            while (!reader.EndOfStream)
            {
                string tileData = reader.ReadLine();

                // Split the tileData using the specified delimiter
                string[] dataParts = tileData.Split('$');

                int i = 0;

                foreach (string data in dataParts)
                {
                    string[] actualData = data.Split('@');

                    if (actualData.Length == 6) // Ensure all parts are present
                    {
                        GameEnums.TileObjectName tileName = (GameEnums.TileObjectName)Enum.Parse(typeof(GameEnums.TileObjectName), actualData[0]);
                        string imagePath = actualData[1];
                        int right = int.Parse(actualData[2]);
                        int top = int.Parse(actualData[3]);
                        int left = int.Parse(actualData[4]);
                        int bottom = int.Parse(actualData[5]);

                        // Load the texture based on the asset path
                        Texture image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath);

                        //adding this tile data to dictionary
                        AddTileData(tileName, image, right, top, left, bottom,i);

                        i++;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid data format: " + tileData);
                    }

                }


            }
        }

        Debug.Log("Tile data loaded from: " + filePath);
    }

    private void AddTileData(GameEnums.TileObjectName tileName, Texture image, int right, int top, int left, int bottom,int index)
    {
        //create metadata for new Tile
        MetaDataModel nMetaData = new MetaDataModel(tileName, right, top, left, bottom);

        //create a new Gameobject and instantiate it
        GameObject obj = Instantiate(modelTile);

        obj.GetComponent<SpriteRenderer>().sprite = ConvertTextureToSprite((Texture2D)image);

        obj.transform.parent = TileObjectHolder.transform;

        obj.GetComponent<TileObjectController>().SetValues(nMetaData);

        // Add the loaded data to your tileObjectcontroller list
        tileObjectList.Add(obj.GetComponent<TileObjectController>());
    }


    Sprite ConvertTextureToSprite(Texture2D texture)
    {
        // Create a Sprite using the Texture2D
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        return sprite;
    }

}
