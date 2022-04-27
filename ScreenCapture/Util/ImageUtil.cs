using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace ScreenCapture.Util
{
    internal class ImageUtil
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hBitmap);

        /// <summary>
        /// BitmapからBitmapSourceへの変換
        /// </summary>
        /// <param name="bitmap">bitmap</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            BitmapSource? imageSource = null;

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                                    hBitmap,
                                    IntPtr.Zero,
                                    Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());
                imageSource.Freeze();
            }
            finally
            {
                DeleteObject(hBitmap);
            }


            return imageSource;
        }
    }
}
