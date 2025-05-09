//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Scripts/Input/GameInput.inputactions
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

public partial class @GameInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""797e8ff9-7afb-4e59-8a36-8048860a73a8"",
            ""actions"": [
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""741dda5e-47cf-4593-b268-77ad35c7ed1f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""49b9f5e9-c7db-43c2-b47e-0c08b9715af7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""1b1ce9d2-6bb3-43ae-8a5a-a0c403d87f5c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OpenOrCloseForm"",
                    ""type"": ""Button"",
                    ""id"": ""a594edb4-7407-4d9e-904a-af10fbfdba65"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""44c51144-4e85-415b-a280-17f6db0a7198"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraMove"",
                    ""type"": ""Value"",
                    ""id"": ""6f5f0ffa-6e7f-43e2-9882-07b1638d434c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""609916ff-ce6c-40ba-beb5-b5a5319e87ec"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e28e8011-0667-4795-9c00-922462d86ea2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press,Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84377e54-f70f-4e64-9104-9bcb1fed5c52"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press,Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c914956e-08b2-4246-b62e-023c73fed275"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""313a857d-9b47-431f-a9ad-6e025b85b0f8"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenOrCloseForm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""2cc6751f-37f3-4cea-bda9-0950d8c324bb"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""fd177d8f-cbef-4392-b558-78bebf34c6f1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""af33020d-12b4-4370-a2e6-026828eea2c1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""08d8ea5d-2e4a-41e5-a931-774640309cbf"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""84daa353-d581-4038-9037-2b62d851409a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Place"",
            ""id"": ""e7a40d88-53ce-40ee-aadf-8be11b1a1732"",
            ""actions"": [
                {
                    ""name"": ""RotateTrap"",
                    ""type"": ""Value"",
                    ""id"": ""4dead57c-e816-49d1-a105-611425f7b3f6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TryPlacePosition"",
                    ""type"": ""Value"",
                    ""id"": ""cc6d81c3-4bdf-4d09-b75d-f50450455739"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TryPlace"",
                    ""type"": ""Button"",
                    ""id"": ""f208ecb2-fbe6-4d5b-b14c-d5ccd52f8af5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CancelPlace"",
                    ""type"": ""Button"",
                    ""id"": ""e697c04e-74ea-412c-8903-3f5ba6b8180f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c09d5498-969a-4484-8615-92dd59b64ede"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateTrap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1961a0e6-6931-4c56-ac30-e3526230b0fd"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TryPlacePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b7a784b7-f722-4212-bb9a-5edb331eeb8b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TryPlace"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39113a14-6914-49bc-91d3-a8fdac194c13"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CancelPlace"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_MousePosition = m_UI.FindAction("MousePosition", throwIfNotFound: true);
        m_UI_LeftClick = m_UI.FindAction("LeftClick", throwIfNotFound: true);
        m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
        m_UI_OpenOrCloseForm = m_UI.FindAction("OpenOrCloseForm", throwIfNotFound: true);
        m_UI_CameraZoom = m_UI.FindAction("CameraZoom", throwIfNotFound: true);
        m_UI_CameraMove = m_UI.FindAction("CameraMove", throwIfNotFound: true);
        // Place
        m_Place = asset.FindActionMap("Place", throwIfNotFound: true);
        m_Place_RotateTrap = m_Place.FindAction("RotateTrap", throwIfNotFound: true);
        m_Place_TryPlacePosition = m_Place.FindAction("TryPlacePosition", throwIfNotFound: true);
        m_Place_TryPlace = m_Place.FindAction("TryPlace", throwIfNotFound: true);
        m_Place_CancelPlace = m_Place.FindAction("CancelPlace", throwIfNotFound: true);
    }

    ~@GameInput()
    {
        UnityEngine.Debug.Assert(!m_UI.enabled, "This will cause a leak and performance issues, GameInput.UI.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_Place.enabled, "This will cause a leak and performance issues, GameInput.Place.Disable() has not been called.");
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

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_MousePosition;
    private readonly InputAction m_UI_LeftClick;
    private readonly InputAction m_UI_RightClick;
    private readonly InputAction m_UI_OpenOrCloseForm;
    private readonly InputAction m_UI_CameraZoom;
    private readonly InputAction m_UI_CameraMove;
    public struct UIActions
    {
        private @GameInput m_Wrapper;
        public UIActions(@GameInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MousePosition => m_Wrapper.m_UI_MousePosition;
        public InputAction @LeftClick => m_Wrapper.m_UI_LeftClick;
        public InputAction @RightClick => m_Wrapper.m_UI_RightClick;
        public InputAction @OpenOrCloseForm => m_Wrapper.m_UI_OpenOrCloseForm;
        public InputAction @CameraZoom => m_Wrapper.m_UI_CameraZoom;
        public InputAction @CameraMove => m_Wrapper.m_UI_CameraMove;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
            @LeftClick.started += instance.OnLeftClick;
            @LeftClick.performed += instance.OnLeftClick;
            @LeftClick.canceled += instance.OnLeftClick;
            @RightClick.started += instance.OnRightClick;
            @RightClick.performed += instance.OnRightClick;
            @RightClick.canceled += instance.OnRightClick;
            @OpenOrCloseForm.started += instance.OnOpenOrCloseForm;
            @OpenOrCloseForm.performed += instance.OnOpenOrCloseForm;
            @OpenOrCloseForm.canceled += instance.OnOpenOrCloseForm;
            @CameraZoom.started += instance.OnCameraZoom;
            @CameraZoom.performed += instance.OnCameraZoom;
            @CameraZoom.canceled += instance.OnCameraZoom;
            @CameraMove.started += instance.OnCameraMove;
            @CameraMove.performed += instance.OnCameraMove;
            @CameraMove.canceled += instance.OnCameraMove;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
            @LeftClick.started -= instance.OnLeftClick;
            @LeftClick.performed -= instance.OnLeftClick;
            @LeftClick.canceled -= instance.OnLeftClick;
            @RightClick.started -= instance.OnRightClick;
            @RightClick.performed -= instance.OnRightClick;
            @RightClick.canceled -= instance.OnRightClick;
            @OpenOrCloseForm.started -= instance.OnOpenOrCloseForm;
            @OpenOrCloseForm.performed -= instance.OnOpenOrCloseForm;
            @OpenOrCloseForm.canceled -= instance.OnOpenOrCloseForm;
            @CameraZoom.started -= instance.OnCameraZoom;
            @CameraZoom.performed -= instance.OnCameraZoom;
            @CameraZoom.canceled -= instance.OnCameraZoom;
            @CameraMove.started -= instance.OnCameraMove;
            @CameraMove.performed -= instance.OnCameraMove;
            @CameraMove.canceled -= instance.OnCameraMove;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);

    // Place
    private readonly InputActionMap m_Place;
    private List<IPlaceActions> m_PlaceActionsCallbackInterfaces = new List<IPlaceActions>();
    private readonly InputAction m_Place_RotateTrap;
    private readonly InputAction m_Place_TryPlacePosition;
    private readonly InputAction m_Place_TryPlace;
    private readonly InputAction m_Place_CancelPlace;
    public struct PlaceActions
    {
        private @GameInput m_Wrapper;
        public PlaceActions(@GameInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @RotateTrap => m_Wrapper.m_Place_RotateTrap;
        public InputAction @TryPlacePosition => m_Wrapper.m_Place_TryPlacePosition;
        public InputAction @TryPlace => m_Wrapper.m_Place_TryPlace;
        public InputAction @CancelPlace => m_Wrapper.m_Place_CancelPlace;
        public InputActionMap Get() { return m_Wrapper.m_Place; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlaceActions set) { return set.Get(); }
        public void AddCallbacks(IPlaceActions instance)
        {
            if (instance == null || m_Wrapper.m_PlaceActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlaceActionsCallbackInterfaces.Add(instance);
            @RotateTrap.started += instance.OnRotateTrap;
            @RotateTrap.performed += instance.OnRotateTrap;
            @RotateTrap.canceled += instance.OnRotateTrap;
            @TryPlacePosition.started += instance.OnTryPlacePosition;
            @TryPlacePosition.performed += instance.OnTryPlacePosition;
            @TryPlacePosition.canceled += instance.OnTryPlacePosition;
            @TryPlace.started += instance.OnTryPlace;
            @TryPlace.performed += instance.OnTryPlace;
            @TryPlace.canceled += instance.OnTryPlace;
            @CancelPlace.started += instance.OnCancelPlace;
            @CancelPlace.performed += instance.OnCancelPlace;
            @CancelPlace.canceled += instance.OnCancelPlace;
        }

        private void UnregisterCallbacks(IPlaceActions instance)
        {
            @RotateTrap.started -= instance.OnRotateTrap;
            @RotateTrap.performed -= instance.OnRotateTrap;
            @RotateTrap.canceled -= instance.OnRotateTrap;
            @TryPlacePosition.started -= instance.OnTryPlacePosition;
            @TryPlacePosition.performed -= instance.OnTryPlacePosition;
            @TryPlacePosition.canceled -= instance.OnTryPlacePosition;
            @TryPlace.started -= instance.OnTryPlace;
            @TryPlace.performed -= instance.OnTryPlace;
            @TryPlace.canceled -= instance.OnTryPlace;
            @CancelPlace.started -= instance.OnCancelPlace;
            @CancelPlace.performed -= instance.OnCancelPlace;
            @CancelPlace.canceled -= instance.OnCancelPlace;
        }

        public void RemoveCallbacks(IPlaceActions instance)
        {
            if (m_Wrapper.m_PlaceActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlaceActions instance)
        {
            foreach (var item in m_Wrapper.m_PlaceActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlaceActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlaceActions @Place => new PlaceActions(this);
    public interface IUIActions
    {
        void OnMousePosition(InputAction.CallbackContext context);
        void OnLeftClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnOpenOrCloseForm(InputAction.CallbackContext context);
        void OnCameraZoom(InputAction.CallbackContext context);
        void OnCameraMove(InputAction.CallbackContext context);
    }
    public interface IPlaceActions
    {
        void OnRotateTrap(InputAction.CallbackContext context);
        void OnTryPlacePosition(InputAction.CallbackContext context);
        void OnTryPlace(InputAction.CallbackContext context);
        void OnCancelPlace(InputAction.CallbackContext context);
    }
}
