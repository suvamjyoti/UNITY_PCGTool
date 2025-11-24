using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    
    public GameObject VisualizeEdges()
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

        return lineRendererObject;
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
    
    // Add these methods to your Triangle class:
    // 
    public bool IsPointInCircumcircle(Vector2 point)
    {
        // Calculate circumcircle center and radius
        float ax = vertex1.x - point.x;
        float ay = vertex1.y - point.y;
        float bx = vertex2.x - point.x;
        float by = vertex2.y - point.y;
        float cx = vertex3.x - point.x;
        float cy = vertex3.y - point.y;
        
        float det = (ax * ax + ay * ay) * (bx * cy - cx * by) -
                    (bx * bx + by * by) * (ax * cy - cx * ay) +
                    (cx * cx + cy * cy) * (ax * by - bx * ay);
        
        return det > 0;
    }

    public bool HasVertex(Vector2 v)
    {
        return vertex1 == v || vertex2 == v || vertex3 == v;
    }

    public bool HasEdge(Edge edge)
    {
        return (vertex1 == edge.p1 && vertex2 == edge.p2) ||
               (vertex2 == edge.p1 && vertex3 == edge.p2) ||
               (vertex3 == edge.p1 && vertex1 == edge.p2) ||
               (vertex1 == edge.p2 && vertex2 == edge.p1) ||
               (vertex2 == edge.p2 && vertex3 == edge.p1) ||
               (vertex3 == edge.p2 && vertex1 == edge.p1);
    }

}

public class Edge
{
    public Vector2 p1;
    public Vector2 p2;
    
    public Edge(Vector2 point1, Vector2 point2)
    {
        p1 = point1;
        p2 = point2;
    }
    
    public bool Equals(Edge other)
    {
        return (p1 == other.p1 && p2 == other.p2) || 
               (p1 == other.p2 && p2 == other.p1);
    }
}

public class DelaunayTriangulation: MonoBehaviour
{
    private const string LogChannel = "DelaunayTriangle";

    [SerializeField] private  float MultiplierSuperTriangle = 1;

 public List<Triangle> TriangleList { get; private set; }

public List<Triangle> GenerateTriangleGrid(List<Vector2> vertexList)
{
    TriangleList = new List<Triangle>();
    
    // Create super triangle and add to list
    TriangleList.Add(CreateSuperTriangle(vertexList));
    
    // Add each vertex one by one
    for (int i = 0; i < vertexList.Count; i++)
    {
        Vector2 point = vertexList[i];
        List<Triangle> badTriangles = new List<Triangle>();
        
        // Find all triangles whose circumcircle contains this point
        foreach (Triangle triangle in TriangleList)
        {
            if (triangle is null)
            {
                continue;
            }
            
            if (triangle.IsPointInCircumcircle(point))
            {
                badTriangles.Add(triangle);
            }
        }
        
        // Find the boundary of the polygonal hole
        List<Edge> polygon = new List<Edge>();
        
        foreach (Triangle triangle in badTriangles)
        {
            Edge edge1 = new Edge(triangle.vertex1, triangle.vertex2);
            Edge edge2 = new Edge(triangle.vertex2, triangle.vertex3);
            Edge edge3 = new Edge(triangle.vertex3, triangle.vertex1);
            
            // Add edge if it's not shared by another bad triangle
            if (!IsEdgeShared(edge1, triangle, badTriangles))
                polygon.Add(edge1);
            if (!IsEdgeShared(edge2, triangle, badTriangles))
                polygon.Add(edge2);
            if (!IsEdgeShared(edge3, triangle, badTriangles))
                polygon.Add(edge3);
        }
        
        // Remove bad triangles
        foreach (Triangle triangle in badTriangles)
        {
            TriangleList.Remove(triangle);
        }
        
        // Create new triangles from the point to each edge of the polygon
        foreach (Edge edge in polygon)
        {
            TriangleList.Add(new Triangle(edge.p1, edge.p2, point));
        }
    }

    if (TriangleList.Count < 0)
        return null;

    if (TriangleList[0] is null)
        return null;
    
    // Remove triangles that share vertices with super triangle
    Vector2 superVertex1 = TriangleList[0].vertex1; // Store super triangle vertices before removal
    TriangleList.RemoveAll(t => 
        t.HasVertex(CreateSuperTriangle(vertexList).vertex1) ||
        t.HasVertex(CreateSuperTriangle(vertexList).vertex2) ||
        t.HasVertex(CreateSuperTriangle(vertexList).vertex3));
    
    VisualiseTriangleList();
    return TriangleList;
}

private bool IsEdgeShared(Edge edge, Triangle currentTriangle, List<Triangle> triangles)
{
    foreach (Triangle triangle in triangles)
    {
        if (triangle == currentTriangle)
            continue;
            
        if (triangle.HasEdge(edge))
            return true;
    }
    return false;
}

Triangle CreateSuperTriangle(List<Vector2> points)
{
    if(points is null)
        return null;
    
    if(points.Count < 3)
        return null;
    
    
    // Find minimum and maximum coordinates
    var max_x = points.Max(p => p.x);
    var max_y = points.Max(p => p.y);
    var min_x = points.Min(p => p.x);
    var min_y = points.Min(p => p.y);
    
    // Define vertices for the super triangle
    float a = max_x - min_x;
    float b = max_y - min_y;
    float dx = ((a > b) ? a : b) * MultiplierSuperTriangle;
    
    Triangle super_triangle = new Triangle(
        new Vector2(min_x - dx, min_y - dx),
        new Vector2(max_x + dx, min_y - dx),
        new Vector2((min_x + max_x) / 2, max_y + dx * 2));
    
    return super_triangle;
}

List<GameObject> EdgeList = new List<GameObject>();      

private void VisualiseTriangleList()
{

    if (EdgeList.Count > 0)
    {
        foreach (var edge in EdgeList)
        {
            Destroy(edge);
        }
    }
    
    foreach(Triangle triangle in TriangleList)
    {
        EdgeList.Add(triangle.VisualizeEdges());
    }
}

// Helper class for edges



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
