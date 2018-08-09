using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Соединение через скрипт
public class Rope_tube4 : MonoBehaviour
{
    public Transform targetTransform;            // конечная точка
    public Material material;
    public Material SharMaterial;
    public float ropeWidth = 0.3f;
    public float resolution = 3.0f;
    public int radialSegments = 6;
    public bool startRestrained = false;
    public bool endRestrained = false;

    // параметры объектов
    public float ObjMass = 0.1f;
    public float ObjLinearDrag = 3;
    public float ObjAngularDrag = 3;

    // параметры соединения
    public float DragСompensation = 0.9f;       // на сколько за шаг устаняется возникшее линейное расхождение при таскании
    public float FreeСompensation = 0.05f;      // на сколько за шаг устаняется возникшее линейное расхождение без таскания
    public float CollСompensation = 0.05f;      // на сколько за шаг устаняется возникшее линейное расхождение при коллизии
    public float Precision = 0.01f;             // если расхождение меньше, переставать двигать шар 

    public float MinDistance = 0.02f;           // минимально допустимое сближение
    public float MaxDistance = 0.3f;            // максимально допустимое расхождения

    public float Dist = 0f;                     // вычисляемое при построении расстояние между объектами

    // объекты, участвующие в моделировании
    private GameObject[] _parts;
    // координаты объектов, участвующих в моделировании
    private Vector3[] _segmentPos;
    // количество сегментов, вычисляется с учетом resolution
    private int _segments;
    // объект, к которому будет присоединен класс (компонент) TubeRenderer2
    private GameObject _tubeRendererObject;
    // экземпляр класса TubeRenderer2
    private TubeRenderer4 _tube;
    // признак, что первоначальный рассчет был проведен, меш трубы уже создан
    private bool _isTube = false;
    // для правильной ориентации соединения
    private Vector3 _jointAxis;

    // индекс шара, за который тащим
    public int _dragIndex = -1;
    // индексы закрепленных шаров
    public List<int> fixedIdx;

    // назначить шар, за который таскаем, если передать null - перестаем таскать
    public void SetCurDragObject(GameObject shar)
    {
        _dragIndex = -1;
        if (shar == null) return;
        _dragIndex = shar.GetComponent<TubeShar>().Index;
        print(_dragIndex);
    }
    // Можно просто указать индекс (в массиве _parts) таскаемого шара
    public void SetCurDragIndex(int idx)
    {
        _dragIndex = -1;
        if ((idx >= 0) && (idx < _parts.Length)) _dragIndex = idx;

    }

    // Закрепить или освободить шар
    public void FixObject(GameObject shar, bool needFix )
    {
        int idx = shar.GetComponent<TubeShar>().Index;
        if(needFix)
        {
            if( !fixedIdx.Contains(idx) )
            {
                fixedIdx.Add(idx);
                shar.GetComponent<Rigidbody>().isKinematic = true;
                shar.GetComponent<TubeShar>().isFixed = true;
            }
        }
        else
        {
            shar.GetComponent<Rigidbody>().isKinematic = false;
            shar.GetComponent<TubeShar>().isFixed = false;
            fixedIdx.Remove(idx);
        }
        // TODO - возможно, массив индексов не нужен, если есть информация в самом шаре
        // можно вести пересчет позиций от таскаемого шара до ближайшего закрепленного
    }

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
                for (int s = 0; s < _segments; s++)
                {
                    _segmentPos[s] = _parts[s].transform.position;
                }
            }
        }
    }

    // смещение при таскании
    private void FixedUpdate()
    {
        GameObject curObj, mainObj;
       // TubeShar shar;
        int i;

        if (!_isTube) return;

        // цикл для разрешения имеющихся коллизий
        for (i = 0; i < _parts.Length - 1; ++i)
        {
            curObj = _parts[i];
            if (curObj.GetComponent<TubeShar>().isCollision)
            {
                curObj.transform.position += curObj.GetComponent<TubeShar>().ColliderImpulse;
            }
        }

        if (_dragIndex != -1)        // идет таскание
        {
            // цикл от таскаемого объекта к меньшим индексам
            for (i= _dragIndex-1; i>=0; --i )
            {
                mainObj = _parts[i+1];
                curObj = _parts[i];
                CorrectPos(mainObj, curObj, DragСompensation, i);
            }
            // цикл от таскаемого объекта к большим индексам
            for( i=_dragIndex; i<_parts.Length-1; ++i )
            {
                mainObj = _parts[i];
                curObj = _parts[i+1];
                CorrectPos(mainObj, curObj, DragСompensation, i);
            }
        }
        else                        // нет таскания
        {
            for (i = 0; i < _parts.Length - 1; ++i)
            {
                mainObj = _parts[i];
                curObj = _parts[i + 1];
                CorrectPos(mainObj, curObj, FreeСompensation, i);
                CorrectPos(curObj, mainObj, FreeСompensation, i);
            }

        }
    }

    private void CorrectPos(GameObject mainObj, GameObject curObj, float Сompensation, int i )
    {
        Vector3 direct, newCurPos;
        float curDist, correct;

        direct = curObj.transform.position - mainObj.transform.position;
        curDist = direct.magnitude;
        if (Mathf.Abs(Dist - curDist) > Precision)
        {
            correct = (curDist / Dist - 1) * Сompensation + 1;
            //print(i + " " + correct);
            newCurPos = mainObj.transform.position + direct / correct;
            curObj.transform.position = newCurPos;
        }
    }

    // Построение трубы по начальной и конечной точкам
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

        Vector3 seperation = ((targetTransform.position - transform.position) / (_segments - 1));
        Dist = seperation.magnitude;
        _jointAxis = seperation;         // направление соединений (для пружинного соединения не используется)

        for (int s = 0; s < _segments - 1; s++)
        {
            Vector3 vector = (seperation * s) + transform.position; // позиции объектов
            _segmentPos[s] = vector;
            _parts[s] = new GameObject("Joint_" + s);
            _parts[s].transform.parent = transform;     // сделали созданный объект дочерним данного
            _parts[s].transform.position = _segmentPos[s];
            AddJointPhysics(s);                         // добавление к каждому объекту колайдера и rigidbody
        }
        targetTransform.transform.parent = transform;   // присоединили к объектам хвостовой
        _segmentPos[_segments - 1] = targetTransform.position;
        _parts[_segments - 1] = targetTransform.gameObject;
        AddJointPhysics(_segments - 1);

        // запретить коллизию объектов, идущих через подряд
        for (int i = 0; i < _parts.Length - 1; ++i)
        {
            Collider col1 = _parts[i].GetComponent<Collider>();
            Collider col2 = _parts[i + 1].GetComponent<Collider>();
            Physics.IgnoreCollision(col1, col2);
        }
        // запретить коллизию объектов, идущих через одного
        for (int i = 0; i < _parts.Length - 2; ++i)
        {
            Collider col1 = _parts[i].GetComponent<Collider>();
            Collider col2 = _parts[i + 2].GetComponent<Collider>();
            Physics.IgnoreCollision(col1, col2);
        }

        // ориентация объектов, ось x по направлению соединения 
        Vector3 dirX = _jointAxis.normalized;
        for (int i = 0; i < _parts.Length; ++i)
        {
            Quaternion q = _parts[i].transform.rotation;
            q.SetFromToRotation(new Vector3(1, 0, 0), dirX);
            _parts[i].transform.rotation = q;
        }

        if (endRestrained)
        {
            targetTransform.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (startRestrained)
        {
            _parts[0].GetComponent<Rigidbody>().isKinematic = true;
        }

        // создание объекта - рисовальщика меша
        _tubeRendererObject = new GameObject("TubeRenderer_" + gameObject.name);
        // присоединяем к GameObject класс, получаем ссылку на объект этого класса
        _tube = _tubeRendererObject.AddComponent<TubeRenderer4>();  // теперь _tube - экземпляр TubeRenderer2  
        _tube.useMeshCollision = false;
        _tube.vertices = new TubeVertex[_segments];
        _tube.crossSegments = radialSegments;
        _tube.material = material;

        _isTube = true;        // Труба создана и присутствует на сцене
    }

    private void AddJointPhysics(int n)
    {
        SphereCollider col = _parts[n].AddComponent<SphereCollider>();
        col.radius = ropeWidth;
        //SharMaterial
        Rigidbody rigid = _parts[n].AddComponent<Rigidbody>();
        TubeShar tubeShar = _parts[n].AddComponent<TubeShar>();
//        Debug.Log("_parts[n]= " + _parts[n]);
        Mesh mesh = new Mesh();
        MeshFilter MF1 = _parts[n].AddComponent<MeshFilter>();
        MeshRenderer MR1 = _parts[n].AddComponent<MeshRenderer>();
        MR1.material = SharMaterial;
//        Debug.Log("MR1= " + MR1);
        MF1.mesh = mesh;
        OVRGrabbable myGrab1 =_parts[n].AddComponent<OVRGrabbable>();

        tubeShar.Index = n;

        rigid.drag = ObjLinearDrag;
        rigid.angularDrag = ObjAngularDrag;
        rigid.mass = ObjMass;
        rigid.useGravity = false;
    }



}
