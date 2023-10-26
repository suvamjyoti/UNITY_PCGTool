using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DomainObject:MonoBehaviour
{
    [SerializeField] public GameEnums.TileObjectName tileName;
    private SpriteRenderer correspondingSprite;

    [SerializeField] public VisualisationObject visualisationObject;

    private void Awake()
    {
        correspondingSprite = GetComponent<SpriteRenderer>();
    }

    public void VisualisationStatus(bool status)
    {
        correspondingSprite.enabled = status;
    }
}
