using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Export
{
    public static class ArchiveManageHelper
    {
        /// <summary>
        /// 保存当前存档实例
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        public static void SaveInstantArchive(string folder, string fileName)
        {
            string json = "";
            if (Application.isPlaying)
            {
                json = FullSerializerAPI.Serialize(typeof(PlayerData), DataCtrl.Instance.Data, false, false);
            }
            else
            {
                json = PlayerPrefs.GetString("PlayerData", string.Empty);
            }

            if (string.IsNullOrEmpty(json))
            {
                EditorUtility.DisplayDialog("存档失败", "存档失败 无档可存！", "ok");
                return;
            }

            FileHelper.WriteString(Path.Combine(folder, fileName + ".json"), json);
        }

        /// <summary>
        /// 导出一个存档到外部
        /// </summary>
        public static string ExportInstantArchiveTo(string fileName)
        {
            string folderPath = FolderBrowserHelper.GetPathFromWindowsExplorer("保存路径");
            if (string.IsNullOrEmpty(folderPath)) return string.Empty;

            string json = "";
            if (Application.isPlaying)
            {
                json = FullSerializerAPI.Serialize(typeof(PlayerData), DataCtrl.Instance.Data, false, false);
            }
            else
            {
                json = PlayerPrefs.GetString("PlayerData", string.Empty);
            }

            var filePath = Path.Combine(folderPath, fileName + ".json");
            File.WriteAllText(filePath, json);
            return filePath;
        }

        /// <summary>
        /// 从外部导入一个存档
        /// </summary>
        /// <param name="saveFolder"></param>
        public static void ImportArchiveFromOut(string saveFolder)
        {
            string selectFile = "";
            if (OpenFileHelper.SelectFileInExploer("json", out selectFile))
            {
                FileInfo file = new FileInfo(selectFile);
                var destFilePath = Path.Combine(saveFolder, file.Name);
                if (File.Exists(destFilePath))
                {
                    EditorUtility.DisplayDialog("Error!", "存档文件已存在，想导入需要重新命名导入的文件！", "ok");
                    return;
                }
                var json = FileHelper.ReadString(selectFile);
                if (CheckJsonFormat(json))
                {
                    file.CopyTo(destFilePath);
                }
            }
        }

        /// <summary>
        /// 使用选择的存档
        /// </summary>
        /// <param name="archiveFile"></param>
        public static void ReplaceUsingArchive(string archiveFile)
        {
            if (!File.Exists(archiveFile))
            {
                Debug.LogError("存档文件路径不存在：" + archiveFile);
                return;
            }

            var json = FileHelper.ReadString(archiveFile);
            if (CheckJsonFormat(json))
            {
                PlayerPrefs.SetString("PlayerData", json);
                EditorUtility.DisplayDialog("ok", "ok", "ok");
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "导入的存档解析失败，看看错误日志！", "ok");
            }

        }

        /// <summary>
        /// 检查json是否符合格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool CheckJsonFormat(string json)
        {
            try
            {
                var data = FullSerializerAPI.Deserialize(typeof(PlayerData), json, false) as PlayerData;
                if (data != null)
                {// 成功了 
                    return true;
                }
            }
            catch (Exception err)
            {
                Debug.LogError(err);
            }
            return false;
        }
    }
}
