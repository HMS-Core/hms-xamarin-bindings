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
using Huawei.Hms.Mlsdk.Model.Download.Impl;


#region ModelExecutor Workarounds
namespace Huawei.Hms.Mlsdk.Custom.Download.Strategy
{

    public partial class ConfigManager
    {

        // Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.translate.local.download.strategy']/class[@name='ConfigManager']/method[@name='isModelFileNeedUpdate' and count(parameter)=2 and parameter[1][@type='com.huawei.hms.mlsdk.model.download.MLRemoteModel'] and parameter[2][@type='java.util.Map&lt;java.lang.String, java.lang.String&gt;']]"
        [Register("isModelFileNeedUpdate", "(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Z", "GetIsModelFileNeedUpdate_Lcom_huawei_hms_mlsdk_model_download_MLRemoteModel_Ljava_util_Map_Handler")]
        public override unsafe bool IsModelFileNeedUpdate(global::Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel p0, global::System.Collections.Generic.IDictionary<string, string> p1)
        {
            const string __id = "isModelFileNeedUpdate.(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Z";
            IntPtr native_p1 = global::Android.Runtime.JavaDictionary<string, string>.ToLocalJniHandle(p1);
            try
            {
                Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[2];
                __args[0] = new Java.Interop.JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
                __args[1] = new Java.Interop.JniArgumentValue(native_p1);
                var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, __args);
                return __rm;
            }
            finally
            {
                JNIEnv.DeleteLocalRef(native_p1);
            }
        }

        // Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.translate.local.download.strategy']/class[@name='ConfigManager']/method[@name='isModelVersionNeedUpdated' and count(parameter)=3 and parameter[1][@type='com.huawei.hms.mlsdk.model.download.MLRemoteModel'] and parameter[2][@type='com.huawei.hms.mlsdk.model.download.impl.ModelResponse'] and parameter[3][@type='java.util.Map&lt;java.lang.String, java.lang.String&gt;']]"
        [Register("isModelVersionNeedUpdated", "(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Lcom/huawei/hms/mlsdk/model/download/impl/ModelResponse;Ljava/util/Map;)Z", "GetIsModelVersionNeedUpdated_Lcom_huawei_hms_mlsdk_model_download_MLRemoteModel_Lcom_huawei_hms_mlsdk_model_download_impl_ModelResponse_Ljava_util_Map_Handler")]
        public override unsafe bool IsModelVersionNeedUpdated(global::Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel p0, global::Huawei.Hms.Mlsdk.Model.Download.Impl.ModelResponse p1, global::System.Collections.Generic.IDictionary<string, string> p2)
        {
            const string __id = "isModelVersionNeedUpdated.(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Lcom/huawei/hms/mlsdk/model/download/impl/ModelResponse;Ljava/util/Map;)Z";
            IntPtr native_p2 = global::Android.Runtime.JavaDictionary<string, string>.ToLocalJniHandle(p2);
            try
            {
                Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[3];
                __args[0] = new Java.Interop.JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
                __args[1] = new Java.Interop.JniArgumentValue((p1 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p1).Handle);
                __args[2] = new Java.Interop.JniArgumentValue(native_p2);
                var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, __args);
                return __rm;
            }
            finally
            {
                JNIEnv.DeleteLocalRef(native_p2);
            }
        }
    }

    public partial class FileManager
    {
        // Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.custom.download.strategy']/class[@name='FileManager']/method[@name='getModelFilePath' and count(parameter)=2 and parameter[1][@type='com.huawei.hms.mlsdk.model.download.MLRemoteModel'] and parameter[2][@type='java.util.Map&lt;java.lang.String, java.lang.String&gt;']]"
        [Register("getModelFilePath", "(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Ljava/lang/String;", "GetGetModelFilePath_Lcom_huawei_hms_mlsdk_model_download_MLRemoteModel_Ljava_util_Map_Handler")]
        public override unsafe string GetModelFilePath(global::Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel p0, global::System.Collections.Generic.IDictionary<string, string> p1)
        {
            const string __id = "getModelFilePath.(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Ljava/lang/String;";
            IntPtr native_p1 = global::Android.Runtime.JavaDictionary<string, string>.ToLocalJniHandle(p1);
            try
            {
                Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[2];
                __args[0] = new Java.Interop.JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
                __args[1] = new Java.Interop.JniArgumentValue(native_p1);
                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
                return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef);
            }
            finally
            {
                JNIEnv.DeleteLocalRef(native_p1);
            }
        }
    }
}

namespace Huawei.Hms.Mlsdk.Custom
{
    public partial class ModelManager
    {
        System.Collections.ICollection IRemoteModelManagerInterface.GetDownloadedModels(IDictionary<string, string> p0)
        {
            throw new NotImplementedException();
        }
    }
}

#endregion


namespace Huawei.Hms.Mlsdk.Custom
{
    public partial class MLModelExecutor
    {
        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<MLModelOutputs> ExecAsync(Huawei.Hms.Mlsdk.Custom.MLModelInputs inputs, Huawei.Hms.Mlsdk.Custom.MLModelInputOutputSettings options) =>
            await Exec(inputs, options).CastTask<MLModelOutputs>();

        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<int> GetOutputIndexAsync(string name) =>
            (int) await GetOutputIndex(name).CastTask<Java.Lang.Integer>();
    }
}

namespace Huawei.Hms.Mlsdk.Custom
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