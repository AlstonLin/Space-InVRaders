﻿using UnityEngine;
using SocketIO;

[RequireComponent(typeof(Collider))]
public class SpaceShipSkeleton : MonoBehaviour, IGvrGazeResponder {
	private const float MOVE_SPEED = 1.0f;

	public GameObject laser, spaceShip, socketObj;

	public int hp;

	private SocketIOComponent socket;
    private Vector3 startingPosition;

	private float id = -1;

    void Start() {
		socket = socketObj.GetComponent (typeof(SocketIOComponent)) as SocketIOComponent;
        startingPosition = transform.localPosition;
        SetGazedAt(false);
    }

    void LateUpdate() {
        GvrViewer.Instance.UpdateState();
        if (GvrViewer.Instance.BackButtonPressed) {
            Application.Quit();
        }
    }

    public void SetGazedAt(bool gazedAt) {
        if(gazedAt) {
            GetComponent<Renderer>().material.color = Color.red;
        } else {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset() {
        transform.localPosition = startingPosition;
    }

    public void ToggleVRMode() {
        GvrViewer.Instance.VRModeEnabled = !GvrViewer.Instance.VRModeEnabled;
    }

    public void ToggleDistortionCorrection() {
        switch (GvrViewer.Instance.DistortionCorrection) {
            case GvrViewer.DistortionCorrectionMethod.Unity:
                GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Native;
                break;
            case GvrViewer.DistortionCorrectionMethod.Native:
                GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.None;
                break;
            case GvrViewer.DistortionCorrectionMethod.None:
            default:
                GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Unity;
                break;
        }
    }

    public void ToggleDirectRender() {
        GvrViewer.Controller.directRender = !GvrViewer.Controller.directRender;
    }

    public void Fire() {
        GameObject newLaser = Instantiate(laser, transform.TransformPoint(Vector3.forward * 15), Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, 0)) as GameObject;
    }

    void OnCollisionEnter(Collision collisionInfo) {
        Debug.Log("spaceship: onCollisionEnter");
        GetHit();
    }

    public void GetHit() {
        hp--;
        if(hp < 1) {
            onDeath();
        }
    }

    public void onDeath() {
        Destroy(spaceShip);
    }

    #region IGvrGazeResponder implementation

        /// Called when the user is looking on a GameObject with this script,
        /// as long as it is set to an appropriate layer (see GvrGaze).
    public void OnGazeEnter() {
        SetGazedAt(true);
    }

    /// Called when the user stops looking on the GameObject, after OnGazeEnter
    /// was already called.
    public void OnGazeExit() {
        SetGazedAt(false);
    }

    /// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
    public void OnGazeTrigger() {    }

    #endregion
}