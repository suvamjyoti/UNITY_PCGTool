using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class VisualisationObject : MonoBehaviour
{
    [SerializeField]private List<DomainObject> tileDomainList;

    [SerializeField] private Material _activeMatrial;
    [SerializeField] private Material _evaluationMatrial;
    [SerializeField] private Material _CollapsedMatrial;

    [SerializeField] private MeshRenderer objectRenderer; 

    public int tileXLocation { get; private set;}
    public int tileZLocation { get; private set; }

    public GameEnums.VisualisationObjectState _currentState { get; private set; }

    [SerializeField] public TileManager tileManager; 

    private void Awake()
    {
        
    }
    private void Start()
    {
        tileManager = TileManager.GetInstance();
    }

    public void SetLocation(int XAxis,int ZAxis)
    {
        tileXLocation = XAxis;
        tileZLocation = ZAxis;
    }

    public void UpdateObjectState(GameEnums.VisualisationObjectState setState)
    {
        _currentState = setState;

        switch (setState)
        {
            case GameEnums.VisualisationObjectState.Active:
                objectRenderer.material = _activeMatrial;
                break;
            case GameEnums.VisualisationObjectState.Evaluation:
                objectRenderer.material = _evaluationMatrial;
                break;
            case GameEnums.VisualisationObjectState.Collapsed:
                objectRenderer.material = _CollapsedMatrial;
                //TODO: Remove all the possible DomainObject after collapsed
                break;

        }
         
    }

    public void RemoveTheseValuesFromDomainList(List<GameEnums.TileObjectName> domainList)
    {

        foreach (DomainObject dObject in tileDomainList)
        {
            foreach(GameEnums.TileObjectName name in domainList)
            {
                if(dObject.tileName == name)
                {
                    dObject.VisualisationStatus(false);
                }
            }
        }
    }

}
