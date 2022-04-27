using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using ScreenCapture.Util;

namespace ScreenCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// キャプチャウィンドウ
        /// </summary>
        readonly CaptureWindow captureWindow = new();

        /// <summary>
        /// DispatcherTimer
        /// </summary>
        DispatcherTimer dispatcherTimer = new();

        /// <summary>
        /// Bitmap
        /// </summary>
        Bitmap? bitmap = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            captureWindow.Owner = this;
            captureWindow.Show();
            SetCaptureMode();
        }

        /// <summary>
        /// 撮影ボタンクリック時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            // 撮影前処理
            captureWindow.BeforeCapture();

            // buttonの制御
            CaptureButton.IsEnabled = false;

            // タイマー生成と開始
            dispatcherTimer = CreateTimer(1000, CaptureOnlyOnce);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// 連続撮影ボタンクリック時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BurstShotButton_Click(object sender, RoutedEventArgs e)
        {
            // 撮影間隔が取得できない場合
            if (CaptureInterval.Value == null) {
                return;
            }

            // checkboxとbuttonの制御
            BurstShotButton.IsEnabled = false;
            StopBurestShotButton.IsEnabled = true;
            BurstShotCheckBox.IsEnabled = false;

            // 撮影前処理
            captureWindow.BeforeCapture();

            // タイマー生成と開始
            dispatcherTimer = CreateTimer(Convert.ToInt32(CaptureInterval.Value), BurstShot);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// 撮影停止ボタンクリック時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopBurestShotButton_Click(object sender, RoutedEventArgs e)
        {
            // タイマー停止
            dispatcherTimer.Stop();

            // 撮影後処理
            captureWindow.AfterCapture();

            // checkboxとbuttonの制御
            BurstShotButton.IsEnabled = true;
            StopBurestShotButton.IsEnabled = false;
            BurstShotCheckBox.IsEnabled = true;
        }

        /// <summary>
        /// 保存ボタンクリック時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (bitmap == null)
            {
                return;
            }

            string savePath = FileUtil.GetSavePath(FileNamePrefixText.Text);
            if (savePath.Equals(string.Empty))
            {
                return;
            }

            FileUtil.SaveAsPng(savePath, bitmap);
        }

        /// <summary>
        /// 連写撮影チェックボックスクリック時処理
        /// </summary>
        /// <remarks>撮影モードの切り替えを行う</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BurstShotCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (BurstShotCheckBox.IsChecked == true)
            {
                SetBurstShotMode();
                return;
            }
            SetCaptureMode();
        }

        /// <summary>
        /// 自動保存チェックボックスクリック時処理
        /// </summary>
        /// <remarks>撮影モードの切り替えを行う</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoSaveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AutoSaveCheckBox.IsChecked == true)
            {
                SaveButton.IsEnabled = false;
                return;
            }
            SaveButton.IsEnabled = true;
        }

        /// <summary>
        /// Windowクローズ前処理
        /// </summary>
        /// <remarks>キャプチャWindowをクローズする</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            captureWindow.Close();
        }

        /// <summary>
        /// 自動保存
        /// </summary>
        private void AutoSave()
        {
            if (bitmap == null)
            {
                return;
            }

            string savePath = FileUtil.CreateSavePath(FileNamePrefixText.Text);
            if (savePath.Equals(string.Empty))
            {
                return;
            }

            FileUtil.SaveAsPng(savePath, bitmap);
        }

        /// <summary>
        /// 一度の撮影
        /// </summary>
        private void CaptureOnlyOnce()
        {
            Capture();
            if (AutoSaveCheckBox.IsChecked == true)
            {
                AutoSave();
            }
            dispatcherTimer.IsEnabled = false;
            CaptureButton.IsEnabled = true;
            captureWindow.AfterCapture();
        }

        /// <summary>
        /// 連射撮影
        /// </summary>
        private void BurstShot()
        {
            Capture();
            AutoSave();
        }

        /// <summary>
        /// 撮影
        /// </summary>
        private void Capture()
        {
            bitmap = captureWindow.GetCapture();
            BitmapSource bitmapsource = ImageUtil.ConvertBitmapToBitmapSource(bitmap);
            captureImage.Source = bitmapsource;

            captureImage.Height = bitmapsource.Height;
            captureImage.Width = bitmapsource.Width;
        }

        private DispatcherTimer CreateTimer(int interval, Action action)
        {
            DispatcherTimer dispatcherTimer = new(DispatcherPriority.Background);
            dispatcherTimer.Tick += (e, s) => { action(); };
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);

            // クローズ時にはタイマーを停止
            this.Closing += (e, s) => { dispatcherTimer.Stop(); };

            return dispatcherTimer;
        }


        /// <summary>
        /// 撮影モード設定
        /// </summary>
        private void SetCaptureMode()
        {
            BurstShotModePanel.Visibility = Visibility.Collapsed;
            BurstShotButton.IsEnabled = false;
            StopBurestShotButton.IsEnabled = false;
            CaptureModePanel.Visibility = Visibility.Visible;
            CaptureButton.IsEnabled = true;
            SaveButton.IsEnabled = false;

            // 自動保存の変更可
            AutoSaveCheckBox.IsEnabled = true;
        }


        /// <summary>
        /// 連写モード設定
        /// </summary>
        private void SetBurstShotMode()
        {
            BurstShotModePanel.Visibility = Visibility.Visible;
            BurstShotButton.IsEnabled = true;
            StopBurestShotButton.IsEnabled = false;
            CaptureModePanel.Visibility = Visibility.Collapsed;
            CaptureButton.IsEnabled = false;
            SaveButton.IsEnabled = false;

            // 自動保存の変更不可
            AutoSaveCheckBox.IsEnabled = false;
            AutoSaveCheckBox.IsChecked = true;
        }

    }
}
