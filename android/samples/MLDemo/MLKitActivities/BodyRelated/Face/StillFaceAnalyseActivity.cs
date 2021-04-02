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

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Face;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face
{
    [Activity(Label = "StillFaceAnalyseActivity")]
    public class StillFaceAnalyseActivity : Activity ,View.IOnClickListener
    {
        private const string Tag = "StillFaceAnalyseActivity";
        private readonly int PhotoRequestCode = 0;

        private TextView mTextView;
        private ImageView mImageView;
        private MLFaceAnalyzer analyzer;
        private Bitmap mBitmap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_face_analyse);
            this.mTextView = (TextView)this.FindViewById(Resource.Id.result);
            this.mImageView = (ImageView)this.FindViewById(Resource.Id.face_image);
            this.FindViewById(Resource.Id.face_detect).SetOnClickListener(this);
            this.FindViewById(Resource.Id.change_photo).SetOnClickListener(this);

            //Set default image
            this.mBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.face_image);
            mImageView.SetImageBitmap(mBitmap);
        }

        private async void Analyzer()
        {
            // Use default parameter settings.
            this.analyzer = MLAnalyzerFactory.Instance.FaceAnalyzer;
            // Create an MLFrame by using the bitmap. Recommended image size: large than 320*320, less than 1920*1920.
            MLFrame frame = MLFrame.FromBitmap(this.mBitmap);

            // Call the AnalyseFrameAsync method to perform face detection
            System.Threading.Tasks.Task<IList<MLFace>> faceAnalyseTask = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await faceAnalyseTask;

                if(faceAnalyseTask.IsCompleted && faceAnalyseTask.Result != null)
                {
                    IList<MLFace> faces = faceAnalyseTask.Result;
                    if (faces.Count > 0)
                    {
                        DisplaySuccess(faces.ElementAt(0));
                    }
                }
                else
                {
                    DisplayFailure();
                }

            }
            catch (Exception e)
            {
                //Operation failed.
                DisplayFailure();
            }

        }

        /// <summary>
        /// When analyze failed,
        /// displays exception message in TextView
        /// </summary>
        private void DisplayFailure()
        {
            this.mTextView.Text = ("Failure");
        }

        /// <summary>
        /// Display result in TextView
        /// </summary>
        private void DisplaySuccess(MLFace mFace)
        {
            DecimalFormat decimalFormat = new DecimalFormat("0.000");
            string result =
                "Left eye open Probability: " + decimalFormat.Format(mFace.Features.LeftEyeOpenProbability);
            result +=
                "\nRight eye open Probability: " + decimalFormat.Format(mFace.Features.RightEyeOpenProbability);
            result += "\nMoustache Probability: " + decimalFormat.Format(mFace.Features.MoustacheProbability);
            result += "\nGlass Probability: " + decimalFormat.Format(mFace.Features.SunGlassProbability);
            result += "\nHat Probability: " + decimalFormat.Format(mFace.Features.HatProbability);
            result += "\nAge: " + mFace.Features.Age;
            result += "\nGender: " + ((mFace.Features.SexProbability > 0.5f) ? "Female" : "Male");
            result += "\nRotationAngleY: " + decimalFormat.Format(mFace.RotationAngleY);
            result += "\nRotationAngleZ: " + decimalFormat.Format(mFace.RotationAngleZ);
            result += "\nRotationAngleX: " + decimalFormat.Format(mFace.RotationAngleX);
            this.mTextView.Text=(result);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.face_detect:
                    this.Analyzer();
                    break;
                case Resource.Id.change_photo:
                    ChangePhoto();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Change the target photo
        /// </summary>
        private void ChangePhoto()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, this.PhotoRequestCode);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //Take picture activity result
            if(requestCode == this.PhotoRequestCode)
            {
                if (data != null)
                {
                    this.mBitmap = (Bitmap)data.Extras.Get("data");
                    mImageView.SetImageBitmap(this.mBitmap);
                    //clean textView for new image analyzing
                    mTextView.Text = "";
                }
            }
        }

        /// <summary>
        /// Stop analyzer on OnDestroy() event.
        /// </summary>
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
                Log.Info(StillFaceAnalyseActivity.Tag, "Stop failed: " + e.Message);
            }
        }
    }
}