using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using System;

public class HandGestureHandler : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    
    private XRHand lHand;
    public XRHand LHand
    {
        get => lHand;
    }

    private XRHand rHand;
    public XRHand RHand
    {
        get => rHand;
    }

    public float pinchThreshold = 0.01f;

    public event Action<Vector3> OnRightPinchDown;
    public event Action<Vector3> OnRightPinch;
    public event Action<Vector3> OnRightPinchRelease;
    
    public event Action<Vector3> OnLeftPinchDown;
    public event Action<Vector3> OnLeftPinch;
    public event Action<Vector3> OnLeftPinchRelease;

    private bool isRightPinching = false;
    private bool isLeftPinching = false;
    
    public static HandGestureHandler Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (handSubsystem != null)
        {
        }
        else
        {
            handSubsystem = GetHandSubsystem();
            if(handSubsystem != null)
            {
                handSubsystem.updatedHands += OnHandDataUpdated;
            }
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
                return subsystem;
            }
        }
        return null;
    }

    void OnHandDataUpdated(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags, XRHandSubsystem.UpdateType updateType)
    {
        if (updateType == XRHandSubsystem.UpdateType.Dynamic)
        {
            lHand = subsystem.leftHand;
            rHand = subsystem.rightHand;

            UpdatePinching();
        }
    }

    void UpdatePinching()
    {
        bool rightPinching = IsFingerPinching(rHand, XRHandJointID.IndexTip, out Vector3 rightPinchPosition);
        bool leftPinching = IsFingerPinching(lHand, XRHandJointID.IndexTip, out Vector3 leftPinchPosition);

        // Index finger pinch checks
        if (!isRightPinching && rightPinching)
        {
            isRightPinching = true;
            OnRightPinchDown?.Invoke(rightPinchPosition);
        }
        else if (isRightPinching && !rightPinching)
        {
            isRightPinching = false;
            OnRightPinchRelease?.Invoke(rightPinchPosition);
        }
        else if (isRightPinching && rightPinching)
        {
            OnRightPinch?.Invoke(rightPinchPosition);
        }

        // Middle finger pinch checks
        if (!isLeftPinching && leftPinching)
        {
            isLeftPinching = true;
            OnLeftPinchDown?.Invoke(leftPinchPosition);
        }
        else if (isLeftPinching && !leftPinching)
        {
            isLeftPinching = false;
            OnLeftPinchRelease?.Invoke(leftPinchPosition);
        }
        else if (isLeftPinching && leftPinching)
        {
            OnLeftPinch?.Invoke(leftPinchPosition);
        }
    }

    bool IsFingerPinching(XRHand hand, XRHandJointID fingerTip, out Vector3 pinchPosition)
    {
        pinchPosition = Vector3.zero;

        if (hand.isTracked)
        {
            XRHandJoint thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
            XRHandJoint fingerTipJoint = hand.GetJoint(fingerTip);

            Pose thumbPose, fingerPose;
            if (thumbTip.TryGetPose(out thumbPose) && fingerTipJoint.TryGetPose(out fingerPose))
            {
                float distance = Vector3.Distance(thumbPose.position, fingerPose.position);
                if (distance < pinchThreshold)
                {
                    // Set the pinch position to be used by the caller
                    pinchPosition = (thumbPose.position + fingerPose.position) * 0.5f;
                    return true;
                }
            }
        }
        return false;
    }

    public Pose GetHandPose(XRHand hand, XRHandJointID fingerTip)
    {
        if (hand.isTracked)
        {
            XRHandJoint joint = hand.GetJoint(fingerTip);

            if (joint.TryGetPose(out Pose jointPose))
            {
                return jointPose;
            }
        }
        return new Pose();
    }

    void OnDestroy()
    {
        if (handSubsystem != null)
        {
            handSubsystem.updatedHands -= OnHandDataUpdated;
        }
    }
}
