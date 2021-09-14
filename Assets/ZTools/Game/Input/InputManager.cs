using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game
{
    public enum RawInputProviderType
    {
        playerTouch,
        playerJoystick,
        ai,
    }

    public enum RawInputType
    {
        PressStart,
        Press,
        PressEnd,
        OnButtonDown,
        OnButtonUp,
        OnButtonPressed
    }

    public interface IRawInputProvider
    {
        RawInputProviderType iType { get; }
        event Action<RawInput> OnRawInputEvent;
    }

    public interface IRawInputHandler
    {
        void OnMoveStart(Vector2 point);
        void OnMoveEnd(Vector2 point);
        void OnMove(Vector2 point, Vector2 externPoint = default);

        void OnButtonDown(int buttonID);
        void OnButtonPressed(int buttonID);
        void OnButtonUp(int buttonID);
    }

    /// <summary>
    /// unified input type. Use struct to avoid GC
    /// </summary>
    public struct RawInput
    {
        public RawInputType type;
        public bool isOverUI;
        public Vector2 vec2Data; //screen position
        public Vector2 vec2ExtData; 
    }

    /// <summary>
    /// switch between different IRawInputProviders.
    /// switch between different IRawInputHandlers.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        
    }
}