using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System;
using System.Collections.Generic;

public class KonamiCodeListener : MonoBehaviour
{
    [Tooltip("Max time allowed between inputs before resetting the sequence.")]
    public float maxTimeBetweenInputs = 1.0f;

    public GameObject topHat;

    private List<Func<bool>> _sequenceChecks;
    private int _currentIndex = 0;
    private float _timeSinceLastInput = 0f;

    void Awake()
    {
        _sequenceChecks = new List<Func<bool>> {
            () => Gamepad.current?.dpad.up.wasPressedThisFrame        ?? false,
            () => Gamepad.current?.dpad.up.wasPressedThisFrame        ?? false,
            () => Gamepad.current?.dpad.down.wasPressedThisFrame      ?? false,
            () => Gamepad.current?.dpad.down.wasPressedThisFrame      ?? false,
            () => Gamepad.current?.dpad.left.wasPressedThisFrame      ?? false,
            () => Gamepad.current?.dpad.right.wasPressedThisFrame     ?? false,
            () => Gamepad.current?.dpad.left.wasPressedThisFrame      ?? false,
            () => Gamepad.current?.dpad.right.wasPressedThisFrame     ?? false,
            () => Gamepad.current?.buttonEast.wasPressedThisFrame     ?? false,
            () => Gamepad.current?.buttonSouth.wasPressedThisFrame    ?? false
        };
    }

    void Update()
    {
        var pad = Gamepad.current;
        if (pad == null)
            return;

        if (_timeSinceLastInput > maxTimeBetweenInputs)
            ResetSequence();

        if (_sequenceChecks[_currentIndex].Invoke())
        {
            _currentIndex++;
            _timeSinceLastInput = 0f;

            if (_currentIndex >= _sequenceChecks.Count)
            {
                Debug.Log("🎉 Konami Code entered! 🎉");
                if (topHat != null)
                    topHat.SetActive(true);
                ResetSequence();
                ResetSequence();
            }
        }
        else if (AnyButtonPressedThisFrame(pad))
        {
            ResetSequence();
        }

        _timeSinceLastInput += Time.deltaTime;
    }

    private void ResetSequence()
    {
        _currentIndex = 0;
        _timeSinceLastInput = 0f;
    }

    private bool AnyButtonPressedThisFrame(Gamepad pad)
    {
        foreach (var control in pad.allControls)
            if (control is ButtonControl btn && btn.wasPressedThisFrame)
                return true;
        return false;
    }
}
