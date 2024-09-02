using System;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.View_Controller.Input_System
{
    public interface IInputProperty<T>
    {
        T Value { get; }
        public void Reset();
    }

    public class InputProperty<T> : IInputProperty<T>
    {
        public T Value { get; private set; }

        /// <summary>
        /// Create a InputProperty and add it to a InputAction
        /// </summary>
        /// <param name="action">The InputAction link to</param>
        /// <param name="startedSetter">The Value equals startedSetter when action.started</param>
        /// <param name="performedSetter">The Value equals performedSetter when action.performed</param>
        /// <param name="canceledSetter">The Value equals canceledSetter when action.canceled</param>
        public InputProperty(
            InputAction action,
            Func<InputAction.CallbackContext, T> startedSetter = null, 
            Func<InputAction.CallbackContext, T> performedSetter = null, 
            Func<InputAction.CallbackContext, T> canceledSetter = null)
        {
            Value = default;
            action.started += context => { if (startedSetter != null) { Value = startedSetter(context); } };
            action.performed += context => { if (performedSetter != null) { Value = performedSetter(context); } };
            action.canceled += context => { if (canceledSetter != null) { Value = canceledSetter(context); } };
        }

        public void Reset()
        {
            Value = default;
        }

        public static implicit operator T(InputProperty<T> inputProperty) => inputProperty.Value;
    }

    public class InputKit : Singleton<InputKit>
    {
        private InputKit() {}

        private InputControls mControls;

        public InputProperty<Vector2> move;
        public InputProperty<bool> jump;
        public InputProperty<bool> reset;
        public InputProperty<bool> esc;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();

            mControls = new InputControls();
            mControls.Enable();

            move = new InputProperty<Vector2>(
                mControls.Player.Move,
                performedSetter: context => context.ReadValue<Vector2>(),
                canceledSetter: context => Vector2.zero);

            jump = new InputProperty<bool>(
                mControls.Player.Jump,
                performedSetter: context => true,
                canceledSetter: context => false);
            
            reset = new InputProperty<bool>(
                mControls.Player.Reset,
                performedSetter: context => true,
                canceledSetter: context => false);

            esc = new InputProperty<bool>(
                mControls.Player.Esc,
                performedSetter: context => true,
                canceledSetter: context => false);
        }
    }
}