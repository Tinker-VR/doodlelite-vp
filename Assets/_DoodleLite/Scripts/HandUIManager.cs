using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands;
using TMPro;

public class HandUIManager : MonoBehaviour
{
    public static HandUIManager Instance { get; private set; }

    public GameObject[] uiElements;
    private int activeUIIndex = -1;
    private GameObject activeUIElement => uiElements[activeUIIndex];

    private bool isActive = false;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
    }

    private Vector3 lastPinchPosition;

    private Coroutine hideUICoroutine;
    private Coroutine cycleUICoroutine;

    private bool isPinching = false;
    private float lastPinchTime = 0f;
    private float lastCycleTime = 0f;

    // [SerializeField] TextMeshProUGUI debugText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (uiElements.Length > 0)
        {
            activeUIIndex = 0; // Start with the first UI element
        }

        // HandGestureHandler.Instance.OnLeftPinchDown += ActivateUIElement;
        // HandGestureHandler.Instance.OnLeftPinchRelease += StartHideUIElement;
    }
    
    private void Start()
    {
        if (HandGestureHandler.Instance != null)
        {
            HandGestureHandler.Instance.OnLeftPinchDown += HandlePinchDown;
            HandGestureHandler.Instance.OnLeftPinchRelease += HandlePinchRelease;
        }
        else
        {
            Debug.Log("HAND GESTURE NULL");
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            CycleUI(lastPinchPosition);
        }
    }
    
    void HandlePinchDown(Vector3 pinchPosition)
    {
        lastPinchPosition = pinchPosition;

        if (!isPinching)
        {
            // ActivateUIElement(pinchPosition);
            isPinching = true;
            lastPinchTime = Time.time;

            if (hideUICoroutine != null)
            {
                StopCoroutine(hideUICoroutine);
                hideUICoroutine = null;

                // debugText.text = "CANCEL HIDE";
                // return;
            }
        }
    }

    void HandlePinchRelease(Vector3 pinchPosition)
    {
        if (isPinching)
        {
            if (Time.time - lastPinchTime < 0.6f && Time.time - lastPinchTime > 0.05f)
            {
                if (isActive)
                {
                    // debugText.text = "UI ACTIVE,CYCLE TO NEXT";
                    // cycleUICoroutine = StartCoroutine(StartCycleUI(pinchPosition));
                    if(Time.time - lastCycleTime > 0.25)
                    {
                        CycleUI(lastPinchPosition);
                    }
                }
                else
                {
                    // debugText.text = "NO UI ACTIVE, SHOW UI";
                    isActive = true;
                    ActivateUIElement(lastPinchPosition);
                }
            }
            else
            {
                // debugText.text = "CYCLE TOO FAST";
            }
            
            if (isActive)
            {
                // debugText.text = "PINCH RELEASED, START HIDING UI";
                StartHideUI();
            }

            isPinching = false;
        }
    }

    void ActivateUIElement(Vector3 pinchPosition)
    {
        if (activeUIIndex < 0 || activeUIIndex >= uiElements.Length)
        {
            return;
        }

        foreach (var uiElement in uiElements)
        {
            uiElement.SetActive(uiElement == activeUIElement);
            uiElement.GetComponent<HandUIElement>().IsActive = uiElement == activeUIElement;

            // if(uiElement == activeUIElement)
            // {
            //     debugText.text = "ACTIVE UI IS " + uiElement.name.ToString();
            // }
        }
        
        LeanTween.cancel(activeUIElement);
        
        activeUIElement.transform.localScale = Vector3.zero;
        activeUIElement.GetComponent<CanvasGroup>().alpha = 0f;
        LeanTween.scale(activeUIElement, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.alphaCanvas(activeUIElement.GetComponent<CanvasGroup>(), 1f, 0.2f).setFrom(0f);

        Pose handPose = HandGestureHandler.Instance.GetHandPose(HandGestureHandler.Instance.LHand, XRHandJointID.IndexTip); 
        
        Vector3 uiPositionOffset = handPose.forward * 0.05f; 
        
        activeUIElement.transform.position = pinchPosition + uiPositionOffset;

        Vector3 tangent = Quaternion.Euler(25, 0, 0) * handPose.forward;
        activeUIElement.transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
    }
      
    void CycleUI(Vector3 pinchPosition)
    {
        if (uiElements.Length == 0) return;

        lastCycleTime = Time.time;
        
        // HideUIElementInstantly();
        HideActiveUIElement();

        // Calculate the next UI index and activate the next UI element.
        activeUIIndex = (activeUIIndex + 1) % uiElements.Length;
        ActivateUIElement(pinchPosition);
    }
    
    IEnumerator StartCycleUI(Vector3 pinchPosition)
    {
        yield return new WaitForSeconds(0.5f); // Wait for the possibility of a quick succession pinch
        cycleUICoroutine = null;
    }

    void StartHideUI()
    {
        if (hideUICoroutine == null)
        {
            hideUICoroutine = StartCoroutine(HideUIWithDelay(1.5f));
        }
    }

    IEnumerator HideUIWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        HideActiveUIElement(true);
    }
    
    void HideActiveUIElement(bool hideEntireUI = false)
    {
        // if (!isActive) return;

        // isActive = false;
        GameObject uiElement = activeUIElement;
        
        LeanTween.cancel(uiElement);
        LeanTween.scale(uiElement, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInBack);
        LeanTween.alphaCanvas(uiElement.GetComponent<CanvasGroup>(), 0f, 0.1f).setDelay(0.1f).setOnComplete(() => 
        {
            if(hideEntireUI) 
            {
                isActive = false;
                hideUICoroutine = null;
                // debugText.text = "UI NOW HIDDEN";
            }
            uiElement.SetActive(false);
        });

        // // Reset coroutine reference
    }

    void OnDestroy()
    {
        HandGestureHandler.Instance.OnLeftPinchDown -= HandlePinchDown;
        HandGestureHandler.Instance.OnLeftPinchRelease -= HandlePinchRelease;
    }

    // public void SetActiveUIElement(Transform newUIElement)
    // {
    //     activeUIElement = newUIElement;
    // }
}