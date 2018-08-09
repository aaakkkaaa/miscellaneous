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

// Скрипт присваивается объектам LeftHandAnchor и RightHandAnchor
// Ориентир - синий кубик

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows grabbing and throwing of objects with the OVRGrabbable component on them.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OVRGrabber : MonoBehaviour
{
	// Grip trigger thresholds for picking up objects, with some hysteresis.
	public float grabBegin = 0.55f;
	public float grabEnd = 0.35f;

	// Demonstrates parenting the held object to the hand's transform when grabbed.
	// When false, the grabbed object is moved every FixedUpdate using MovePosition.
	// Note that MovePosition is required for proper physics simulation. If you set this to true, you can
	// easily observe broken physics simulation by, for example, moving the bottom cube of a stacked
	// tower and noting a complete loss of friction.
	[SerializeField]
	protected bool m_parentHeldObject = false;

	// Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
	// Also used for ranking grab targets in case of multiple candidates.
	[SerializeField]
	protected Transform m_gripTransform = null;
	// Child/attached Colliders to detect candidate grabbable objects.
	[SerializeField]
	protected Collider[] m_grabVolumes = null;

	// Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
	[SerializeField]
	protected OVRInput.Controller m_controller;

	[SerializeField]
	protected Transform m_parentTransform;

	protected bool m_grabVolumeEnabled = true;
	protected Vector3 m_lastPos;
	protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
	protected OVRGrabbable m_grabbedObj = null;
//	protected OVRGrabbable m_grabbedObjParent = null;
	protected Vector3 m_grabbedObjectPosOff;
    protected Quaternion m_grabbedObjectRotOff;
	protected Dictionary<OVRGrabbable, int> m_grabCandidates = new Dictionary<OVRGrabbable, int>();
	protected bool operatingWithoutOVRCameraRig = true;

    // Mitya+++
    protected int updateCount = 0;
//    protected Vector3 PositionHand0;
//    protected Quaternion RotationHand0;

    protected GameObject myHandAnchor;
    protected Vector3 myHandAnchorPos;
    protected Quaternion myHandAnchorRot;

    protected GameObject myLocalAvatar;
    protected Vector3 myLocalAvatarPos;
    protected Quaternion myLocalAvatarRot;

	protected GameObject myHand;
	protected Vector3 myHandPos;
	protected Quaternion myHandRot;

//    protected Vector3 myHandLocalPos;

	protected GameObject myPlayerController;
	protected Vector3 myPlayerControllerPos;
	protected Quaternion myPlayerControllerRot;

//	protected Vector3 myDragMove;

	protected int myGrabMoveCount = 0;

	protected GameObject myController;
	protected Vector3 myControllerPos;
	protected Quaternion myControllerRot;

	protected GameObject myISS;

	protected Vector3 myGrabPos0;
	protected Quaternion myGrabRot0;
	
	/*
		protected GameObject PlayerController;
		protected GameObject PlayerControllerLeft;

		protected Vector3 PositionController0;
		protected Quaternion RotationController0;
		protected Vector3 PositionAvatar0;
		protected Quaternion RotationAvatar0;

		protected Vector3 PositionHandLeft0;
		protected Quaternion RotationHandLeft0;
		protected Vector3 PositionHandRight0;
		protected Quaternion RotationHandRight0;

		protected Vector3 PositionControllerLeft0;
		protected Quaternion RotationControllerLeft0;

		protected Vector3 PositionControllerLeft1;
	*/

	// Mitya -------------

	// Mitya---

    /// <summary>
    /// The currently grabbed object.
    /// </summary>
    public OVRGrabbable grabbedObject
	{
		get { return m_grabbedObj; }
	}

	public void ForceRelease(OVRGrabbable grabbable)
	{
        bool canRelease = (
            (m_grabbedObj != null) &&
            (m_grabbedObj == grabbable)
        );
        if (canRelease)
        {
			Debug.Log("GrabEnd"); 			
            GrabEnd();
        }
    }

	protected virtual void Awake()
	{
		m_anchorOffsetPosition = transform.localPosition;
		m_anchorOffsetRotation = transform.localRotation;
//		Debug.Log("m_anchorOffsetPosition "+ m_anchorOffsetPosition.ToString("F4") );
		// В первый момент m_anchorOffsetPosition=0,0,0


		// If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.

		// OVRCameraRig - VR-камера, заменяющая стандартную камеру Unity
		// (parent.parent относительно данного объекта)
		OVRCameraRig rig = null;
		if (transform.parent != null && transform.parent.parent != null)
			rig = transform.parent.parent.GetComponent<OVRCameraRig>();

		if (rig != null)
		{
			rig.UpdatedAnchors += (r) => {OnUpdatedAnchors();};
			operatingWithoutOVRCameraRig = false; // Не нужно
		}

//        myPlayerController = GameObject.Find("OVRPlayerController");
        myPlayerController = GameObject.Find("PlayerController");

    }

    protected virtual void Start()
	{
		m_lastPos = transform.position;
		m_lastRot = transform.rotation;
		if(m_parentTransform == null)
		{
			if(gameObject.transform.parent != null)
			{
				m_parentTransform = gameObject.transform.parent.transform;
			}
			else
			{
				m_parentTransform = new GameObject().transform;
				m_parentTransform.position = Vector3.zero;
				m_parentTransform.rotation = Quaternion.identity;
			}
		}
	}

	// Mitya ++++++++
	void Update()
	{
			OnUpdatedAnchors();

	}
	// Mitya ---------



	void FixedUpdate()
	{
		// Mitya ++++++++
			OnUpdatedAnchors();
		// Mitya ---------

/* Mitya
		if (operatingWithoutOVRCameraRig)
			OnUpdatedAnchors();
*/
	}

	// Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
	// This is done instead of parenting to achieve workable physics. If you don't require physics on
	// your hands or held objects, you may wish to switch to parenting.
	void OnUpdatedAnchors()
	{
//        Debug.Log("OnUpdatedAnchors");
		Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
		Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
		Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition + handPos);
		Quaternion destRot = m_parentTransform.rotation * handRot * m_anchorOffsetRotation;

		//Mitya +++++++++++++++

		//        Debug.Log("handPos1=" + handPos);
		//        Debug.Log("destPos1=" + destPos);

				if (m_grabbedObj != null)
				{

					if ((m_grabbedObj.tag != "Hold") && (m_grabbedObj.tag != "Cap"))
					{
						//Mitya ---------------
						GetComponent<Rigidbody>().MovePosition(destPos);
						GetComponent<Rigidbody>().MoveRotation(destRot);
						//Mitya +++++++++++++++
					}
				}
		//Mitya ---------------


		//GetComponent<Rigidbody>().MovePosition(destPos);
		//GetComponent<Rigidbody>().MoveRotation(destRot);

		if (!m_parentHeldObject)
		{
			// Mitya +++++++++++
//            MoveGrabbedObject(destPos, destRot);
			MoveGrabbedObject(destPos, destRot,true);
			// Mitya -----------
		}
		m_lastPos = transform.position;
		m_lastRot = transform.rotation;

		float prevFlex = m_prevFlex; // Запоминается предыдущее значение бокового триггера
		// Update values from inputs

		// Новое значение бокового триггера (Hand Trigger)
		m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

		// Проверяется нажатие или отпускание бокового триггера
		CheckForGrabOrRelease(prevFlex);

    }

    void OnDestroy()
    {
		Debug.Log("OnDestroy Grabber"); 
        if (m_grabbedObj != null)
		{
			GrabEnd();
		}
	}

	void OnTriggerEnter(Collider otherCollider)
	{
		Debug.Log("OnTriggerEnter Grabber");
		// Get the grab trigger
		OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
		if (grabbable == null) return;

		// Add the grabbable
		int refCount = 0;
		m_grabCandidates.TryGetValue(grabbable, out refCount);
		m_grabCandidates[grabbable] = refCount + 1;
	}

	void OnTriggerExit(Collider otherCollider)
	{
		////Debug.Log("OnTriggerExit Grabber");
		OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
		if (grabbable == null) return;

		// Remove the grabbable
		int refCount = 0;
		bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
		if (!found)
		{
			return;
		}

		if (refCount > 1)
		{
			m_grabCandidates[grabbable] = refCount - 1;
		}
		else
		{
			m_grabCandidates.Remove(grabbable);
		}
	}

	protected void CheckForGrabOrRelease(float prevFlex)
	{
		///Debug.Log("CheckForGrabOrRelease Grabber");

		if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
		{
			GrabBegin(); // Захватили объект
		}
		else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
		{
            GrabEnd();   // Отпустили объект
        }
    }

    protected virtual void GrabBegin()
	{
        float closestMagSq = float.MaxValue;
		OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

		Debug.Log("GrabBegin Grabber");
		// Iterate grab candidates and find the closest grabbable candidate
		foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
		{
			bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
			if (!canGrab)
			{
				continue;
			}

			//Mitya+++++++++++++++
			Debug.Log("grabbable.grabPoints.Length=" + grabbable.grabPoints.Length);
			Debug.Log("grabbable.name="+grabbable.name);

			// Счетчик вызовов MoveGrabbedObject (чисто для информации)
			myGrabMoveCount = 0;

            myISS = GameObject.Find("ISS");
            //				if (myISS != null)
            //				{
            //					Debug.Log("myISS.name=" + myISS.name);
            //				}

            if (grabbable.tag == "Hold")
			{

				Debug.Log("this.name=" + this.name);
				Debug.Log("grabbable.tag=" + grabbable.tag);

				// Положение схваченного объекта ???
//				myGrabPos0 = grabbable.transform.position;
//				myGrabRot0 = grabbable.transform.rotation;

				// Якорь (синий кубик)
				myHandAnchor = this.gameObject;  //GameObject.Find("LeftHandAnchor");
				myHandAnchorPos = myHandAnchor.transform.position;
				myHandAnchorRot = myHandAnchor.transform.rotation;
				Debug.Log("myHandAnchorPos=" + myHandAnchorPos.ToString("F4"));

				
				myLocalAvatar = GameObject.Find("LocalAvatar");

//				myLocalAvatarPos = myLocalAvatar.transform.position;
//				myLocalAvatarRot = myLocalAvatar.transform.rotation;
//				Debug.Log("myLocalAvatarPos=" + myLocalAvatarPos.ToString("F4"));

				// Рука (красный кубик) и контроллер (зеленый кубик)
				if (this.name== "LeftHandAnchor")
				{
					myHand = myLocalAvatar.transform.Find("hand_left").gameObject;
//					myHand = GameObject.Find("LocalAvatar/hand_left");
					myController = myLocalAvatar.transform.Find("controller_left").gameObject;
				}
				else
				{
					myHand = myLocalAvatar.transform.Find("hand_right").gameObject;
//					myHand = GameObject.Find("LocalAvatar/hand_right");
					myController = myLocalAvatar.transform.Find("controller_right").gameObject;
				}
				myHandPos = myHand.transform.position;
				myHandRot = myHand.transform.rotation;

//                myHandLocalPos = myHand.transform.localPosition;
				Debug.Log("myHandPos=" + myHandPos.ToString("F4"));



				myPlayerControllerPos = myPlayerController.transform.position;
				myPlayerControllerRot = myPlayerController.transform.rotation;
				Debug.Log("myPlayerControllerPos=" + myPlayerControllerPos.ToString("F4"));

//				Debug.Log("transform.position=" + transform.position.ToString("F4"));

				// Контроллер (зеленый кубик)
				myControllerPos = myController.transform.position;
				myControllerRot = myController.transform.rotation;
				Debug.Log("myControllerPos=" + myControllerPos.ToString("F4"));

				// Проверка
				if (myControllerPos != myHandAnchorPos)
				{
                    Debug.Log("!!!!!!!!!!!!!");
                }
				
                	
			}
            else if (grabbable.tag == "Cap")
            {
                // Якорь (синий кубик)
                myHandAnchor = this.gameObject;  //GameObject.Find("LeftHandAnchor");
                myHandAnchorPos = myHandAnchor.transform.position;
                myHandAnchorRot = myHandAnchor.transform.rotation;
                Debug.Log("myHandAnchorPos=" + myHandAnchorPos.ToString("F4"));

                myLocalAvatar = GameObject.Find("LocalAvatar");

                // Рука (красный кубик) и контроллер (зеленый кубик)
                if (this.name == "LeftHandAnchor")
                {
                    myHand = GameObject.Find("LocalAvatar/hand_left");
                    myController = myLocalAvatar.transform.Find("controller_left").gameObject;
                }
                else
                {
                    myHand = GameObject.Find("LocalAvatar/hand_right");
                    myController = myLocalAvatar.transform.Find("controller_right").gameObject;
                }
                myHandPos = myHand.transform.position;
                myHandRot = myHand.transform.rotation;

                //                myHandLocalPos = myHand.transform.localPosition;
                Debug.Log("myHandPos=" + myHandPos.ToString("F4"));

                myPlayerControllerPos = myPlayerController.transform.position;
                myPlayerControllerRot = myPlayerController.transform.rotation;
                Debug.Log("myPlayerControllerPos=" + myPlayerControllerPos.ToString("F4"));

                //				Debug.Log("transform.position=" + transform.position.ToString("F4"));

                // Контроллер (зеленый кубик)
                myControllerPos = myController.transform.position;
                myControllerRot = myController.transform.rotation;
                Debug.Log("myControllerPos=" + myControllerPos.ToString("F4"));

            }
            else if (grabbable.tag == "Room")
            {
                Debug.Log("grabbable.transform.parent=" + grabbable.transform.parent);
            }
            else
            {
                Debug.Log("grabbable.name=" + grabbable.name);
                Debug.Log("grabbable.transform.parent=" + grabbable.transform.parent);
            }

            //Mitya-------------

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
				Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
					closestMagSq = grabbableMagSq;
					closestGrabbable = grabbable;
					closestGrabbableCollider = grabbableCollider;
                }
            }
		}

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;

			// Вызов GrabBegin перетаскиваемого объекта
			m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
			m_lastRot = transform.rotation;

			// Set up offsets for grabbed object desired position relative to hand.
            if(m_grabbedObj.snapPosition)
            {
				m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if(m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

			if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if(m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
			}
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
				m_grabbedObjectRotOff = relOri;
			}

			// Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
			// speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
			// is beyond the scope of this demo.
			MoveGrabbedObject(m_lastPos, m_lastRot, true);
			if(m_parentHeldObject)
			{
				m_grabbedObj.transform.parent = transform;
			}
		}
	}

	protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
	{
		if (m_grabbedObj == null)
		{
			return;
		}

//        Debug.Log("MoveGrabbedObject");
//        Debug.Log("m_grabbedObj=" + m_grabbedObj);

		// Mitya ++++++++++
		myGrabMoveCount++;

        if (m_grabbedObj.tag == "Hold")
        {
            // Зафиксировать положение схваченного объекта
            //			m_grabbedObj.transform.position = myGrabPos0;
            //			m_grabbedObj.transform.rotation = myGrabRot0;

            this.transform.position = myHandAnchorPos; // HandAnchor остается на месте (Синий кубик)
                                                       // можно написать   myHandAnchor.transform.position = myHandAnchorPos;
            this.transform.rotation = myHandAnchorRot;

            //			myHand.transform.position = myHandPos; // hand_left или hand_right остаются на месте (сама рука) (Красный кубик)
            //			myHand.transform.rotation = myHandRot;


            FuncMoveStation();

        }
        else if (m_grabbedObj.tag == "Cap")
        {
            this.transform.position = myHandAnchorPos; // HandAnchor остается на месте (Синий кубик)
                                                       // можно написать   myHandAnchor.transform.position = myHandAnchorPos;
                                                       //            this.transform.rotation = myHandAnchorRot;
            FuncOpenCap();

        }
        else if (m_grabbedObj.tag == "Reader")
        {   // ???
            if (m_grabbedObj.isAnimation == true) // Прыгать в руку не сразу
            {
                Debug.Log("isAnimation");
                return;
            }
        }


		// Mitya ----------

		Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
		Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
		Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

		//        Debug.Log("grabbedRigidbody= " + grabbedRigidbody);
//		Debug.Log("forceTeleport=" + forceTeleport);
//		Debug.Log("1 grabbedRigidbody.transform.position=" + grabbedRigidbody.transform.position.ToString("F4"));
//		Debug.Log("1 grabbablePosition=" + grabbablePosition.ToString("F4"));

		if (forceTeleport)
		{
			// Mitya ++++++++++
			if (m_grabbedObj.tag == "Hold")
			{

//				myDragMove.x = grabbedRigidbody.transform.position.x - grabbablePosition.x;
//				myDragMove.z = grabbedRigidbody.transform.position.z - grabbablePosition.z;
//				myDragMove.y = 0;


//				Debug.Log("myDragMove.x=" + myDragMove.x.ToString("F4"));
//				Debug.Log("myDragMove.z=" + myDragMove.z.ToString("F4"));

				//grabbablePosition = grabbedRigidbody.transform.position;
				//grabbablePosition = Vector3.zero;
				//grabbedRigidbody.transform.rotation = grabbableRotation;
				//myPlayerController.transform.position -= myDragMove;

			}
            else if (m_grabbedObj.tag == "Cap")
            {
                //                grabbableRotation.x = 0;
                //                grabbableRotation.y = 0;

                //                Debug.Log("   m_grabbedObj.position=" + m_grabbedObj.transform.position.ToString("F4"));
                //                Debug.Log("   grabbablePosition=" + grabbablePosition.ToString("F4"));

                //                grabbedRigidbody.transform.rotation = grabbableRotation;


                //Vector3 dir = myController.transform.position - grabbedRigidbody.transform.position;



                Transform myBagTr = grabbedRigidbody.transform.parent.transform;
                Transform myCoverTr = grabbedRigidbody.transform;


                // Вычислить локальное положение руки относительно точки крепления крышки сумки
                Vector3 myLocalPos = myBagTr.InverseTransformPoint(myController.transform.position) - myCoverTr.localPosition;
                // Сделать поправку, чтобы оказаться в верикальной плоскости XY сумки
                myLocalPos.z = 0.0f;
                // Получить угол
                float myAngle = Vector3.SignedAngle(myBagTr.right, myLocalPos, myBagTr.forward);
                
                // Позиционировать крышку сумки
                Vector3 myOri = myCoverTr.localEulerAngles;
                myOri.z = Mathf.Clamp(myAngle, 0, 135); // ограничение вращения крышки
                myCoverTr.localEulerAngles = myOri;
                

/*
                Vector3 dir = myBagTr.InverseTransformPoint(myController.transform.position) - myCoverTr.localPosition;
                Debug.Log("   dir=" + dir.ToString("F4"));
                //                Quaternion newRot = Quaternion.LookRotation(dir, Vector3.up);
                Quaternion newRot = Quaternion.LookRotation(dir);
                Debug.Log("   newRot=" + newRot.ToString("F4"));
                newRot.x = 0;
                newRot.y = 0;

                //dir.x = 0;
                //dir.z = 0;
                //                grabbedRigidbody.transform.rotation = Quaternion.LookRotation(-dir.normalized, Vector3.forward);
                //                grabbedRigidbody.transform.rotation = Quaternion.LookRotation(-dir.normalized);
                //                grabbedRigidbody.transform.rotation = Quaternion.LookRotation(dir.normalized);
                //              grabbedRigidbody.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                //grabbedRigidbody.transform.rotation = newRot;
                grabbedRigidbody.transform.rotation = Quaternion.Slerp(grabbedRigidbody.transform.rotation, newRot, Time.deltaTime * 8);
*/

                /*

                //grabbedRigidbody.transform.LookAt(GameObject.Find("Cube_Hand_smallR2").transform);
                grabbedRigidbody.transform.LookAt(myController.transform);
                Vector3 myAngle = grabbedRigidbody.transform.localEulerAngles;
                myAngle.y = 0;
                //                myAngle.z = -myAngle.x;
                //                myAngle.z = myFuncNormalizeAngle(-myAngle.x);
                float myX = myAngle.x;
                
                myAngle.z = Mathf.Clamp(myFuncNormalizeAngle(-myAngle.x), 0, 135);
                myAngle.x = 0;

                Debug.Log("myAngle=" + myX + "," + myAngle.z);
                grabbedRigidbody.transform.localEulerAngles = myAngle;
//                grabbedRigidbody.transform.Rotate(0, -90, 0);
//                Debug.Log("   grabbedRigidbody.transform.localEulerAngles=" + grabbedRigidbody.transform.localEulerAngles.ToString("F4"));
//                Debug.Log("   Cube_Hand_smallR1.transform=" + GameObject.Find("Cube_Hand_smallR2").transform.position.ToString("F4"));

                //                grabbedRigidbody.transform.position = grabbablePosition;
                                */


            }
            else
            {
				grabbedRigidbody.transform.position = grabbablePosition;
				grabbedRigidbody.transform.rotation = grabbableRotation;
			}
			// Mitya ----------
		}
		else
		{
			grabbedRigidbody.MovePosition(grabbablePosition);
			grabbedRigidbody.MoveRotation(grabbableRotation);
		}
//		Debug.Log("2 grabbedRigidbody.transform.position=" + grabbedRigidbody.transform.position.ToString("F4"));
//		Debug.Log("2 grabbablePosition=" + grabbablePosition.ToString("F4"));

	}

	protected void GrabEnd()
	{
		Debug.Log("GrabEnd Grabber");
		if (m_grabbedObj != null)
		{
			myGrabMoveCount = 0;

			if (m_grabbedObj.tag == "Hold")
			{
				FuncMoveStation();
//?				myGrabPos0 = Vector3.zero;
			}

            if (m_grabbedObj.tag == "Cap")
            {
                FuncOpenCap();
            }

            OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
			OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
			Debug.Log("localPose.position= " + localPose.position.ToString("F4"));
			Debug.Log("localPose.orientation= " + localPose.orientation.ToString("F4"));
			Debug.Log("offsetPose.position= " + offsetPose.position.ToString("F4"));
			Debug.Log("offsetPose.orientation= " + offsetPose.orientation.ToString("F4"));

			localPose = localPose * offsetPose;
			// offsetPose нулевой, возможно, он не нужен

			OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
			Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
			Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

			GrabbableRelease(linearVelocity, angularVelocity);


		}

		// Re-enable grab volumes to allow overlap events
		GrabVolumeEnable(true);
	}

	protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
	{
		m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
		if(m_parentHeldObject) m_grabbedObj.transform.parent = null;
		m_grabbedObj = null;
	}

	protected virtual void GrabVolumeEnable(bool enabled)
	{
		if (m_grabVolumeEnabled == enabled)
		{
			return;
		}

		m_grabVolumeEnabled = enabled;
		for (int i = 0; i < m_grabVolumes.Length; ++i)
		{
			Collider grabVolume = m_grabVolumes[i];
			grabVolume.enabled = m_grabVolumeEnabled;
		}

		if (!m_grabVolumeEnabled)
		{
			m_grabCandidates.Clear();
		}
	}

	protected virtual void OffhandGrabbed(OVRGrabbable grabbable)
	{
		if (m_grabbedObj == grabbable)
		{
			GrabbableRelease(Vector3.zero, Vector3.zero);
		}
	}

	void FuncMoveStation()
	{
        Vector3 myMove;

        // Расхождение между контроллером и якорем, который остался в точке хватания
        //myMove = myController.transform.position - myHandAnchorPos;
        myMove = myController.transform.position - myControllerPos; // Другой вариант

        if (myMove != Vector3.zero)
		{
					Debug.Log("myMove.x=" + myMove.x);
					Debug.Log("myMove.y=" + myMove.y);
					Debug.Log("myMove.z=" + myMove.z);
		}

        // Сдвигаем станцию на расхождение между контроллером и якорем
        //		myISS.transform.position += myMove;

        // Сдвигаем аватара на расхождение между контроллером и якорем
        myPlayerController.transform.position -= myMove;

        /*
                // Якорь (синий кубик)
                myHandAnchor = this.gameObject;  //GameObject.Find("LeftHandAnchor");
                myHandAnchorPos = myHandAnchor.transform.position;
                myHandAnchorRot = myHandAnchor.transform.rotation;
                Debug.Log("myHandAnchorPos=" + myHandAnchorPos.ToString("F4"));
        */
        // Контроллер (зеленый кубик)
        myControllerPos = myController.transform.position;
        myControllerRot = myController.transform.rotation;
        Debug.Log("myControllerPos=" + myControllerPos.ToString("F4"));

    }


    void FuncOpenCap()
    {
        Vector3 myMove;
        Quaternion myRotate;
        Vector3 myAngles;

        // Расхождение между контроллером и якорем, который остался в точке хватания
        //myMove = myController.transform.position - myHandAnchorPos;
        myMove = myController.transform.position - myControllerPos; // Другой вариант

//        myRotate = myController.transform.rotation;
//        myRotate = myControllerRot;



        if (myMove != Vector3.zero)
        {
            Debug.Log("myMove.x=" + myMove.x);
            Debug.Log("myMove.y=" + myMove.y);
            Debug.Log("myMove.z=" + myMove.z);
        }


        // Сдвигаем аватара на расхождение между контроллером и якорем
//        myPlayerController.transform.position -= myMove;

//        m_grabbedObj.transform.position += myMove;


        /*
                // Якорь (синий кубик)
                myHandAnchor = this.gameObject;  //GameObject.Find("LeftHandAnchor");
                myHandAnchorPos = myHandAnchor.transform.position;
                myHandAnchorRot = myHandAnchor.transform.rotation;
                Debug.Log("myHandAnchorPos=" + myHandAnchorPos.ToString("F4"));
        */
        // Контроллер (зеленый кубик)
        myControllerPos = myController.transform.position;
        myControllerRot = myController.transform.rotation;
        Debug.Log("myControllerPos=" + myControllerPos.ToString("F4"));

    }

    // Приведем угол от (0/360) к (-180/+180)
    float myFuncNormalizeAngle(float myAngle)
    {
        while (myAngle > 180.0f)
        {
            myAngle -= 360.0f;
        }
        while (myAngle < -180.0f)
        {
            myAngle += 360.0f;
        }
        return myAngle;
    }

}



/*
 Rigidbody
 Было:
 Mass=1
 IsKinematic = false
 UseGravity = false
 */


/*
OVRPlayerController
    ForwardDirection
	OVRCameraRig
		TrackingSpace
			LeftEyeAnchor
			CenterEyeAnchor
				Cube_Hand_smallY
			RightEyeAnchor
			TrackerAnchor
				Cube_Hand_smallC
			LeftHandAnchor
				Cube_Hand_smallB1
			RightHandAnchor
				Cube_Hand_smallB2
			LocalAvatar
				base
				body
				hand_left
					Cube_Hand1
					Cube_Hand_smallR1
				controller_left
					Cube_Hand_smallG1
				hand_right
					Cube_Hand2
					Cube_Hand_smallR2
				controller_right
					Cube_Hand_smallG2

 */

 /*
OVRCameraRig contains one Unity camera, the pose of which is controlled by head tracking; two “anchor”
Game Objects for the left and right eyes; and one “tracking space” Game Object that allows you to fine-
tune the relationship between the head tracking reference frame and your world. The rig is meant to be
attached to a moving object, such as a character walking around, a car, a gun turret, et cetera. This replaces the
conventional Camera.
 */
