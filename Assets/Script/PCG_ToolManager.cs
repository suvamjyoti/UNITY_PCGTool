using UnityEngine;

//the manager it controlls the genration and the placement ofn th4e tile in the space 
//it will be the centeral controlliong unit and will be responsible for controlling the entire thing
public class PCG_ToolManager : MonoBehaviour
{
    private const string LogChannel = "TileManager";

    public WaveFunctionCollapse waveFunctionCollapse { get; private set;}

    private VisualisationManager visualizationManager;

    private Camera MainCamera;


    #region SINGELTON

    private static PCG_ToolManager _instance = null;

    public static PCG_ToolManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<PCG_ToolManager>();
            if (_instance is null)
            {
                GameObject obj = Utils.InstantiateEmptyObject("GlobalConfigData");
                _instance = obj.AddComponent<PCG_ToolManager>();
                DontDestroyOnLoad(obj);
                return _instance;
            }
        }

        return _instance;
    }

    #endregion

    private void Awake()
    {
        MainCamera = Camera.main;
        waveFunctionCollapse =  this.GetComponent<WaveFunctionCollapse>();
        visualizationManager = this.GetComponent<VisualisationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //generation will start when this event is trigger from UI
        GlobalConfigData.GetInstance().Vanilla_GenerationEvent += Vanilla_GenerateLevel;

        //EditMode will be initialised with this
        GlobalConfigData.GetInstance().Edit_StartEvent += StartEditMode;

        GlobalConfigData.GetInstance().Edit_GenerationEvent += Edit_GenerateLevel;
    }


    private async void StartEditMode()
    {
        if (GlobalConfigData.GetInstance().isBorderTilesEnabled)
        {
            //first we need to fill the border with empty tile to make it look nice
            await waveFunctionCollapse.FillTheBorder(GlobalConfigData.GetInstance().mapLength, GlobalConfigData.GetInstance().mapBreadth);
        }

        MainCamera.GetComponent<CameraController>().MoveCameraToTopView(new Vector3((GlobalConfigData.GetInstance().mapLength/2)- 2,0,GlobalConfigData.GetInstance().mapBreadth/2), GlobalConfigData.GetInstance().mapBreadth);
    }

    private async void Edit_GenerateLevel()
    {
        //generate level
        await waveFunctionCollapse.GenerateLevel(GlobalConfigData.GetInstance().mapLength, GlobalConfigData.GetInstance().mapBreadth);
    }

    //Generation part
    private async void Vanilla_GenerateLevel() 
    {

        if (GlobalConfigData.GetInstance().isBorderTilesEnabled)
        {
            //first we need to fill the border with empty tile to make it look nice
            await waveFunctionCollapse.FillTheBorder(GlobalConfigData.GetInstance().mapLength, GlobalConfigData.GetInstance().mapBreadth);
        }

        //generate level
        await waveFunctionCollapse.GenerateLevel(GlobalConfigData.GetInstance().mapLength, GlobalConfigData.GetInstance().mapBreadth);

    }



    public GameEnums.TileObjectName GetTileName(int tileNumber) 
    {
        return (GameEnums.TileObjectName)tileNumber;
    }

    public int GetTileNumber(TileObjectController tileObjectController)
    {
        return (int)tileObjectController.metaData.name;
    }


    public void CollapseALocation(int length, int breadth, GameEnums.TileObjectName tilename)
    {
        waveFunctionCollapse.CollapseALocation(length, breadth, GlobalConfigData.GetInstance().tileObjectDict[tilename]);
    }


    public void SaveLevelMatrix()
    {
        waveFunctionCollapse.SaveLevelMatrixData();
    }
}
