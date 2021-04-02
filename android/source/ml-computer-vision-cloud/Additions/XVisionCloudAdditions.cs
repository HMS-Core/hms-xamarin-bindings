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


#region XVisionCloud Workarounds
namespace Huawei.Hms.Mlsdk.Text
{
    
    public class MLTextAddition
    {
        global::System.Collections.Generic.IList<global::Huawei.Hms.Mlsdk.Text.MLText.Base> Contents
        {
            // Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.hms.mlsdk.text']/interface[@name='MLText.Text']/method[@name='getContents' and count(parameter)=0]"
            [Register("getContents", "()Ljava/util/List;", "GetGetContentsHandler:Huawei.Hms.Mlsdk.Text.MLText/ITextInvoker, XVisionCloud-2.0.5.300")]
            get;
        }
    }
}
#endregion

namespace Huawei.Hms.Mlsdk.Document
{
    public partial class MLDocumentAnalyzer
    {
        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<MLDocument> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<MLDocument>();
    }
}

namespace Huawei.Hms.Mlsdk.Landmark
{
    public partial class MLRemoteLandmarkAnalyzer
    {
        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLRemoteLandmark>> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<JavaList<MLRemoteLandmark>>();
    }
}

namespace Huawei.Hms.Mlsdk.Text
{
    public partial class MLTextAnalyzer
    {
        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<MLText> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<MLText>();
    }
}

namespace Huawei.Hms.Mlsdk.Classification
{
    public partial class MLImageClassificationAnalyzer
    {
        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLImageClassification>> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<JavaList<MLImageClassification>>();
    }
}

namespace Huawei.Hms.Mlsdk.Face
{
    public sealed partial class MLFaceAnalyzer
    {
        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLFace>> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<JavaList<MLFace>>();
    }
}

namespace Huawei.Hms.Mlsdk.Face.Face3d
{
    public sealed partial class ML3DFaceAnalyzer
    {
        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<ML3DFace>> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<JavaList<ML3DFace>>();
    }
}

namespace Huawei.Hms.Mlsdk.Imgseg
{
    public partial class MLImageSegmentationAnalyzer
    {
        //async Task<NAME> MyMethod(params) => await MyNativeMethod(params).CastTask<NAME>();
        public async Task<MLImageSegmentation> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<MLImageSegmentation>();
    }
}

namespace Huawei.Hms.Mlsdk.Objects
{
    public partial class MLObjectAnalyzer
    {
        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLObject>> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<JavaList<MLObject>>();
    }
}

namespace Huawei.Hms.Mlsdk.Productvisionsearch.Cloud
{
    public partial class MLRemoteProductVisionSearchAnalyzer
    {
        //async Task<IList<TYPE>> MyMethod(params) => await MyNativeMethod(params).CastTask<JavaList<TYPE>>();
        public async Task<IList<MLProductVisionSearch>> AnalyseFrameAsync(Huawei.Hms.Mlsdk.Common.MLFrame frame) =>
            await AsyncAnalyseFrame(frame).CastTask<JavaList<MLProductVisionSearch>>();
    }
}

namespace Huawei.Hms.Mlsdk
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



