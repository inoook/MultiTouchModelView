using UnityEngine;
using System.Collections;

public class BtnInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SyncParam ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0) || Input.GetMouseButton (1)) {
			SyncParam ();
		}
	}

	void SyncParam()
	{
		targetZoom = mouseOrbit.Zoom;
		targetRotY = mouseOrbit.RotateY;
		targetPanX = mouseOrbit.PanX;
		targetPanY = mouseOrbit.PanY;
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
	public void RotateY(float d)
	{
		float to = targetRotY + d * rotateSensitivity;
		targetRotY = to;
		LeanTween.value (mouseOrbit.RotateY, to, 0.5f).setEase(LeanTweenType.easeOutSine).setOnUpdate ((v) => {
			mouseOrbit.RotateY = v;
		});
	}
	public void PanX(float d)
	{
		float to = targetPanX + d * panSensitivity;
		targetPanX = to;
		LeanTween.value (mouseOrbit.PanX, to, 0.5f).setEase(LeanTweenType.easeOutSine).setOnUpdate ((v) => {
			mouseOrbit.PanX = v;
		});
	}
	public void PanY(float d)
	{
		float to = targetPanY + d * panSensitivity;
		targetPanY = to;
		LeanTween.value (mouseOrbit.PanY, to, 0.5f).setEase(LeanTweenType.easeOutSine).setOnUpdate ((v) => {
			mouseOrbit.PanY = v;
		});
	}
	public void MoveZ(float d)
	{
		mouseOrbit.MoveZ(d*panSensitivity);
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
