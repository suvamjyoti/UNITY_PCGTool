using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triangle
{

    //      v1
    //     /\        
    //    /  \        
    //   /____\      
    //  v3      v2

    //we are going clockwise

    public Vector2 vertex1 {get; private set;}
    public Vector2 vertex2 { get; private set;}
    public Vector2 vertex3 { get; private set;}

    //could be a pointers list to vertex
    //or line renderer
    private Vector2[] _edge1;
    private Vector2[] _edge2;
    private Vector2[] _edge3;

    public Vector2[] edge1 => _edge1;
    public Vector2[] edge2 => _edge2;
    public Vector2[] edge3 => _edge3;
    public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        vertex1 = point1;
        vertex2 = point2;
        vertex3 = point3;

        _edge1 = new Vector2[2];
        _edge2 = new Vector2[2];
        _edge3 = new Vector2[2];

        //      v1
        //     /\        
        //    /  \        
        //   /____\      
        //  v3      v2

        //we are going clockwise

        AddVertexToEdge(ref _edge1, vertex1, vertex2);
        AddVertexToEdge(ref _edge2, vertex2, vertex3);
        AddVertexToEdge(ref _edge3, vertex3, vertex1);

    }

    private void AddVertexToEdge(ref Vector2[] edge, Vector2 a, Vector2 b)
    {
        edge[0] = a;
        edge[1] = b;
    }

    public void VisualizeEdges()
    {
        GameObject lineRendererObject = new GameObject("TriangleEdges");
        LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 4; // Four vertices for three edges + closing edge
        lineRenderer.useWorldSpace = false;

        lineRenderer.SetPositions(new Vector3[] {
            vertex1, vertex2, // Edge 1
            vertex2, vertex3, // Edge 2
            vertex3, vertex1, // Edge 3
            vertex1  // Closing edge
        });

        lineRenderer.loop = true;  // Close the loop for a closed triangle
        lineRenderer.startWidth = 0.05f; // Adjust line width as needed
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // You can change the material as needed
        lineRenderer.startColor = Color.red; // Adjust line color as needed
        lineRenderer.endColor = Color.red;
    }

    public bool IsPointInside(Vector2 point)
    {
        // Calculate barycentric coordinates
        float denominator = ((vertex2.y - vertex3.y) * (vertex1.x - vertex3.x) + (vertex3.x - vertex2.x) * (vertex1.y - vertex3.y));
        float w1 = ((vertex2.y - vertex3.y) * (point.x - vertex3.x) + (vertex3.x - vertex2.x) * (point.y - vertex3.y)) / denominator;
        float w2 = ((vertex3.y - vertex1.y) * (point.x - vertex3.x) + (vertex1.x - vertex3.x) * (point.y - vertex3.y)) / denominator;
        float w3 = 1.0f - w1 - w2;

        // Check if the point is inside the triangle using barycentric coordinates
        return (w1 >= 0 && w2 >= 0 && w3 >= 0);
    }

}

public class DelaunayTriangulation: MonoBehaviour
{
    private const string LogChannel = "DelaunayTriangle";

    [SerializeField] private  float MultiplierSuperTriangle = 1;

    public List<Triangle> TriangleList { get; private set;}

    public List<Triangle> GenerateTriangleGrid(List<Vector2> vertexList)
    {

        int n = FibonaciSequence(10);

        Debug.Log(n);

        TriangleList = new List<Triangle>();

        //first create a superTriangle circuscribing all the points
        //and add to TriangleList
        TriangleList.Add(CreateSuperTriangle(vertexList));

        //now we need to iteratively add each vertex one by one
        for (int i = 0; i < 10; i++)
        {
            Triangle tempTriangle = null;
            //check in which triangle this point lies
            {
                foreach (Triangle triangle in TriangleList)
                {
                    if (triangle.IsPointInside(vertexList[i]))
                    {
                        tempTriangle = triangle;
                    }
                }
            }

            if(tempTriangle != null)
            {
                //connect the vertex of that triangle to this point
                //and create three new triangles
                {
                    TriangleList.Add(new Triangle(tempTriangle.vertex1,tempTriangle.vertex2, vertexList[i]));
                    TriangleList.Add(new Triangle(tempTriangle.vertex2, vertexList[i], tempTriangle.vertex3));
                    TriangleList.Add(new Triangle(vertexList[i], tempTriangle.vertex3, tempTriangle.vertex1));

                    //also remove the previous triangle which is no longer relevant
                    //say {0,1,2,3,4} in list, we add 3 more total count 8,
                    //8 - 4 = 4, which the index at which original triangle was 
                    TriangleList.RemoveAt(TriangleList.Count - 4);
                }

                //now check for the circumscribe rule

                //swap if rule fails
            }
            else
            {
                WFCDebugLogger.logError(LogChannel,"no proper triangle for the point found");
            }
        }


        return TriangleList;
    }

    Triangle CreateSuperTriangle(List<Vector2> points)
    {
        //Find minimum and maximum coordinates
        float min_x = points.Min(p => p.x);
        float max_x = points.Max(p => p.x);
        float min_y = points.Min(p => p.y);
        float max_y = points.Max(p => p.y);

        //Define vertices for the super triangle
        //Extend the triangle beyond the bounding box
        float a = max_x - min_x;
        float b = max_y - min_y;
        float dx = ((a > b) ? a : b) * MultiplierSuperTriangle;  // Extend length (adjust multiplier as needed)


        Triangle super_triangle = new Triangle(
        new Vector2(min_x - dx, min_y - dx),
        new Vector2(max_x + dx, min_y - dx),
        new Vector2((min_x + max_x) / 2, max_y + dx * 2));

        return super_triangle;
    }

    private void VisualiseTriangleList()
    {
        foreach(Triangle triangle in TriangleList)
        {
            triangle.VisualizeEdges();
        }
    }

    private int FibonaciSequence(int n)
    {
        int a = 0;
        int b = 1;
        int c = 0;

        for (int i = 0; i < n; i++)
        {
            if (i <= 1)
            {
                c = i;
            }
            else
            {
                c = a + b;
                a = b;
                b = c;
            }
            
            Debug.Log(c);
        }

        return c;
    
    }

}
