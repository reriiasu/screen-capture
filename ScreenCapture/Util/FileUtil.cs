using System;
using System.IO;
using System.Drawing;
using Microsoft.Win32;

namespace ScreenCapture.Util
{
    internal class FileUtil
    {
        /// <summary>
        /// ファイル名作成
        /// </summary>
        /// <param name="prefix">file prefix</param>
        /// <returns>ファイル名</returns>
        public static string CrateFileName(string prefix)
        {
            return $"{prefix}{DateTime.Now:_yyyyMMdd_HHmmssFFF}.png";
        }

        /// <summary>
        /// 保存パス生成
        /// </summary>
        /// <param name="prefix">file prefix</param>
        /// <returns>保存パス</returns>
        public static string CreateSavePath(string prefix)
        {
            string? path = GetCurrentAppDir();
            if (path == null)
            {
                return string.Empty;
            }
            return Path.Combine(path, CrateFileName(prefix));
        }

        /// <summary>
        /// 保存パス取得
        /// </summary>
        /// <param name="prefix">file prefix</param>
        /// <returns>保存パス取得</returns>
        public static string GetSavePath(string prefix)
        {
            SaveFileDialog dialog = new();
            dialog.FileName = CrateFileName(prefix);
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG|*.png";

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }

            return string.Empty;
        }


        /// <summary>
        /// PNG保存
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="bitmap">Bitmap</param>
        public static void SaveAsPng(string filePath, Bitmap bitmap)
        {
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

        }

        /// <summary>
        /// カレントディレクトリ取得
        /// </summary>
        /// <returns>カレントディレクトリ</returns>
        private static string? GetCurrentAppDir()
        {
            return Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
