/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Custom;
using Huawei.Hms.Mlsdk.Model.Download;
using HmsXamarinMLDemo.Common;

namespace HmsXamarinMLDemo.MLKitActivities.CustomModel
{
    [Activity(Label = "CustomModelActivity")]
    public class CustomModelActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "CustomModelActivity";

        // this model is converted from TensorFlow model, for more details please refer below:
        // https://www.tensorflow.org/lite/models/image_classification/overview
        private static readonly string ModelName = "mobilenet_v1_1_0_224_quant";
        private static readonly string LabelFileName = "labels_mobilenet_quant_v1_224.txt";
        private static readonly string ModelFullName = ModelName + ".ms";
        private static readonly string RemoteModelName = "ModelRemotenew"; //Replace with your model name that uploaded by you to Huawei Developer Console.
        private static readonly int BitmapWidth = 224;
        private static readonly int BitmapHeight = 224;
        private static readonly int OutputSize = 1001;
        private static readonly long M = 1024 * 1024;

        private IDictionary<string, float> result;
        private IList<string> mLabels = new List<string>();
        private Bitmap analysisBitmap;
        private MLCustomLocalModel localModel;
        private MLCustomRemoteModel remoteModel;
        private ImageView captured;
        private TextView mTvLog;
        private Button Download;
        private const int DownloadCase = 1;
        public MLModelExecutor modelExecutor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_custom_model);

            ReadLabels(LabelFileName);
            captured = (ImageView) this.FindViewById(Resource.Id.capturedImageView);
            mTvLog = (TextView) this.FindViewById(Resource.Id.tv_log);
            Download = (Button)this.FindViewById(Resource.Id.remoteModel);
            Download.SetOnClickListener(this);
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 16;
            analysisBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.custom_model_image, options);
            ProcessBitmap();
        }

        private void ProcessBitmap()
        {
            if (analysisBitmap != null)
            {
                DumpBitmapInfo(analysisBitmap);
                Bitmap local = Bitmap.CreateScaledBitmap(analysisBitmap, BitmapWidth, BitmapHeight, false);
                this.RunOnUiThread(delegate ()
                {
                    if (local != null)
                    {
                        captured.SetImageBitmap(local);
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "No picture", ToastLength.Short).Show();
                    }

                });
            }
        }

        private bool DumpBitmapInfo(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return true;
            }
            int width = bitmap.Width;
            int height = bitmap.Height;
            Log.Error(Tag, "bitmap width is " + width + " height " + height);
            return false;
        }

        /// <summary>
        /// OnClick listener implementation
        /// </summary>
        /// <param name="v"></param>
        public void OnClick(View v)
        {
            if (analysisBitmap != null)
            {
                PictureAnalysis(analysisBitmap);
            }
        }

        /// <summary>
        /// Download Model Method
        /// </summary>
        public async void DownloadModels(MLCustomRemoteModel customRemoteModel)
        {
            MLModelDownloadStrategy strategy = new MLModelDownloadStrategy.Factory()
                            .NeedWifi()
                            .SetRegion(MLModelDownloadStrategy.RegionDrEurope)
                            .Create();
            MLModelDownloadListener modelDownloadListener = new MLModelDownloadListener(this, DownloadCase);
            
            System.Threading.Tasks.Task downloadTask = MLLocalModelManager.Instance.DownloadModelAsync(customRemoteModel, strategy, modelDownloadListener);
            try
            {
                await downloadTask;
                if (downloadTask.IsCompleted)
                {
                    //Detection success
                    Toast.MakeText(this, "Model download successful", ToastLength.Short).Show();
                }
                else
                {
                    //Detection success
                    Toast.MakeText(this, "Model download failed", ToastLength.Short).Show();
                }
            }
            catch (System.Exception e)
            {
                //Operation failed
                Log.Debug(Tag, "Download operation failed: " +e.Message);
            }
        }

        /// <summary>
        /// Mixed Mode Analyze
        /// Recommended
        /// </summary>
        private void PictureAnalysis(Bitmap bitmap)
        {
            this.localModel = new MLCustomLocalModel.Factory(ModelName).SetAssetPathFile(ModelFullName).Create();
            this.remoteModel = new MLCustomRemoteModel.Factory(RemoteModelName).Create();
            DownloadModels(remoteModel);
            MLLocalModelManager.Instance
                               .IsModelExist(remoteModel)
                               .ContinueWithTask(new CustomModelContinuation(
                                                   delegate (Huawei.Hmf.Tasks.Task task)
                                                   {
                                                       if (!task.IsSuccessful)
                                                       {
                                                           throw task.Exception;
                                                       }
                                                       Java.Lang.Boolean isDownloaded = task.Result.JavaCast<Java.Lang.Boolean>();
                                                       MLModelExecutorSettings settings = null;
                                                       if ((bool)isDownloaded)
                                                       {
                                                           Toast.MakeText(this, "Executing Remote Model", ToastLength.Short).Show();
                                                           settings = new MLModelExecutorSettings.Factory(remoteModel).Create();
                                                       }
                                                       else
                                                       {
                                                           Toast.MakeText(this, "Model download failed. Executing Local Model", ToastLength.Long).Show();
                                                           settings = new MLModelExecutorSettings.Factory(localModel).Create();
                                                       }
                                                       try
                                                       {
                                                           this.modelExecutor = MLModelExecutor.GetInstance(settings);
                                                           ExecutorImpl(modelExecutor, bitmap);
                                                       }
                                                       catch (System.Exception e)
                                                       {
                                                           Log.Info(Tag, e.ToString());
                                                       }

                                                   }
                                   ));

        }

        /// <summary>
        /// Display result to TextView
        /// </summary>
        /// <param name="result"></param>
        public void DisplayResult(string result)
        {
            this.mTvLog.Text = result;
        }

        /// <summary>
        /// Obtain bitmap object from resources
        /// </summary>
        public Bitmap GetBitmap()
        {
            Bitmap bm = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.custom_model_image);

            return bm;
        }

        /// <summary>
        /// Read classification labels
        /// </summary>
        public void ReadLabels(string labelFileName)
        {
            // Read the contents of our asset
            string content;
            AssetManager assets = this.Assets;
            using (StreamReader sr = new StreamReader(assets.Open(labelFileName)))
            {
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    mLabels.Add(s);
                }
            }
        }

        /// <summary>
        /// Jagged array creator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lengths"></param>
        /// <returns></returns>
        static T CreateJaggedArray<T>(params int[] lengths)
        {
            return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
        }

        /// <summary>
        /// Recursive jagged array method
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="lengths"></param>
        /// <returns></returns>
        static object InitializeJaggedArray(Type type, int index, int[] lengths)
        {
            Array array = Array.CreateInstance(type, lengths[index]);
            Type elementType = type.GetElementType();

            if (elementType != null)
            {
                for (int i = 0; i < lengths[index]; i++)
                {
                    array.SetValue(
                        InitializeJaggedArray(elementType, index + 1, lengths), i);
                }
            }
            return array;
        }

        /// <summary>
        /// Convert bitmap to pixel array
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        static byte[][][][] ConvertBitmapToInputFormat(Bitmap bitmap)
        {
            Bitmap inputBitmap = Bitmap.CreateScaledBitmap(bitmap, BitmapWidth, BitmapHeight, true);
            int batchNum = 0;
            byte[][][][] input = CreateJaggedArray<byte[][][][]>(1, 3, 224, 224);

            // prepare the input data, if converted from tensorflow, pls use NCHW format, DO not use NHWC。
            for (int i = 0; i < BitmapWidth; i++)
            {
                for (int j = 0; j < BitmapHeight; j++)
                {
                    int pixel = inputBitmap.GetPixel(i, j);
                    input[batchNum][0][j][i] = (byte)Color.GetRedComponent(pixel);
                    input[batchNum][1][j][i] = (byte)Color.GetGreenComponent(pixel);
                    input[batchNum][2][j][i] = (byte)Color.GetBlueComponent(pixel);
                }
            }

            return input;
        }

        /// <summary>
        /// For input model
        /// </summary>
        static byte[] FourDimenArrayToOneDimen(byte[][][][] inputFour)
        {
            byte[] inputByteBuffer = new byte[BitmapHeight * BitmapWidth * 3];
            int index = 0;
            int batchNum = 0;

            for (int k = 0; k < 3; k++)
            {
                for (int j = 0; j < BitmapWidth; j++)
                {
                    for (int i = 0; i < BitmapHeight; i++)
                    {
                        if (inputByteBuffer.Length > index)
                        {

                            inputByteBuffer[index++] = inputFour[batchNum][k][j][i];

                        }

                    }

                }

            }

            return inputByteBuffer;
        }

        /// <summary>
        /// Execute Custom Model
        /// classification
        /// </summary>
        public void ExecutorImpl(MLModelExecutor modelExecutor, Bitmap bitmap)
        {
            byte[][][][] convertedBitmap = ConvertBitmapToInputFormat(bitmap);
            byte[] modelInputArray = FourDimenArrayToOneDimen(convertedBitmap);

            MLModelInputs inputs = null;
            try
            {
                inputs = new MLModelInputs.Factory().Add(modelInputArray).Create();
                // If the model requires multiple inputs, you need to call Add() for multiple times so that image data can be input to the inference engine at a time.
            }
            catch (MLException e)
            {
                // Handle the input data formatting exception.
                Log.Info(Tag, " input data format exception! " + e.Message);
            }

            MLModelInputOutputSettings inOutSettings = null;
            try
            {
                // according to the model requirement, set the in and out format.
                inOutSettings = new MLModelInputOutputSettings.Factory()
                        .SetInputFormat(0, MLModelDataType.Byte, new int[] { BitmapHeight * BitmapWidth * 3 })
                        .SetOutputFormat(0, MLModelDataType.Byte, new int[] { OutputSize })
                        .Create();
            }
            catch (MLException e)
            {
                Log.Info(Tag, "set input output format failed! " + e.Message);
            }
            modelExecutor.Exec(inputs, inOutSettings).AddOnSuccessListener(new OnSuccessListener(
                                delegate (Java.Lang.Object myobj)
                                {
                                    Log.Info(Tag, "interpret get result");
                                    MLModelOutputs mlModelOutputs = (MLModelOutputs)myobj;
                                    if (mlModelOutputs.Outputs.Count != 0)
                                    {
                                        byte[] output = (byte[])mlModelOutputs.GetOutput(0); // index

                                        var myFlo = GetFloatRest(output);
                                        PrepareResult(myFlo);

                                        // display the result
                                        string totalResult = "";
                                        foreach (var item in result)
                                        {
                                            if (item.Value != 0)
                                            {
                                                totalResult += item.Key.ToString() + ": " + item.Value.ToString();
                                                totalResult += "\n";
                                            }
                                        }

                                        DisplayResult(totalResult);
                                    }

                                }

                        )).AddOnFailureListener(new OnFailureListener(

                                delegate (Java.Lang.Exception e)
                                {
                                    Log.Info(Tag, " ModelExecutor.Exec() failed: "+ e.Message);
                                }
                       ));

        }

        /// <summary>
        /// Convert probabilities
        /// from byte to float
        /// </summary>
        /// <param name="byteResult"></param>
        /// <returns></returns>
        private float[] GetFloatRest(byte[] byteResult)
        {
            int length = byteResult.Length;
            if (length > 0)
            {
                float[] local = new float[length];
                for (int i = 0; i < length; i++)
                {
                    local[i] = byteResult[i] / 255f;
                }
                return local;
            }
            return new float[0];
        }

        /// <summary>
        /// Match classification labels
        /// with probabilities
        /// </summary>
        /// <param name="probabilities"></param>
        private void PrepareResult(float[] probabilities)
        {
            Dictionary<string, float> localResult = new Dictionary<string, float>();
            for (int i = 0; i < OutputSize; i++)
            {
                localResult.Add(mLabels.ElementAt(i), probabilities[i]);
            }
            result = localResult;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// Show model downloading process
        /// </summary>
        public void ShowProcess(long alreadyDownLength, string buttonText, long totalLength, int location)
        {
            double downDone = alreadyDownLength * 1.0 / M;
            double downTotal = totalLength * 1.0 / M;
            string downD = string.Format("%.2f", downDone);
            string downT = string.Format("%.2f", downTotal);

            string text = downD + "M" + "/" + downT + "M";
            Log.Info(Tag, "string format:" + downD);
            UpdateButton(text, location);
            if (downD.Equals(downT))
            {
                UpdateButton(buttonText, location);
            }
        }

        /// <summary>
        /// Update button text with downloading process
        /// </summary>
        private void UpdateButton(string text, int buttonSwitch)
        {

            this.RunOnUiThread(delegate () {
                switch (buttonSwitch)
                {
                    case DownloadCase:
                        //Show model downloading process with text parameter.
                        Download.Text = text;
                        break;
                }
            });

        }


    }
    /// <summary>
    /// Remote Model Download Listener
    /// </summary>
    public class MLModelDownloadListener : Java.Lang.Object, IMLModelDownloadListener
    {
        CustomModelActivity mActivity;
        int location;

        public MLModelDownloadListener(CustomModelActivity mActivity, int Location)
        {
            this.mActivity = mActivity;
            this.location = Location;
        }
        public void OnProcess(long alreadyDownLength, long totalLength)
        {
            mActivity.ShowProcess(alreadyDownLength, "DownLoad", totalLength, location);
        }
    }

    /// <summary>
    /// IContinuation implementation
    /// </summary>
    public class CustomModelContinuation : Java.Lang.Object, IContinuation
    {
        private Action<Huawei.Hmf.Tasks.Task> successAction;

        public CustomModelContinuation(Action<Huawei.Hmf.Tasks.Task> SuccessAction)
        {
            this.successAction = SuccessAction;
        }

        public Java.Lang.Object Then(Huawei.Hmf.Tasks.Task task)
        {

            this.successAction(task);
            return null;
        }
    }
}