using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 重新处理
    /// </summary>
    public class AfreshTaskEventArgs : EventArgs
    {
        private TaskEntity _task;
        public TaskEntity Task
        {
            get { return _task; }
            set { _task = value; }
        }

        public AfreshTaskEventArgs()
        {
            //
        }

        public AfreshTaskEventArgs(TaskEntity task)
        {
            this._task = task;
        }
    }
}
