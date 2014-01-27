using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{   
    //NOTE: this is just a copy of some OpenTKs enums, it is included so that OpenTK does not need to be referenced the main app

    /// <summary>
    /// Represents a mouse button
    /// </summary>
    public enum MouseButton
    {       
        //No mouse button
        None = -1,
        //The left mouse button.
        Left = 0,
        //The middle mouse button.
        Middle = 1,
        //The right mouse button.
        Right = 2,
        //The first extra mouse button.
        Button1 = 3,
        //The second extra mouse button.
        Button2 = 4,
        //The third extra mouse button.
        Button3 = 5,
        //The fourth extra mouse button.
        Button4 = 6,
        //The fifth extra mouse button.
        Button5 = 7,
        //The sixth extra mouse button.
        Button6 = 8,
        //The seventh extra mouse button.
        Button7 = 9,
        //The eigth extra mouse button.
        Button8 = 10,
        //The ninth extra mouse button.
        Button9 = 11,
        //Indicates the last available mouse button.
        LastButton = 12,
    }


    /// <summary>
    /// Represents a keyboard key
    /// </summary>
    public enum Key
    {
        //A key outside the known keys.
        Unknown = 0,
        //The left shift key (equivalent to ShiftLeft).
        LShift = 1,
        //The left shift key.
        ShiftLeft = 1,
        //The right shift key (equivalent to ShiftRight).
        RShift = 2,
        //The right shift key.
        ShiftRight = 2,
        //The left control key (equivalent to ControlLeft).
        LControl = 3,
        //The left control key.
        ControlLeft = 3,
        //The right control key (equivalent to ControlRight).
        RControl = 4,
        //The right control key.
        ControlRight = 4,
        //The left alt key.
        AltLeft = 5,
        //The left alt key (equivalent to AltLeft.
        LAlt = 5,
        //The right alt key.
        AltRight = 6,
        //The right alt key (equivalent to AltRight).
        RAlt = 6,
        //The left win key.
        WinLeft = 7,
        //The left win key (equivalent to WinLeft).
        LWin = 7,
        //The right win key (equivalent to WinRight).
        RWin = 8,
        //The right win key.
        WinRight = 8,
        //The menu key.
        Menu = 9,
        //The F1 key.
        F1 = 10,
        //The F2 key.
        F2 = 11,
        //The F3 key.
        F3 = 12,
        //The F4 key.
        F4 = 13,
        //The F5 key.
        F5 = 14,
        //The F6 key.
        F6 = 15,
        //The F7 key.
        F7 = 16,
        //The F8 key.
        F8 = 17,
        //The F9 key.
        F9 = 18,
        //The F10 key.
        F10 = 19,
        //The F11 key.
        F11 = 20,
        //The F12 key.
        F12 = 21,
        //The F13 key.
        F13 = 22,
        //The F14 key.
        F14 = 23,
        //The F15 key.
        F15 = 24,
        //The F16 key.
        F16 = 25,
        //The F17 key.
        F17 = 26,
        //The F18 key.
        F18 = 27,
        //The F19 key.
        F19 = 28,
        //The F20 key.
        F20 = 29,
        //The F21 key.
        F21 = 30,
        //The F22 key.
        F22 = 31,
        //The F23 key.
        F23 = 32,
        //The F24 key.
        F24 = 33,
        //The F25 key.
        F25 = 34,
        //The F26 key.
        F26 = 35,
        //The F27 key.
        F27 = 36,
        //The F28 key.
        F28 = 37,
        //The F29 key.
        F29 = 38,
        //The F30 key.
        F30 = 39,
        //The F31 key.
        F31 = 40,
        //The F32 key.
        F32 = 41,
        //The F33 key.
        F33 = 42,
        //The F34 key.
        F34 = 43,
        //The F35 key.
        F35 = 44,
        //The up arrow key.
        Up = 45,
        //The down arrow key.
        Down = 46,
        //The left arrow key.
        Left = 47,
        //The right arrow key.
        Right = 48,
        //The enter key.
        Enter = 49,
        //The escape key.
        Escape = 50,
        //The space key.
        Space = 51,
        //The tab key.
        Tab = 52,
        //The backspace key (equivalent to BackSpace).
        Back = 53,
        //The backspace key.
        BackSpace = 53,
        //The insert key.
        Insert = 54,
        //The delete key.
        Delete = 55,
        //The page up key.
        PageUp = 56,
        //The page down key.
        PageDown = 57,
        //The home key.
        Home = 58,
        //The end key.
        End = 59,
        //The caps lock key.
        CapsLock = 60,
        //The scroll lock key.
        ScrollLock = 61,
        //The print screen key.
        PrintScreen = 62,
        //The pause key.
        Pause = 63,
        //The num lock key.
        NumLock = 64,
        //The clear key (Keypad5 with NumLock disabled, on typical keyboards).
        Clear = 65,
        //The sleep key.
        Sleep = 66,
        //The keypad 0 key.
        Keypad0 = 67,
        //The keypad 1 key.
        Keypad1 = 68,
        //The keypad 2 key.
        Keypad2 = 69,
        //The keypad 3 key.
        Keypad3 = 70,
        //The keypad 4 key.
        Keypad4 = 71,
        //The keypad 5 key.
        Keypad5 = 72,
        //The keypad 6 key.
        Keypad6 = 73,
        //The keypad 7 key.
        Keypad7 = 74,
        //The keypad 8 key.
        Keypad8 = 75,
        //The keypad 9 key.
        Keypad9 = 76,
        //The keypad divide key.
        KeypadDivide = 77,
        //The keypad multiply key.
        KeypadMultiply = 78,
        //The keypad minus key (equivalent to KeypadSubtract).
        KeypadMinus = 79,
        //The keypad subtract key.
        KeypadSubtract = 79,
        //The keypad add key.
        KeypadAdd = 80,
        //The keypad plus key (equivalent to KeypadAdd).
        KeypadPlus = 80,
        //The keypad decimal key.
        KeypadDecimal = 81,
        //The keypad enter key.
        KeypadEnter = 82,
        //The A key.
        A = 83,
        //The B key.
        B = 84,
        //The C key.
        C = 85,
        //The D key.
        D = 86,
        //The E key.
        E = 87,
        //The F key.
        F = 88,
        //The G key.
        G = 89,
        //The H key.
        H = 90,
        //The I key.
        I = 91,
        //The J key.
        J = 92,
        //The K key.
        K = 93,
        //The L key.
        L = 94,
        //The M key.
        M = 95,
        //The N key.
        N = 96,
        //The O key.
        O = 97,
        //The P key.
        P = 98,
        //The Q key.
        Q = 99,
        //The R key.
        R = 100,
        //The S key.
        S = 101,
        //The T key.
        T = 102,
        //The U key.
        U = 103,
        //The V key.
        V = 104,
        //The W key.
        W = 105,
        //The X key.
        X = 106,
        //The Y key.
        Y = 107,
        //The Z key.
        Z = 108,
        //The number 0 key.
        Number0 = 109,
        //The number 1 key.
        Number1 = 110,
        //The number 2 key.
        Number2 = 111,
        //The number 3 key.
        Number3 = 112,
        //The number 4 key.
        Number4 = 113,
        //The number 5 key.
        Number5 = 114,
        //The number 6 key.
        Number6 = 115,
        //The number 7 key.
        Number7 = 116,
        //The number 8 key.
        Number8 = 117,
        //The number 9 key.
        Number9 = 118,
        //The tilde key.
        Tilde = 119,
        //The minus key.
        Minus = 120,
        //The plus key.
        Plus = 121,
        //The left bracket key (equivalent to BracketLeft).
        LBracket = 122,
        //The left bracket key.
        BracketLeft = 122,
        //The right bracket key.
        BracketRight = 123,
        //The right bracket key (equivalent to BracketRight).
        RBracket = 123,
        //The semicolon key.
        Semicolon = 124,
        //The quote key.
        Quote = 125,
        //The comma key.
        Comma = 126,
        //The period key.
        Period = 127,
        //The slash key.
        Slash = 128,
        //The backslash key.
        BackSlash = 129,
        //Indicates the last available keyboard key.
        LastKey = 130,
    }
    
}
