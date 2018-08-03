using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rope_tube2 : MonoBehaviour
{
    public Transform targetTransform;
    public Material material;
    public float ropeWidth = 0.5f;
    public float resolution = 0.5f;
    public float ropeDrag = 0.1f;
    public float ropeMass = 0.5f;
    public int radialSegments = 6;
    public bool startRestrained = true;
    public bool endRestrained = false;
    public bool useMeshCollision = false;

    // Private Variables (Only change if you know what your doing)
    private Vector3[] _segmentPos;
    private GameObject[] _parts;        
    private GameObject _tubeRendererObject;
    private TubeRenderer2 _tube;
    private int _segments = 4;
    private bool _isTube = false;

    private Vector3 _jointAxis;      // для правильной ориентации соединения Ф.

    //Joint Settings
    public Vector3 swingAxis = new Vector3(0, 1, 0);
    public float lowTwistLimit = 0.0f;
    public float highTwistLimit = 0.0f;
    public float swing1Limit = 20.0f;
    // Fedor
    public bool enableProjection = true;

    void OnDrawGizmos()
    {
        if (targetTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetTransform.position);
            Gizmos.DrawWireSphere((transform.position + targetTransform.position) / 2, ropeWidth);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ropeWidth);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetTransform.position, ropeWidth);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ropeWidth);
        }
    }

    void Awake()
    {
        if (targetTransform != null)
        {
            BuildRope();
        }
        else
        {
            Debug.LogError("You must have a gameobject attached to target: " + this.name, this);
        }
    }

    void LateUpdate()
    {
        if (targetTransform != null)
        {
            // Does rope exist? If so, update its position
            if (_isTube)
            {
                _tube.SetPoints(_segmentPos, ropeWidth, Color.white);
                _tube.enabled = true;
                _segmentPos[0] = transform.position;
                for (int s = 1; s < _segments; s++)
                {
                    _segmentPos[s] = _parts[s].transform.position;
                }
            }
        }
    }

    private void BuildRope()
    {
        // segments - количество сегментов: [resolution of 1.0 = 1 joint per unit of distance]
        _segments = Mathf.FloorToInt(Vector3.Distance(transform.position, targetTransform.position) * resolution);

        if (material != null)
        {
            material.SetTextureScale("_MainTex", new Vector2(1, _segments + 2));
            if (material.GetTexture("_BumpMap"))
                material.SetTextureScale("_BumpMap", new Vector2(1, _segments + 2));
        }

        _segmentPos = new Vector3[_segments];
        _parts = new GameObject[_segments];
        _segmentPos[0] = transform.position;
        _segmentPos[_segments - 1] = targetTransform.position;

        int segs = _segments - 1;        // количество интервалов
        Vector3 seperation = ((targetTransform.position - transform.position) / segs);
        _jointAxis = seperation;         // направление соединений
       // for (int s = 0; s < _segments; s++)
        for (int s = 0; s < _segments-1; s++)
        {
            Vector3 vector = (seperation * s) + transform.position; // позиции сегментов
            _segmentPos[s] = vector;
            _parts[s] = new GameObject("Joint_" + s);
            AddJointPhysics(s);             // добавление к каждому сегменту физики и соединений
        }

        _segmentPos[_segments - 1] = targetTransform.position;
        _parts[_segments - 1] = targetTransform.gameObject;
        AddJointPhysics(_segments - 1);

        if (endRestrained)
        {
            targetTransform.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (startRestrained)
        {
            transform.GetComponent< Rigidbody > ().isKinematic = true;
        }

        // запретить коллизию объектов, идущих через одного
        for( int i=0; i<_parts.Length-2; ++i )
        {
            Collider col1 = _parts[i].GetComponent<Collider>();
            Collider col2 = _parts[i+2].GetComponent<Collider>();
            Physics.IgnoreCollision(col1, col2);
        }

        _tubeRendererObject = new GameObject("TubeRenderer_" + gameObject.name);
        // присоединяем к GameObject класс, получаем ссылку на объект этого класса
        _tube = _tubeRendererObject.AddComponent<TubeRenderer2>();  // теперь _tube - экземпляр TubeRenderer2  
        _tube.useMeshCollision = useMeshCollision;
        _tube.vertices = new TubeVertex[_segments];
        _tube.crossSegments = radialSegments;
        _tube.material = material;

        _isTube = true;        // Rope = true, The rope now exists in the scene!
    }

    private void AddJointPhysics(int n)
    {
        //_parts[n] = new GameObject("Joint_" + n);
        _parts[n].transform.parent = transform;     // сделали созданный объект дочерним данного
        Rigidbody rigid = _parts[n].AddComponent<Rigidbody>();

        if (!useMeshCollision)
        {
            SphereCollider col = _parts[n].AddComponent<SphereCollider>();
            col.radius = ropeWidth;
        }
        CharacterJoint ph = _parts[n].AddComponent<CharacterJoint>();

        SetJointParam(ph);

        _parts[n].transform.position = _segmentPos[n];
        rigid.drag = ropeDrag;
        rigid.mass = ropeMass;

        rigid.useGravity = gameObject.GetComponent<Rigidbody>().useGravity;
        ph.enableProjection = enableProjection; // возможно, будет восстанавливать порядок после разрушения

        if (n == 0)
        {
            ph.connectedBody = transform.GetComponent<Rigidbody>();     // зачем нулевой шарик конектится к родителю?
        }
        else
        {
            ph.connectedBody = _parts[n - 1].GetComponent<Rigidbody>();
        }
    }

    private void SetJointParam(CharacterJoint ph)
    {
        ph.axis = _jointAxis;           // Ф.
        ph.swingAxis = swingAxis;
        SoftJointLimit limit1 = ph.lowTwistLimit;
        limit1.limit = lowTwistLimit;
        ph.lowTwistLimit = limit1;
        SoftJointLimit limit2 = ph.highTwistLimit;
        limit2.limit = highTwistLimit;
        ph.highTwistLimit = limit2;
        SoftJointLimit limit3 = ph.swing1Limit;
        limit3.limit = swing1Limit;
        ph.swing1Limit = limit3;
    }
}
