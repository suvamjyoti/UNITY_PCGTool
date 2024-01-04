using System;
using UnityEngine;

//this script is responsible for indivisual tile object and is responsible for 
//indivisual tiule manipulation such as rotation and trasform.

public class TileObjectController : MonoBehaviour
{
    [SerializeField] private  MetaDataModel _metaData = null;

    public MetaDataModel metaData => _metaData; 

    private Transform objectTransform;

    private void Awake()
    {
        objectTransform = this.transform;
       // CalculateAttachmentRuleValue();
    }



}
