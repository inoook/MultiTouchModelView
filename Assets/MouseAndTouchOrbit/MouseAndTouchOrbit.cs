using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseAndTouchOrbit: MonoBehaviour
{
	[SerializeField]
	Transform target;
	public float distance = 10.0f;
	public float zoomSensitivity = 10f;

	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float zoomMinLimit = 1;
	public float zoomMaxLimit = 10f;

	[SerializeField]
	private float x = 0.0f;
	private float y = 0.0f;

	public float zoomDistance = 0;
	public Vector2 panDistance = Vector2.zero;

	void Start ()
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		zoomDistance = distance;
		// Make the rigid body not change rotation
		Rigidbody rigidBody = GetComponent<Rigidbody> ();
		if (rigidBody)
			rigidBody.freezeRotation = true;

		zoomDeltaList = new List<float> ();
		centerDeltaList = new List<float> ();
	}

	private void LateUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.D)) {
			debug = !debug;
			DeviceInputManager.Instance.debug = debug;
		}

		if (target == null)
			return;
		
		MultiTouchUpdate ();
            
		zoomDistance += DeviceInputManager.GetAxis ("Mouse ScrollWheel") * zoomSensitivity;
		zoomDistance = Mathf.Clamp (zoomDistance, zoomMinLimit, zoomMaxLimit);

		if (target && DeviceInputManager.GetMouseButton (0) && DeviceInputManager.touchCount == 1) {
			// left mouse orbit
			x += (float)(DeviceInputManager.GetAxis ("Mouse X") * xSpeed * 0.02);
			y -= (float)(DeviceInputManager.GetAxis ("Mouse Y") * ySpeed * 0.02);

			y = ClampAngle (y, yMinLimit, yMaxLimit);
		}

		if (target && DeviceInputManager.GetMouseButton (1)) {
			// right mouse Pan
			panDistance += DeviceInputManager.GetTouch (0).deltaPosition * -0.01f;
		}

		Quaternion rotation = Quaternion.Euler (y, x, 0);
		target.rotation = rotation;

		Vector3 position = target.position + rotation * (new Vector3 (0.0f, 0.0f, -zoomDistance));
		transform.rotation = rotation;
		transform.position = position + transform.rotation * new Vector3(panDistance.x, panDistance.y, 0);
	}

	public bool IsTouchInput()
	{
		return (DeviceInputManager.touchCount > 0);
	}

	//
	Vector2 preCenter;
	List<float> zoomDeltaList;
	List<float> centerDeltaList;

	[SerializeField]
	int avgBufferNum = 10;

	public bool isStartTowFinger = false;


	//TODO: リファクタリング
	void MultiTouchUpdate () 
	{
		if (DeviceInputManager.touchCount >= 2) {
			TouchObject touch1 = DeviceInputManager.GetTouch (0);
			TouchObject touch2 = DeviceInputManager.GetTouch (1);

			if (!isStartTowFinger) {
				// Start Two Finger;
				isStartTowFinger = true;
				
				zoomDeltaList.Clear ();
				centerDeltaList.Clear ();

				preCenter = (touch1.position + touch2.position) / 2.0f;
			}

			// Find out how the touches have moved relative to eachother:
			Vector2 curDist = touch1.position - touch2.position;
			Vector2 prevDist = (touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition);

			Vector2 center = (touch1.position + touch2.position) / 2.0f;
			Vector2 centerDelta = center - preCenter;
			preCenter = center;

			float zoomDelta = (curDist.magnitude - prevDist.magnitude) * 0.01f;
			Vector2 panDelta = (curDist - prevDist) * 0.01f;

			// avg
			zoomDeltaList.Add (Mathf.Abs (zoomDelta));
			if (zoomDeltaList.Count > avgBufferNum) {
				zoomDeltaList.RemoveAt (0);
			}

			float avgZoomDelta = 0;
			for (int i = 0; i < zoomDeltaList.Count; i++) {
				avgZoomDelta += zoomDeltaList [i];
			}
			avgZoomDelta /= zoomDeltaList.Count;
			//
			centerDeltaList.Add (Mathf.Abs (centerDelta.magnitude));
			if (centerDeltaList.Count > avgBufferNum) {
				centerDeltaList.RemoveAt (0);
			}

			float avgCenterDelta = 0;
			for (int i = 0; i < centerDeltaList.Count; i++) {
				avgCenterDelta += centerDeltaList [i];
			}
			avgCenterDelta /= centerDeltaList.Count;
			//

			if (avgZoomDelta > 0.05f) {
				zoomDistance += zoomDelta;
			}
			if (avgCenterDelta > 2.0f) {
				panDistance += -centerDelta * 0.01f;
			}
		}else{
			isStartTowFinger = false;
		}
	}

	private static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}

	//
	public float RotateY{
		set{ x = value; }
		get{ return x; }
	}
	public float Zoom{
		set{ zoomDistance = value; }
		get{ return zoomDistance; }
	}

	public float PanX
	{
		set{ panDistance.x = value; }
		get{ return panDistance.x; }
	}
	public float PanY
	{
		set{ panDistance.y = value; }
		get{ return panDistance.y; }
	}

	float _z;
	public void MoveZ(float v)
	{
			_z = v;
			target.position += target.rotation * new Vector3(0, 0, _z);
	}

	public void Reset()
	{
		zoomDistance = distance;
		x = 0;
		y = 0;
		panDistance = Vector2.zero;

		var rotation = Quaternion.Euler (y, x, 0);
		var position = rotation * (new Vector3 (0.0f, 0.0f, -zoomDistance)) + target.position;

		transform.rotation = rotation;
		transform.position = position + transform.rotation * new Vector3(panDistance.x, panDistance.y, 0);

		target.rotation = rotation;
		target.position = Vector3.zero;

	}

	public bool debug = false;
	void OnGUI()
	{
		if (!debug) {
			return;
		}
		float w = 20;
		float h = 20;
		Rect rect = new Rect (preCenter.x-w/2, Screen.height-preCenter.y-w/2, w, h);
		GUI.color = Color.red;
		GUI.Label (rect, "X");
		GUI.color = Color.white;
	}
}
