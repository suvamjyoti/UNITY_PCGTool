

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static GameEnums;

#region CUSTOM_DATA_STRUCTURES
public struct TileLocation
{
    public int xAxis;
    public int zAxis;

    public TileLocation(int xAxis,int yAxis)
    {
        this.xAxis = xAxis;
        this.zAxis = yAxis;
    }

    public string GetLocationinString() 
    {
        return "xAxis:  " + xAxis.ToString() + " , " + "zAxis:  " + zAxis.ToString();
    }
};

public class UnitObject
{
    private const string LogChannel = "UnitObject"; 

    public List<TileObjectController> domain;                                                                //possibole values that a unit can take
    public TileObjectController collapsedTileValue { get; private set; }                                                                     //final value of unit
    
    public int entropy;                                                                  
    public TileLocation unitLocation;                                                       //the location of unit in matrix
    public bool isInEvaluationList;
    public bool hasCollapsed { get; private set; }
    public GameEnums.Rotations rotationValue { get; private set;}

    public int evaluationIteration { get; private set;}

    public UnitObject(TileLocation tileLocation)
    {
        //TODO:modify this to accomodate user defined numbers
        domain = new List<TileObjectController>(GlobalConfigData.GetInstance().tileObjectList);
        collapsedTileValue = null;
        entropy = 5;
        unitLocation = tileLocation;
        isInEvaluationList = false;
        hasCollapsed = false;
        evaluationIteration = 0;
    }

    public int GetEntropy()
    {
        return domain.Count; 
    }

    public void IncreaseEvaluationIteration()
    {
        evaluationIteration += 1;
    }

    public void SetValue(TileObjectController value,GameEnums.Rotations rotation) 
    {
        if (value.metaData is null)
        {
            WFCDebugLogger.logError(LogChannel, "Trying to set Null value");
            return;
        }

        //_value can only be changes via this method
        //helps in debugging 
        this.collapsedTileValue = value;

        //also set the rotation value
        this.rotationValue = rotation;

        //once value is set that is the unit has collapsed
        hasCollapsed = true;
    }
}

# endregion

public class WaveFunctionCollapse : MonoBehaviour
{
    private const string LogChannel = "WaveFunctionCollapse";
    private const int SecondsToWaitForVisualisatioin = 1;

    private Queue<UnitObject> _evaluationCheckList;
    public UnitObject[,] _levelMatrix { get; private set; }

    public int _levelLength { get; private set; }
    public int _levelBreadth { get; private set; }

    public VisualisationManager visualizationManager { get; private set; }
    protected TileManager tileManager;

    private WFCRules wFCRules;

    //this is a dictionary which returns a 
    //dictionary of neighbours for a given unitObject
    //which can be used to find specific neighbour using neighbourType as key
    public Dictionary<TileLocation, Dictionary<neighbourType, UnitObject>> NeighbourDictionaryAllTile { get; private set; }

    [SerializeField] private GameObject _levelGameObject;

    private void Awake()
    {
        tileManager = GetComponent<TileManager>();
        visualizationManager = GetComponent<VisualisationManager>();
        wFCRules = GetComponent<WFCRules>();
        
        //Create a neighbour dictionary at the start
    }

    private void Start()
    {
        GlobalConfigData.GetInstance().Vanilla_ResetEvent += ResetGeneration;
        GlobalConfigData.GetInstance().Edit_ResetEvent += ResetGeneration;
    }

    #region INITIAL_FUNCTION

    public void CreateEmptyLevleMatrix()
    {
        //create a emptyLevelMatrix
        for (int i = 0; i < _levelLength; i++)
        {
            for (int j = 0; j < _levelBreadth; j++)
            {
                _levelMatrix[i, j] = new UnitObject(new TileLocation(i, j));
            }
        }

        WFCDebugLogger.log(LogChannel, "Empty _LevelMatrix created");
    }



    private void CreateCompleteNeighbourDictionary()
    {
        for (int i = 0; i < _levelLength; i++)
        {
            for (int j = 0; j < _levelBreadth; j++)
            {
                TileLocation currentTileLocation = new TileLocation(i, j);
                NeighbourDictionaryAllTile.Add(currentTileLocation, CreateNeighbourDicitionary(currentTileLocation));
            }
        }
    }

    private Dictionary<GameEnums.neighbourType, UnitObject> CreateNeighbourDicitionary(TileLocation oTile)
    {
        Dictionary<GameEnums.neighbourType, UnitObject> neighbourDictionary = new Dictionary<GameEnums.neighbourType, UnitObject>();


        //TODO: fix the order problem wherein 
        //there is problem if we dont add neighour in LTRB order

        //left neighbour 
        //for x=0 does not have a left
        if (oTile.xAxis != 0)
        {
            UnitObject leftUnit = _levelMatrix[oTile.xAxis - 1, oTile.zAxis];
            neighbourDictionary.Add(GameEnums.neighbourType.Left, leftUnit);
        }
        else
        {
            neighbourDictionary.Add(GameEnums.neighbourType.Left, null);
        }

        //top neighbour
        //for y=LevelBreadth doesnot have a top
        if (oTile.zAxis < _levelBreadth - 1)
        {
            //WFCDebugLogger.log(LogChannel, "found top");
            UnitObject bottomUnit = _levelMatrix[oTile.xAxis, oTile.zAxis + 1];
            neighbourDictionary.Add(GameEnums.neighbourType.Top, bottomUnit);
            // WFCDebugLogger.log(LogChannel, "added top");
        }
        else
        {
            neighbourDictionary.Add(GameEnums.neighbourType.Top, null);
        }


        //right neighbour
        //for x=LevelLength doesnot have a right
        if (oTile.xAxis < _levelLength - 1)
        {
            // WFCDebugLogger.log(LogChannel, "found right");
            UnitObject rightUnit = _levelMatrix[oTile.xAxis + 1, oTile.zAxis];
            neighbourDictionary.Add(GameEnums.neighbourType.Right, rightUnit);
            // WFCDebugLogger.log(LogChannel, "found right");
        }
        else
        {
            neighbourDictionary.Add(GameEnums.neighbourType.Right, null);
        }

        //bottom neighbour
        if (oTile.zAxis != 0)
        {
            // WFCDebugLogger.log(LogChannel,"found bottom");
            UnitObject topUnit = _levelMatrix[oTile.xAxis, oTile.zAxis - 1];
            neighbourDictionary.Add(GameEnums.neighbourType.Bottom, topUnit);
            // WFCDebugLogger.log(LogChannel, "added bottom");
        }
        else
        {
            neighbourDictionary.Add(GameEnums.neighbourType.Bottom, null);
        }

        return neighbourDictionary;

    }

    #endregion

    public async Task FillTheBorder(int length, int breadth)
    {
        //clength and cbreadth will be used to get the current active tile in consideration
        int count;
        TileObjectController randTile = null;
        InitialiseGeneration(length, breadth, out count);



        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < breadth; j++)
            {
                if (i == 0 || i == length - 1 || j == 0 || j == breadth - 1)
                {
                    //filling here with empty tile

                    //Draw the first random value tile

                    randTile = GlobalConfigData.GetInstance().tileObjectDict[GameEnums.TileObjectName.TileSet_0];


                    if (i == length - 1 && j == breadth - 1)
                    {
                       await GenerationMethods(i, j, randTile, GameEnums.Rotations.NoRotation,true);
                    }
                    else
                    {
                        await BorderGenerationMethods(i, j, randTile, GameEnums.Rotations.NoRotation);
                    }

                }
            }
        }


    }

    public async Task CollapseALocation(int length,int breadth,TileObjectController tile)
    {
        //in edit mode
        if(GlobalConfigData.GetInstance().currentToolMode == GameEnums.ToolMode.EditMode //in case of edit mode
            && !GlobalConfigData.GetInstance().isBorderTilesEnabled         //if no border selcted
            && GlobalConfigData.GetInstance().globalEvaluationIteration==0) //only want initialisation for first cyclr
        {
            int count;
            //TileObjectController randTile = null;

            InitialiseGeneration(GlobalConfigData.GetInstance().mapLength, GlobalConfigData.GetInstance().mapBreadth, out count);
        }
        GameEnums.Rotations rotations = wFCRules.GetTileRotation(new TileLocation(length, breadth),tile);
        await GenerationMethods(length, breadth, tile, rotations);
    }

    public async Task GenerateLevel(int length, int breadth)
    {


        //clength and cbreadth will be used to get the current active tile in consideration
        //int cLength, cBreadth;

        //if bordertile not enabled
        if (!GlobalConfigData.GetInstance().isBorderTilesEnabled)
        {
            //clength and cbreadth will be used to get the current active tile in consideration
            int count;
            TileObjectController randTile = null;

            InitialiseGeneration(length, breadth, out count);

            //we need to create a random tile at random position
            TileLocation location = new TileLocation((int)UnityEngine.Random.Range(0, length), (int)UnityEngine.Random.Range(0, breadth));

            GameEnums.TileObjectName name = GetRandomEnumValue<GameEnums.TileObjectName>();

            if(name == GameEnums.TileObjectName.None)
            {
                //TODO: coul be problemetic, check if necessary
                name = GameEnums.TileObjectName.TileSet_0;
            }

            randTile = GlobalConfigData.GetInstance().tileObjectDict[name];
            Rotations tileRotation = GameEnums.Rotations.NoRotation;

            //for the first random tile in case no border
            await GenerationMethods(location.xAxis, location.zAxis, randTile, tileRotation);
        }



        //If Wave has Not Collapsed
        while (CheckIfWaveHasNotCollapsed())
        {
            TileLocation location = GetUnitWithLeastEntropy();

            //cLength = location.xAxis;
            //cBreadth = location.zAxis;

            ReturnTile returnTile = wFCRules.GetTileValue(location);

            TileObjectController randTile = returnTile.tile;
            Rotations tileRotation = returnTile.rotation;

            await GenerationMethods(location.xAxis, location.zAxis, randTile, tileRotation);
        }

        //if wave has collapsed hide the visualisationParent
        visualizationManager.HideVisualisationParentObject();

    }

    public void SaveLevelMatrixData()
    {
        SaveLoadSystem.SaveLevelMatrixToDisk(_levelMatrix, _levelLength, _levelBreadth);
    }

    public void CreateLevelFromLoadData(int length, int breadth, int[,] tileMatrix,int[,] rotationMatrix)
    {
        for(int i=0; i<length; i++)
        {
            for(int j=0;j<breadth; j++)
            {
                TileObjectController tile = GlobalConfigData.GetInstance().tileObjectDict[(GameEnums.TileObjectName)tileMatrix[i,j]];
                Rotations rota = (GameEnums.Rotations)rotationMatrix[i, j];

                DrawCollapsedTile(tile, i, j, rota);
            }
        }
    }

    private void InitialiseGeneration(int length, int breadth, out int count)
    {
       // randTile = null;
        _levelLength = length;
        _levelBreadth = breadth;
        _levelMatrix = new UnitObject[length, breadth];

        //if evaluationList null it assigns a new list
        _evaluationCheckList ??= new Queue<UnitObject>();
        _evaluationCheckList.Clear();

        //create a dummy level with default value
        CreateEmptyLevleMatrix();

        NeighbourDictionaryAllTile = new Dictionary<TileLocation, Dictionary<neighbourType, UnitObject>>();

        //create the complete neighbour list
        CreateCompleteNeighbourDictionary();

        count = 0;
    }

    private async Task GenerationMethods(int randLength, int randBreadth, TileObjectController randTile, GameEnums.Rotations tileRotation,bool dontCheckCollapsed=false)
    {
        if (randTile is null)
        {
            WFCDebugLogger.logError(LogChannel, "Trying to send null tile for collapsing");
            return;
        }

        //set Unit tile value
        _levelMatrix[randLength, randBreadth].SetValue(randTile, tileRotation);

        //Draw the first random value tile
        await DrawCollapsedTile(randTile, randLength, randBreadth, tileRotation);

        //add neighbours of this tile for evaluation
        await AddNeighboursForEvaluation(_levelMatrix[randLength, randBreadth].unitLocation,dontCheckCollapsed);

        //this will evaluate all the unit in the check list
        await EvaluatetheUnitInList();
    }

    private async Task BorderGenerationMethods(int randLength, int randBreadth, TileObjectController randTile, GameEnums.Rotations tileRotation) 
    {
        if (randTile is null)
        {
            WFCDebugLogger.logError(LogChannel, "Trying to send null tile for collapsing");
            return;
        }

        //set Unit tile value
        _levelMatrix[randLength, randBreadth].SetValue(randTile, tileRotation);

        //in case of border generation we dont need to add to neighbour list 
        //or evaluate it will be done once we reach main tiles
        
        await DrawCollapsedTile(randTile, randLength, randBreadth, tileRotation);
    }


    private async Task DrawCollapsedTile(TileObjectController tileObjectController, int Xaxis, int Zaxis, GameEnums.Rotations rotation = GameEnums.Rotations.NoRotation)
    {
        Quaternion Qrotation = Quaternion.identity;

        await visualizationManager.addDelayForVisualisation(SecondsToWaitForVisualisatioin);


        switch (rotation)
        {
            case GameEnums.Rotations.NoRotation:
                Qrotation = Quaternion.Euler(90, 0, 0);
                //do nothing
                break;
            case GameEnums.Rotations.QuaterRotation:
                Qrotation = Quaternion.Euler(90, 90, 0);
                break;
            case GameEnums.Rotations.HalfRotation:
                Qrotation = Quaternion.Euler(90, 180, 0);
                break;
            case GameEnums.Rotations.ThreeFourthRotation:
                Qrotation = Quaternion.Euler(90, 270, 0);
                break;
            case GameEnums.Rotations.Empty:
                WFCDebugLogger.logError(LogChannel, "Empty roation value passed for generation");
                break;

        }


        GameObject objk = Utils.InstantiateAObject(tileObjectController.gameObject, new Vector3(Xaxis, 0, Zaxis), Qrotation);

        objk.transform.SetParent(_levelGameObject.transform);

        visualizationManager.SetVisualisationObjectState(Xaxis, Zaxis, VisualisationObjectState.Collapsed);

    }

    private bool CheckIfWaveHasNotCollapsed()
    {
        for (int i = 0; i < _levelLength; i++)
        {
            for (int j = 0; j < _levelBreadth; j++)
            {
                if (!_levelMatrix[i, j].hasCollapsed)
                {
                    //if even a single unit has not collapsed
                    return true;
                }
            }

        }

        //if we have checked that all unit have value other then -1 then
        //wave has copllapsed
        return false; ;
    }

    private TileLocation GetUnitWithLeastEntropy()
    {
        UnitObject elementWithLeastEntropy = null;
        int leastEntropy = int.MaxValue;

        // Iterate over the matrix and compare entropies
        for (int row = 0; row < _levelLength; row++)
        {
            for (int column = 0; column < _levelBreadth; column++)
            {
                if (!_levelMatrix[row, column].hasCollapsed)
                {

                    int currentEntropy = _levelMatrix[row, column].GetEntropy();

                    if (currentEntropy == 1)
                    {
                        elementWithLeastEntropy = _levelMatrix[row, column];

                        //if we find one with entropy 1 we dont need to check further
                        return elementWithLeastEntropy.unitLocation;
                    }

                    if (currentEntropy > 1 && currentEntropy < leastEntropy)
                    {
                        leastEntropy = currentEntropy;
                        elementWithLeastEntropy = _levelMatrix[row, column];
                    }
                }
                else
                {
                    // continue;
                }
            }
        }


        if (elementWithLeastEntropy is null)
        {
            WFCDebugLogger.logError(LogChannel, "*elementWithLeastEntropy* not found");
        }

        return elementWithLeastEntropy.unitLocation;
    }

    private async Task AddNeighboursForEvaluation(TileLocation oTile,bool dontCheckCollapsed = false)
    {
        try
        {
            //add the neighbours which need to be evaluated
            //which also needs to to be evaluated after the current tile

            //left neighbour 
            //for x=0 does not have a left
            if (oTile.xAxis != 0)
            {
                // WFCDebugLogger.log(LogChannel, "found left");
                UnitObject leftUnit = _levelMatrix[oTile.xAxis - 1, oTile.zAxis];

                //a small delay after each addition to alow 
                //for user to see visualisation
                await visualizationManager.addDelayForVisualisation(SecondsToWaitForVisualisatioin);

                PushUnitToEvaluationList(leftUnit,dontCheckCollapsed);
                // WFCDebugLogger.log(LogChannel, "added left");
            }

            //top neighbour
            //for y=LevelBreadth doesnot have a top
            if (oTile.zAxis < _levelBreadth - 1)
            {
                //WFCDebugLogger.log(LogChannel, "found bottom");
                UnitObject topUnit = _levelMatrix[oTile.xAxis, oTile.zAxis + 1];

                //a small delay after each addition to alow 
                //for user to see visualisation
                await visualizationManager.addDelayForVisualisation(SecondsToWaitForVisualisatioin);

                PushUnitToEvaluationList(topUnit,dontCheckCollapsed);
                // WFCDebugLogger.log(LogChannel, "added bottom");
            }



            //right neighbour
            //for x=LevelLength doesnot have a right
            if (oTile.xAxis < _levelLength - 1)
            {
                // WFCDebugLogger.log(LogChannel, "found right");
                UnitObject rightUnit = _levelMatrix[oTile.xAxis + 1, oTile.zAxis];

                //a small delay after each addition to alow 
                //for user to see visualisation
                await visualizationManager.addDelayForVisualisation(SecondsToWaitForVisualisatioin);

                PushUnitToEvaluationList(rightUnit,dontCheckCollapsed);
                // WFCDebugLogger.log(LogChannel, "found right");
            }

            //bottom neighbour | (x,y-1)
            //for y=0 doesNot have a bottom neighbour
            if (oTile.zAxis != 0)
            {
                // WFCDebugLogger.log(LogChannel,"found top");
                UnitObject bottomUnit = _levelMatrix[oTile.xAxis, oTile.zAxis - 1];

                //a small delay after each addition to alow 
                //for user to see visualisation
                await visualizationManager.addDelayForVisualisation(SecondsToWaitForVisualisatioin);

                PushUnitToEvaluationList(bottomUnit,dontCheckCollapsed);
                // WFCDebugLogger.log(LogChannel, "added top");
            }


        }

        catch (Exception e)
        {
            WFCDebugLogger.logError(LogChannel, "Problem while pushing to evaluation List:   " + e.Message);
        }
    }

    //just for pushing unit iunto evaluation list
    private void PushUnitToEvaluationList(UnitObject unit, bool dontCheckCollapsed = false)
    {
        if (dontCheckCollapsed)
        {
            if (unit.isInEvaluationList == false                                                      //to stop already in queue
                && unit.evaluationIteration == GlobalConfigData.GetInstance().globalEvaluationIteration)     //to stop tile which are already evaluated in this cycle
            {
                unit.isInEvaluationList = true;
                _evaluationCheckList.Enqueue(unit);

                //the unit which is in evaluation must be made red for visualisation
                visualizationManager.SetVisualisationObjectState(unit.unitLocation.xAxis, unit.unitLocation.zAxis, VisualisationObjectState.Evaluation);
            }
        }
        else
        {
            if (unit.isInEvaluationList == false && !unit.hasCollapsed                                                      //to stop already in queue //stop collapsed tile from entering
                && unit.evaluationIteration == GlobalConfigData.GetInstance().globalEvaluationIteration)     //to stop tile which are already evaluated in this cycle
            {
                unit.isInEvaluationList = true;
                _evaluationCheckList.Enqueue(unit);

                //the unit which is in evaluation must be made red for visualisation
                visualizationManager.SetVisualisationObjectState(unit.unitLocation.xAxis, unit.unitLocation.zAxis, VisualisationObjectState.Evaluation);
            }
        }
    }

    static T GetRandomEnumValue<T>()
    {
        Array enumValues = Enum.GetValues(typeof(T));
        return (T)enumValues.GetValue(UnityEngine.Random.Range(0,enumValues.Length));
    }

    //this method used to actually evaluate each unit one by one
    private async Task EvaluatetheUnitInList()
    {
        //loop untill all elements in list are processed
        while (_evaluationCheckList.Count != 0)
        {

            UnitObject unit = _evaluationCheckList.Dequeue();

            visualizationManager.SetVisualisationObjectState(unit.unitLocation.xAxis, unit.unitLocation.zAxis, VisualisationObjectState.Active);

            //add the neighbours of current unit to list
            //as they will also require evaluation later

            await AddNeighboursForEvaluation(unit.unitLocation);

            //we need to remove domain based on neighbours
            await wFCRules.EvaluateTheTileForDomainValues(new TileLocation(unit.unitLocation.xAxis, unit.unitLocation.zAxis));

            unit.isInEvaluationList = false;

            //this unit has been evaluated in this cycle
            unit.IncreaseEvaluationIteration();
        }

        //once we hae evaluated all the unit in the map
        //we need to increase the globalevaluationCycle count
        GlobalConfigData.GetInstance().IncreaseGlobalEvaluationIteration();
    }


    public void ResetGeneration()
    {
        //destroy all children
        while (_levelGameObject.transform.childCount > 0)
        {
            DestroyImmediate(_levelGameObject.transform.GetChild(0).gameObject);
        }

        _levelLength = 0;
        _levelBreadth = 0;
        _levelMatrix = null;

        _evaluationCheckList.Clear();
        NeighbourDictionaryAllTile = null;

    }
}


