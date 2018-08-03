using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rope_tube : MonoBehaviour
{
    public Transform target;
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
    private Vector3[] segmentPos;
    private GameObject[] joints;
    private GameObject tubeRenderer;
    private TubeRenderer2 line;
    private int segments = 4;
    private bool rope = false;

    //Joint Settings
    public Vector3 swingAxis = new Vector3(0, 1, 0);
    public float lowTwistLimit = 0.0f;
    public float highTwistLimit = 0.0f;
    public float swing1Limit = 20.0f;

    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere((transform.position + target.position) / 2, ropeWidth);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ropeWidth);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, ropeWidth);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ropeWidth);
        }
    }

    void Awake()
    {
        if (target != null)
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
        if (target != null)
        {
            // Does rope exist? If so, update its position
            if (rope)
            {
                line.SetPoints(segmentPos, ropeWidth, Color.white);
                line.enabled = true;
                segmentPos[0] = transform.position;
                for (int s = 1; s < segments; s++)
                {
                    segmentPos[s] = joints[s].transform.position;
                }
            }
        }
    }

    private void BuildRope()
    {
        tubeRenderer = new GameObject("TubeRenderer_" + gameObject.name);
        // присоединяем к GameObject класс, получаем ссылку на объект этого класса
        line = tubeRenderer.AddComponent<TubeRenderer2>();  // теперь line - экземпляр TubeRenderer2  
        line.useMeshCollision = useMeshCollision;
        // Find the amount of segments based on the distance and resolution
        // Example: [resolution of 1.0 = 1 joint per unit of distance]
        segments = Mathf.FloorToInt(Vector3.Distance(transform.position, target.position) * resolution);

        if (material != null)
        {
            material.SetTextureScale("_MainTex", new Vector2(1, segments + 2));
            if (material.GetTexture("_BumpMap"))
                material.SetTextureScale("_BumpMap", new Vector2(1, segments + 2));
        }
        line.vertices = new TubeVertex[segments];
        line.crossSegments = radialSegments;
        line.material = material;
        segmentPos = new Vector3[segments];
        joints = new GameObject[segments];
        segmentPos[0] = transform.position;
        segmentPos[segments - 1] = target.position;

        // Find the distance between each segment
        int segs = segments - 1;
        Vector3 seperation = ((target.position - transform.position) / segs);
        for (int s = 0; s < segments; s++)
        {
            // Find the each segments position using the slope from above
            Vector3 vector = (seperation * s) + transform.position;
            segmentPos[s] = vector;
            //Add Physics to the segments
            AddJointPhysics(s);
        }
        // Attach the joints to the target object and parent it to this object
        CharacterJoint end  = target.gameObject.AddComponent< CharacterJoint > ();
        end.connectedBody = joints[joints.Length - 1].transform.GetComponent< Rigidbody > ();
        end.swingAxis = swingAxis;
        SoftJointLimit limit1 = end.lowTwistLimit;
        limit1.limit = lowTwistLimit;
        end.lowTwistLimit = limit1;
        SoftJointLimit limit2 = end.highTwistLimit;
        limit2.limit = highTwistLimit;
        end.highTwistLimit = limit2;
        SoftJointLimit limit3 = end.swing1Limit;
        limit3.limit = swing1Limit;
        end.swing1Limit = limit3;

        target.parent = transform;
        if (endRestrained)
        {
            end.GetComponent< Rigidbody > ().isKinematic = true;
        }

        // FEDOR ***************** 
        end.GetComponent<Rigidbody>().useGravity = gameObject.GetComponent<Rigidbody>().useGravity;
        // ***********************

        if (startRestrained)
        {
            transform.GetComponent< Rigidbody > ().isKinematic = true;
        }
        // Rope = true, The rope now exists in the scene!
        rope = true;
    }

    private void AddJointPhysics(int n)
    {
        joints[n] = new GameObject("Joint_" + n);
        joints[n].transform.parent = transform;
        Rigidbody rigid = joints[n].AddComponent<Rigidbody>();
        if (!useMeshCollision)
        {
            SphereCollider col = joints[n].AddComponent<SphereCollider>();
            col.radius = ropeWidth;
        }
        CharacterJoint ph = joints[n].AddComponent<CharacterJoint>();
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

        //ph.breakForce = ropeBreakForce; <--------------- TODO
        joints[n].transform.position = segmentPos[n];
        rigid.drag = ropeDrag;
        rigid.mass = ropeMass;

        // FEDOR ***************** 
        rigid.useGravity = gameObject.GetComponent<Rigidbody>().useGravity;
        // ***********************

        if (n == 0)
        {
            ph.connectedBody = transform.GetComponent<Rigidbody>();
        }
        else
        {
            ph.connectedBody = joints[n - 1].GetComponent<Rigidbody>();
        }
    }

}
