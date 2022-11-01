using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinaKEYBIND : MonoBehaviour
{
    public bool isInputEnabled;
    public bool checkYourKeys;
    public KeyCode currentKeyCode;
    public int bufferLength = 8;
    public KeyCode[] keyBuffer;

    readonly string funcGroup = "FUNC";
    readonly KeyCode[] funcKeys = new KeyCode[] {
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12
    };

    readonly string alphGroup = "1234";
    readonly KeyCode[] alphKeys = new KeyCode[] {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,
        KeyCode.Minus,
        KeyCode.Equals
    };

    readonly string npadGroup = "#PAD";
    readonly KeyCode[] npadKeys = new KeyCode[] {
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.Keypad8,
        KeyCode.Keypad9,
        KeyCode.KeypadDivide,
        KeyCode.KeypadMultiply,
        KeyCode.KeypadMinus
    };

    readonly string stndGroup = "STND";
    readonly KeyCode[] stndKeys = new KeyCode[] {
        KeyCode.LeftShift,
        KeyCode.LeftControl,
        KeyCode.LeftAlt,
        KeyCode.Space,
        KeyCode.RightShift,
        KeyCode.RightControl,
        KeyCode.RightAlt,
        KeyCode.Return,
        KeyCode.BackQuote,
        KeyCode.Tab,
        KeyCode.Backslash,
        KeyCode.Backspace
    };

    readonly string cpadGroup = "CPAD";
    readonly KeyCode[] cpadKeys = new KeyCode[] {
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
        KeyCode.UpArrow,
        KeyCode.Delete,
        KeyCode.End,
        KeyCode.PageDown,
        KeyCode.Insert,
        KeyCode.Home,
        KeyCode.PageUp,
        KeyCode.ScrollLock,
        KeyCode.Pause
    };

    readonly string qwerGroup = "QWER";
    readonly KeyCode[] qwerKeys = new KeyCode[] {
        KeyCode.Q,
        KeyCode.W,
        KeyCode.E,
        KeyCode.R,
        KeyCode.T,
        KeyCode.Y,
        KeyCode.U,
        KeyCode.I,
        KeyCode.O,
        KeyCode.P,
        KeyCode.LeftBracket,
        KeyCode.RightBracket
    };

    readonly string asdfGroup = "ASDF";
    readonly KeyCode[] asdfKeys = new KeyCode[] {
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.Semicolon,
        KeyCode.Quote,
        KeyCode.Slash
    };
    public KeyCode[] customKeys = new KeyCode[12];  
    readonly string defaultCustomKeystring = "Alpha1-Alpha2-Alpha3-Alpha4-Alpha5-Alpha6-Alpha7-Alpha8-Alpha9-Alpha0-Minus-Equals";

    public KeyCode[] GetMyKeys(string keybindProfile, string customBinding)
    {
        if (keybindProfile == funcGroup) { return funcKeys; }
        else if (keybindProfile == alphGroup) { return alphKeys; }
        else if (keybindProfile == npadGroup) { return npadKeys; }
        else if (keybindProfile == stndGroup) { return stndKeys; }
        else if (keybindProfile == cpadGroup) { return cpadKeys; }
        else if (keybindProfile == qwerGroup) { return qwerKeys; }
        else if (keybindProfile == asdfGroup) { return asdfKeys; }
        else // assume keybindProfile == "Custom"
        {
            string[] keyArray = customBinding.Split((Char)"-"[0]);
            if (keyArray.Length != 12)
            {
                Debug.LogError("INFINAKEYBIND NOTIFICATION: customBinding not valid, using default (1234)");
                keyArray = defaultCustomKeystring.Split((Char)"-"[0]);
            }
            for (int b = 0; b < 12; b++)
            {
                customKeys[b] = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[b]);
            }
            return customKeys;
        }
    }

    public int KeybindRequest(KeyCode[] theKeys)
    {
        int output = 0;
        if (checkYourKeys)
        {
            for (int b = 0; b < 12; b++)
            {
                if (CheckKeyBuffer(theKeys[b])) { output = b + 1; RemoveKeyFromBuffer(theKeys[b]); }
            }
        }
        if (CheckKeyBufferEmpty()) { checkYourKeys = false; }
        return output;
    }

    private void Awake()
    {
        if ((bufferLength < 2) || (bufferLength > 64)) { bufferLength = 8; }
        keyBuffer = new KeyCode[bufferLength];
        FlushKeyBuffer();
    }

    private void Update()
    {
        if (isInputEnabled) { InputCheck(); } 
    }

    /**
     * Keybind Checking Loop.
     */
    public void InputCheck()
    {
        if (Input.anyKeyDown)
        {
            checkYourKeys = true;
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    AddKeyToBuffer(k);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Escape)) // Exits the game.
            {
                Application.Quit();
            }
        }
    }

    public bool CheckKeyBuffer(KeyCode key)
    {
        for (int b = 0; b < bufferLength; b++)
        {
            if (keyBuffer[b] == key) { return true; }
        }
        return false;
    }

    public bool CheckKeyBufferEmpty()
    {
        for (int b = 0; b < bufferLength; b++)
        {
            if (keyBuffer[b] != KeyCode.None) { return false; }
        }
        return true;
    }

    public void FlushKeyBuffer()
    {
        for (int b = 0; b < bufferLength; b++)
        {
            keyBuffer[b] = KeyCode.None;
        }
    }

    public void AddKeyToBuffer(KeyCode key)
    {
        for (int b = bufferLength - 1; b >= 1; b--)
        {
            keyBuffer[b] = keyBuffer[b - 1];
        }
        keyBuffer[0] = key;
    }

    public void RemoveKeyFromBuffer(KeyCode key)
    {
        for (int b = 0; b < bufferLength; b++)
        {
            if (keyBuffer[b] == key) { keyBuffer[b] = KeyCode.None; }
        }
    }
}
