using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Data;
using System.Windows.Controls;

namespace DoubleX.Upload
{
    /// <summary>
    /// 控件工具类
    /// </summary>
    public class ControlUtil
    {
        #region 消息控件

        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="msg"></param>
        public static MessageBoxResult ShowMsg(string msg, string title = "提示", MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            if (!string.IsNullOrWhiteSpace(msg) && !(msg.EndsWith("?") || msg.EndsWith("？"))) {
                msg = msg.TrimEnd('!', '！', '.', '。', ',', '，') + "！";
            }
            return System.Windows.MessageBox.Show(msg, title, btn, icon);
        }

        #endregion

        #region 异步处理

        /// <summary>
        /// 通知主线程去完成更新(执行方法)
        /// </summary>
        /// <param name="win"></param>
        /// <param name="action"></param>
        public static void ExcuteAction(Window win, Action action)
        {
            //正确的写法：通知主线程去完成更新
            new Thread(() =>
            {
                win.Dispatcher.Invoke(new Action(() =>
                {
                    action();
                }));
            }).Start();
        }

        #endregion

        #region DataGrid 处理

        public static void DataGridSyncBinding(DataGrid grid, object srouce)
        {
            Binding binding = new Binding();
            binding.IsAsync = false;//指定异步方式  
            binding.Source = srouce;
            grid.SetBinding(DataGrid.ItemsSourceProperty, binding);
            grid.Items.Refresh();
        }

        #endregion

        #region ListView 处理


        #endregion
    }
}
