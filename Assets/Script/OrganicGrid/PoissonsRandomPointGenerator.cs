using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonsRandomPointGenerator : MonoBehaviour
{
    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSample = 30;
    public float displayRadius = 1;
    
    public  List<Vector2> GeneratePoint()
    {
        float cellSize = radius/Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(regionSize.x / cellSize), Mathf.CeilToInt(regionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();

        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(regionSize / 2);

        while(spawnPoints.Count>0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];

            bool candidateAccepted = false;
            for(int i=0;i<rejectionSample;i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter * dir * Random.Range(radius, 2 * radius);

                if (isValid(candidate,regionSize,cellSize,radius,points,grid))
                {
                    candidateAccepted = true;

                    points.Add(candidate);
                    spawnPoints.Add(candidate);

                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    break;
                }
            }

            if(!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }

        }

        return points;
    }


    static bool isValid(Vector2 candidate,Vector2 sampleRegionSize,float cellSize,float radius,List<Vector2> pointsList,int[,] grid)
    {
        if(candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int searchStartx = Mathf.Max(0,cellX - 2);
            int searchEndx = Mathf.Min(cellX + 2,grid.GetLength(0)-1);

            int searchStarty = Mathf.Max(0, cellY - 2);
            int searchEndy = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for(int x = searchStartx;x<=searchEndx;x++)
            {
                for(int y = searchStarty;y<=searchEndy;y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if(pointIndex != -1)
                    {
                        float distance = (candidate - pointsList[pointIndex]).magnitude;
                        if(distance<radius)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;

        }
        return false;
    }

}
