﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hook
{
    class MouseHook : Hook
    {
        private static int hookHandle = 0;
        private static HookProc callBackDelegate;

        public static event EventHandler<MouseHookEventArgs> mouseEvent = delegate { };

        public class MouseHookEventArgs : EventArgs
        {
            public MouseMessages type;
            public int X;
            public int Y;
            public DateTime invokedTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookConstruct
        {
            public POINT pos;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
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
            hookHandle = SetWindowsHookEx(WH_MOUSE_LL, callBackDelegate, IntPtr.Zero, 0);
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
            if(nCode >= 0)
            {
                MouseHookConstruct mouseInput = (MouseHookConstruct)Marshal.PtrToStructure(IParam, typeof(MouseHookConstruct));
                MouseHookEventArgs args = new MouseHookEventArgs();
                args.X = mouseInput.pos.x;
                args.Y = mouseInput.pos.y;
                args.type = (MouseMessages)wParam;
                args.invokedTime = DateTime.Now;
                mouseEvent(null, args);

                /*
                if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
                {
                    args.type = (MouseMessages)wParam;
                    clickEvent(null, new HookEventArgs());
                }

                if(MouseMessages.WM_MOUSEMOVE == (MouseMessages)wParam)
                {
                    args.type = (MouseMessages)wParam;
                    moveEvent(null, new HookEventArgs());
                }
                */

            }
            return CallNextHookEx(hookHandle, nCode, wParam, IParam);
        }

        public static int getHookHandle() { return hookHandle;  }
    }
}