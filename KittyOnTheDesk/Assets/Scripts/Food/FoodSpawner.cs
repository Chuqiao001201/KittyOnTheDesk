using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Food
{
    public class FoodSpawner : Singleton<FoodSpawner>
    {
        [Header("生成设置")]
        public GameObject prefabToSpawn;
        public Vector3 spawnOffset = Vector3.zero;
        public bool spawnAtMousePosition = true;
        
        [Header("限制设置")]
        public int spawnEveryXPresses = 3; // 每按X次生成一个预制体
        private int pressCount = 0; // 按键计数
        public List<GameObject> spawnedObjects;

        // Windows API相关定义
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101; // 键盘释放事件
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        // 键盘钩子回调函数委托
        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        // 跟踪按键状态
        private static bool isKeyDown = false;
        private static int currentKeyCode = 0;
        private static bool keyClicked = false;

        private void Start()
        {
            // 安装键盘钩子
            _hookID = SetHook(_proc);
        }

        private void OnDestroy()
        {
            // 卸载键盘钩子
            UnhookWindowsHookEx(_hookID);
        }

        private void Update()
        {
            // 检测到完整的键盘点击
            if (keyClicked && prefabToSpawn != null)
            {
                pressCount++; // 增加按键计数
                keyClicked = false; // 重置状态

                // 当按键次数达到设定值时生成预制体并重置计数
                if (pressCount >= spawnEveryXPresses)
                {
                    SpawnPrefab();
                    pressCount = 0; // 重置计数
                }
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KeyCode key = (KeyCode)vkCode;

                // 过滤不需要监听的按键
                if (key.ToString().Contains("Mouse") ||
                    key == KeyCode.LeftShift || key == KeyCode.RightShift ||
                    key == KeyCode.LeftControl || key == KeyCode.RightControl ||
                    key == KeyCode.LeftAlt || key == KeyCode.RightAlt)
                {
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }

                // 处理按键按下事件
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    isKeyDown = true;
                    currentKeyCode = vkCode;
                }
                // 处理按键释放事件
                else if (wParam == (IntPtr)WM_KEYUP && isKeyDown && vkCode == currentKeyCode)
                {
                    // 只有当同一个键按下并释放时，才视为一次完整点击
                    isKeyDown = false;
                    keyClicked = true;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void SpawnPrefab()
        {
            Vector3 spawnPosition;
            spawnOffset.x = Random.Range(-0.25f, 0.25f);

            if (spawnAtMousePosition)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                if (plane.Raycast(ray, out float distance))
                {
                    spawnPosition = ray.GetPoint(distance);
                }
                else
                {
                    spawnPosition = transform.position + spawnOffset;
                }
            }
            else
            {
                spawnPosition = transform.position + spawnOffset;
            }

            var obj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
            spawnedObjects.Add(obj);
        }

        // 导入Windows API函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public int DeliverFood()
        {
            var num = spawnedObjects.Count>120 ? 120 : spawnedObjects.Count;
            for (var i = 0; i < num; i++)
            {
                Destroy(spawnedObjects[i]);
            }
            return num;
        }
    }
}
