using System;
using UnityEngine;

public class MoveAndFly : MonoBehaviour {
	//Unity vars - Main
	[Header("Main Settings")] 
	[SerializeField] private bool lockMouseOnStart = true;

	//Unity vars - Look
	[Header("Look Settings")] 
	[SerializeField] private float   mouseSensitivity = 10;
	[SerializeField] private float   mouseSmoothTime  = 0.1f;
	[SerializeField] private Vector2 pitchMinMax      = new Vector2(-89, 89);

	//Unity vars - Fly
	[Header("Fly Settings")] 
	[SerializeField] private float moveSpeed = 2;
	[SerializeField] private float moveSmoothTime       = 0.1f;
	[SerializeField] private float shiftSpeedMultiplier = 1.5f;
	[SerializeField] private float scrollSensitivity = 2;

	//Main vars
	private Camera _camera;

	//Smooth look vars
	private float _yaw;
	private float _pitch;
	private float _smoothYawV;
	private float _smoothPitchV;

	//Smooth fly
	private Vector3 _currentVelocity = Vector3.zero;

	//The start function
	private void Start() {
		if (lockMouseOnStart) {
			Cursor.lockState = CursorLockMode.Locked;
		}

		_camera = Camera.main;

		if (_camera != null) {
			var localCamEuler = _camera.transform.localEulerAngles;
			_yaw = localCamEuler.y;
			_pitch = localCamEuler.x;
		}
	}

	//Called per frame
	private void Update() {
		moveSpeed += Input.mouseScrollDelta.y * scrollSensitivity;
		moveSpeed = Mathf.Clamp(moveSpeed, 0, 30);
		
		SmoothLook();
		SmoothFly();
	}

	//Enables smooth flight
	private void SmoothFly() {
		var localShiftMult = 1f;
		if (Input.GetKey(KeyCode.LeftControl)) {
			localShiftMult = shiftSpeedMultiplier;
		}
		
		var sideways = Input.GetAxisRaw("Horizontal");
		var forwards = Input.GetAxisRaw("Vertical");
		var upwards = Input.GetKey(KeyCode.Space) ? 1 : 0;
		upwards += Input.GetKey(KeyCode.LeftShift) ? -1 : 0;
		
		var camTrans     = _camera.transform;
		var moveSideWays = sideways * camTrans.right;
		var moveForward  = forwards * camTrans.forward;
		var moveUpwards  = upwards * camTrans.up;
		
		var camPos    = camTrans.position;
		var endCamPos = camPos + (moveSideWays + moveForward + moveUpwards) * (Time.deltaTime*moveSpeed * localShiftMult);
		camTrans.position = Vector3.SmoothDamp(camPos, endCamPos, ref _currentVelocity, moveSmoothTime);
	}

	//Enables smooth look around
	private void SmoothLook() {
		var mX = Input.GetAxisRaw("Mouse X");
		var mY = Input.GetAxisRaw("Mouse Y");

		_yaw += mX * mouseSensitivity*Time.deltaTime;
		_pitch -= mY * mouseSensitivity*Time.deltaTime;
		_pitch = Mathf.Clamp(_pitch, pitchMinMax.x, pitchMinMax.y);

		var localCamEuler = _camera.transform.localEulerAngles;
		localCamEuler.x = Mathf.SmoothDampAngle(localCamEuler.x, _pitch, ref _smoothPitchV, mouseSmoothTime);
		localCamEuler.y = Mathf.SmoothDampAngle(localCamEuler.y, _yaw, ref _smoothYawV, mouseSmoothTime);

		_camera.transform.localEulerAngles = localCamEuler;
	}
}