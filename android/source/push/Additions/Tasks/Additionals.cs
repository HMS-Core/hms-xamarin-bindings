/*
       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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

using Android.Runtime;
using System;
using HuaweiTask = Huawei.Hmf.Tasks.Task;
using System.Threading.Tasks;

namespace Huawei.Hms.Push
{
    public partial class HmsProfile
    {
        public async System.Threading.Tasks.Task AddProfileAsync(int type, string profileId) => await AddProfile(type, profileId).CastTask();
        public async System.Threading.Tasks.Task AddProfileAsync(string subjectId, int type, string profileId) => await AddProfile(subjectId, type, profileId).CastTask();
        public async System.Threading.Tasks.Task DeleteProfileAsync(string profileId) => await DeleteProfile(profileId).CastTask();
        public async System.Threading.Tasks.Task DeleteProfileAsync(string subjectId, string profileId) => await DeleteProfile(subjectId, profileId).CastTask();
    }
    public partial class HmsMessaging
    {
        public async System.Threading.Tasks.Task SubscribeAsync(string topic) => await Subscribe(topic).CastTask();
        public async System.Threading.Tasks.Task UnsubscribeAsync(string topic) => await Unsubscribe(topic).CastTask();
        public async System.Threading.Tasks.Task TurnOnPushAsync() => await TurnOnPush().CastTask();
        public async System.Threading.Tasks.Task TurnOffPushAsync() => await TurnOffPush().CastTask();
    }

    #region Task Extensions
    /// <summary>
    /// Task Extension Class for convert HuaweiTask to System.Threading.Task
    /// </summary>
    internal static class HuaweiTaskExtensions
    {
        /// <summary>
        /// Convert HuaweiTask to System.Threading.Task without return type
        /// </summary>
        /// <param name="HuaweiTask">Huawei.Hmf.Tasks.Task</param>
        /// <returns>System.Threading.Task</returns>
        public static System.Threading.Tasks.Task CastTask(this HuaweiTask HuaweiTask)
        {
            var tcs = new TaskCompletionSource<bool>();

            HuaweiTask.AddOnCompleteListener(new HuaweiTaskCompleteListener(
                t =>
                {
                    if (t.Exception == null)
                        tcs.TrySetResult(false);
                    else
                        tcs.TrySetException(t.Exception);
                }
            ));

            return tcs.Task;
        }


        /// <summary>
        /// Convert HuaweiTask to System.Threading.Task with Generic return type
        /// </summary>
        /// <typeparam name="TResult">Return type,a Java object</typeparam>
        /// <param name="HuaweiTask">Huawei.Hmf.Tasks.Task class</param>
        /// <returns>System.Threading.Task with wrapped a generic type</returns>
        public static Task<TResult> CastTask<TResult>(this HuaweiTask HuaweiTask) where TResult : Java.Lang.Object
        {
            var tcs = new TaskCompletionSource<TResult>();

            HuaweiTask.AddOnCompleteListener(new HuaweiTaskCompleteListener(
                t =>
                {
                    if (t.Exception == null)
                        tcs.TrySetResult(t.Result.JavaCast<TResult>());
                    else
                        tcs.TrySetException(t.Exception);
                }));

            return tcs.Task;
        }


        /// <summary>
        /// Modified OnCompleteListener (from Huawei.Hmf.Tasks.Task)
        /// Invoke the given action
        /// </summary>
        class HuaweiTaskCompleteListener : Java.Lang.Object, Huawei.Hmf.Tasks.IOnCompleteListener
        {
            public HuaweiTaskCompleteListener(Action<HuaweiTask> onComplete)
                => OnCompleteHandler = onComplete;

            public Action<HuaweiTask> OnCompleteHandler { get; }

            public void OnComplete(HuaweiTask task)
                => OnCompleteHandler?.Invoke(task);
        }
    }
    #endregion
}


