using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeVertex
{
    public Vector3 point = Vector3.zero;
    public float radius = 1.0f;
    public Color color = Color.white;

	public TubeVertex(Vector3 pt, float r, Color c)
    {
        point = pt;
        radius = r;
        color = c;
    }
}

public class TubeRenderer2 : MonoBehaviour
{
    public TubeVertex[] vertices;
    public Material material;
    public int crossSegments = 3;
    public float flatAtDistance = -1f;
    public float movePixelsForRebuild = 6f;
    public float maxRebuildTime = 0.1f;
    public bool useMeshCollision = false;

    //private Vector3 lastCameraPosition1;
    //private Vector3 lastCameraPosition2;
    private Vector3[] crossPoints;
    private int lastCrossSegments;
    //private float lastRebuildTime = 0.00f;
    private Mesh mesh;
    private bool colliderExists = false;
    private bool usingBumpmap = false;

    void Reset()
    {
        vertices = new TubeVertex[] { new TubeVertex(Vector3.zero, 1.0f, Color.white), new TubeVertex(new Vector3(1, 0, 0), 1.0f, Color.white)};
    }

    //sets all the points to points of a Vector3 array, as well as capping the ends.
    public void SetPoints(Vector3[] points, float radius, Color col)
    {
        if (points.Length < 2) return;
        vertices = new TubeVertex[points.Length + 2];

        Vector3 v0offset = (points[0] - points[1]) * 0.01f;
        vertices[0] = new TubeVertex(v0offset + points[0], 0.0f, col);
        Vector3 v1offset = (points[points.Length - 1] - points[points.Length - 2]) * 0.01f;
        vertices[vertices.Length - 1] = new TubeVertex(v1offset + points[points.Length - 1], 0.0f, col);

        for (int p = 0; p < points.Length; p++)
        {
            vertices[p + 1] = new TubeVertex(points[p], radius, col);
        }
        /*
        Debug.Log("vertices.Length = " + vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log( i + " " + vertices[i].point);
        }
        */
    }

    void Start ()
    {
        Reset();
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = material;
        if ( material != null )
        {
            if (material.GetTexture("_BumpMap")) usingBumpmap = true;
        }
    }

    void LateUpdate()
    {
        if ( vertices == null || vertices.Length <= 1)
        {
            GetComponent<Renderer>().enabled = false;
            return;
        }

        GetComponent<Renderer>().enabled = true;
        if (crossSegments != lastCrossSegments)
        {
            crossPoints = new Vector3[crossSegments];
            float theta = 2.0f * Mathf.PI / crossSegments;
            for (int c = 0; c < crossSegments; c++)
            {
                crossPoints[c] = new Vector3(Mathf.Cos(theta * c), Mathf.Sin(theta * c), 0);
            }
            lastCrossSegments = crossSegments;
        }

        Vector3[] meshVertices = new Vector3[vertices.Length * crossSegments];
        Vector2[] uvs = new Vector2[vertices.Length * crossSegments];
        Color[] colors = new Color[vertices.Length * crossSegments];
        int[] tris = new int[vertices.Length * crossSegments * 6];
        int[] lastVertices = new int[crossSegments];
        int[] theseVertices = new int[crossSegments];
        Quaternion rotation = Quaternion.identity;

        // создаем вершины
        int p;
        for ( p=0; p<vertices.Length; p++ )
        {
            if (p < vertices.Length - 1)
                rotation = Quaternion.FromToRotation(Vector3.forward, vertices[p + 1].point - vertices[p].point);
            for (int c = 0; c < crossSegments; c++)
            {
                int vertexIndex = p * crossSegments + c;
                meshVertices[vertexIndex] = vertices[p].point + rotation * crossPoints[c] * vertices[p].radius;
                uvs[vertexIndex] = new Vector2((0.0f + c) / crossSegments, (0.0f + p) / vertices.Length);
                colors[vertexIndex] = vertices[p].color;
                lastVertices[c] = theseVertices[c];
                theseVertices[c] = p * crossSegments + c;
            }
        // создаем треугольники
            if (p > 0)
            {
                //Debug.Log("tris.Length = " + tris.Length);
                for ( int c = 0; c < crossSegments; c++)
                {
                    int start = (p * crossSegments + c) * 6;
                    //Debug.Log("start = " + start);
                    tris[start] = lastVertices[c];
                    tris[start + 1] = lastVertices[(c + 1) % crossSegments];
                    tris[start + 2] = theseVertices[c];
                    tris[start + 3] = tris[start + 2];
                    tris[start + 4] = tris[start + 1];
                    tris[start + 5] = theseVertices[(c + 1) % crossSegments];
                }
            }
        }

        //Clear mesh for new build  (jf)	
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        if (usingBumpmap)
            mesh.tangents = CalculateTangents(meshVertices);
        mesh.uv = uvs;

        if (useMeshCollision)
            if (colliderExists)
            {
                gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
            else
            {
                gameObject.AddComponent<MeshCollider>();
                colliderExists = true;
            }
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private Vector4[] CalculateTangents(Vector3[] verts )
    {
        Vector4[] tangents = new Vector4[verts.Length];

        for (int i = 0; i < tangents.Length; i++)
	{
            Vector3 vertex1 = i > 0 ? verts[i - 1] :  verts[i];
            Vector3 vertex2 = i < tangents.Length - 1 ? verts[i + 1] : verts[i];
            Vector3 tan = (vertex1 - vertex2).normalized;
            tangents[i] = new Vector4(tan.x, tan.y, tan.z, 1.0f);
        }
        return tangents;
    }

}
