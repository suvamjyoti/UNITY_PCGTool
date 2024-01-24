using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DomainObject:MonoBehaviour
{
    public GameEnums.TileObjectName tileName { get; private set; }

    private SpriteRenderer correspondingSprite;

   // [SerializeField] public VisualisationObject visualisationObject;

    private void Awake()
    {
        correspondingSprite = GetComponent<SpriteRenderer>();
    }

    public void SetValue(GameEnums.TileObjectName tileName)
    {
        this.tileName = tileName ;
    }
    
    public void VisualisationStatus(bool status)
    {
        correspondingSprite.enabled = status;
    }
}
