using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс для создания трубы по массиву точек и радиусу
public class TubeRenderer4 : MonoBehaviour
{
    // точки вдоль центральной оси
    public TubeVertex[] vertices;
    public Material material;
    // на сколько секторов делится окружность
    public int crossSegments = 3;
    public bool useMeshCollision = false;

    private Vector3[] crossPoints;
    private int lastCrossSegments;
    private Mesh mesh;
    private bool colliderExists = false;
    private bool usingBumpmap = false;

    //sets all the points to points of a Vector3 array, as well as capping the ends.
    public void SetPoints(Vector3[] points, float radius, Color col)
    {
        if (points.Length < 2) return;
        //vertices = new TubeVertex[points.Length + 2];
        vertices = new TubeVertex[points.Length];

        /*
        // создаем нулевую точку смещением от points[0] в направлении "наружу"
        Vector3 v0offset = (points[0] - points[1]) * 0.01f;
        vertices[0] = new TubeVertex(v0offset + points[0], 0.0f, col);
        // создаем последнюю точку смещением от points[Length - 1] в направлении "наружу"
        Vector3 v1offset = (points[points.Length - 1] - points[points.Length - 2]) * 0.01f;
        vertices[vertices.Length - 1] = new TubeVertex(v1offset + points[points.Length - 1], 0.0f, col);
        */
        // остальные точки - это координаты шаров, их просто копируем
        for (int p = 0; p < points.Length; p++)
        {
            //vertices[p + 1] = new TubeVertex(points[p], radius, col);
            vertices[p] = new TubeVertex(points[p], radius, col);
        }
    }

    void Start()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = material;
        if (material != null)
        {
            if (material.GetTexture("_BumpMap")) usingBumpmap = true;
        }
    }

    void LateUpdate()
    {
        if (vertices == null || vertices.Length <= 1)
        {
            GetComponent<Renderer>().enabled = false;
            return;
        }

        GetComponent<Renderer>().enabled = true;
        if (crossSegments != lastCrossSegments)
        {
            crossPoints = new Vector3[crossSegments];   // точки на окружности
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
        int trisNum = vertices.Length * crossSegments * 6;
        int[] tris = new int[trisNum];
        int[] lastVertices = new int[crossSegments];
        int[] theseVertices = new int[crossSegments];
        Quaternion rotation = Quaternion.identity;

        // создаем вершины
        int p;
        for (p = 0; p < vertices.Length; p++)
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
                for (int c = 0; c < crossSegments; c++)
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

    private Vector4[] CalculateTangents(Vector3[] verts)
    {
        Vector4[] tangents = new Vector4[verts.Length];

        for (int i = 0; i < tangents.Length; i++)
        {
            Vector3 vertex1 = i > 0 ? verts[i - 1] : verts[i];
            Vector3 vertex2 = i < tangents.Length - 1 ? verts[i + 1] : verts[i];
            Vector3 tan = (vertex1 - vertex2).normalized;
            tangents[i] = new Vector4(tan.x, tan.y, tan.z, 1.0f);
        }
        return tangents;
    }

}
