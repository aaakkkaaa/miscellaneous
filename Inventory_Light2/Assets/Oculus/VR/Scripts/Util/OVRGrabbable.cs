/************************************************************************************

Copyright   :   Copyright 2017 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.4.1 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.4.1

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// An object that can be grabbed and thrown by OVRGrabber.
/// </summary>
public class OVRGrabbable : MonoBehaviour
{
    [SerializeField]
    protected bool m_allowOffhandGrab = true;
    [SerializeField]
    protected bool m_snapPosition = false;
    [SerializeField]
    protected bool m_snapOrientation = false;
    [SerializeField]
    protected Transform m_snapOffset;
    [SerializeField]
    protected Collider[] m_grabPoints = null;

    protected bool m_grabbedKinematic = false;
    protected Collider m_grabbedCollider = null;
    protected OVRGrabber m_grabbedBy = null;

    // Mitya +++++++++++++
    protected bool inPlace = false;
    protected Collider placeCollider = null;
    //    protected string placeName = "DropCube";    
    protected Color oldColor;

    [SerializeField]
    protected Vector3 Position0;

    protected Quaternion Rotation0;

    float myVelocityX1 = 0.0F;
    float myVelocityY1 = 0.0F;
    float myVelocityZ1 = 0.0F;

    float myVelocityX2 = 0.0F;
    float myVelocityY2 = 0.0F;
    float myVelocityZ2 = 0.0F;

    float myBeginTime = 0.0f;

    public bool isAnimation = false;

    // Mitya -------------

    /// <summary>
    /// If true, the object can currently be grabbed.
    /// </summary>
    public bool allowOffhandGrab
    {
        get { return m_allowOffhandGrab; }
    }

	/// <summary>
	/// If true, the object is currently grabbed.
	/// </summary>
    public bool isGrabbed
    {
        
        get { Debug.Log("isGrabbed");  return m_grabbedBy != null; }
    }

	/// <summary>
	/// If true, the object's position will snap to match snapOffset when grabbed.
	/// </summary>
    public bool snapPosition
    {
        get {/* Debug.Log("snapPosition"); */ return m_snapPosition; }
    }

	/// <summary>
	/// If true, the object's orientation will snap to match snapOffset when grabbed.
	/// </summary>
    public bool snapOrientation
    {
        get {/* Debug.Log("snapOrientation"); */ return m_snapOrientation; }
    }

	/// <summary>
	/// An offset relative to the OVRGrabber where this object can snap when grabbed.
	/// </summary>
    public Transform snapOffset
    {
        get { /* Debug.Log("snapOffset"); */ return m_snapOffset; }
    }

	/// <summary>
	/// Returns the OVRGrabber currently grabbing this object.
	/// </summary>
    public OVRGrabber grabbedBy
    {
        get {/* Debug.Log("grabbedBy"); */ return m_grabbedBy; }
    }

	/// <summary>
	/// The transform at which this object was grabbed.
	/// </summary>
    public Transform grabbedTransform
    {
        get { /* Debug.Log("grabbedTransform"); */ return m_grabbedCollider.transform; }
    }

	/// <summary>
	/// The Rigidbody of the collider that was used to grab this object.
	/// </summary>
    public Rigidbody grabbedRigidbody
    {
        get {/* Debug.Log("grabbedRigidbody");*/ return m_grabbedCollider.attachedRigidbody; }
    }

	/// <summary>
	/// The contact point(s) where the object was grabbed.
	/// </summary>
    public Collider[] grabPoints
    {
        get {
            ////Debug.Log("grabPoints");
            return m_grabPoints;
        }
    }

	/// <summary>
	/// Notifies the object that it has been grabbed.
	/// </summary>
	virtual public void GrabBegin(OVRGrabber hand, Collider grabPoint)
	{
		// Mitya +++++++++
		// GrabBegin вызывается из OVRGrabber
		// hand - LeftHandAnchor или RightHandAnchor

		Debug.Log("GrabBegin " + this.gameObject.name);

		m_grabbedBy = hand;
		m_grabbedCollider = grabPoint;

        gameObject.GetComponent<Rigidbody>().isKinematic = true; // ???

        if (tag == "Hold")
		{
//			gameObject.GetComponent<Rigidbody>().isKinematic = true;

		}
        else if (tag == "Cap")
        {
//            gameObject.GetComponent<Rigidbody>().isKinematic = true;
///            Position0 = transform.localPosition;
///            Rotation0 = transform.rotation;
        }
        else if (tag == "Reader")
		{
//            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            // Запомнили родителя
            Transform myParent = transform.parent;

			// Сделали родителем руку (LeftHandAnchor или RightHandAnchor)
			// (Чтобы задать локальные координаты)
			transform.parent = m_grabbedBy.transform;

			// Объект прыгает в руку (в определенные относительные координаты)
			if (hand.name == "RightHandAnchor")
				transform.localPosition = new Vector3(0.02f,0.04f,-0.03f);
			else
				transform.localPosition = new Vector3(-0.02f, 0.04f, -0.03f);

			// Нулевые углы
			transform.localEulerAngles = Vector3.zero;

			// Вернули родителя
			transform.parent = myParent;

// ???
//            gameObject.GetComponent<Rigidbody>().isKinematic = true;


			/*
						myBeginTime = Time.time;
						gameObject.GetComponent<Rigidbody>().isKinematic = true;
						StartCoroutine(FuncMovingReader());
			*/
		}
		else
		{
//			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			if (gameObject.GetComponent<TubeShar>() != null) // Это труба
			{
				transform.parent.gameObject.GetComponent<Rope_tube4>().SetCurDragObject(gameObject);
			}
		}
		// Mitya ---------

	}


	// Coroutine для перемещения объекта в руку (пока не нужно!)
	IEnumerator FuncMovingReader()
	{
		float myTime=Time.time;

		Transform myParent = transform.parent;
		transform.parent = m_grabbedBy.transform;

		Vector3 myNewPos = new Vector3(0.02f, 0.04f, -0.03f);
		Vector3 myPos = transform.localPosition;
		Vector3 myEu = transform.localEulerAngles;
		float myMin = 0.001f;
		float myCenterTime = 0.5f;
		float x0 = myPos.x;
		float y0 = myPos.y;
		float z0 = myPos.z;
		Debug.Log("1 myPos.x=" + myPos.x);
		Debug.Log("1 myPos.y=" + myPos.y);
		Debug.Log("1 myPos.z=" + myPos.z);
		Debug.Log("myTime=" + myTime);
		isAnimation = true;

		while ( (Mathf.Abs(myPos.x - myNewPos.x) > myMin) || (Mathf.Abs(myPos.y - myNewPos.y) > myMin) || (Mathf.Abs(myPos.z - myNewPos.z) > myMin) )
		{

            myTime = Time.time;

            
            //        transform.localEulerAngles = Vector3.zero;

            myEu.x = Mathf.SmoothDampAngle(myEu.x, 0.0f, ref myVelocityX1, myCenterTime);
            myEu.y = Mathf.SmoothDampAngle(myEu.y, 0.0f, ref myVelocityY1, myCenterTime);
            myEu.z = Mathf.SmoothDampAngle(myEu.z, 0.0f, ref myVelocityZ1, myCenterTime);
            transform.localEulerAngles = myEu;

            myPos.x = Mathf.SmoothStep(x0, 0.02f, (myTime - myBeginTime)/myCenterTime);
            myPos.y = Mathf.SmoothStep(x0, 0.04f, (myTime - myBeginTime) / myCenterTime);
            myPos.z = Mathf.SmoothStep(x0, -0.03f, (myTime - myBeginTime) / myCenterTime);

//            myPos.x = Mathf.SmoothDamp(x0, 0.02f, ref myVelocityX2, myCenterTime);
//            myPos.y = Mathf.SmoothDamp(y0, 0.04f, ref myVelocityY2, myCenterTime);
//            myPos.z = Mathf.SmoothDamp(z0, -0.03f, ref myVelocityZ2, myCenterTime);

            transform.localPosition = myPos;
//            Debug.Log("myTime=" + myTime);
//            Debug.Log("myPos.x=" + myPos.x);

            yield return null; // подождать до следующего кадра
        }
        Debug.Log("2 myPos.x=" + myPos.x);
        Debug.Log("2 myPos.y=" + myPos.y);
        Debug.Log("2 myPos.z=" + myPos.z);
        Debug.Log("myTime=" + myTime);
        transform.parent = myParent;
		isAnimation = false;

	}


    /// <summary>
    /// Notifies the object that it has been released.
    /// </summary>
    virtual public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic; // Возвращаем свойство Kinematic=false, чтобы обект продолжал двигаться. Иначе он застывает
        rb.velocity = linearVelocity;
        rb.angularVelocity = angularVelocity;
        m_grabbedBy = null;
        m_grabbedCollider = null;

        if (gameObject.GetComponent<TubeShar>() != null)
        {
            transform.parent.gameObject.GetComponent<Rope_tube4>().SetCurDragObject(null);
        }

        // Mitya +++++++++++++
        ////Debug.Log ("GrabEnd " + this.gameObject.name); 
        ////Debug.Log ("linearVelocity="+linearVelocity); 
        ////Debug.Log ("angularVelocity="+angularVelocity);
        if (inPlace)
        {
            
            //            this.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.2f); // Material Shader - Standard; Rendering Mode - Transparent
            //            this.GetComponent<Renderer>().material.color = Color.clear;
                        Rigidbody body = GetComponent<Rigidbody>();
                        this.transform.position = placeCollider.transform.position;
                        placeCollider.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                        body.velocity = Vector3.zero;
                        body.angularVelocity = Vector3.zero;

        }
        // Mitya -------------
    }

    void Awake()
    {
        //Debug.Log("Awake " + this.gameObject.name);
        //        Debug.Log("m_grabPoints= " + m_grabPoints);
        //        Debug.Log("m_grabPoints.Length= " + m_grabPoints.Length);
        if (m_grabPoints != null)
        {
            if (m_grabPoints.Length == 0)
            {
                //Debug.Log("Awake 1 " + this.gameObject.name);
                // Get the collider from the grabbable
                Collider collider = this.GetComponent<Collider>();
                if (collider == null)
                {
                    throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
                }

                // Create a default grab point
                m_grabPoints = new Collider[1] { collider };
                //Debug.Log("Awake 2 " + this.gameObject.name);
            }
        }
        else
        {
            //Debug.Log("Awake 1 " + this.gameObject.name);
            // Get the collider from the grabbable
            Collider collider = this.GetComponent<Collider>();
            if (collider == null)
            {
                throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
            }

            // Create a default grab point
            m_grabPoints = new Collider[1] { collider };
            //Debug.Log("Awake 2 " + this.gameObject.name);

        }
    }

    protected virtual void Start()
    {
        m_grabbedKinematic = GetComponent<Rigidbody>().isKinematic;

        // Mitya +++++++++
        Position0 = transform.localPosition;
        Rotation0 = transform.rotation;
        // Mitya ---------
    }

    void OnDestroy()
    {
		////Debug.Log ("OnDestroy " + this.gameObject.name);
        if (m_grabbedBy != null)
        {
            // Notify the hand to release destroyed grabbables
            m_grabbedBy.ForceRelease(this);
        }
    }

    // Mitya +++++++++
    // Чтобы в консоли не выводилась лишняя информация: Console - Log Entry - 1 Lines

    // Для того, чтобы объект всегда был на месте, в RigidBody нужно отметить Constraints > Freeze Position и Freeze Rotation
    // А можно в Update/FixedUpdate делать так:			
    //transform.localPosition = Position0;
    //transform.rotation = Rotation0;

    void Update()
    {

        
        if (m_grabbedBy != null)
        {
//            Debug.Log("Update " + this.gameObject.name);
        }


    }


    void FixedUpdate()
    {
        if (m_grabbedBy != null)
        {
//            Debug.Log("FixedUpdate " + this.gameObject.name);
        }

    }


    void OnCollisionEnter(Collision otherCollider)
    {
        ////Debug.Log("OnCollisionEnter");
        ////Debug.Log("Collision Enter: " + this.gameObject.name + "+" + otherCollider.gameObject.name);


        foreach (ContactPoint contact in otherCollider.contacts)
        {
            ////Debug.Log("OnCollisionEnter contact");
            ////Debug.DrawRay(contact.point, contact.normal, Color.red, 2f, true); // ???
        }

        ////Debug.Log("relativeVelocity.magnitude=" + otherCollider.relativeVelocity.magnitude);

/*        if (otherCollider.relativeVelocity.magnitude > 2)
        {
           
        }*/
    }

    void OnCollisionExit(Collision otherCollider)
    {
        ////Debug.Log("OnCollisionExit");
        ////Debug.Log("Collision Exit: " + this.gameObject.name + "+" + otherCollider.gameObject.name);

    }

    void OnTriggerEnter(Collider otherCollider)
    {
        ////Debug.Log("OnTriggerEnter ");
        Debug.Log("Collision with Trigger Enter: " + this.gameObject.name + "+" + otherCollider.gameObject.name);


        //        if (otherCollider.gameObject.name == placeName) // Пересеклись с нужным объектом
        if (tag != "Untagged")
        { 
            if (otherCollider.gameObject.tag.Contains(tag))
            {
                Debug.Log("tag= " + tag);
                oldColor = otherCollider.GetComponent<Renderer>().material.color;
                ////Debug.Log("OnCollisionEnter " + placeName);
                if (m_grabbedBy == null)
                {
                    otherCollider.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f, 0.7f); // Пролетели насквозь
                }
                else
                {
                    otherCollider.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.9f, 0.7f); // Занесли объект, но не отпустили
                }
                inPlace = true;
                placeCollider = otherCollider;
            }
        }

    }

    void OnTriggerExit(Collider otherCollider)
    {
        ////Debug.Log("OnTriggerExit ");
        Debug.Log("Collision with Trigger Exit: " + this.gameObject.name + "+" + otherCollider.gameObject.name);

        //        if (otherCollider.gameObject.name == placeName)
        if (tag != "Untagged")
        {
            if (otherCollider.gameObject.tag.Contains(tag))
            //            if (tag == otherCollider.gameObject.tag)
            {
                //                otherCollider.GetComponent<Renderer>().material.color = new Color(0f, 1.0f, 0f, 0.5f);
                otherCollider.GetComponent<Renderer>().material.color = oldColor;

                ////Debug.Log("OnCollisionExit " + placeName);
                inPlace = false;
            }
        }

    }
    // Mitya ---------


}
