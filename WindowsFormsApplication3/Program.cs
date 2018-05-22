﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


class Program
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static WindowsFormsApplication3.Form1 static_form1;


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [STAThread]
    static void Main(string[] args)
    {

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);


        _hookID = SetHook(_proc);
        //Application.Run();
        static_form1 = new WindowsFormsApplication3.Form1();
        Application.Run(static_form1);

        UnhookWindowsHookEx(_hookID);

    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private static void ToggleCapsLock()
    {
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;

        UnhookWindowsHookEx(_hookID);
        keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
        keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        _hookID = SetHook(_proc);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {


        // name = frm.Textbox1Text;
        //name = name.Replace("Form1", "");




        Keys key = (Keys)Marshal.ReadInt32(lParam);
        if (key == Keys.F2)
        {
            String a;
            a = DateTime.Now.ToString();
            a = "[" + a + "]";
            Clipboard.SetText(a);
            // SendKeys.Send("^{v}");
        }


        if (key == Keys.F3)
        {
            WindowsFormsApplication3.Form1 frm = new WindowsFormsApplication3.Form1();
            String name;

            //name = frm.Textbox1Text + "lol";
            name = static_form1.Textbox1Text;
            String a;
            // String b = textBox1_TextChanged.Text;
            a = DateTime.Now.ToString();
            a = "[" + a + "]" + name;
            Clipboard.SetText(a);
            // SendKeys.Send("^{v}");     

        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }
}