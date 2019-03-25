﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hook
{
    class KeyboardHook : Hook
    {
        private static int hookHandle = 0;
        private static HookProc callBackDelegate;
        private const int WM_KEYDOWN = 0x0100;

        public static event EventHandler keyboardEvent = delegate { };

        public class KeyboardHookEventArgs : EventArgs
        {
            public int KeyCode;
            public DateTime invokedTime;
        }

        public static void startHook()
        {
            if (callBackDelegate != null)
                throw new InvalidOperationException("Hook has not been initalized properly!");
            callBackDelegate = new HookProc(CallBack);
            if (hookHandle != 0)
            {
                return;
            }
            hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, callBackDelegate, IntPtr.Zero, 0);
            Console.WriteLine(hookHandle);
        }

        public static void StopHook()
        {
            UnhookWindowsHookEx(hookHandle);
            hookHandle = 0;
            callBackDelegate = null;
        }

        public static int CallBack(int nCode, IntPtr wParam, IntPtr IParam)
        {
           if(nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int keyCode = Marshal.ReadInt32(IParam);
                KeyboardHookEventArgs args = new KeyboardHookEventArgs();
                args.KeyCode = keyCode;
                args.invokedTime = DateTime.Now;
                keyboardEvent(null, args);
            }
            return CallNextHookEx(hookHandle, nCode, wParam, IParam);
        }

        public static int getHookHandle() { return hookHandle; }
    }
}