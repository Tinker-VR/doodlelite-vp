using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUIElement : MonoBehaviour
{
    private bool isActive = false;
    public bool IsActive
    { 
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
        }
    }


    private bool isInteractable = false;
    public bool IsInteractable
    { 
        get
        {
            return isInteractable;
        }
        set
        {
            isInteractable = value;
        }
    }

    private Coroutine activationCoroutine;

    public void SetInteractable(Vector3 pinchPosition)
    {
        Debug.Log("PINCH PRESSED START SET INTERACTABLE TRUE");
        if (activationCoroutine == null)
        {
            Debug.Log("NO CURRENT SET IENRACTABLE ACTIVE");
            activationCoroutine = StartCoroutine(SetInteractableDelayed(0.1f));
        }
    }
    
    public void SetUninteractable(Vector3 pinchPosition)
    {
        if (activationCoroutine != null)
        {
            StopCoroutine(activationCoroutine);
            activationCoroutine = null;
        }

        IsInteractable = false;
    }

    private IEnumerator SetInteractableDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        IsInteractable = true;
        Debug.Log("INTERACTABLE NOW TRUE");

        activationCoroutine = null;
    }

    void Start()
    {
        HandGestureHandler.Instance.OnLeftPinchDown += SetInteractable;
        HandGestureHandler.Instance.OnLeftPinchRelease += SetUninteractable;
    }

    void OnDestroy()
    {
        HandGestureHandler.Instance.OnLeftPinchDown -= SetInteractable;
        HandGestureHandler.Instance.OnLeftPinchRelease -= SetUninteractable;
    }
}
