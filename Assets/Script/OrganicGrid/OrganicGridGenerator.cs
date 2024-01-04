using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OrganicGridGenerator : MonoBehaviour
{

    private const string LogChannel = "OrganicGridGenerator";

    [SerializeField] private PoissonsRandomPointGenerator poissonsRandomPoint;
    [SerializeField] private DelaunayTriangulation delaunayTriangulation;

    private List<Vector2> poissonsVertexList;
    private List<Triangle> triangles;

    private void Start()
    {


        poissonsVertexList = new List<Vector2>();

        poissonsVertexList = poissonsRandomPoint.GeneratePoint();

        triangles = delaunayTriangulation.GenerateTriangleGrid(poissonsVertexList);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(regionSize / 2, regionSize);

        if (poissonsVertexList != null)
        {
            foreach (Vector2 point in poissonsVertexList)
            {
                Gizmos.DrawSphere(point, poissonsRandomPoint.displayRadius);
            }
        }
    }

    Vector3 FindNearestNeighbor(Vector2 point1, Vector2 point2)
    {

        Vector3 midPoint;
        // float midx = (point1.x + point2.x) / 2f;
        // float midy = (point1.y + point2.y) / 2f;
        //midPoint = new Vector3(midx, midy, 0.0f);

        return GetNearestPointToAVertex(point1);
    }

    Vector2 GetNearestPointToAVertex(Vector2 vertex)
    {
        int nearestPointIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < poissonsVertexList.Count; i++)
        {
            float distance = Vector2.Distance(vertex, poissonsVertexList[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPointIndex = i;
            }
        }

        Vector3 nearestPoint = poissonsVertexList[nearestPointIndex];
        poissonsVertexList.RemoveAt(nearestPointIndex);

        return nearestPoint;

    }

  
}
