using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SockboomClient.Helpers;
using SockboomClient.Model;
using SockboomClient.Views;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.Storage;
using WinUIEx;
using WinRT;
using WinRT.Interop;
using SockboomClient.ViewModel;

namespace SockboomClient
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IntPtr hwnd;

        private AppWindow appWindow;

        private AppWindowTitleBar titleBar;

        private Helpers.SystemBackdrop backdrop;

        private SharedViewModel _vm;

        private UserInfo _user;

        [Obsolete]
        public MainWindow()
        {
            this.InitializeComponent();
            InitWindowFancy();
            
        }

        #region 背景、样式与标题栏 与 最小窗口限制
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            // 保存窗口状态
            var wpl = new Vanara.PInvoke.User32.WINDOWPLACEMENT();
            if (Vanara.PInvoke.User32.GetWindowPlacement(hwnd, ref wpl))
            {
                ApplicationData.Current.LocalSettings.Values["IsMainWindowMaximum"] = wpl.showCmd == Vanara.PInvoke.ShowWindowCommand.SW_MAXIMIZE;
                var p = appWindow.Position;
                var s = appWindow.Size;
                var rect = new WindowRect(p.X, p.Y, s.Width, s.Height);
                ApplicationData.Current.LocalSettings.Values["MainWindowRect"] = rect.Value;
            }
        }

        
        private void InitWindowFancy()
        {
            this.SetWindowSize(width: 1366, 750);
            _vm = SharedViewModel.GetInstance();
            _user = _vm.UserInfo;
            // 设置第一屏
            navigationList.SelectedItem = FindMenuItemByTag(navigationList, "ProxyState");
            contentFrame.Navigate(typeof(ClashConfigPage));
            
            //最小窗口限制
            var manager = WinUIEx.WindowManager.Get(this);
            manager.PersistenceId = "MainWindowPersistanceId";
            manager.MinWidth = 657;
            manager.MinHeight = 480;
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            // 设置云母或亚克力背景
            backdrop = new Helpers.SystemBackdrop(this);
            backdrop.TrySetMica(fallbackToAcrylic: true);

            // 窗口句柄
            hwnd = WindowNative.GetWindowHandle(this);
            WindowId id = Win32Interop.GetWindowIdFromWindow(hwnd);
            appWindow = AppWindow.GetFromWindowId(id);

            // 初始化窗口大小和位置
            this.Closed += MainWindow_Closed;
            if (ApplicationData.Current.LocalSettings.Values["IsMainWindowMaximum"] is true)
            {
                // 最大化
                Vanara.PInvoke.User32.ShowWindow(hwnd, Vanara.PInvoke.ShowWindowCommand.SW_SHOWMAXIMIZED);
            }
            else if (ApplicationData.Current.LocalSettings.Values["MainWindowRect"] is ulong value)
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
            //this.SetWindowSize(width: 1366, 750); // Force Set window size

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
                var scale = (float)Vanara.PInvoke.User32.GetDpiForWindow(hwnd) / 96;
                // 48 这个值是应用标题栏的高度，不是唯一的，根据自己的 UI 设计而定
                titleBar.SetDragRectangles(new RectInt32[] { new RectInt32((int)(48 * scale), 0, 10000, (int)(48 * scale)) });
            }
            else
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(AppTitleBar);
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

        /// <summary>
        /// 找到 NavigationView 中的选项
        /// </summary>
        /// <param name="list">Navigation</param>
        /// <param name="tag">对应的 Tag</param>
        /// <returns></returns>
        private NavigationViewItem FindMenuItemByTag(NavigationView list, string tag)
        {
            foreach (NavigationViewItemBase item in list.MenuItems)
            {
                if (item is NavigationViewItem menuItem && menuItem.Tag.ToString() == tag)
                {
                    return menuItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Navigation View 选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            //先判断是否选中了setting
            if (args.IsSettingsInvoked)
            {
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                //选中项的内容
                switch (args.InvokedItem)
                {
                    case "代理状态":
                        contentFrame.Navigate(typeof(ClashConfigPage));
                        break;
                    case "用户信息":
                        contentFrame.Navigate(typeof(AccountInfoPage));
                        break;
                    case "充值余额":
                        contentFrame.Navigate(typeof(RechargePage));
                        break;
                    case "套餐购买":
                        contentFrame.Navigate(typeof(ShopPage));
                        break;
                    default:
                        break;
                }
            }
        }
    }

}
