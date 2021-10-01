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
using XamarinAccountKitDemo.Logger;
using Android.Content;
using Android.Content.PM;
using Log = XamarinAccountKitDemo.Logger.Log;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Huawei.Hms.Support.Sms;
using XamarinAccountKitDemo.SMSBroadcastReceiver;
using Huawei.Hms.Support.Sms.Common;
using Result = Android.App.Result;
using System;
using XamarinAccountKitDemo.Helper;
using Android.Widget;
using Huawei.Hms.Support.Account.Service;
using Huawei.Hms.Support.Account.Request;
using Huawei.Hms.Support.Account;
using Huawei.Hms.Support.Account.Result;
using System.Threading.Tasks;

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
        private IAccountAuthService mAuthManager;
        //HUAWEI ID authorization service request parameters
        private AccountAuthParams mAuthParam;
        //AccessToken for Independent Auth after sign in
        string accessToken;

        private Button btnSignIn, btnSignInCode, btnSilentSignIn, btnIndependentAuth, btnStartSmsManager, btnCancelAuthorization, btnSignOut;

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
            btnIndependentAuth = FindViewById<Button>(Resource.Id.hwid_independentAuth);
            btnStartSmsManager = FindViewById<Button>(Resource.Id.hwid_startSmsManager);
            btnCancelAuthorization = FindViewById<Button>(Resource.Id.hwid_cancelAuthorization);
            btnSignOut = FindViewById<Button>(Resource.Id.hwid_signout);
            
            //Set Click event to buttons
            btnSignIn.Click += BtnSignIn_Click;
            btnSignInCode.Click += BtnSignInCode_Click;
            btnSilentSignIn.Click += BtnSilentSignIn_Click;
            btnIndependentAuth.Click += BtnIndependentAuth_Click;
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

            //Inflate log fragment
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

        private void BtnIndependentAuth_Click(object sender, EventArgs e)
        {
            IndependentAuthorization();
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
            mAuthParam = new AccountAuthParamsHelper(AccountAuthParams.DefaultAuthRequestParam)
                            .SetIdToken()
                            .SetAccessToken()
                            .CreateParams();
            mAuthManager = AccountAuthManager.GetService(this, mAuthParam);

            StartActivityForResult(mAuthManager.SignInIntent, Constant.RequestSignInLogin);
        }

        /// <summary>
        /// Signing In with HUAWEI ID (Authorization Code)
        /// </summary>
        private void SignInCode()
        {
            mAuthParam = new AccountAuthParamsHelper(AccountAuthParams.DefaultAuthRequestParam)
                    .SetProfile()
                    .SetAuthorizationCode()
                    .CreateParams();
            mAuthManager = AccountAuthManager.GetService(this, mAuthParam);

            StartActivityForResult(mAuthManager.SignInIntent, Constant.RequestSignInLoginCode);
        }

        /// <summary>
        /// Independent Authorization
        /// </summary>
        private void IndependentAuthorization()
        {
            mAuthParam = new AccountAuthParamsHelper()
                            .SetProfile()
                            .CreateParams();
            mAuthManager = AccountAuthManager.GetService(this, mAuthParam);

            StartActivityForResult(mAuthManager.GetIndependentSignInIntent(accessToken), Constant.RequestIndependentSignInLoginCode);
        }

        /// <summary>
        /// Signing Out from HUAWEI ID
        /// </summary>
        private async void SignOut()
        {
            if(mAuthManager == null)
            {
                Log.InfoFunc(TAG, "Please, sign in first");
                return;
            }
            Task signOutTask = mAuthManager.SignOutAsync();

            try
            {
                await signOutTask;

                if (signOutTask.Exception == null)
                {
                    Log.InfoFunc(TAG, "Sign Out success");
                }
                else
                {
                    Log.InfoFunc(TAG, "Sign Out failed: " + (signOutTask.Exception).ToString());
                }          

            }
            catch (Exception e)
            {
                Log.InfoFunc(TAG, "Sign Out failed: " + e.Message);
            }
        }

        /// <summary>
        /// Silently Signing In With HUAWEI ID
        /// </summary>
        private async void SilentSignIn()
        {
            if (mAuthManager == null)
            {
                Log.InfoFunc(TAG, "Please, sign in by HuaweiID first!");
                return;
            }
            Task<AuthAccount> silentSignInTask = mAuthManager.SilentSignInAsync();

            try
            {
                await silentSignInTask;

                if(silentSignInTask.Result != null)
                {
                    Log.InfoFunc(TAG, "SilentSignIn success");
                }
                else
                {
                    Log.InfoFunc(TAG, "SilentSignIn failed: " + (silentSignInTask.Exception).ToString());
                    SignIn();
                }

            }
            catch(Exception e)
            {
                Log.InfoFunc(TAG, "SilentSignIn failed: " + e.Message);
            }

        }
        /// <summary>
        /// Revoking HUAWEI ID Authorization
        /// </summary>
        private async void CancelAuthorization()
        {
            mAuthParam = new AccountAuthParamsHelper(AccountAuthParams.DefaultAuthRequestParam)
                            .SetProfile()
                            .SetAuthorizationCode()
                            .CreateParams();
            mAuthManager = AccountAuthManager.GetService(this, mAuthParam);
            Task cancelAuthorizationTask = mAuthManager.CancelAuthorizationAsync();
            try
            {
                await cancelAuthorizationTask;

                if(cancelAuthorizationTask != null)
                {
                    Log.InfoFunc(TAG, "Cancel Authorization success");
                }
                else
                {
                    Log.InfoFunc(TAG, "Cancel Authorization failed: " + (cancelAuthorizationTask.Exception).ToString());
                }

            }
            catch(Exception e)
            {
                Log.InfoFunc(TAG, "Cancel Authorization failed: " + e.Message);
            }

        }

        /// <summary>
        /// Start read sms manager
        /// </summary>
        private async void StartReadSmsManager()
        {
            System.Threading.Tasks.Task readSmsManagerTask = ReadSmsManager.StartAsync(this);
            try
            {
                await readSmsManagerTask;

                if (readSmsManagerTask.Exception == null)
                {
                    Log.InfoFunc(TAG, "Read Sms Manager Service Started");
                }
                else
                {
                    Log.InfoFunc(TAG, "Read Sms Manager Service failed: " + (readSmsManagerTask.Exception).ToString());
                }

            }
            catch (Exception e)
            {
                Log.InfoFunc(TAG, "Read Sms Manager Service failed: " + e.Message);
            }
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

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constant.RequestSignInLogin)
            {
                // Sign In Operation
                Task<AuthAccount> authAccountTask = AccountAuthManager.ParseAuthResultFromIntentAsync(data);
                try
                {
                    await authAccountTask;

                    if(authAccountTask.Result != null)
                    {
                        AuthAccount authAccount = authAccountTask.Result;
                        Log.InfoFunc(TAG, authAccount.DisplayName + " signIn success ");
                        Log.InfoFunc(TAG, "AccessToken: " + authAccount.AccessToken);
                        accessToken = authAccount.AccessToken;
                    }
                    else
                    {
                        Log.InfoFunc(TAG, "signIn failed: " + (authAccountTask.Exception).ToString());
                    }
                }
                catch (Exception e)
                {
                    Log.InfoFunc(TAG, "signIn failed: " + e.Message);
                }
            }
            if (requestCode == Constant.RequestSignInLoginCode)
            {
                // Sign In Code Operation
                Task<AuthAccount> authAccountTask = AccountAuthManager.ParseAuthResultFromIntentAsync(data);
                try
                {
                    await authAccountTask;

                    if (authAccountTask.Result != null)
                    {
                        AuthAccount authAccount = authAccountTask.Result;
                        Log.InfoFunc(TAG, "signIn get code success.");
                        Log.InfoFunc(TAG, "ServerAuthCode: " + authAccount.AuthorizationCode);
                    }
                    else
                    {
                        Log.InfoFunc(TAG, "signIn get code failed: " + (authAccountTask.Exception).ToString());
                    }
                }
                catch (Exception e)
                {
                    Log.InfoFunc(TAG, "signIn get code failed: " + e.Message);
                }
            }
            if (requestCode == Constant.RequestIndependentSignInLoginCode)
            {
                // Independent Auth Operation
                Task<AuthAccount> authAccountTask = AccountAuthManager.ParseAuthResultFromIntentAsync(data);
                try
                {
                    await authAccountTask;

                    if (authAccountTask.Result != null)
                    {
                        AuthAccount authAccount = authAccountTask.Result;
                        Log.InfoFunc(TAG, "Independent Auth success.");
                        Log.InfoFunc(TAG, "ServerAuthCode: " + authAccount.AuthorizationCode);
                    }
                    else
                    {
                        Log.InfoFunc(TAG, "Independent Auth failed: " + (authAccountTask.Exception).ToString());
                    }
                }
                catch (Exception e)
                {
                    Log.InfoFunc(TAG, "Independent Auth failed: " + e.Message);
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