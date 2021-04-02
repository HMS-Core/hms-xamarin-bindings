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
using Android.App;
using Android.OS;
using Android.Runtime;
using Huawei.Hms.Support.Hwid.Request;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Result;
using Huawei.Hms.Support.Hwid.Service;
using XamarinAccountKitDemo.HmsSample;
using Task = Huawei.Hmf.Tasks.Task;
using XamarinAccountKitDemo.Logger;
using Android.Content;
using Android.Views;
using Android.Content.PM;
using Log = XamarinAccountKitDemo.Logger.Log;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using XamarinAccountKitDemo.HmsSample.Common;
using Huawei.Hms.Support.Sms;
using XamarinAccountKitDemo.SMSBroadcastReceiver;
using Huawei.Hms.Support.Sms.Common;
using Result = Android.App.Result;
using System;
using System.Collections.Generic;
using Huawei.Hms.Support.Api.Entity.Auth;
using XamarinAccountKitDemo.Helper;
using Android.Widget;

namespace XamarinAccountKitDemo.HmsSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class HuaweiIdActivity : LoggerActivity
    {
        //Log tag
        public static readonly string TAG = "HuaweiIdActivity";
        //Broadcast Receiver for sms validation
        public MySMSBroadcastReceiver mySMSBroadcastReceiver;
        //HUAWEI ID authorization service interface 
        private IHuaweiIdAuthService authManager;
        //HUAWEI ID authorization service request parameters
        private HuaweiIdAuthParams authParam;
        private Button btnSignIn, btnSignInCode, btnSilentSignIn, btnStartSmsManager, btnCancelAuthorization, btnSignOut;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            //Define buttons
            btnSignIn = FindViewById<Button>(Resource.Id.hwid_signin);
            btnSignInCode = FindViewById<Button>(Resource.Id.hwid_signInCode);
            btnSilentSignIn = FindViewById<Button>(Resource.Id.hwid_silentSignIn);
            btnStartSmsManager = FindViewById<Button>(Resource.Id.hwid_startSmsManager);
            btnCancelAuthorization = FindViewById<Button>(Resource.Id.hwid_cancelAuthorization);
            btnSignOut = FindViewById<Button>(Resource.Id.hwid_signout);
            
            //Set Click event to buttons
            btnSignIn.Click += BtnSignIn_Click;
            btnSignInCode.Click += BtnSignInCode_Click;
            btnSilentSignIn.Click += BtnSilentSignIn_Click;
            btnStartSmsManager.Click += BtnStartSmsManager_Click;
            btnCancelAuthorization.Click += BtnCancelAuthorization_Click;
            btnSignOut.Click += BtnSignOut_Click;
            FindViewById(Resource.Id.hwid_huawei_button).Click += BtnHwidHuawei_Click;
            //Create instance of Broadcast Receiver for sms read service
            mySMSBroadcastReceiver = new MySMSBroadcastReceiver();

            /*Obtain App Hash Key by AppHashKeyHelper for validation sms format
            * https://developer.huawei.com/consumer/en/doc/development/HMS-References/account-readsmsmanager#messagerule
            */
            var appHashKey = AppHashKeyHelper.GetAppHashKey(this);

            //sample log Please ignore
            AddLogFragment();

            //check permissions
            CheckPermission(new string[] { Android.Manifest.Permission.Internet,
                                           Android.Manifest.Permission.AccessNetworkState,
                                           Android.Manifest.Permission.ReadSms,
                                           Android.Manifest.Permission.ReceiveSms,
                                           Android.Manifest.Permission.SendSms,
                                           Android.Manifest.Permission.BroadcastSms}, 100);

        }

        public void CheckPermission(string[] permissions, int requestCode)
        {
            foreach (string permission in permissions)
            {
                if (ContextCompat.CheckSelfPermission(this, permission) == Permission.Denied)
                {
                    ActivityCompat.RequestPermissions(this, permissions, requestCode);
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            //Register to receiver for sms read service
            RegisterReceiver(mySMSBroadcastReceiver, new IntentFilter(ReadSmsConstant.ReadSmsBroadcastAction));
        }

        protected override void OnPause()
        {
            base.OnPause();
            //UnRegister to receiver for sms read service
            UnregisterReceiver(mySMSBroadcastReceiver);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void BtnHwidHuawei_Click(object sender, EventArgs e)
        {
            SignIn();
        }

        private void BtnSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
        }

        private void BtnCancelAuthorization_Click(object sender, EventArgs e)
        {
            CancelAuthorization();
        }

        private void BtnStartSmsManager_Click(object sender, EventArgs e)
        {
            StartReadSmsManager();
        }

        private void BtnSilentSignIn_Click(object sender, EventArgs e)
        {
            SilentSignIn();
        }

        private void BtnSignInCode_Click(object sender, EventArgs e)
        {
            SignInCode();
        }

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            SignIn();
        }

        /// <summary>
        /// Signing In with HUAWEI ID (ID Token)
        /// </summary>
        private void SignIn()
        {
            authParam = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DefaultAuthRequestParam)
                    .SetIdToken()
                    .SetAccessToken()
                    .CreateParams();
            authManager = HuaweiIdAuthManager.GetService(this, authParam);

            StartActivityForResult(authManager.SignInIntent, Constant.RequestSignInLogin);
        }

        /// <summary>
        /// Signing In with HUAWEI ID (Authorization Code)
        /// </summary>
        private void SignInCode()
        {
            authParam = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DefaultAuthRequestParam)
                     .SetProfile()
                     .SetAuthorizationCode()
                     .CreateParams();
            authManager = HuaweiIdAuthManager.GetService(this, authParam);

            StartActivityForResult(authManager.SignInIntent, Constant.RequestSignInLoginCode);
        }

        /// <summary>
        /// Signing Out from HUAWEI ID
        /// </summary>
        private void SignOut()
        {
            try
            {
                Task signOutTask = authManager.SignOut();
                signOutTask.AddOnSuccessListener
                 (
                        new OnSuccessListener
                        (
                            "SignOut Success"
                        )
                 ).AddOnFailureListener(
                         new OnFailureListener
                        (
                            "SignOut Failed"
                        )
                 );
            }
            catch (System.Exception e)
            {
                Log.InfoFunc(TAG, "SignOut Failed: "+ e.Message);
            }
        }

        /// <summary>
        /// Silently Signing In With HUAWEI ID
        /// </summary>
        private void SilentSignIn()
        {
            try
            {
                Task silentSignInTask = authManager.SilentSignIn();
                silentSignInTask.AddOnSuccessListener
                 (
                        new OnSuccessListener
                        (
                            "SilentSignIn Success"
                        )
                 ).AddOnFailureListener(
                         new OnFailureListener
                        (
                            "SilentSignIn Failed"
                        )
                 );
            }
            catch (System.Exception e)
            {
                //if Failed use SignIn
                SignIn();
            }

        }
        /// <summary>
        /// Revoking HUAWEI ID Authorization
        /// </summary>
        private void CancelAuthorization()
        {
            try
            {

                Task cancelAuthorizationTask = authManager.CancelAuthorization();
                cancelAuthorizationTask.AddOnCompleteListener
                 (
                        new OnCompleteListener
                        (
                            "Cancel Authorization Success",
                            "Cancel Authorization Failed"
                        )
                 );
            }
            catch (Exception e)
            {
                Log.InfoFunc(TAG, "Cancel Authorization failed: "+ e.Message);
            }

        }

        /// <summary>
        /// Start read sms manager
        /// </summary>
        private void StartReadSmsManager()
        {

            Task readSmsManagerTask = ReadSmsManager.Start(this);
            readSmsManagerTask.AddOnCompleteListener
             (
                    new OnCompleteListener
                    (
                        "Read Sms Manager Service Started",
                        "Read Sms Manager Service Failed"
                    )
             );
        }

        /// <summary>
        /// Verifying ID Token Validity   
        /// </summary>
        /// <param name="idToken">provide from service</param>
        private void ValidateIdToken(string idToken)
        {

            if (string.IsNullOrEmpty(idToken))
            {
                Log.InfoFunc(TAG, "ID Token is empty");
            }
            else
            {
                IDTokenParser idTokenParser = new IDTokenParser();

                try
                {
                    idTokenParser.Verify(idToken);
                }
                catch (System.Exception e)
                {
                    Log.InfoFunc(TAG, "token verification failed: "+ e.Message);
                }
            }

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constant.RequestSignInLogin)
            {
                //login success
                //get user message by ParseAuthResultFromIntent
                Task authHuaweiIdTask = HuaweiIdAuthManager.ParseAuthResultFromIntent(data);
                if (authHuaweiIdTask.IsSuccessful)
                {
                    AuthHuaweiId huaweiAccount = (AuthHuaweiId)authHuaweiIdTask.Result;
                    Log.InfoFunc(TAG, huaweiAccount.DisplayName + " signIn success ");
                    Log.InfoFunc(TAG, "AccessToken: " + huaweiAccount.AccessToken);

                    ValidateIdToken(huaweiAccount.IdToken);
                }
                else
                {
                    Log.InfoFunc(TAG, "signIn failed: " + (authHuaweiIdTask.Exception).ToString());
                }
            }
            if (requestCode == Constant.RequestSignInLoginCode)
            {
                //login success
                Task authHuaweiIdTask = HuaweiIdAuthManager.ParseAuthResultFromIntent(data);
                if (authHuaweiIdTask.IsSuccessful)
                {
                    AuthHuaweiId huaweiAccount = (AuthHuaweiId)authHuaweiIdTask.Result;
                    Log.InfoFunc(TAG, "signIn get code success.");
                    Log.InfoFunc(TAG, "ServerAuthCode: " + huaweiAccount.AuthorizationCode);

                    /**** english doc:For security reasons, the operation of changing the code to an AT must be performed on your server. The code is only an example and cannot be run. ****/
                    /**********************************************************************************************/
                }
                else
                {
                    Log.InfoFunc(TAG, "signIn get code failed: " + (authHuaweiIdTask.Exception).ToString());
                }
            }
        }

        /// <summary>
        /// Adds log fragment
        /// to activity
        /// </summary>
        private void AddLogFragment()
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            LogFragment fragment = new LogFragment();
            transaction.Replace(Resource.Id.framelog, fragment);
            transaction.Commit();
        }
    }


}