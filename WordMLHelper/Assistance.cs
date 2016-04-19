using System.Drawing;
using System.IO;
using System;
using System.Collections.Generic;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordMLHelperUtil
{
    /// <summary>
    /// 辅助方法
    /// </summary>
    public static class Assistance
    {
        private static List<string> tmpFilePaths = new List<string>();// 临时文件全路径集合

        /// <summary>
        /// 获取图片信息
        /// </summary>
        /// <param name="path">图片全路径</param>
        /// <param name="width" direction="out">宽</param>
        /// <param name="height" direction="out">高</param>
        /// <param name="horizontalResolution" direction="out">水平分辨率</param>
        /// <param name="verticalResolution" direction="out">垂直分辨率</param>
        internal static void GetImgInfo(string path
            , out int width
            , out int height
            , out float horizontalResolution
            , out float verticalResolution)
        {
            using (Bitmap img = new Bitmap(path))
            {
                width = img.Width;
                height = img.Height;
                horizontalResolution = img.HorizontalResolution;
                verticalResolution = img.VerticalResolution;
            }
        }

        /// <summary>
        /// 生成临时文件
        /// </summary>
        /// <param name="tmpDirectory">临时文件目录</param>
        /// <param name="fs">临时文件流（该流会被关闭）</param>
        /// <param name="extension">临时文件后缀名（带.）</param>
        /// <returns>临时文件全路径</returns>
        public static string CreateTmpFile(string tmpDirectory, Stream fs, string extension)
        {
            // 生成临时文件名
            string tmpFileName = string.Empty;
            Random random = new Random(DateTime.Now.Second);
            tmpFileName = string.Format("tmp{0}{1}", random.Next().ToString(), extension);
            string tmpFilePath = Path.Combine(tmpDirectory, tmpFileName);

            // 生成临时文件夹
            DirectoryInfo tmpDir = new DirectoryInfo(tmpDirectory);
            if (!tmpDir.Exists)
            {
                tmpDir.Create();
            }

            // 生成临时文件
            FileStream tmpFs = File.Create(tmpFilePath);
            byte[] fsByte = new byte[fs.Length];
            fs.Read(fsByte, 0, fsByte.Length);
            fs.Close();
            tmpFs.Write(fsByte, 0, fsByte.Length);
            tmpFs.Flush();
            tmpFs.Close();

            tmpFilePaths.Add(tmpFilePath);
            return tmpFilePath;
        }

        /// <summary>
        /// 删除所有临时文件
        /// </summary>
        public static void RemoveAllTmpFile()
        {
            string curFilePath = string.Empty;
            while (tmpFilePaths.Count >= 1)
            {
                File.Delete(tmpFilePaths[tmpFilePaths.Count - 1]);
                tmpFilePaths.RemoveAt(tmpFilePaths.Count - 1);
            }
        }

        /// <summary>
        /// 删除指定的临时文件
        /// </summary>
        /// <param name="tmpFilePath">要删除的临时文件全路径</param>
        public static void RemoveTmpFile(string tmpFilePath)
        {
            if (tmpFilePaths.Contains(tmpFilePath))
            {
                File.Delete(tmpFilePath);
                tmpFilePaths.Remove(tmpFilePath);
            }
        }

        /// <summary>
        /// 检查是否为.docx或.dotx文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool IsDocxEtc(string path)
        {
            string ext = Path.GetExtension(path).Substring(1);
            string[] newWT = Enum.GetNames(typeof(NewWordType));
            if (!newWT.Contains(ext))
            {
                throw new ArgumentOutOfRangeException("path的后缀名必须为.docx或.dotx");
            }

            return true;
        }
    }

    /// <summary>
    /// Office2003后的Word后缀名
    /// </summary>
    internal enum NewWordType
    {
        DOCX,
        DOTX
    }
}
