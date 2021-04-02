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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Huawei.Hms.Mlplugin.Productvisionsearch;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Productvisionsearch;
using Huawei.Hms.Mlsdk.Productvisionsearch.Cloud;
using Java.Lang;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ProductVisionSearch
{
    [Activity(Label = "ProductVisionSearchAnalyseActivity")]
    public class ProductVisionSearchAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "ProductVisionSearchTestActivity";

        private static readonly int PermissionRequest = 0x1000;
        private int CameraPermissionCode = 1;
        private static readonly int MaxResults = 1;

        private TextView mTextView;
        private ImageView productResult;
        private Bitmap bitmap;
        private MLRemoteProductVisionSearchAnalyzer analyzer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_product_vision_search_analyse);
            this.mTextView = (TextView)this.FindViewById(Resource.Id.result);
            this.productResult = (ImageView)this.FindViewById(Resource.Id.image_product);
            this.bitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.custom_model_image);
            this.productResult.SetImageResource(Resource.Drawable.custom_model_image);
            this.FindViewById(Resource.Id.product_detect_plugin).SetOnClickListener(this);
            this.FindViewById(Resource.Id.product_detect).SetOnClickListener(this);
            // Checking Camera Permissions
            if (!(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted))
            {
                this.RequestCameraPermission();
            }
        }

        private void RequestCameraPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, this.CameraPermissionCode);
                return;
            }
        }

        private void CheckPermissions(string[] permissions)
        {
            bool shouldRequestPermission = false;
            foreach (string permission in permissions)
            {
                if (ContextCompat.CheckSelfPermission(this, permission) != Permission.Granted)
                {
                    shouldRequestPermission = true;
                }
            }

            if (shouldRequestPermission)
            {
                ActivityCompat.RequestPermissions(this, permissions, PermissionRequest);
                return;
            }
            StartVisionSearchPluginCapture();
        }

        private async void RemoteAnalyze()
        {
            // Use customized parameter settings for cloud-based recognition.
            MLRemoteProductVisionSearchAnalyzerSetting setting =
                    new MLRemoteProductVisionSearchAnalyzerSetting.Factory()
                            // Set the maximum number of products that can be returned.
                            .SetLargestNumOfReturns(MaxResults)
                            .SetProductSetId("vmall")
                            .SetRegion(MLRemoteProductVisionSearchAnalyzerSetting.RegionDrChina)
                            .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetRemoteProductVisionSearchAnalyzer(setting);
            // Create an MLFrame by using the bitmap.
            MLFrame frame = MLFrame.FromBitmap(bitmap);
            Task<IList<MLProductVisionSearch>> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    // Analyze success.
                    var productVisionSearchList = task.Result;
                    if(productVisionSearchList.Count != 0)
                    {
                        Toast.MakeText(this, "Product detected successfully", ToastLength.Long).Show();
                        this.DisplaySuccess(productVisionSearchList);
                    }
                    else
                    {
                        Toast.MakeText(this, "Product not found", ToastLength.Long);
                    }
                    
                }
                else
                {
                    // Analyze failure.
                    Log.Debug(Tag, " remote analyze failed");
                }
            }
            catch (System.Exception e)
            {
                // Operation failure.
                this.DisplayFailure(e);
            }
        }

        private void StartVisionSearchPluginCapture()
        {
               // Set the config params.
               MLProductVisionSearchCaptureConfig config = new MLProductVisionSearchCaptureConfig.Factory()
                    //Set the largest OM detect Result,default is 20，values in 1-100
                    .SetLargestNumOfReturns(16)
                    //Set the fragment you created (the fragment should implement AbstractUIExtendProxy)
                    .SetProductFragment(new ProductFragment())
                    //Set region，current values：RegionDrChina，RegionDrSiangapore，RegionDrGerman，RegionDrRussia
                    .SetRegion(MLProductVisionSearchCaptureConfig.RegionDrChina)
                    //设set product id，you can get the value by AGC
                    //.SetProductSetId("xxxxx")
                    .Create();
            MLProductVisionSearchCapture capture = MLProductVisionSearchCaptureFactory.Instance.Create(config);
            //Start plugin
            capture.StartCapture(this);
        }

        private void DisplayFailure(System.Exception exception)
        {
            string error = "Failure. ";
            try
            {
                MLException mlException = (MLException)exception;
                error += "error code: " + mlException.ErrCode + "\n" + "error message: " + mlException.Message;
            }
            catch (System.Exception e)
            {
                error += e.Message;
            }
            this.mTextView.Text = error;
        }

        private void DrawBitmap(ImageView imageView, Rect rect, string product)
        {
            Paint boxPaint = new Paint();
            boxPaint.Color = Color.White;
            boxPaint.SetStyle(Paint.Style.Stroke);
            boxPaint.StrokeWidth = (4.0f);
            Paint textPaint = new Paint();
            textPaint = new Paint();
            textPaint.Color = Color.White;
            textPaint.TextSize = 100.0f;

            imageView.DrawingCacheEnabled = true;
            Bitmap bitmapDraw = Bitmap.CreateBitmap(this.bitmap.Copy(Bitmap.Config.Argb8888, true));
            Canvas canvas = new Canvas(bitmapDraw);
            canvas.DrawRect(rect, boxPaint);
            canvas.DrawText("product type: " + product, rect.Left, rect.Top, textPaint);
            this.productResult.SetImageBitmap(bitmapDraw);
        }

        private void DisplaySuccess(IList<MLProductVisionSearch> productVisionSearchList)
        {
            List<MLVisionSearchProductImage> productImageList = new List<MLVisionSearchProductImage>();
            foreach (MLProductVisionSearch productVisionSearch in productVisionSearchList)
            {
                this.DrawBitmap(this.productResult, productVisionSearch.Border, productVisionSearch.Type);
                foreach (MLVisionSearchProduct product in productVisionSearch.ProductList)
                {
                    productImageList.AddRange(product.ImageList);
                }
            }
            StringBuffer buffer = new StringBuffer();
            foreach (MLVisionSearchProductImage productImage in productImageList)
            {
                string str = "ProductID: " + productImage.ProductId + "\nImageID: " + productImage.ImageId + "\nPossibility: " + productImage.Possibility;
                buffer.Append(str);
                buffer.Append("\n");
            }

            this.mTextView.Text = buffer.ToString();
            
            this.bitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.custom_model_image);
            this.productResult.SetImageResource(Resource.Drawable.custom_model_image);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.product_detect:
                    this.RemoteAnalyze();
                    break;
                case Resource.Id.product_detect_plugin:
                    CheckPermissions(new string[]{Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage, Manifest.Permission.AccessNetworkState});
                    break;
                default:
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.analyzer == null)
            {
                return;
            }
            this.analyzer.Stop();
        }
    }
    
}