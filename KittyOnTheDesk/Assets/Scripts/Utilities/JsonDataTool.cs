using System.IO;
using DataSystem;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// 用于序列化/反序列化 SaveData 的 JSON 工具类
    /// 支持保存 Vector3 和 List<MemoData> 类型数据
    /// </summary>
    public static class JsonDataTool
    {
        #region 配置：可自定义存档文件名和路径
        // 1. 存档文件名（可根据需求修改，如 "GameSaveData.json"）
        private const string SaveFileName = "SaveData.json";
    
        // 2. 最终存档路径：结合 Unity 可写目录 + 文件名
        // Application.persistentDataPath 是系统为游戏分配的专属可写目录，打包后稳定
        public static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        #endregion


        #region 核心方法：保存 SaveData 到 JSON 文件
        /// <summary>
        /// 保存 SaveData 数据到本地 JSON 文件
        /// </summary>
        /// <param name="saveData">要保存的 SaveData 对象（包含 mainViewPos 和 memoList）</param>
        /// <param name="isPrettyPrint">是否格式化 JSON 文本（true 便于调试，false 减小文件体积）</param>
        public static void SaveData(SaveData saveData, bool isPrettyPrint = true)
        {
            // 安全校验：避免传入空对象导致报错
            if (saveData == null)
            {
                Debug.LogError("[JsonDataTool] 保存失败：传入的 SaveData 为空！");
                return;
            }

            try
            {
                // 1. 将 SaveData 对象序列化为 JSON 字符串
                // JsonUtility.ToJson(对象, 是否格式化)
                string jsonContent = JsonUtility.ToJson(saveData, isPrettyPrint);

                // 2. 写入文件（不存在则自动创建，存在则覆盖）
                // File.WriteAllText 会自动处理文件流，无需手动关闭
                File.WriteAllText(SaveFilePath, jsonContent);

                // 3. 调试信息：方便查看存档路径（打包后可通过日志定位文件）
                Debug.Log($"[JsonDataTool] 存档成功！路径：\n{SaveFilePath}");
            }
            catch (System.Exception e)
            {
                // 捕获异常（如权限不足、磁盘满等），避免游戏崩溃
                Debug.LogError($"[JsonDataTool] 存档失败！错误信息：\n{e.Message}");
            }
        }
        #endregion


        #region 核心方法：从 JSON 文件读取 SaveData
        /// <summary>
        /// 从本地 JSON 文件读取 SaveData 数据
        /// </summary>
        /// <param name="defaultData">当存档文件不存在时，返回的默认数据（避免空引用）</param>
        /// <returns>读取到的 SaveData 对象 / 无存档时返回默认数据</returns>
        public static SaveData LoadData(SaveData defaultData = null)
        {
            // 1. 先判断存档文件是否存在
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning($"[JsonDataTool] 未找到存档文件！路径：\n{SaveFilePath}\n将返回默认数据");
                // 无存档时，返回传入的默认数据（若没传默认值，返回空的 SaveData）
                return defaultData ?? DataManager.Instance.saveData;
            }

            try
            {
                // 2. 读取 JSON 文件内容
                string jsonContent = File.ReadAllText(SaveFilePath);

                // 3. 校验 JSON 内容是否为空（避免文件损坏导致反序列化失败）
                if (string.IsNullOrEmpty(jsonContent))
                {
                    Debug.LogError("[JsonDataTool] 读取失败：存档文件为空！");
                    return defaultData ?? DataManager.Instance.saveData;
                }

                // 4. 将 JSON 字符串反序列化为 SaveData 对象
                // 注意：JsonUtility.FromJson 需要先创建空对象，再通过 FromJsonOverwrite 赋值
                // （直接用 FromJson<SaveData>() 对 ScriptableObject 兼容性稍差，推荐用 Overwrite 方式）
                SaveData loadedData = DataManager.Instance.saveData;
                JsonUtility.FromJsonOverwrite(jsonContent, loadedData);

                Debug.Log($"[JsonDataTool] 读档成功！路径：\n{SaveFilePath}");
                return loadedData;
            }
            catch (System.Exception e)
            {
                // 捕获反序列化异常（如 JSON 格式损坏）
                Debug.LogError($"[JsonDataTool] 读档失败！错误信息：\n{e.Message}");
                return defaultData ?? DataManager.Instance.saveData;
            }
        }
        #endregion


        #region 辅助方法：删除存档文件（可选）
        /// <summary>
        /// 删除本地存档文件（用于“重置存档”功能）
        /// </summary>
        public static void DeleteSaveData()
        {
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    File.Delete(SaveFilePath);
                    Debug.Log($"[JsonDataTool] 存档已删除！路径：\n{SaveFilePath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[JsonDataTool] 删除存档失败！错误信息：\n{e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("[JsonDataTool] 无存档文件可删除！");
            }
        }
        #endregion
    }
}
