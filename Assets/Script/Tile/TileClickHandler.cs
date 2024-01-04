using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickHandler : MonoBehaviour
{
    private void Update()
    {

        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject == gameObject)
                {

                    DomainObject domainObject = GetComponent<DomainObject>();
                    VisualisationObject visObject = domainObject.visualisationObject;
                    GameEnums.TileObjectName tileName = domainObject.tileName;

                    //spawn objects when click detected
                    visObject.tileManager.CollapseALocation(visObject.tileXLocation,visObject.tileZLocation,tileName);
                }
            }
        }
    }
}
