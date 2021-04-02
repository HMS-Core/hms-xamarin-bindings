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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using HuaweiTask = Huawei.Hmf.Tasks.Task;

namespace Huawei.Hms.Mlsdk.Model.Download
{
    public partial class MLLocalModelManager
    {
        //async Task MyMethod() => await MyNativeMethod().CastTask();
        public async Task DeleteModelAsync(Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel model) =>
            await DeleteModel(model).CastTask();

        //async Task MyMethod() => await MyNativeMethod().CastTask();
        public async Task DownloadModelAsync(Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel model) =>
            await DownloadModel(model).CastTask();

        //async Task MyMethod() => await MyNativeMethod().CastTask();
        public async Task DownloadModelAsync(Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel model, Huawei.Hms.Mlsdk.Model.Download.MLModelDownloadStrategy strategy) =>
            await DownloadModel(model, strategy).CastTask();

        //async Task MyMethod() => await MyNativeMethod().CastTask();
        public async Task DownloadModelAsync(Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel model, Huawei.Hms.Mlsdk.Model.Download.MLModelDownloadStrategy strategy, Huawei.Hms.Mlsdk.Model.Download.IMLModelDownloadListener listener) =>
            await DownloadModel(model, strategy, listener).CastTask();

        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<System.Collections.ICollection> GetModelsAsync(Java.Lang.Class javaClass) =>
            await GetModels(javaClass).CastTask<JavaSet>();

        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<Java.IO.File> GetRecentModelFileAsync(Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel remoteModel) =>
            await GetRecentModelFile(remoteModel).CastTask<Java.IO.File>();

        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<bool> IsModelExistAsync(Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel remoteModel) =>
            (bool) await IsModelExist(remoteModel).CastTask<Java.Lang.Boolean>();
    }
}

namespace Huawei.Hms.Mlsdk.Model.Download
{
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
        public static Task CastTask(this HuaweiTask HuaweiTask)
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