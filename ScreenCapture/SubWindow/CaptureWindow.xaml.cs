using System.Drawing;
using MahApps.Metro.Controls;

namespace ScreenCapture
{
    /// <summary>
    /// CaptureWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureWindow : MetroWindow
    {
        public CaptureWindow()
        {
            InitializeComponent();

            EnableTopMost();
        }

        /// <summary>
        /// 最前面有効
        /// </summary>
        public void EnableTopMost()
        {
            Topmost = true;
        }

        /// <summary>
        /// 最前面無効
        /// </summary>
        public void DisableTopMost()
        {
            Topmost = false;
        }


        /// <summary>
        /// キャプチャ前処理
        /// キャプチャに不要な情報を透過にする
        /// </summary>
        public void BeforeCapture()
        {
            GlowBrush = System.Windows.Media.Brushes.Transparent;
            NonActiveGlowBrush = System.Windows.Media.Brushes.Transparent;
            TitleForeground = System.Windows.Media.Brushes.Transparent;
        }

        /// <summary>
        /// キャプチャ後処理
        /// キャプチャ後に元のスタイルへ戻す
        /// </summary>
        public void AfterCapture()
        {
            GlowBrush = System.Windows.Media.Brushes.Red;
            NonActiveGlowBrush = System.Windows.Media.Brushes.Red;
            TitleForeground = System.Windows.Media.Brushes.Red;
        }


        /// <summary>
        /// キャプチャ取得
        /// </summary>
        /// <returns>キャプチャ</returns>
        public Bitmap GetCapture()
        {
            System.Windows.Point screen = PointToScreen(new System.Windows.Point(0.0d, 0.0d));
            Bitmap resizedBmp = new ((int)Width, (int)Height);
            using (var bmp = new Bitmap((int)Width, (int)Height))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Graphics resizedG = Graphics.FromImage(resizedBmp))
            {
                // スクリーンショットを撮る
                g.CopyFromScreen(new System.Drawing.Point((int)screen.X, (int)screen.Y), new System.Drawing.Point(0, 0), bmp.Size);

                // 動画サイズを減らすためリサイズする
                resizedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                resizedG.DrawImage(bmp, 0, 0, (int)Width, (int)Height);
            }

            return resizedBmp;
        }
    }
}
