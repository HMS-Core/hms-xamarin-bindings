/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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

namespace Huawei.Hms.Mlsdk.Langdetect.Cloud
{
    public partial class MLRemoteLangDetector
    {
        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<string> FirstBestDetectAsync(string text) =>
            (string) await FirstBestDetect(text).CastTask<Java.Lang.String>();

        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLDetectedLang>> ProbabilityDetectAsync(string text) =>
             await ProbabilityDetect(text).CastTask<JavaList<MLDetectedLang>>();
    }
}

namespace Huawei.Hms.Mlsdk.Langdetect.Local
{
    public partial class MLLocalLangDetector
    {
        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<string> FirstBestDetectAsync(string text) =>
            (string)await FirstBestDetect(text).CastTask<Java.Lang.String>();

        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLDetectedLang>> ProbabilityDetectAsync(string text) =>
             await ProbabilityDetect(text).CastTask<JavaList<MLDetectedLang>>();
    }
}

namespace Huawei.Hms.Mlsdk.Langdetect
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