using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow instance = null;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(instance);
	}

	[Header("Refrence")]
	public Transform target;

	[Header("Misc")]
	public float smoothSpeed;
	public float smoothSpeedAtSlowMotion;
	public Vector3 offset;

	public bool lookAtPlayer;

	[SerializeField] private float targetSmoothSpeed;

	[Header("Zoom Effect")]
	public float fovZoomIn;
	public float fovZoomOut;
	public float zoomOutTime;
	public float zoomInTime;


	private Camera cam;


    public void Init(GameObject playerRef)
    {
		ResetToNormalSpeed();

		target = playerRef.transform;
		cam = Camera.main.GetComponent<Camera>();
		cam.fieldOfView = fovZoomOut;
    }

    void FixedUpdate()
	{
		if (!target) return;

		Vector3 desiredPosition = target.position + offset;

		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, targetSmoothSpeed * Time.deltaTime);
		transform.position = smoothedPosition;

		if(lookAtPlayer)
			transform.LookAt(target);
	}

	public void DeletePlayer()
    {
		target = null;
    }

	public void ChangePlayer(Transform newPlayer)
    {
		target = newPlayer;
    }

	public void ZoomOutFunc()
    {
		if(cam.fieldOfView != fovZoomOut)
        {
			StopAllCoroutines();
			StartCoroutine(ZoomOutEffect());
        }
    }

	IEnumerator ZoomOutEffect()
	{
		float startVal = cam.fieldOfView;
		float counter = 0f;

		while (counter < zoomOutTime)
		{
			cam.fieldOfView = Mathf.Lerp(startVal, fovZoomOut, counter / zoomOutTime);

			counter += Time.unscaledDeltaTime;
			yield return null;
		}

		cam.fieldOfView = fovZoomOut;
		ResetToNormalSpeed();
	}

	public void ZoomInFunc()
	{
		if (cam.fieldOfView != fovZoomIn)
		{
			StopAllCoroutines();
			StartCoroutine(ZoomInEffect());
		}
	}

	IEnumerator ZoomInEffect()
	{
		float startVal = cam.fieldOfView;
		float counter = 0f;

		while (counter < zoomInTime)
		{
			cam.fieldOfView = Mathf.Lerp(startVal, fovZoomIn, counter / zoomOutTime);

			counter += Time.unscaledDeltaTime;
			yield return null;
		}

		cam.fieldOfView = fovZoomIn;
	}

	public void SetSpeedAtSlowMotion()
    {
		targetSmoothSpeed = smoothSpeedAtSlowMotion;
	}

	public void ResetToNormalSpeed()
    {
		targetSmoothSpeed = smoothSpeed;
	}
}
