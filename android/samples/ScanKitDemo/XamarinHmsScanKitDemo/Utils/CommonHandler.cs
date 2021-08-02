/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Hmsscankit;
using Huawei.Hms.Ml.Scan;
using Huawei.Hms.Mlsdk.Common;
using Java.IO;
using XamarinHmsScanKitDemo.Activities;
using XamarinHmsScanKitDemo.Draw;
using static Android.OS.Handler;
using Console = System.Console;

namespace XamarinHmsScanKitDemo.Utils
{
    public class CommonHandler : Handler
    {
        public double DefaultZoom = 1.0;
        public CameraOperation cameraOperation;
        public HandlerThread decodeThread;
        public Handler decodeHandle;
        public Activity activity;
        public int mode;
        public HmsScan[] tempData;

        public CommonHandler(Activity activity, CameraOperation cameraOperation, int mode)
        {
            this.cameraOperation = cameraOperation;
            this.activity = activity;
            this.mode = mode;
            decodeThread = new HandlerThread("DecodeThread");
            decodeThread.Start();
            decodeHandle = new Handler(decodeThread.Looper, new HandleMessageCallback(this));
            cameraOperation.StartPreview();
            Restart(DefaultZoom);
        }
        public void Restart(double defaultZoon)
        {
            cameraOperation.CallbackFrame(decodeHandle, defaultZoon);
        }

        internal void Quit()
        {
            try
            {
                cameraOperation.StopPreview();
                decodeHandle.Looper.Quit();
                decodeThread.Join(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public override void HandleMessage(Message message)
        {
            RemoveMessages(1);
            if (message.What == 0)
            {
                BitmapActivity bitmapActivity = (BitmapActivity)activity;
                bitmapActivity.scanResultView.Clear();
                Intent intent = new Intent();
                if (tempData != null)
                {
                    intent.PutExtra(Constants.ScanResult, tempData);
                    activity.SetResult(Result.Ok, intent);
                }
                //Show the scanning result on the screen.
                if (mode == Constants.MultiProcessorAsyncCode || mode == Constants.MultiProcessorSyncCode)
                {
                    HmsScan[] arr = tempData;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0)
                        {
                            bitmapActivity.scanResultView.Add(new ScanResultView.HmsScanGraphic(bitmapActivity.scanResultView, arr[i], Color.Yellow));
                        }
                        else if (i == 1)
                        {
                            bitmapActivity.scanResultView.Add(new ScanResultView.HmsScanGraphic(bitmapActivity.scanResultView, arr[i], Color.Blue));
                        }
                        else if (i == 2)
                        {
                            bitmapActivity.scanResultView.Add(new ScanResultView.HmsScanGraphic(bitmapActivity.scanResultView, arr[i], Color.Red));
                        }
                        else if (i == 3)
                        {
                            bitmapActivity.scanResultView.Add(new ScanResultView.HmsScanGraphic(bitmapActivity.scanResultView, arr[i], Color.Green));
                        }
                        else
                        {
                            bitmapActivity.scanResultView.Add(new ScanResultView.HmsScanGraphic(bitmapActivity.scanResultView, arr[i], null));
                        }
                    }
                    bitmapActivity.scanResultView.SetCameraInfo(1080, 1920);
                    bitmapActivity.scanResultView.Invalidate();
                    SendEmptyMessageDelayed(1, 1000);
                }
                else
                {
                    activity.Finish();
                }
            }
            else if (message.What == 1)
            {
                BitmapActivity bitmapActivity = (BitmapActivity)activity;
                bitmapActivity.scanResultView.Clear();
            }
            tempData = null;
        }
    }
    public class HandleMessageCallback : Java.Lang.Object, ICallback
    {
        public CommonHandler commonHandler;


        public HandleMessageCallback(CommonHandler commonHandler)
        {
            this.commonHandler = commonHandler;
        }

        // Convert an object to a byte array
        private byte[] ObjectToByteArray(Java.Lang.Object obj)
        {
            MemoryStream ms = new MemoryStream();
            ObjectOutputStream outputStream = null;
            try
            {
                outputStream = new ObjectOutputStream(ms);
                outputStream.WriteObject(obj);
                outputStream.Flush();
                byte[] yourBytes = ms.ToArray();
                return yourBytes;
            }
            finally
            {
                try
                {
                    ms.Close();
                }
                catch (Exception ex)
                {
                    // ignore close exception
                }
            }
        }
        public bool HandleMessage(Message msg)
        {

            if (msg == null)
            {
                return false;
            }

            if (commonHandler.mode == Constants.BitmapCode || commonHandler.mode == Constants.MultiProcessorSyncCode)
            {
                HmsScan[] result = DecodeSyn(msg.Arg1, msg.Arg2, ObjectToByteArray(msg.Obj), commonHandler.activity, HmsScan.AllScanType, commonHandler.mode);
                if (result == null || result.Length == 0)
                {
                    commonHandler.Restart(commonHandler.DefaultZoom);
                }
                else if (string.IsNullOrEmpty(result[0].OriginalValue) && result[0].ZoomValue != 1.0)
                {
                    commonHandler.Restart(result[0].ZoomValue);
                }
                else if (!string.IsNullOrEmpty(result[0].OriginalValue))
                {
                    Message message = new Message();
                    message.What = msg.What;
                    message.Obj = result;
                    commonHandler.tempData = (HmsScan[])result;
                    commonHandler.SendMessage(message);
                    commonHandler.Restart(commonHandler.DefaultZoom);
                }
                else
                {
                    commonHandler.Restart(commonHandler.DefaultZoom);
                }
            }
            else if (commonHandler.mode == Constants.MultiProcessorAsyncCode)
            {
                DecodeAsyn(msg.Arg1, msg.Arg2, ObjectToByteArray(msg.Obj), commonHandler.activity, HmsScan.AllScanType);
            }
            return false;
        }

        private void DecodeAsyn(int width, int height, byte[] data, Activity activity, int type)
        {
            Bitmap bitmap = ConvertToBitmap(width, height, data);
            MLFrame image = MLFrame.FromBitmap(bitmap);
            HmsScanAnalyzerOptions options = new HmsScanAnalyzerOptions.Creator().SetHmsScanTypes(type).Create();
            HmsScanAnalyzer analyzer = new HmsScanAnalyzer(options);
            analyzer.AnalyzInAsyn(image).AddOnSuccessListener(new AnalyzeSuccessListener(commonHandler)).AddOnFailureListener(new AnalyzeFailListener(commonHandler));
            //bitmap.Recycle();
        }

        private Bitmap ConvertToBitmap(int width, int height, byte[] data)
        {
            YuvImage yuv = new YuvImage(data, ImageFormatType.Nv21, width, height, null);
            MemoryStream stream = new MemoryStream();
            yuv.CompressToJpeg(new Rect(0, 0, width, height), 100, stream);
            return BitmapFactory.DecodeByteArray(stream.ToArray(), 0, stream.ToArray().Length);
        }

        private HmsScan[] DecodeSyn(int width, int height, byte[] data, Activity activity, int type, int mode)
        {
            Bitmap bitmap = ConvertToBitmap(width, height, data);
            if (mode == Constants.BitmapCode)
            {
                HmsScanAnalyzerOptions options = new HmsScanAnalyzerOptions.Creator().SetHmsScanTypes(type).SetPhotoMode(false).Create();
                return ScanUtil.DecodeWithBitmap(activity, bitmap, options);
            }
            else if (mode == Constants.MultiProcessorSyncCode)
            {
                MLFrame image = MLFrame.FromBitmap(bitmap);
                HmsScanAnalyzerOptions options = new HmsScanAnalyzerOptions.Creator().SetHmsScanTypes(type).Create();
                HmsScanAnalyzer analyzer = new HmsScanAnalyzer(options);
                SparseArray result = analyzer.AnalyseFrame(image);
                if (result != null && result.Size() > 0 && result.ValueAt(0) != null && !string.IsNullOrEmpty(((HmsScan)result.ValueAt(0)).OriginalValue))
                {
                    HmsScan[] info = new HmsScan[result.Size()];
                    for (int index = 0; index < result.Size(); index++)
                    {
                        info[index] = (HmsScan)result.ValueAt(index);
                    }
                    return info;
                }
            }
            return null;
        }



    }

    public class AnalyzeSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private CommonHandler commonHandler;

        public AnalyzeSuccessListener(CommonHandler commonHandler)
        {
            this.commonHandler = commonHandler;
        }

        public void OnSuccess(Java.Lang.Object scanObj)
        {
            var hmsScans = (scanObj as System.Collections.IList).Cast<object>().Select(x => x as HmsScan).ToArray();
            if (hmsScans != null && hmsScans.Length > 0 && !string.IsNullOrEmpty(hmsScans[0].OriginalValue))
            {
                Message message = new Message
                {
                    Obj = hmsScans
                };
                commonHandler.tempData = hmsScans;
                commonHandler.SendMessage(message);
                commonHandler.Restart(commonHandler.DefaultZoom);
            }
            else
            {
                commonHandler.Restart(commonHandler.DefaultZoom);
            }
        }
    }

    public class AnalyzeFailListener : Java.Lang.Object, IOnFailureListener
    {
        private CommonHandler commonHandler;

        public AnalyzeFailListener(CommonHandler commonHandler)
        {
            this.commonHandler = commonHandler;
        }

        public void OnFailure(Java.Lang.Exception p0)
        {
            commonHandler.Restart(commonHandler.DefaultZoom);
        }
    }
}