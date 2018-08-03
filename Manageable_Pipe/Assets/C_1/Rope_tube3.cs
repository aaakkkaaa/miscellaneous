using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Соединение - пружина
public class Rope_tube3 : MonoBehaviour
{
    public Transform targetTransform;            // конечная точка
    public Material material;
    public float ropeWidth = 0.3f;
    public float resolution = 3.0f;
    public int radialSegments = 6;
    public bool startRestrained = false;
    public bool endRestrained = false;

    // параметры соединения
    public float Spring = 100;
    public float Damper = 100;
    public float MinDistance = 0;
    public float MaxDistance = 0;
    public bool enableProjection = true;
    // моделируемые параметры соединения
    public float AngularLuft = 3;               // TODO ?
    public float AngularForce = 0.1f;           // TODO ?
    // параметры объектов
    public float ObjMass = 0.1f;
    public float ObjLinearDrag = 3;
    public float ObjAngularDrag = 3;

    // объекты, участвующие в моделировании
    private GameObject[] _parts;
    // координаты объектов, участвующих в моделировании
    private Vector3[] _segmentPos;
    // количество сегментов, вычисляется с учетом resolution
    private int _segments;
    // объект, к которому будет присоединен класс (компонент) TubeRenderer2
    private GameObject _tubeRendererObject;
    // экземпляр класса TubeRenderer2
    private TubeRenderer2 _tube;
    // признак, что перваночальный рассчет был проведен, меш трубы уже создан
    private bool _isTube = false;
    // для правильной ориентации соединения
    private Vector3 _jointAxis;

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
            if (_isTube)        // если труба уже создана
            {
                _tube.SetPoints(_segmentPos, ropeWidth, Color.white);
                _tube.enabled = true;

                //_segmentPos[0] = transform.position;
                for (int s = 0; s < _segments; s++)
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

        Vector3 seperation = ((targetTransform.position - transform.position) / (_segments - 1) );
        _jointAxis = seperation;         // направление соединений (для пружинного соединения не используется)

        for (int s = 0; s < _segments - 1; s++)
        {
            Vector3 vector = (seperation * s) + transform.position; // позиции объектов
            _segmentPos[s] = vector;
            _parts[s] = new GameObject("Joint_" + s);
            _parts[s].transform.parent = transform;     // сделали созданный объект дочерним данного
            AddJointPhysics(s);                         // добавление к каждому объекту физики и соединений
        }
        targetTransform.transform.parent = transform;
        _segmentPos[_segments - 1] = targetTransform.position;
        _parts[_segments - 1] = targetTransform.gameObject;
        AddJointPhysics(_segments - 1);

        if (endRestrained)
        {
            targetTransform.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (startRestrained)
        {
            _parts[0].GetComponent<Rigidbody>().isKinematic = true;
        }

        // запретить коллизию объектов, идущих через одного
        for (int i = 0; i < _parts.Length - 2; ++i)
        {
            Collider col1 = _parts[i].GetComponent<Collider>();
            Collider col2 = _parts[i + 2].GetComponent<Collider>();
            Physics.IgnoreCollision(col1, col2);
        }

        // создание объекта - рисовальщика меша
        _tubeRendererObject = new GameObject("TubeRenderer_" + gameObject.name);
        // присоединяем к GameObject класс, получаем ссылку на объект этого класса
        _tube = _tubeRendererObject.AddComponent<TubeRenderer2>();  // теперь _tube - экземпляр TubeRenderer2  
        _tube.useMeshCollision = false;
        _tube.vertices = new TubeVertex[_segments];
        _tube.crossSegments = radialSegments;
        _tube.material = material;

        _isTube = true;        // Труба создана и присутствует на сцене

    }

    private void AddJointPhysics(int n)
    {
        _parts[n].transform.position = _segmentPos[n];

        SphereCollider col = _parts[n].AddComponent<SphereCollider>();
        col.radius = ropeWidth;

        Rigidbody rigid = _parts[n].AddComponent<Rigidbody>();
        rigid.drag = ObjLinearDrag;
        rigid.angularDrag = ObjAngularDrag;
        rigid.mass = ObjMass;
        rigid.useGravity = gameObject.GetComponent<Rigidbody>().useGravity;
        
        if (n != 0)
        {
            SpringJoint joint = _parts[n].AddComponent<SpringJoint>();
            joint.axis = _jointAxis;
            joint.spring = Spring;
            joint.damper = Damper;
            joint.enableCollision = false;
            joint.connectedBody = _parts[n - 1].GetComponent<Rigidbody>();
        }
        
    }



}
