using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static GameEnums;

public class VisualisationManager : MonoBehaviour
{
    private const string LogChannel = "VisualizationManager";

    public VisualisationObject[,] _domainVisualisationGrid { get; private set; }

    private int _domainLength;
    private int _domainBreadth;

    [SerializeField] private GameObject visualisationObject;

    private int     VisualisationSpeed;
    private bool    _isVisualizationON = true;
    //private bool    CreateBorderPlain;

    [SerializeField] private GameObject _VisualisationGridHolderParent;

    private void Start()
    {
        GlobalConfigData.GetInstance().Vanilla_GenerationEvent += StartVisualisation;
        GlobalConfigData.GetInstance().Vanilla_ResetEvent += ResetVisualisation;

        GlobalConfigData.GetInstance().Edit_StartEvent += StartVisualisation;
        GlobalConfigData.GetInstance().Edit_ResetEvent += ResetVisualisation;

    }

    private void StartVisualisation()
    {
        InitialiseValues();

        if (_isVisualizationON)
        {
            InitialiseVisualisation();
        }
    }

    private void InitialiseValues()
    {
        VisualisationSpeed = GlobalConfigData.GetInstance().visualisationSpeed;
        _isVisualizationON = GlobalConfigData.GetInstance().isVisualisationOn;
    }

    private void InitialiseVisualisation()
    {
        if (_isVisualizationON)
        {
            _domainLength = GlobalConfigData.GetInstance().mapLength;
            _domainBreadth = GlobalConfigData.GetInstance().mapBreadth;

            _domainVisualisationGrid = new VisualisationObject[_domainLength, _domainBreadth];

            //draw the grid of visualisation object
            drawTheGridOfVisualisationObjects();
        }
    }

    private void drawTheGridOfVisualisationObjects()
    {
        for(int i=0;i<_domainLength;i++)
        {
            for (int j = 0; j < _domainBreadth; j++)
            {
                VisualisationObject vObject = Utils.InstantiateAObject(visualisationObject.gameObject, new Vector3(i, 0, j), Quaternion.identity).GetComponent<VisualisationObject>();
                _domainVisualisationGrid[i,j] = vObject;
                vObject.SetLocation(i, j);
                vObject.transform.SetParent(_VisualisationGridHolderParent.transform);
            }
        }
    }

    public async Task changeDomainValueForVisualisationObject(List<GameEnums.TileObjectName> ItemsToRemoveFromDomain,int iLocation,int jLocation)
    {
        if (_isVisualizationON)
        {
            VisualisationObject visual = _domainVisualisationGrid[iLocation, jLocation];
            visual.RemoveTheseValuesFromDomainList(ItemsToRemoveFromDomain);
        }
    }


    public void SetVisualisationObjectState(int iLocation,int jLocation,GameEnums.VisualisationObjectState state)
    {
        if (_isVisualizationON)
        {
            VisualisationObject visual = _domainVisualisationGrid[iLocation, jLocation];
            visual.UpdateObjectState(state);
        }
    }

    public async Task addDelayForVisualisation(int secondsToWait)
    {
        if (_isVisualizationON)
        {

            await Task.Delay(secondsToWait * VisualisationSpeed);
        }
    }

    public void HideVisualisationParentObject()
    {
        _VisualisationGridHolderParent.SetActive(false);
    }

    private void ResetVisualisation()
    {
        //destroy all children
        while (_VisualisationGridHolderParent.transform.childCount > 0)
        {
            DestroyImmediate(_VisualisationGridHolderParent.transform.GetChild(0).gameObject);
        }

        _VisualisationGridHolderParent.SetActive(true);

       //reset grid values
       _domainVisualisationGrid = null;
    }

}
