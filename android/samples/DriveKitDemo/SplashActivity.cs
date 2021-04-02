/*
        Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Huawei.Cloud.Base.Auth;
using Huawei.Cloud.Client.Exception;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Common;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Result;
using XHms_Drive_Kit_Demo_Project.DriveKit.Common;
using XHms_Drive_Kit_Demo_Project.DriveKit.Hms;
using XHms_Drive_Kit_Demo_Project.DriveKit.Log;
using XHms_Drive_Kit_Demo_Project.DriveKit.Task;

namespace XHms_Drive_Kit_Demo_Project
{
    [Activity(Label = "Xamarin HMS Drive Demo", Theme = "@style/AppTheme", MainLauncher = true)]
    public class SplashActivity : Activity, View.IOnClickListener
    {
        private const string Tag = "SplashActivity";

        // This is the permission to apply
        private static string[] Permissions = {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera};

        //Refresh acccess token method for Drive sdk
        private static DriveCredential.IAccessMethod refreshAT = new DriveCredentialAccessMethod();

        /// <summary>
        /// Solve the problem that Android 6.0 and above cannot read external storage permissions.
        /// </summary>
        /// <param name="activity">activity</param>
        /// <param name="requestCode">requestCode</param>
        public static bool IsGrantExternalRW(Activity activity, int requestCode)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {

                Permission storagePermission = activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
                Permission cameraPermission = activity.CheckSelfPermission(Manifest.Permission.Camera);
                // Check if there is permission, if you do not have permission, you need to apply
                if (storagePermission != Permission.Granted ||
                        cameraPermission != Permission.Granted)
                {
                    // request for access
                    activity.RequestPermissions(Permissions, requestCode);
                    // Returns false. Description no authorization
                    return false;
                }
            }
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case 1:
                    if (grantResults != null && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        // Verify that the permission is obtained. If the permission is obtained,
                        // the external storage will be open and a toast prompt will pop up to obtain authorization.
                        string sdCard = Android.OS.Environment.ExternalStorageState;
                        if (sdCard.Equals(Android.OS.Environment.MediaMounted))
                        {
                            Toast.MakeText(this, "Permissions Access", ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        RunOnUiThread(delegate()
                            {
                                Toast.MakeText(this, "Permissions Deny", ToastLength.Short).Show();
                            }
                        );
                    }
                    break;
                default:
                    break;
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_splash);
            TextView infoView = (TextView) FindViewById(Resource.Id.splash_info);
            infoView.SetText(Resource.String.str_copyright);
            if (!IsGrantExternalRW(this, 1))
            {
                Android.Util.Log.Info(Tag, "Permissions Deny");
            }

            // Call singIn method
            HmsProxyImpl.Instance.SingIn(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void OnClick(View v)
        {
            
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Logger.Info(Tag, "onActivityResult, requestCode = " + requestCode + ", resultCode = " + resultCode);
            try
            {
                // Handle HMS SDK authentication sign in callback results
                if (requestCode == HmsProxyImpl.REQUEST_SIGN_IN_LOGIN)
                {
                    // login success, get user message by getSignedInAccountFromIntent
                    Task authHuaweiIdTask = HuaweiIdAuthManager.ParseAuthResultFromIntent(data);
                    if (authHuaweiIdTask.IsSuccessful)
                    {
                        AuthHuaweiId huaweiAccount = (AuthHuaweiId)authHuaweiIdTask.Result;
                        Logger.Info(Tag, "onActivityResult, signIn success " + huaweiAccount.DisplayName);
                        HmsProxyImpl.Instance.DealSignInResult(huaweiAccount);
                        InitDrive();
                    }
                    else
                    {
                        // login failed
                        Logger.Info(Tag, "onActivityResult, signIn failed: " + ((ApiException)authHuaweiIdTask.Exception).StatusCode);
                        Toast.MakeText(ApplicationContext, "onActivityResult, signIn failed.", ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Info(Tag, "onActivityResult, catch Exception");
            }
        }

        private void InitDrive()
        {
            string unionID = HmsProxyImpl.Instance.UnionId;
            string at = HmsProxyImpl.Instance.AccessToken;
            if (TextUtils.IsEmpty(unionID) || TextUtils.IsEmpty(at))
            {
                Logger.Error(Tag, "initDrive error, unionID or at is null: " + "unionID:" + unionID + " at " + at);
                return;
            }

            TaskManager.Instance.Execute(new DriveTaskImplementor(delegate() 
                {
                    int returnCode = CredentialManager.Instance.Init(unionID, at, refreshAT);
                    if (DriveCode.Success == returnCode)
                    {
                        // Jump to the app home page after successful initialization.
                        JumpToMainActivity();
                    }
                    else if (DriveCode.ServiceUrlNotEnabled == returnCode)
                    {
                        Toast.MakeText(ApplicationContext, "drive is not enabled", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "drive init error", ToastLength.Long).Show();
                    }
                }
            ));
        }

        private void JumpToMainActivity()
        {
            try
            {
                Logger.Info(Tag, "signIn OK, jump to MainActivity");
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            } 
            catch (Exception e)
            {
                Logger.Error(Tag, "jumpToMainActivity exception:" + e.Message);
            }
        }

    }

    /// <summary>
    /// Refresh acccess token method for Drive sdk.
    /// </summary>
    public class DriveCredentialAccessMethod : Java.Lang.Object, DriveCredential.IAccessMethod
    {
        public string RefreshToken()
        {
            return HmsProxyImpl.Instance.RefreshAccessToken();
        }
    }
}