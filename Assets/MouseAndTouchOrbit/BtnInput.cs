using UnityEngine;
using System.Collections;

public class BtnInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SyncParam ();
	}
	
	// Update is called once per frame
	void Update () {
		if (DeviceInputManager.GetMouseButton (0) || DeviceInputManager.GetMouseButton (1)) {
			SyncParam ();
		}
	}

	void SyncParam()
	{
		targetZoom = mouseOrbit.Zoom;
		targetRotY = mouseOrbit.RotateY;
//		targetPanX = mouseOrbit.PanX;
//		targetPanY = mouseOrbit.PanY;
	}

	public float targetZoom = 0;
	public float targetRotY = 0;
	public float targetPanX = 0;
	public float targetPanY = 0;
	public float targetMoveZ = 0;



	[SerializeField]
	MouseAndTouchOrbit mouseOrbit;

	[SerializeField]
	float zoomSensitivity = 1.0f;

	[SerializeField]
	float rotateSensitivity = 1.0f;

	[SerializeField]
	float panSensitivity = 1.0f;

	public void Zoom(float d)
	{
		float to = targetZoom - d * zoomSensitivity;
		targetZoom = to;
		LeanTween.value (mouseOrbit.Zoom, to, 0.5f).setEase(LeanTweenType.easeOutSine).setOnUpdate ((v) => {
			mouseOrbit.Zoom = v;
		});
	}

	LTDescr rotTwn;
	public void RotateY(float d)
	{
		if (rotTwn != null) {
			LeanTween.cancel (rotTwn.uniqueId);
		}

		float to = targetRotY + d * rotateSensitivity;
		targetRotY = to;
		rotTwn = LeanTween.value (mouseOrbit.RotateY, to, 0.5f).setEase (LeanTweenType.easeOutSine).setOnUpdate ((v) => {
			mouseOrbit.RotateY = v;
		});
	}
	public void PanX(float d)
	{
		LeanTween.cancel (mouseOrbit.gameObject);
		Vector3 toPos = mouseOrbit.transform.position + mouseOrbit.transform.rotation * new Vector3 (d * panSensitivity, 0,0);
		LeanTween.move (mouseOrbit.gameObject, toPos, 0.5f).setEase (LeanTweenType.easeOutSine);
	}
	public void PanY(float d)
	{
		LeanTween.cancel (mouseOrbit.gameObject);
		Vector3 toPos = mouseOrbit.transform.position + mouseOrbit.transform.rotation * new Vector3 (0, d * panSensitivity,0);
		LeanTween.move (mouseOrbit.gameObject, toPos, 0.5f).setEase (LeanTweenType.easeOutSine);
	}
	public void MoveZ(float d)
	{
		LeanTween.cancel (mouseOrbit.gameObject);
		Vector3 toPos = mouseOrbit.transform.position + mouseOrbit.transform.rotation * new Vector3 (0, 0,d*panSensitivity);
		LeanTween.move (mouseOrbit.gameObject, toPos, 0.5f).setEase (LeanTweenType.easeOutSine);
	}

	void Reset()
	{
		mouseOrbit.Reset ();

		SyncParam ();
	}

	[SerializeField]
	Rect drawRect = new Rect(10,10,200,200);

	void OnGUI()
	{
		GUILayout.BeginArea (drawRect);
		if (GUILayout.Button ("zoom +")) {
			Zoom (1);
		}
		if (GUILayout.Button ("zoom -")) {
			Zoom (-1);
		}
		GUILayout.Space (10);
		if (GUILayout.Button ("RotateY >")) {
			RotateY (1);
		}
		if (GUILayout.Button ("RotateY <")) {
			RotateY (-1);
		}
		GUILayout.Space (10);

		if (GUILayout.Button ("PanX >")) {
			PanX (1);
		}
		if (GUILayout.Button ("PanX <")) {
			PanX (-1);
		}
		GUILayout.Space (10);

		if (GUILayout.Button ("PanY >")) {
			PanY (1);
		}
		if (GUILayout.Button ("PanY <")) {
			PanY (-1);
		}

		GUILayout.Space (10);

		if (GUILayout.Button ("MoveZ >")) {
			MoveZ (1);
		}
		if (GUILayout.Button ("MoveZ <")) {
			MoveZ (-1);
		}

		GUILayout.Space (10);

		if (GUILayout.Button ("Reset")) {
			Reset ();
		}
		GUILayout.EndArea ();
	}
}
