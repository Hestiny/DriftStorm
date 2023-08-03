using System.IO;
using System.Threading;
using UnityEngine;

namespace Export
{

    public static class OpenFileHelper
    {
        public static void OpenFolder(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            System.Diagnostics.Process.Start("explorer.exe", fileInfo.Directory.FullName);
        }

        /// <summary>
        /// 直接使用默认应用打开文件
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory(string path)
        {
            // 新开线程防止锁死
            Thread newThread = new Thread(new ParameterizedThreadStart(delegate (object obj)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
#if UNITY_EDITOR_WIN
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c start " + obj.ToString();
#elif UNITY_EDITOR_OSX
            p.StartInfo.FileName = "bash";
            string shellPath = "";
            string shPath = shellPath + "openDir.sh";
            p.StartInfo.Arguments = shPath + " " + obj.ToString();
#endif
                UnityEngine.Debug.Log(p.StartInfo.Arguments);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                p.WaitForExit();
                p.Close();
            }));
            newThread.Start(path);
        }

        /// <summary>
        /// 选择文件浏览器中的文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool SelectFileInExploer(string type, out string filePath)
        {

            OpenFileDlg pth = new OpenFileDlg();
            pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);

            pth.filter = "文件(*." + type + ")\0*." + type + "";//筛选文件类型
                                                              //pth.filter = "图片文件(*.jpg*.png)\0*.jpg;*.png";
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = Application.streamingAssetsPath.Replace('/', '\\');  // default path
            pth.title = "选择项目json";
            pth.defExt = type;//显示文件类型
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if (OpenFileDialog.GetOpenFileName(pth))
            {
                filePath = pth.file;//选择的文件路径;
                return true;
            }
            else
            {
                filePath = "";
                return false;
            }

        }

    }

}
