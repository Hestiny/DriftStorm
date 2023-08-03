using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace Export
{
    /// <summary> 
    /// 存档管理弹框
    /// </summary>
    public class ArchiveManageWindow : EditorWindow
    {
        [MenuItem("工具栏/存档管理面板", false, 1)]
        public static void OpenWindow()
        {
            Win = GetWindow<ArchiveManageWindow>("Archive", true);
#if UNITY_2019_3_OR_NEWER
            Win.minSize = new Vector2(600f, 640f);
#else
            Win.minSize = new Vector2(600f, 600f);
#endif
        }
        public static ArchiveManageWindow Win;
        private readonly string ArchiveManageTag = "ArchiveManageTag";

        GUIStyle whiteFontStyle = new GUIStyle();
        GUIStyle redFontStyle = new GUIStyle();

        private string ArchiveFolder;

        private List<ArchiveItem> items = new List<ArchiveItem>();
        private string tag = string.Empty;

        private bool isDeleteMode = false;

        private void Awake()
        {
            whiteFontStyle.normal.textColor = new Color(1, 1, 1, 1);
            redFontStyle.normal.textColor = new Color(1, 0, 0, 1);
        }

        private void OnEnable()
        {
            RefleshArchives();
        }
        private void OnFocus()
        {
            RefleshArchives();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal("box");
                {
                    if (GUILayout.Button("↓即时存档"))
                    {
                        if (string.IsNullOrEmpty(tag))
                        {
                            EditorGUILayout.LabelField("必须设置一个独特的存档Tag：", redFontStyle);
                        }
                        else
                        {
                            var fileName = tag + DateTime.Now.ToString("-yyyy-MM-dd-(HH-mm-ss-fff)");
                            ArchiveManageHelper.SaveInstantArchive(ArchiveFolder, fileName);
                            RefleshArchives();
                        }
                    }
                    GUILayout.Space(10);

                    if (GUILayout.Button("→导出即时存档"))
                    {
                        if (string.IsNullOrEmpty(tag))
                        {
                            EditorGUILayout.LabelField("必须设置一个独特的存档Tag：", redFontStyle);
                        }
                        else
                        {
                            var fileName = tag + DateTime.Now.ToString("-yyyy-MM-dd-(HH-mm-ss-fff)");
                            var filePath = ArchiveManageHelper.ExportInstantArchiveTo(fileName);
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                OpenFileHelper.OpenFolder(filePath);
                            }
                        }
                    }
                    if (GUILayout.Button("↓加载外部存档"))
                    {
                        ArchiveManageHelper.ImportArchiveFromOut(ArchiveFolder);
                        RefleshArchives();
                    }
                    GUILayout.Space(40);
                    if (GUILayout.Button(isDeleteMode ? "*退出删除模式" : "删除模式"))
                    {
                        isDeleteMode = !isDeleteMode;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (string.IsNullOrEmpty(tag))
                    {
                        EditorGUILayout.LabelField("必须设置一个独特的存档Tag：", redFontStyle);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("存档Tag：");
                    }
                    var newTag = EditorGUILayout.TextField(tag);
                    if (newTag != tag)
                    {
                        tag = newTag;
                        EditorPrefs.SetString(ArchiveManageTag, newTag);
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);
                if (isDeleteMode)
                {
                    EditorGUILayout.LabelField("*** 当前处于删除模式中！***", redFontStyle);
                    GUILayout.Space(10f);
                }

                EditorGUILayout.BeginVertical();
                {
                    if (items.Count <= 0)
                    {
                        EditorGUILayout.LabelField("还没有存档文件！点击 “即时存档” 按钮保存一份吧！");
                    }
                    else
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            items[i]?.OnGUI(isDeleteMode);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        public int WaitFrame = 0;

        public void StartDelayReflesh()
        {
            WaitFrame = 1;
            EditorApplication.update += WaitDelayReflesh;
        }

        void WaitDelayReflesh()
        {
            if (WaitFrame-- <= 0)
            {
                EditorApplication.update -= WaitDelayReflesh;
                RefleshArchives();
            }
        }

        void RefleshArchives()
        {
            Win = this;
            tag = EditorPrefs.GetString(ArchiveManageTag, "");
            try
            {
                ArchiveFolder = Application.dataPath + "/../Library/MyArchives";
                if (!Directory.Exists(ArchiveFolder))
                {
                    Directory.CreateDirectory(ArchiveFolder);
                }
                var allFiles = Directory.GetFiles(ArchiveFolder);

                items.Clear();
                for (int i = 0; i < allFiles.Length; i++)
                {
                    var item = new ArchiveItem(i, allFiles[i]);
                    items.Add(item);
                }
            }
            catch (Exception err)
            {
                Debug.LogError(err);
            }
        }

    }

    class ArchiveItem
    {
        public int Index { private set; get; }
        public string FilePath { private set; get; }

        public FileInfo fileInfo { private set; get; }

        public ArchiveItem(int index, string path)
        {
            Index = index;
            FilePath = path;
            fileInfo = new FileInfo(path);
        }

        public void OnGUI(bool deleteMode)
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(600 - 10));
            {
                EditorGUILayout.LabelField($"{Index}. {fileInfo.Name}");
                if (deleteMode)
                {
                    if (GUILayout.Button("删除存档"))
                    {
                        if (File.Exists(FilePath))
                        {
                            File.Delete(FilePath);
                            ArchiveManageWindow.Win?.StartDelayReflesh();
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("使用存档"))
                    {
                        if (Application.isPlaying)
                        {
                            EditorUtility.DisplayDialog("警告", "游戏停止时才可以替换存档！", "ok");
                        }
                        else
                        {
                            ArchiveManageHelper.ReplaceUsingArchive(FilePath);
                        }
                    }
                    GUILayout.Space(20);
                    if (GUILayout.Button("文件位置"))
                    {
                        OpenFileHelper.OpenFolder(FilePath);
                    }

                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
