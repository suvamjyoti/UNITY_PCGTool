using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class VisualisationObject : MonoBehaviour
{
    public List<DomainObject> tileDomainList { get; private set; }

    [SerializeField] private Material _activeMatrial;
    [SerializeField] private Material _evaluationMatrial;
    [SerializeField] private Material _CollapsedMatrial;

    [SerializeField] private MeshRenderer objectRenderer;

    [SerializeField] private GameObject _tileVisualeDomainGridParent;
    public GameObject tileVisualeDomainGridParent => _tileVisualeDomainGridParent;

    public int tileXLocation { get; private set;}
    public int tileZLocation { get; private set; }

    public GameEnums.VisualisationObjectState _currentState { get; private set; }

    [SerializeField] public TileManager tileManager; 

    private void Awake()
    {
        tileDomainList = new List<DomainObject>();
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

    public void AddTileDomainObjectToList(DomainObject domainObject)
    {
        tileDomainList.Add(domainObject);
    }

}
