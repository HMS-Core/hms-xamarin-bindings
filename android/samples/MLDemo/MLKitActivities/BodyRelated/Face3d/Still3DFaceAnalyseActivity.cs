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
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Face;
using Huawei.Hms.Mlsdk.Face.Face3d;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face3d
{
    [Activity(Label = "Still3DFaceAnalyseActivity")]
    public class Still3DFaceAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "Still3DFaceAnalyseActivity";

        private ML3DFaceAnalyzer analyzer;
        private TextView mTextView;       
        private Bitmap mBitmap;
        private ImageView mImageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_face_analyse);
            this.mTextView = (TextView) FindViewById(Resource.Id.result);
            this.FindViewById(Resource.Id.face_detect).SetOnClickListener(this);
            //Set default image
            this.mBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.face_image);
            this.mImageView = (ImageView)this.FindViewById(Resource.Id.face_image);
            mImageView.SetImageBitmap(mBitmap);
        }

        private async void Analyze()
        {
            // Create a face analyzer. You can create an analyzer using the provided customized face detection parameter
            ML3DFaceAnalyzerSetting setting = new ML3DFaceAnalyzerSetting.Factory()
                    // Fast detection of continuous video frames.
                    // MLFaceAnalyzerSetting.TypePrecision: indicating the precision preference mode.
                    // This mode will detect more faces and be more precise in detecting key points and contours, but will run slower.
                    // MLFaceAnalyzerSetting.TypeSpeed: representing a preference for speed.
                    // This will detect fewer faces and be less precise in detecting key points and contours, but will run faster.
                    .SetPerformanceType(MLFaceAnalyzerSetting.TypePrecision)
                    .SetTracingAllowed(false)
                    .Create();
            this.analyzer = MLAnalyzerFactory.Instance.Get3DFaceAnalyzer(setting);
            // Create an MLFrame by using the bitmap. Recommended image size: large than 320*320, less than 1920*1920.
            Bitmap bitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.face_image);
            MLFrame frame = MLFrame.FromBitmap(bitmap);
            // Call the AnalyseFrameAsync method to perform face detection
            Task<IList<ML3DFace>> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;

                if(task.IsCompleted && task.Result != null)
                {
                    //Analyze success
                    var faces = task.Result;
                    if (faces.Count > 0)
                    {
                        this.DisplaySuccess(faces.ElementAt(0));
                    }
                }
                else
                {
                    //Analyze failed
                    this.DisplayFailure();
                }

            }
            catch(Exception e)
            {
                //Operation failed
                Log.Error(Tag, e.Message);
            }
        }

        private void DisplayFailure()
        {
            this.mTextView.Text = "Failure";
        }

        private void DisplaySuccess(ML3DFace mLFace)
        {
            float[] projectionMatrix = new float[4 * 4];
            float[] viewMatrix = new float[4 * 4];
            mLFace.Get3DProjectionMatrix(projectionMatrix, 1, 10);
            mLFace.Get3DViewMatrix(viewMatrix);
            DecimalFormat decimalFormat = new DecimalFormat("0.00");
            string result = "3DFaceEulerX: " + decimalFormat.Format(mLFace.Get3DFaceEulerX());
            result += "\n3DFaceEulerY: " + decimalFormat.Format(mLFace.Get3DFaceEulerY());
            result += "\n3DFaceEulerZ: " + decimalFormat.Format(mLFace.Get3DFaceEulerZ());
            result += "\n3DProjectionMatrix:";
            for (int i = 0; i < 16; i++)
            {
                result += " " + decimalFormat.Format(projectionMatrix[i]);
            }
            result += "\nViewMatrix:";
            for (int i = 0; i < 16; i++)
            {
                result += " " + decimalFormat.Format(viewMatrix[i]);
            }

            this.mTextView.Text = result;
        }

        public void OnClick(View v)
        {
            this.Analyze();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.analyzer == null)
            {
                return;
            }
            try
            {
                this.analyzer.Stop();
            }
            catch (Exception e)
            {
                Log.Error(Tag, "Stop failed: " + e.Message);
            }
        }

    }
}