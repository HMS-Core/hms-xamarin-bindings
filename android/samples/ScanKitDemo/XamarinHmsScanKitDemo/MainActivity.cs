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
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using XamarinHmsScanKitDemo.Utils;
using Android.Support.V4.App;
using Android;
using Android.Content;
using XamarinHmsScanKitDemo.Activities;
using Huawei.Hms.Hmsscankit;
using Huawei.Hms.Ml.Scan;


namespace XamarinHmsScanKitDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            FindViewById<Button>(Resource.Id.button_defaultView).Click += (s, e) => RequestPermission(Constants.CameraRequestCode);
            FindViewById<Button>(Resource.Id.button_newView).Click += (s, e) => RequestPermission(Constants.DefinedCode);
            FindViewById<Button>(Resource.Id.button_multisyn).Click += (s, e) => RequestPermission(Constants.MultiProcessorSyncCode);
            FindViewById<Button>(Resource.Id.button_multiAsyn).Click += (s, e) => RequestPermission(Constants.MultiProcessorAsyncCode);
            FindViewById<Button>(Resource.Id.button_bitmap).Click += (s, e) => RequestPermission(Constants.BitmapCode);
            FindViewById<Button>(Resource.Id.button_generateQR).Click += (s,e) => RequestPermission(Constants.GenerateCode);
            
        }

        private void RequestPermission(int requestCode)
        {
            ActivityCompat.RequestPermissions(
                    this,
                    new string[] { Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage,Manifest.Permission.WriteExternalStorage },
                    requestCode);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (permissions == null || grantResults == null)
            {
                return;
            }

            
            if (grantResults.Length < 2 || grantResults[0] != Android.Content.PM.Permission.Granted || grantResults[1] != Android.Content.PM.Permission.Granted)
            {
                return;
            }

            //Customized View Mode
            if (requestCode == Constants.GenerateCode)
            {
                Intent intent = new Intent(this, typeof(GenerateCodeActivity));
                StartActivity(intent);
            }

            //Default View Mode
            if (requestCode == Constants.CameraRequestCode)
            {
                ScanUtil.StartScan(this, Constants.RequestCodeScanOne, new HmsScanAnalyzerOptions.Creator().Create());
            }
            //Customized View Mode
            if (requestCode == Constants.DefinedCode)
            {
                Intent intent = new Intent(this, typeof(DefinedActivity));
                StartActivityForResult(intent, Constants.RequestCodeDefine);
            }
            //Bitmap Mode
            if (requestCode == Constants.BitmapCode)
            {
                Intent intent = new Intent(this, typeof(BitmapActivity));
                intent.PutExtra(Constants.DecodeMode, Constants.BitmapCode);
                StartActivityForResult(intent, Constants.RequestCodeScanMulti);
            }
            //Multiprocessor Synchronous Mode
            if (requestCode == Constants.MultiProcessorSyncCode)
            {
                Intent intent = new Intent(this, typeof(BitmapActivity));
                intent.PutExtra(Constants.DecodeMode, Constants.MultiProcessorSyncCode);
                StartActivityForResult(intent, Constants.RequestCodeScanMulti);
            }
            //Multiprocessor Asynchronous Mode
            if (requestCode == Constants.MultiProcessorAsyncCode)
            {
                Intent intent = new Intent(this, typeof(BitmapActivity));
                intent.PutExtra(Constants.DecodeMode, Constants.MultiProcessorAsyncCode);
                StartActivityForResult(intent, Constants.RequestCodeScanMulti);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok || data == null)
            {
                return;
            }
            //Default View
            if (requestCode == Constants.RequestCodeScanOne)
            {
                HmsScan obj = (HmsScan)data.GetParcelableExtra(ScanUtil.Result);
                if (obj != null)
                {
                    Intent intent = new Intent(this, typeof(DisplayActivity));
                    intent.PutExtra(Constants.Result, obj);
                    StartActivity(intent);
                }
            }
                //MultiProcessor & Bitmap
            else if (requestCode == Constants.RequestCodeScanMulti)
            {
                IParcelable[] obj = (IParcelable[])data.GetParcelableArrayExtra(Constants.ScanResult);
                if (obj != null && obj.Length > 0)
                {
                    //Get one result.
                    if (obj.Length == 1)
                    {
                        if (obj[0] != null && !string.IsNullOrEmpty(((HmsScan)obj[0]).OriginalValue))
                        {
                            Intent intent = new Intent(this, typeof(DisplayActivity));
                            intent.PutExtra(Constants.Result, obj[0]);
                            StartActivity(intent);
                        }
                    }
                    else
                    {
                        Intent intent = new Intent(this, typeof(DisplayMuActivity));
                        intent.PutExtra(Constants.Result, obj);
                        StartActivity(intent);
                    }
                }
                //Customized View
            }
            else if (requestCode == Constants.RequestCodeDefine)
            {
                HmsScan obj = (HmsScan)data.GetParcelableExtra(Constants.ScanResult);
                if (obj != null)
                {
                    Intent intent = new Intent(this, typeof(DisplayActivity));
                    intent.PutExtra(Constants.Result, obj);
                    StartActivity(intent);
                }
            }
        }
    }
}