using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using System;

public class HandGestureHandler : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    
    private XRHand hand;

    public float pinchThreshold = 0.01f;

    public event Action<Vector3> OnPinchDown;
    public event Action<Vector3> OnPinch;
    public event Action<Vector3> OnPinchRelease;

    private bool isPinching = false;

    void Update()
    {
        if (handSubsystem != null)
        {
            handSubsystem.updatedHands += OnHandDataUpdated;
        }
        else
        {
            Debug.Log("HandGestureHandler: Searching for HandSubsystem...");
            handSubsystem = GetHandSubsystem();
        }
    }

    XRHandSubsystem GetHandSubsystem()
    {
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        foreach (var subsystem in handSubsystems)
        {
            if (subsystem.running)
            {
                Debug.Log("HandGestureHandler: Found HandSubsystem.");
                return subsystem;
            }
        }
        Debug.LogWarning("HandGestureHandler: No running HandSubsystem found.");
        return null;
    }

    void OnHandDataUpdated(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags, XRHandSubsystem.UpdateType updateType)
    {
        if (updateType == XRHandSubsystem.UpdateType.Dynamic)
        {
            hand = subsystem.rightHand;
            bool currentlyPinching = IsPinching(out Vector3 pinchPosition);
            
            Debug.Log($"HandGestureHandler: Pinch Update. IsPinching: {currentlyPinching}, PinchPosition: {pinchPosition}");

            if (!isPinching && currentlyPinching)
            {
                isPinching = true;
                OnPinchDown?.Invoke(pinchPosition);
            }
            else if (isPinching && !currentlyPinching)
            {
                isPinching = false;
                OnPinchRelease?.Invoke(pinchPosition);
            }
            else if (isPinching && currentlyPinching)
            {
                OnPinch?.Invoke(pinchPosition);
            }
        }
    }

    bool IsPinching(out Vector3 pinchPosition)
    {
        // Initialize the out parameter
        pinchPosition = Vector3.zero;

        if (hand.isTracked)
        {
            XRHandJoint thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
            XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);

            Pose thumbPose, indexPose;
            if (thumbTip.TryGetPose(out thumbPose) && indexTip.TryGetPose(out indexPose))
            {
                float distance = Vector3.Distance(thumbPose.position, indexPose.position);
                if (distance < pinchThreshold)
                {
                    // Set the pinch position to be used by the caller
                    pinchPosition = (thumbPose.position + indexPose.position) * 0.5f;
                    return true;
                }
            }
        }
        return false;
    }

    void OnDestroy()
    {
        if (handSubsystem != null)
        {
            handSubsystem.updatedHands -= OnHandDataUpdated;
        }
    }
}
