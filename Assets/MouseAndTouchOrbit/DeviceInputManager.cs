using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchObject
{
	public float deltaTime;
	public Vector2 deltaPosition;
	public int fingerId = 0;
	public TouchPhase phase = TouchPhase.Moved;
	public Vector2 position;
	public int tapCount;
}

public class DeviceInputManager : GenericSingletonClass<DeviceInputManager>{

	#region static
//	static InputManager instance;

	public static int touchCount
	{
		get{ return Instance._touchCount; }
	}
	
	public static TouchObject GetTouch(int id)
	{
		return Instance._GetTouch(id);
	}

	public static bool GetMouseButton(int id)
	{
		return Instance._GetMouseButton(id);
	}
	public static bool GetMouseButtonDown(int id)
	{
		return Instance._GetMouseButtonDown(id);
	}
	public static bool GetMouseButtonUp(int id)
	{
		return Instance._GetMouseButtonUp(id);
	}
	public static float GetAxis (string axisName)
	{
		return Input.GetAxis (axisName);
	}
	#endregion



	public int _touchCount;
	public List<TouchObject> _touches;

	private int maxTouchCount = 10;
	private List<TouchObject> _bufferTouchObjes;

	// Use this for initialization
	void Awake () {
		_bufferTouchObjes = new List<TouchObject>();
		for(int i = 0; i < maxTouchCount; i++){
			_bufferTouchObjes.Add(new TouchObject());
		}

	}
	
	// Update is called once per frame
	void Update () {
	#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
		_touchCount = Input.touchCount;
	#else

		_touchCount = 0;

		if( Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) ){
			TouchObject touch = _bufferTouchObjes[0];
			touch.deltaPosition = Vector2.zero;
			touch.position = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		}

		if( Input.GetMouseButton(0) || Input.GetMouseButton(1) ){
			_touchCount ++;

			TouchObject touch = _bufferTouchObjes[0];
			Vector2 inputPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
			touch.deltaPosition = inputPos - touch.position;
			touch.position = inputPos;

			// copy point
			bool isRot = Input.GetKey(KeyCode.LeftAlt);
			bool isPan = Input.GetKey(KeyCode.LeftShift);
			bool isPanY = Input.GetKey(KeyCode.LeftControl);
			if(isRot || isPan || isPanY)
			{
				_touchCount ++;

				TouchObject touchCopy = _bufferTouchObjes[1];
				Vector2 copyPos = Vector2.zero;
				if(isRot){
					copyPos = new Vector2(Screen.width -touch.position.x, Screen.height-touch.position.y );
				}else{
					if(isPan){
						copyPos = new Vector2(touch.position.x + 30, Screen.height-touch.position.y );
					}else{
						copyPos = new Vector2(Screen.width - touch.position.x, touch.position.y );
					}
				}
				touchCopy.position = copyPos;
			}
		}
	#endif
	}

	public bool debug = false;
	void OnGUI()
	{
		if (!debug) {
			return;
		}
		float w = 20;
		float h = 20;
		for(int i = 0; i < _touchCount; i++){
			TouchObject touch = _bufferTouchObjes[i];
			Rect rect = new Rect (touch.position.x-w/2, Screen.height-touch.position.y-w/2, w, h);
			GUI.Label (rect, "X");
		}
	}

	bool _GetMouseButton(int id)
	{
		return Input.GetMouseButton (id);
	}
	bool _GetMouseButtonDown(int id)
	{
		return Input.GetMouseButtonDown (id);
	}
	bool _GetMouseButtonUp(int id)
	{
		return Input.GetMouseButtonUp (id);
	}

	public TouchObject _GetTouch(int id)
	{
	#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
		Touch touch = Input.GetTouch(id);
		
		if(id >= _bufferTouchObjes.Count){
			Debug.LogError("EROOR too many touch count: "+id);
			return null;
		}
		TouchObject touchObj = _bufferTouchObjes[id];
		// copy to TouchObject
		touchObj.deltaPosition = touch.deltaPosition;
		touchObj.deltaTime = touch.deltaTime;
		touchObj.fingerId = touch.fingerId;
		touchObj.phase = touch.phase;
		touchObj.position = touch.position;
		touchObj.tapCount = touch.tapCount;

		return touchObj;
	#else
		if(id >= _touchCount){ return null; }
		return _bufferTouchObjes[id];
	#endif
	}
}
