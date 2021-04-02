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
using Java.Interop;
using Huawei.Hms.Mlsdk.Model.Download.Impl;

namespace Huawei.Hms.Mlsdk.Tts.Download.Strategy
{
    public partial class ConfigManager
    {
		// Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.tts.download.strategy']/class[@name='ConfigManager']/method[@name='isModelFileNeedUpdate' and count(parameter)=2 and parameter[1][@type='com.huawei.hms.mlsdk.model.download.MLRemoteModel'] and parameter[2][@type='java.util.Map&lt;java.lang.String, java.lang.String&gt;']]"
		[Register("isModelFileNeedUpdate", "(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Z", "GetIsModelFileNeedUpdate_Lcom_huawei_hms_mlsdk_model_download_MLRemoteModel_Ljava_util_Map_Handler")]
		public override unsafe bool IsModelFileNeedUpdate(global::Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel p0, global::System.Collections.Generic.IDictionary<string, string> p1)
		{
			const string __id = "isModelFileNeedUpdate.(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Z";
			IntPtr native_p1 = global::Android.Runtime.JavaDictionary<string, string>.ToLocalJniHandle(p1);
			try
			{
				JniArgumentValue* __args = stackalloc JniArgumentValue[2];
				__args[0] = new JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
				__args[1] = new JniArgumentValue(native_p1);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, __args);
				return __rm;
			}
			finally
			{
				JNIEnv.DeleteLocalRef(native_p1);
			}
		}


		// Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.tts.download.strategy']/class[@name='ConfigManager']/method[@name='isModelVersionNeedUpdated' and count(parameter)=3 and parameter[1][@type='com.huawei.hms.mlsdk.model.download.MLRemoteModel'] and parameter[2][@type='com.huawei.hms.mlsdk.model.download.impl.ModelResponse'] and parameter[3][@type='java.util.Map&lt;java.lang.String, java.lang.String&gt;']]"
		[Register("isModelVersionNeedUpdated", "(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Lcom/huawei/hms/mlsdk/model/download/impl/ModelResponse;Ljava/util/Map;)Z", "GetIsModelVersionNeedUpdated_Lcom_huawei_hms_mlsdk_model_download_MLRemoteModel_Lcom_huawei_hms_mlsdk_model_download_impl_ModelResponse_Ljava_util_Map_Handler")]
		public override unsafe bool IsModelVersionNeedUpdated(global::Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel p0, global::Huawei.Hms.Mlsdk.Model.Download.Impl.ModelResponse p1, global::System.Collections.Generic.IDictionary<string, string> p2)
		{
			const string __id = "isModelVersionNeedUpdated.(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Lcom/huawei/hms/mlsdk/model/download/impl/ModelResponse;Ljava/util/Map;)Z";
			IntPtr native_p2 = global::Android.Runtime.JavaDictionary<string, string>.ToLocalJniHandle(p2);
			try
			{
				JniArgumentValue* __args = stackalloc JniArgumentValue[3];
				__args[0] = new JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
				__args[1] = new JniArgumentValue((p1 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p1).Handle);
				__args[2] = new JniArgumentValue(native_p2);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, __args);
				return __rm;
			}
			finally
			{
				JNIEnv.DeleteLocalRef(native_p2);
			}
		}
	}
}

namespace Huawei.Hms.Mlsdk.Tts
{
	public partial class ModelManager
	{
		System.Collections.ICollection IRemoteModelManagerInterface.GetDownloadedModels(IDictionary<string, string> p0)
		{
			throw new NotImplementedException();
		}
	}
}

namespace Huawei.Hms.Mlsdk.Tts.Download.Strategy
{
	public partial class FileManager
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.tts.download.strategy']/class[@name='FileManager']/method[@name='getModelFilePath' and count(parameter)=2 and parameter[1][@type='com.huawei.hms.mlsdk.model.download.MLRemoteModel'] and parameter[2][@type='java.util.Map&lt;java.lang.String, java.lang.String&gt;']]"
		[Register("getModelFilePath", "(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Ljava/lang/String;", "GetGetModelFilePath_Lcom_huawei_hms_mlsdk_model_download_MLRemoteModel_Ljava_util_Map_Handler")]
		public override unsafe string GetModelFilePath(global::Huawei.Hms.Mlsdk.Model.Download.MLRemoteModel p0, global::System.Collections.Generic.IDictionary<string, string> p1)
		{
			const string __id = "getModelFilePath.(Lcom/huawei/hms/mlsdk/model/download/MLRemoteModel;Ljava/util/Map;)Ljava/lang/String;";
			IntPtr native_p1 = global::Android.Runtime.JavaDictionary<string, string>.ToLocalJniHandle(p1);
			try
			{
				JniArgumentValue* __args = stackalloc JniArgumentValue[2];
				__args[0] = new JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
				__args[1] = new JniArgumentValue(native_p1);
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
