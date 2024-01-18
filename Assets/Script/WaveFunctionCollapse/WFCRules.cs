using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static GameEnums;


#region DataStructures
public class EdgeValue 
{
    public string edgeString;
   // public int _noOfJoiningEdge;
    public int _noOfEmptyneighbour;

    public EdgeValue() 
    {
        edgeString = string.Empty;
        _noOfEmptyneighbour = 0;
       // _noOfJoiningEdge = 0;
    }
}

public struct ReturnTile
{
    public TileObjectController tile;
    public Rotations rotation;

    public ReturnTile(TileObjectController tile,Rotations rotation)
    {
        this.tile = tile;
        this.rotation = rotation;
    }

}

#endregion


public class WFCRules : MonoBehaviour
{
    private const string LogChannel = "WFCRules";

    private WaveFunctionCollapse waveFunctionCollapse;

    private List<GameEnums.neighbourType> neighbourTypeList;

    private const char IgnoreChar = '#'; // this is a random value which we
                                           // are setting for empty neighbour values

    private void Awake()
    {
        waveFunctionCollapse = GetComponent<WaveFunctionCollapse>();
        //neighbourTypeList = new List<neighbourType>{ neighbourType.Bottom,neighbourType.Left,, neighbourType.TopneighbourType.Right};
    }


    #region CollapsingRules

    //this will be used during the collapse stage
    public ReturnTile GetTileValue(TileLocation oTile)
    {
        ReturnTile finalReturnTile = new ReturnTile();
        finalReturnTile.tile = null;
        finalReturnTile.rotation = Rotations.Empty;

        UnitObject unit = waveFunctionCollapse._levelMatrix[oTile.xAxis, oTile.zAxis];
        EdgeValue edgeValue = new EdgeValue();


        List<ReturnTile> possibleTiles = new List<ReturnTile>();

        //here we are assuming the domain must have atleast one value 
        //which can be used to collapse this location
        if (unit.domain.Count <= 0)
        {
            WFCDebugLogger.logError(LogChannel, "domain has no units for collapsing");
        }
        else
        {


            edgeValue = GetEdgeValue(waveFunctionCollapse.NeighbourDictionaryAllTile[oTile]);

            for (int i = unit.domain.Count - 1; i >= 0; i--)
            {
                //in case the given tile location has some empty neighbour
                if (edgeValue._noOfEmptyneighbour > 0)
                {
                    //here we have to choose based on the neighbours which are present

                    foreach (GameEnums.Rotations rotation in Enum.GetValues(typeof(GameEnums.Rotations)))
                    {
                        if (rotation == Rotations.Empty)
                        {
                            //we need to move to next iteration as this 
                            //one wont give any result
                            continue;
                        }

                        string attachmentRule = unit.domain[i].metaData.GetAttachmentValue(rotation);

                        if (compareEdgeStrings(attachmentRule, edgeValue.edgeString))
                        {
                            ReturnTile returnTile = new ReturnTile(unit.domain[i], rotation);
                            possibleTiles.Add(returnTile);
                            break; // Exit the loop after finding a matching rotation
                        }
                    }

                    if(finalReturnTile.tile != null)
                    {
                        break;
                    }
                }
                else
                {
                    //here are picking the first tile which perfectly matches the edgeValue string
                    try
                    {
                        string value = edgeValue.edgeString;

                    foreach (GameEnums.Rotations rotation in Enum.GetValues(typeof(GameEnums.Rotations)))
                    {
                        if (rotation == Rotations.Empty)
                        {
                            continue;
                        }

                        if (value == unit.domain[i].metaData.GetAttachmentValue(rotation))
                        {
                            ReturnTile returnTile = new ReturnTile(unit.domain[i], rotation);
                            possibleTiles.Add(returnTile);
                            break; // Exit the loop after finding a matching rotation
                        }
                    }
                    }
                    catch
                    {
                        WFCDebugLogger.logError(LogChannel, "error in parsing");
                    }
                }

            }
        }

        if (possibleTiles.Count == 1)
        {
            finalReturnTile = possibleTiles[0];
        }
        else if (possibleTiles.Count > 1)
        {
            //this will determine if we will use the _DrawProbability
            int randValue = UnityEngine.Random.Range(0, 11);

            if (randValue > 6)
            {
                //there is a 50% chance that we will use probability

                int Max = 0;
                foreach (ReturnTile tile in possibleTiles)
                {
                    if (tile.tile.metaData.DrawProbability > Max)
                    {
                        Max = tile.tile.metaData.DrawProbability;
                        finalReturnTile = tile;
                    }
                }
            }
            else
            {
                finalReturnTile = possibleTiles[UnityEngine.Random.Range(0, possibleTiles.Count)];
            }
        }
        else
        {
            if(edgeValue.edgeString != String.Empty) 
            {
                WFCDebugLogger.logError(LogChannel, "No Tile object found corresponding to required edge value:    " + edgeValue.edgeString);
            }
            else 
            {
                WFCDebugLogger.logError(LogChannel, "EdgeString Not initialised");
            }

           WFCDebugLogger.logError(LogChannel, "No tile found setting it to empty plain");     
           // //incase no tile is found set it to to TileSet_0
           finalReturnTile.tile = GlobalConfigData.GetInstance().tileObjectDict[GameEnums.TileObjectName.TileSet_0];
        }

        return finalReturnTile;
    }

    public GameEnums.Rotations GetTileRotation(TileLocation oTile, TileObjectController tile)
    {
        GameEnums.Rotations rota = Rotations.Empty;

        EdgeValue edgeValue = GetEdgeValue(waveFunctionCollapse.NeighbourDictionaryAllTile[oTile]);

        //there are empty neighbours thus it will impact the required rotation
        if (edgeValue._noOfEmptyneighbour > 0)
        {
            foreach (GameEnums.Rotations rotation in Enum.GetValues(typeof(GameEnums.Rotations)))
            {
                if (rotation == GameEnums.Rotations.Empty)
                {
                    //do nothing
                    continue;
                }

                string attachmentRule = tile.metaData.GetAttachmentValue(rotation);

                if (compareEdgeStrings(attachmentRule, edgeValue.edgeString))
                {
                    return rotation;
                } 
            }
        }
        else
        {
            //no empty neighbour
            //rotation have to account for that

            string value = edgeValue.edgeString;

            foreach (GameEnums.Rotations rotation in Enum.GetValues(typeof(GameEnums.Rotations)))
            {
                if (rotation == Rotations.Empty)
                {
                    continue;
                }

                if (value == tile.metaData.GetAttachmentValue(rotation))
                {
                    return rotation;
                }
            }
        }

        return rota;
    }

    
    #endregion

    #region PropogationRules

    //this will be used during the propogation phase
    public async Task   EvaluateTheTileForDomainValues(TileLocation oTile)
    {

        //get edge value
        EdgeValue edgeValue = GetEdgeValue(waveFunctionCollapse.NeighbourDictionaryAllTile[oTile]);

        //if all are empty neighbours
        if (edgeValue._noOfEmptyneighbour == 4)
        {
            //in this case we cant remove anything
            return;
        }
        else
        {
            //check for the edge value in the domain
            await CompareEdgeValueInDomain(waveFunctionCollapse._levelMatrix[oTile.xAxis, oTile.zAxis], edgeValue);
        }

    }

    public async Task CompareEdgeValueInDomain(UnitObject unit,EdgeValue edgeValue)
    {
        //this function is responsible for checking which units to keep 
        //or remove from the domain space
        List<TileObjectName> RemovedTileObjectsNameList = new List<TileObjectName>();

        //remove the unitObject from domain based on edge value
        for (int i= unit.domain.Count - 1; i >= 0;i--)
        {


            //if their are some empty neighbour
            if(edgeValue._noOfEmptyneighbour>0)
            {

                //in this case its no use to verify using edgeValue edge string
                //as edge string would contain 2 for empty neighbours

                //TODO:may cause error in rules
                //cant use this logic Anymore
                {
                    //if no of joiningedge in domain unit is less then joiningedge in edgevalue
                    //then remove that edge
                    /*                if (unit.domain[i].metaData.joiningEdge < edgeValue._noOfJoiningEdge)
                                    {
                                        RemovedTileObjectsNameList.Add(unit.domain[i].metaData.name);
                                        unit.domain.RemoveAt(i);
                                        continue;
                                    }*/
                }



                //also we can verify by comparing
                //as in say if edgestring = "1221", then we can elemenate all
                //tiles which can never take the form of 1xx1.
                //but we have to check that for each rotation

                int count = 0;

                foreach (GameEnums.Rotations rotation in Enum.GetValues(typeof(GameEnums.Rotations)))
                {
                    if (rotation == Rotations.Empty)
                    {
                        count++;
                        //we need to move to next iteration as this 
                        //one wont give any result
                        continue;
                    }

                    string attachmentRule = unit.domain[i].metaData.GetAttachmentValue(rotation);

                    //if not matching then remove
                    if (! compareEdgeStrings(attachmentRule, edgeValue.edgeString))
                    {  
                        count++;
                    }
                    else
                    {
                        //WFCDebugLogger.log(LogChannel, "attachmentRule:  " + attachmentRule + "matches   edgevalue:  " + edgeValue.edgeString + "rotation =" + rotation.ToString());
                    }
                }

                //we have to make sure that the value doesnot mathc for 
                //any rotation value and then remove
                if (count == Enum.GetValues(typeof(GameEnums.Rotations)).Length)
                {
                    RemovedTileObjectsNameList.Add(unit.domain[i].metaData.name);
                    unit.domain.RemoveAt(i);
                }


            }
            else 
            {
                //now that their are no empty neighbour 
                //we can rely upon our edge value string
                //and use it to remove TileObject which are no longer relevent

                string value = edgeValue.edgeString;

                //total number of possible rotation
                int totalAttachmnetRules = unit.domain[i].metaData.GetRotationValueList().Count;
                int count = 0;

                //now compare this value to each rotation of tileObject
                foreach (string attachmentvalue in unit.domain[i].metaData.GetRotationValueList()) 
                {
                    if(value != attachmentvalue) 
                    {
                        count++; 
                    }
                }

                if (count == totalAttachmnetRules) 
                {
                    //the given value didnt match with any attachmentrules
                    //so drop this unit

                    RemovedTileObjectsNameList.Add(unit.domain[i].metaData.name);
                    unit.domain.RemoveAt(i);
                }
            }
        }

        //remove the unitObject which are not possible in domain
        //now pass the list to visualisation
        await waveFunctionCollapse.visualizationManager.changeDomainValueForVisualisationObject(RemovedTileObjectsNameList, unit.unitLocation.xAxis, unit.unitLocation.zAxis);
    }

    private static EdgeValue SetEmptyValues(ref EdgeValue edgevalue)
    {
        edgevalue._noOfEmptyneighbour += 1;
        edgevalue.edgeString += IgnoreChar; //=# //setting for edge with empty neighbours
        return edgevalue;
    }

    #endregion

    #region Utility 

    private EdgeValue GetEdgeValue(Dictionary<GameEnums.neighbourType,UnitObject> neighbourList) 
    {
        //O means no edge can be attached to that neighbour
        //1 means ede can be attached
        //2 means that side is empty

        //this edgevalue will be used to compare with other TileObject in domain
        //both for  propogation and collapse phase
        EdgeValue edgeValue = new EdgeValue();

        try
        {
            //it returns in binary order
            //in order LTRB
            foreach (GameEnums.neighbourType neighbour in neighbourList.Keys)
            {
                GameEnums.TileObjectName tileName;
                TileObjectController tileObject;

                UnitObject neighbourUnitObject = neighbourList[neighbour];

                //we need to find a edge for each neighbour unit type
                //that is left,right...etc

                switch (neighbour)
                {
                    //for each neighbour we are checking if that neighbour(UnitObject) has any tile there (isCollapsed) 
                    //or is a empty neighbour

                    case GameEnums.neighbourType.Left:
                        //neghbour value can be null for a unit object if the neighbour lies outside map grid
                        //for example for unitObject(1,0) left neighbour would be null.
                        if (neighbourUnitObject != null && neighbourUnitObject.hasCollapsed)
                        {
                            GameEnums.EdgeType edgeType = GameEnums.EdgeType.noEdge;

                            //we need to check if the unit in question is rotated
                            if (neighbourUnitObject.rotationValue == Rotations.NoRotation)
                            {
                                //for left neighbour we need right edge if no rotation
                                edgeType = GameEnums.EdgeType.Right;
                            }
                            else
                            {
                                //now that it is rotated we need the rotated edge value
                                edgeType = neighbourUnitObject.collapsedTileValue.metaData.GetRotatedEdgeValue(neighbourUnitObject.rotationValue, GameEnums.EdgeType.Right);
                            }

                            SetEdgeValues(ref edgeValue, neighbourUnitObject, out tileName, out tileObject, edgeType);


                        }
                        else
                        {
                            SetEmptyValues(ref edgeValue);
                        }
                        break;
                    case GameEnums.neighbourType.Top:
                        if (neighbourUnitObject != null && neighbourUnitObject.hasCollapsed)
                        {

                            GameEnums.EdgeType edgeType = GameEnums.EdgeType.noEdge;

                            //we need to check if the unit in question is rotated
                            if (neighbourUnitObject.rotationValue == Rotations.NoRotation)
                            {
                                //for top neighbour we need bottom edge if no rotation
                                edgeType = GameEnums.EdgeType.Bottom;
                            }
                            else
                            {
                                //now that it is rotated we need the rotated edge value
                                edgeType = neighbourUnitObject.collapsedTileValue.metaData.GetRotatedEdgeValue(neighbourUnitObject.rotationValue, GameEnums.EdgeType.Bottom);
                            }

                            
                            SetEdgeValues(ref edgeValue, neighbourUnitObject, out tileName, out tileObject, edgeType);
                        }
                        else
                        {
                            SetEmptyValues(ref edgeValue);
                        }
                        break;
                    case GameEnums.neighbourType.Right:
                        if (neighbourUnitObject != null && neighbourUnitObject.hasCollapsed)
                        {

                            GameEnums.EdgeType edgeType = GameEnums.EdgeType.noEdge;

                            //we need to check if the unit in question is rotated
                            if (neighbourUnitObject.rotationValue == Rotations.NoRotation)
                            {
                                //for right neighbour we need left edge if no rotation
                                edgeType = GameEnums.EdgeType.Left;
                            }
                            else
                            {
                                //now that it is rotated we need the rotated edge value
                                edgeType = neighbourUnitObject.collapsedTileValue.metaData.GetRotatedEdgeValue(neighbourUnitObject.rotationValue, GameEnums.EdgeType.Left);
                            }

                            
                            SetEdgeValues(ref edgeValue, neighbourUnitObject, out tileName, out tileObject, edgeType);
                        }
                        else
                        {
                            SetEmptyValues(ref edgeValue);
                        }
                        break;
                    case GameEnums.neighbourType.Bottom:
                        if (neighbourUnitObject != null && neighbourUnitObject.hasCollapsed)
                        {
                            GameEnums.EdgeType edgeType = GameEnums.EdgeType.noEdge;

                            //we need to check if the unit in question is rotated
                            if (neighbourUnitObject.rotationValue == Rotations.NoRotation)
                            {
                                //for bottom neighbour we need top edge if no rotation
                                edgeType = GameEnums.EdgeType.Top;
                            }
                            else
                            {
                                //now that it is rotated we need the rotated edge value
                                edgeType = neighbourUnitObject.collapsedTileValue.metaData.GetRotatedEdgeValue(neighbourUnitObject.rotationValue, GameEnums.EdgeType.Top);
                            }

                            
                            SetEdgeValues(ref edgeValue, neighbourUnitObject, out tileName, out tileObject, edgeType);
                        }
                        else
                        {
                            SetEmptyValues(ref edgeValue);
                        }
                        break;

                }

            }
        }
        catch(Exception e)
        {
            WFCDebugLogger.logError(LogChannel, "Error while creating edgeValue using all neighbour:    " + e);
        }

        if(edgeValue.edgeString.Length < 4)
        {
            WFCDebugLogger.logError(LogChannel, "EdgeString value is set wrong");
        }

        return edgeValue;
    }

    private void SetEdgeValues(ref EdgeValue edgeValue, UnitObject unit, out GameEnums.TileObjectName tileName, out TileObjectController tileObject, GameEnums.EdgeType edgeToCheck)
    {
        tileName = GameEnums.TileObjectName.None;
        tileObject = null;

        try
        {
            tileName = unit.collapsedTileValue.metaData.name;

            tileObject = GlobalConfigData.GetInstance().tileObjectDict[tileName];


            switch (edgeToCheck)
            {
                case GameEnums.EdgeType.Left:
                   // edgeValue._noOfJoiningEdge += tileObject.metaData.left ? 1 : 0;
                    edgeValue.edgeString += tileObject.metaData.left;
                    break;
                case GameEnums.EdgeType.Right:
                   // edgeValue._noOfJoiningEdge += tileObject.metaData.right ? 1 : 0;
                    edgeValue.edgeString += tileObject.metaData.right;
                    break;
                case GameEnums.EdgeType.Top:
                    //edgeValue._noOfJoiningEdge += tileObject.metaData.top ? 1 : 0;
                    edgeValue.edgeString += tileObject.metaData.top;
                    break;
                case GameEnums.EdgeType.Bottom:
                   // edgeValue._noOfJoiningEdge += tileObject.metaData.bottom ? 1 : 0;
                    edgeValue.edgeString += tileObject.metaData.bottom;
                    break;

            }
        }
        catch(Exception e)
        {
            WFCDebugLogger.logError(LogChannel,"Error while assigning edgeValue for " + tileName + "exception : " + e);
        }
    }



    private bool compareEdgeStrings(string attachmentRule, string edgeValue) 
    {
        // we need to compare two string say "1101" and "1#01" ;
        //we can ignore the ignore char = "#" value but other value must match

        bool stringsMatch = true;
        int count = 0;

            for (int i = 0; i <= edgeValue.Length-1; i++)
            {
                if (edgeValue[i] == IgnoreChar)
                {
                    count++;
                    continue; // Skip the comparison at the ignoreIndex
                }

                if (edgeValue[i] != attachmentRule[i])
                {
                    stringsMatch = false;
                    continue;
                }
            }

            //in case all the number are #
            if(count == attachmentRule.Length)
            {
                stringsMatch = true;
            }

        return stringsMatch;
    }

    #endregion

}
