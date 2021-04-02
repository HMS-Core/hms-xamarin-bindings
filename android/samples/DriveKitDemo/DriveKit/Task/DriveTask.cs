/*
        Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

        Licensed under the Apache License, Version 2.0 (the "License");
        you may not use this file except in compliance with the License.
        You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

        Unless required by applicable law or agreed to in writing, software
        distributed under the License is distributed on an "AS IS" BASIS,
        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
        See the License for the specific language governing permissions and
        limitations under the License.
*/
using Huawei.Cloud.Base.Util;
using Java.Lang;
using Java.Util.Concurrent;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Task
{
    public abstract class DriveTask : Java.Lang.Object, IRunnable
    {

        public const string Tag = "DriveTask";
        static readonly Logger Logger = Logger.GetLogger(Tag);

        private IFuture future;
        public IFuture Future
        {
           get { return future; }
           set { future = value; }
        }

        /// <summary>
        /// Implemented from IRunnable.
        /// </summary>
        public void Run()
        {
            try
            {
                Call();
            }
            catch (Java.Lang.Exception e)
            {
                Logger.W("task error: " + e.ToString());
            }
        }

        /// <summary>
        /// DriveTask implementation.
        /// </summary>
        public abstract void Call();

        /// <summary>
        /// Used to terminate the current task.
        /// </summary>
        public bool Cancel()
        {
            if (null != future)
            {
                return future.Cancel(true);
            }

            return false;
        }
    }

}