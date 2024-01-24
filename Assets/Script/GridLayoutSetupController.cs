using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayoutSetupController : MonoBehaviour
{
    const string Logchannel = "GreidLayout";

    [SerializeField] private int rows ;
    [SerializeField] private int cols ;

    [SerializeField] private float distanceBetweenCells;

    [SerializeField] private float offsetFromRight;
    [SerializeField] private float offsetFromTop;



    public void AlignInGrid()
    {
        List<Transform> ChildObjectList = new List<Transform>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            ChildObjectList.Add(this.transform.GetChild(i));
        }

        if(rows*cols == ChildObjectList.Count)
        {
            int count = 0;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Vector3 newPosition = ChildObjectList[count].transform.position;

                    newPosition.z = (i * ChildObjectList[count].transform.localScale.y) + (distanceBetweenCells * i) ;
                    newPosition.x = (j * ChildObjectList[count].transform.localScale.x) + (distanceBetweenCells * j) ;

                    ChildObjectList[count].transform.position = newPosition;
                    count++;
                }
            }

            Vector3 nPosParent = this.gameObject.transform.position;
            nPosParent.z += offsetFromRight;
            nPosParent.x += offsetFromTop;

            this.gameObject.transform.position = nPosParent;

        }
        else
        {
            WFCDebugLogger.logError(Logchannel,"Cant be arranged in grid as child count is less then number of items required");
        }
    }

}
