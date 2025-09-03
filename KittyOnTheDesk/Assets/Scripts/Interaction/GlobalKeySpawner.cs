using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

namespace Interaction
{
    public class GlobalKeySpawner : MonoBehaviour
    {
        [Header("生成设置")]
        public GameObject prefabToSpawn;
        public Vector3 spawnOffset = Vector3.zero;
        public bool spawnAtMousePosition = true;
        
        [Header("限制设置")]
        public float cooldown = 0.2f;
        private float lastSpawnTime;

        // Windows API相关定义
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        // 键盘钩子回调函数委托
        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        // 按键状态标记
        private static bool keyPressed = false;

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
            // 检测到全局按键且满足条件时生成预制体
            if (keyPressed && prefabToSpawn != null && 
                Time.time - lastSpawnTime >= cooldown)
            {
                SpawnPrefab();
                lastSpawnTime = Time.time;
                keyPressed = false; // 重置状态
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
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                
                // 可以在这里过滤不需要监听的按键
                KeyCode key = (KeyCode)vkCode;
                if (!key.ToString().Contains("Mouse") &&
                    key != KeyCode.LeftShift && key != KeyCode.RightShift &&
                    key != KeyCode.LeftControl && key != KeyCode.RightControl &&
                    key != KeyCode.LeftAlt && key != KeyCode.RightAlt)
                {
                    keyPressed = true;
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

            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
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
    }
}
