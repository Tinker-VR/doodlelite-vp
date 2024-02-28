//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Samples/Apple visionOS XR Plugin/1.0.3/VR Sample - Built-in/Scripts/Input/PointerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PointerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PointerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PointerInput"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""d9c85186-69e8-4a3a-a161-3d4c6cc1e4fa"",
            ""actions"": [
                {
                    ""name"": ""PrimaryPointer"",
                    ""type"": ""PassThrough"",
                    ""id"": ""137f651a-c48c-4d8a-931a-e34a275627bc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6beadd1e-fbe7-42ca-9fcf-b2fedcf81cc7"",
                    ""path"": ""<VisionOSSpatialPointerDevice>/primarySpatialPointer"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""PrimaryPointer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Default"",
            ""bindingGroup"": ""Default"",
            ""devices"": []
        }
    ]
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_PrimaryPointer = m_Default.FindAction("PrimaryPointer", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Default
    private readonly InputActionMap m_Default;
    private List<IDefaultActions> m_DefaultActionsCallbackInterfaces = new List<IDefaultActions>();
    private readonly InputAction m_Default_PrimaryPointer;
    public struct DefaultActions
    {
        private @PointerInput m_Wrapper;
        public DefaultActions(@PointerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @PrimaryPointer => m_Wrapper.m_Default_PrimaryPointer;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void AddCallbacks(IDefaultActions instance)
        {
            if (instance == null || m_Wrapper.m_DefaultActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DefaultActionsCallbackInterfaces.Add(instance);
            @PrimaryPointer.started += instance.OnPrimaryPointer;
            @PrimaryPointer.performed += instance.OnPrimaryPointer;
            @PrimaryPointer.canceled += instance.OnPrimaryPointer;
        }

        private void UnregisterCallbacks(IDefaultActions instance)
        {
            @PrimaryPointer.started -= instance.OnPrimaryPointer;
            @PrimaryPointer.performed -= instance.OnPrimaryPointer;
            @PrimaryPointer.canceled -= instance.OnPrimaryPointer;
        }

        public void RemoveCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDefaultActions instance)
        {
            foreach (var item in m_Wrapper.m_DefaultActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DefaultActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DefaultActions @Default => new DefaultActions(this);
    private int m_DefaultSchemeIndex = -1;
    public InputControlScheme DefaultScheme
    {
        get
        {
            if (m_DefaultSchemeIndex == -1) m_DefaultSchemeIndex = asset.FindControlSchemeIndex("Default");
            return asset.controlSchemes[m_DefaultSchemeIndex];
        }
    }
    public interface IDefaultActions
    {
        void OnPrimaryPointer(InputAction.CallbackContext context);
    }
}
