using WinUIEx;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SockboomClient.Helpers;
using System;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage;
using Windows.UI.ViewManagement;
using WinRT.Interop;
using SockboomClient.Client;
using System.Collections.Generic;
using SockboomClient.Model;
using Microsoft.UI.Xaml.Controls;
using SockboomClient.Componse;
using static Vanara.PInvoke.Kernel32.REASON_CONTEXT;

namespace SockboomClient
{
    /// <summary>
    /// 登录窗口
    /// </summary>
    public sealed partial class LoginWindow : Window
    {
        private IntPtr hwnd;

        private AppWindow appWindow;

        private AppWindowTitleBar titleBar;

        private Helpers.SystemBackdrop backdrop;

        public LoginWindow()
        {
            this.InitializeComponent();
            this.SetWindowSize(500, 300);

            #region 背景、样式与标题栏
            // 设置云母或亚克力背景
            backdrop = new Helpers.SystemBackdrop(this);
            backdrop.TrySetMica(fallbackToAcrylic: true);

            // 窗口句柄
            hwnd = WindowNative.GetWindowHandle(this);
            WindowId id = Win32Interop.GetWindowIdFromWindow(hwnd);
            appWindow = AppWindow.GetFromWindowId(id);

            // 初始化窗口大小和位置
            this.Closed += MainWindow_Closed;
            if (ApplicationData.Current.LocalSettings.Values["IsLoginWindowMaximum"] is true)
            {
                // 最大化
                User32.ShowWindow(hwnd, ShowWindowCommand.SW_SHOWMAXIMIZED);
            }
            else if (ApplicationData.Current.LocalSettings.Values["LoginWindowRect"] is ulong value)
            {
                var rect = new WindowRect(value);
                // 屏幕区域
                var area = DisplayArea.GetFromWindowId(windowId: id, DisplayAreaFallback.Primary);
                // 若窗口在屏幕范围之内
                if (rect.Left > 0 && rect.Top > 0 && rect.Right < area.WorkArea.Width && rect.Bottom < area.WorkArea.Height)
                {
                    appWindow.MoveAndResize(rect.ToRectInt32());  
                }
            }

            // 自定义标题栏
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                // 不支持时 titleBar 为 null
                titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                // 标题栏按键背景色设置为透明
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                // 获取系统缩放率
                var scale = (float)User32.GetDpiForWindow(hwnd) / 96;
                // 48 这个值是应用标题栏的高度，不是唯一的，根据自己的 UI 设计而定
                titleBar.SetDragRectangles(new RectInt32[] { new RectInt32((int)(48 * scale), 0, 10000, (int)(48 * scale)) });
            }
            else
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(AppTitleBar);
            }
        }
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            // 保存窗口状态
            var wpl = new User32.WINDOWPLACEMENT();
            if (User32.GetWindowPlacement(hwnd, ref wpl))
            {
                ApplicationData.Current.LocalSettings.Values["IsLoginWindowMaximum"] = wpl.showCmd == ShowWindowCommand.SW_MAXIMIZE;
                var p = appWindow.Position;
                var s = appWindow.Size;
                var rect = new WindowRect(p.X, p.Y, s.Width, s.Height);
                ApplicationData.Current.LocalSettings.Values["LoginWindowRect"] = rect.Value;
            }

        }

        /// <summary>
        /// RectInt32 和 ulong 相互转换
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct WindowRect
        {
            [FieldOffset(0)]
            public short X;
            [FieldOffset(2)]
            public short Y;
            [FieldOffset(4)]
            public short Width;
            [FieldOffset(6)]
            public short Height;
            [FieldOffset(0)]
            public ulong Value;

            public int Left => X;
            public int Top => Y;
            public int Right => X + Width;
            public int Bottom => Y + Height;

            public WindowRect(int x, int y, int width, int height)
            {
                X = (short)x;
                Y = (short)y;
                Width = (short)width;
                Height = (short)height;
            }

            public WindowRect(ulong value)
            {
                Value = value;
            }

            public RectInt32 ToRectInt32()
            {
                return new RectInt32(X, Y, Width, Height);
            }
        }
        #endregion


        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var KeepLogin = KeepLoginCheckBox.IsChecked;
            var Email = LoginInput.Text;
            var Password = PasswordInput.Password;
            SetElementStatus(false);
            // 检查邮箱和密码是否为空
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ShowDialog("登录失败", "邮箱或密码不能为空");
                SetElementStatus(true);
                return;
            }
            // 获取用户 Token
            var Result = await ApiClient.PostRequest<string>(Client.Apis.PostPaths.GETTOKEN, new Dictionary<string, string>
            {
                { "email" , Email },
                { "passwd", Password }
            });
            // 前端显示处理
            

            if (!Result.Success)
            {
                SetElementStatus(true);
                // 登录失败
                ShowDialog("登录失败", "邮箱或密码错误:" + Result.Code +"/" + Result.Message);
                return;
            }

            if (KeepLogin == true)
            {
                // 保存登录信息与是否自动登录
                ApplicationData.Current.LocalSettings.Values["AutoLogin"] = "true";
                ApplicationData.Current.LocalSettings.Values["Token"] = Result.Token;
            }

            var user = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string> { { "token", Result.Token } });
            this.Hide();
            new MainWindow(user.Data).Activate();
            this.Close();
        }

        private async void LoginByTokenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var KeepLogin = KeepLoginCheckBox.IsChecked;
            var Token = PasswordInput.Password;
            SetElementStatus(false);

            // 检查邮箱和密码是否为空
            if (string.IsNullOrEmpty(Token))
            {
                ShowDialog("登录失败", "你未填写 TOKEN (在密码一栏中填写 Token 即可，邮箱留空）");
                SetElementStatus(true);
                return;
            }

            var user = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string> { { "token", Token } });

            // 登录失败
            if (!user.Success)
            {
                SetElementStatus(true);
                ShowDialog("登录失败", "Token 有误:" + user.Code + "/" + user.Message);
                return;
            }

            if (KeepLogin == true)
            {
                // 保存登录信息与是否自动登录
                ApplicationData.Current.LocalSettings.Values["AutoLogin"] = "true";
                ApplicationData.Current.LocalSettings.Values["Token"] = Token;
            }

            
            this.Hide();
            new MainWindow(user.Data).Activate();
            this.Close();


        }

        /// <summary>
        /// 在 false 时停用所有元素编辑功能, true 时启用
        /// false 时设置 ProgressRing 启用, 修改 Login Button 按钮文本, true 反之
        /// </summary>
        /// <param name="status"></param>
        private void SetElementStatus(bool status)
        {
            if (status)
            {
                LoginProgressRing.Visibility = Visibility.Collapsed;
                LoginButton.Content = "登录";
            }
            else
            {
                LoginProgressRing.Visibility = Visibility.Visible;
                LoginButtonText.Text = "登录中...";
            }
            LoginInput.IsEnabled = status;
            PasswordInput.IsEnabled = status;
            KeepLoginCheckBox.IsEnabled = status;
            LoginButton.IsEnabled = status;
            LoginByTokenButton.IsEnabled = status;
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        private async void ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = title;
            dialog.PrimaryButtonText = "好";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new Dialog(message);
            var result = await dialog.ShowAsync();
        }
    }
}
