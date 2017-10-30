using Microsoft.Win32;
using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DoubleXUI.Controls
{
    public class DxWindow : Window
    {
        #region 窗体属性扩展定义

        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(DxWindow));
        public static readonly DependencyProperty ContentSourceProperty = DependencyProperty.Register("ContentSource", typeof(Uri), typeof(DxWindow));
        public static readonly DependencyProperty ContentLoaderProperty = DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(DxWindow), new PropertyMetadata(new DefaultContentLoader()));
        public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(DxWindow), new PropertyMetadata(false));
        public object BackgroundContent
        {
            get { return GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }
        public Uri ContentSource
        {
            get { return (Uri)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }
        public bool IsTitleVisible
        {
            get { return (bool)GetValue(IsTitleVisibleProperty); }
            set { SetValue(IsTitleVisibleProperty, value); }
        }

        #endregion

        private Storyboard backgroundAnimation;

        public DxWindow()
        {
            this.DefaultStyleKey = typeof(DxWindow);
            this.InitAttachEvent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var border = GetTemplateChild("WindowBorder") as Border;
            if (border != null)
            {
                this.backgroundAnimation = border.Resources["BackgroundAnimation"] as Storyboard;
                if (this.backgroundAnimation != null)
                {
                    this.backgroundAnimation.Begin();
                }
            }
        }

        private void InitAttachEvent()
        {
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

    }
}
