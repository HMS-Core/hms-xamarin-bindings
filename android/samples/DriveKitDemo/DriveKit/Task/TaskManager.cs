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
using Java.Util.Concurrent;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Task
{
    public class TaskManager
    {
        private IExecutorService cached = Executors.NewCachedThreadPool();

        private static readonly TaskManager task = new TaskManager();

        /// <summary>
        /// Get TaskManager instance
        /// </summary>
        public static TaskManager Instance
        {
            get { return task; }
        }


        /// <summary>
        /// Execute runnable task
        /// </summary>
        /// <param name="driveTask">DriveTask</param>
        public void Execute(DriveTask driveTask)
        {
            IFuture future = cached.Submit(driveTask);
            driveTask.Future = future;
        }
    }
}