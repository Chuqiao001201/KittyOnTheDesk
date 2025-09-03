using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class WindowTransparent : MonoBehaviour
{
    // ���� user32.dll ����ʹ�� Windows API ����
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    // ����һ���ṹ���洢���ڱ߿�ı߾��С
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    // ���� user32.dll �Ի�ȡ����ھ�� (HWND)
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // ���� Dwmapi.dll �Խ����ڱ߿���չ���ͻ�����
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // ���� user32.dll ���޸Ĵ�������
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // ���� user32.dll �����ô���λ��
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    // ���� user32.dll �����÷ֲ㴰������ (͸����)
    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    // ������ʹ�õĳ����ͱ���
    const int GWL_EXSTYLE = -20;  // �޸Ĵ�����ʽ������
    const uint WS_EX_LAYERED = 0x00080000;  // �ֲ㴰�ڵ���չ��ʽ
    const uint WS_EX_TRANSPARENT = 0x00000020;  // ͸�����ڵ���չ��ʽ
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);  // ���ڲ���λ�ã�ʼ���ö���
    const uint LWA_COLORKEY = 0x00000001;  // ������ɫ���ı�־������͸���ȣ�
    private IntPtr hWnd;  // ����ڵľ��

    private void Start()
    {
        // ��ʾһ����Ϣ�򣨽�������ʾĿ�ģ�
        // MessageBox(new IntPtr(0), "Hello world", "Hello Dialog", 0);

#if !UNITY_EDITOR
        // ��ȡ����ڵľ��
        hWnd = GetActiveWindow();
 
        // ����һ���߾�ṹ������߿��С
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
 
        // �����ڱ߿���չ���ͻ����򣨲���Ч����
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
 
        // ��������ʽ����Ϊ�ֲ��͸��
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
 
        // ���ô�����ɫ��������͸���ȣ�
        SetLayeredWindowAttributes(hWnd, 1, 0, LWA_COLORKEY);
 
        // ������λ������Ϊʼ���ö�
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif

        // ����Ӧ���ں�̨����
        Application.runInBackground = true;
    }
}
