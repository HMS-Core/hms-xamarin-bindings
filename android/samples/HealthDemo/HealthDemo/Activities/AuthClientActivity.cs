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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Hihealth;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Support.Api.Entity.Auth;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Request;
using Huawei.Hms.Support.Hwid.Result;
using Huawei.Hms.Support.Hwid.Service;

namespace XamarinHmsHealthDemo
{
    [Activity(Label = "AuthClientActivity")]
    public class AuthClientActivity : Activity
    {
        private static string TAG = "AuthClient";

        // HUAWEI Health kit SettingController
        private SettingController MySettingController;

        // display authorization result
        private TextView AuthDescTitle;

        // display authorization failure message
        private TextView AuthFailTips;

        // Login in to the HUAWEI ID and authorize
        private Button LoginAuth;

        // confirm result
        private Button Confirm;

        // retry HUAWEI health authorization
        private Button AuthRetry;

        public static IHuaweiIdAuthService AuthService;

        public static PopupWindow popupWindow;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AuthClient_layout);

            InitView();
            InitService();
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            // Handle the sign-in response.
            HandleSignInResult(requestCode, data);
            // Handle the HAUWEI Health authorization Activity response.
            HandleHealthAuthResult(requestCode);
        }

        private void InitView()
        {
            AuthDescTitle = FindViewById<TextView>(Resource.Id.health_auth_desc_title);
            AuthFailTips = FindViewById<TextView>(Resource.Id.health_auth_fail_tips);
            LoginAuth = FindViewById<Button>(Resource.Id.health_login_auth);
            Confirm = FindViewById<Button>(Resource.Id.health_auth_confirm);
            AuthRetry = FindViewById<Button>(Resource.Id.health_auth_retry);

            AuthDescTitle.Visibility = ViewStates.Gone;
            AuthFailTips.Visibility = ViewStates.Gone;
            Confirm.Visibility = ViewStates.Gone;
            AuthRetry.Visibility = ViewStates.Gone;

            // Click event of login HUAWEI ID and authorization
            LoginAuth.Click += delegate (object sender, EventArgs e)
            {
                SignIn();
            };

            // Click event of retry HUAWEI Health authorization
            AuthRetry.Click += delegate (object sender, EventArgs e)
            {
                CheckOrAuthorizeHealth();
            };

            // Finish this Activity
            Confirm.Click += delegate (object sender, EventArgs e)
            {
                this.Finish();
            };
        }

        private void InitService()
        {

            Log.Info(TAG, "HiHealthKitClient connect to service");
            // Initialize SettingController
            HiHealthOptions fitnessOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
            AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(fitnessOptions);
            MySettingController = HuaweiHiHealth.GetSettingController(this, signInHuaweiId);
        }


        //Sign-in and authorization method.
        //The authorization screen will display up if authorization has not granted by the current account.
        private async void SignIn()
        {
            Log.Info(TAG, "Begin sign in");
            IList<Scope> scopeList = new List<Scope>();
            // Add scopes to apply for. The following only shows an example.
            // Developers need to add scopes according to their specific needs.

            // View and save steps in HUAWEI Health Kit.
            scopeList.Add(new Scope(Scopes.HealthkitStepRead));
            scopeList.Add(new Scope(Scopes.HealthkitStepWrite));
            // View and save height and weight in HUAWEI Health Kit.
            scopeList.Add(new Scope(Scopes.HealthkitHeightweightRead));
            scopeList.Add(new Scope(Scopes.HealthkitHeightweightWrite));
            // View and save the heart rate data in HUAWEI Health Kit.
            scopeList.Add(new Scope(Scopes.HealthkitHeartrateRead));
            scopeList.Add(new Scope(Scopes.HealthkitHeartrateWrite));
            // View and save activityRecord in HUAWEI Health Kit.
            scopeList.Add(new Scope(Scopes.HealthkitActivityRead));
            scopeList.Add(new Scope(Scopes.HealthkitActivityWrite));
            // View and save sleep data in HUAWEI Health Kit.
            scopeList.Add(new Scope(Scopes.HealthkitSleepRead));
            scopeList.Add(new Scope(Scopes.HealthkitSleepWrite));
            // Configure authorization parameters.
            HuaweiIdAuthParamsHelper AuthParamsHelper = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DefaultAuthRequestParam);
            HuaweiIdAuthParams AuthParams = AuthParamsHelper.SetIdToken().SetAccessToken().SetScopeList(scopeList).CreateParams();

            // Initialize the HuaweiIdAuthService object.
            AuthService = HuaweiIdAuthManager.GetService(this, AuthParams);

            // Silent sign-in. If authorization has been granted by the current account,
            // the authorization screen will not display. This is an asynchronous method.
            var AuthHuaweiIdTask = AuthService.SilentSignInAsync();
            try
            {
                await AuthHuaweiIdTask;

                if (AuthHuaweiIdTask.IsCompleted && AuthHuaweiIdTask.Result != null)
                {
                    if (AuthHuaweiIdTask.Exception == null)
                    {

                        Log.Info(TAG, "SilentSignIn success");
                        Toast.MakeText(this, "SilentSignIn success", ToastLength.Long).Show();

                        // anfter Huawei ID authorization, perform Huawei Health authorization.
                        CheckOrAuthorizeHealth();
                    }
                    else
                    {
                        // The silent sign-in fails.
                        // This indicates that the authorization has not been granted by the current account.
                        Log.Info(TAG, "Sign failed status:" + AuthHuaweiIdTask.Exception.Message);
                        Log.Info(TAG, "Begin sign in by intent");

                        // Call the sign-in API using the getSignInIntent() method.
                        Intent signInIntent = AuthService.SignInIntent;

                        // Display the authorization screen by using the startActivityForResult() method of the activity.
                        StartActivityForResult(signInIntent, Constants.RequestSignInLogin);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Info(TAG, "Sign failed :" + ex.Message);
                Log.Info(TAG, "Begin sign in by intent");
                // Call the sign-in API using the getSignInIntent() method.
                Intent signInIntent = AuthService.SignInIntent;

                // Display the authorization screen by using the startActivityForResult() method of the activity.
                StartActivityForResult(signInIntent, Constants.RequestSignInLogin);
            }

        }


        //Method of handling authorization result responses
        //@param requestCode (indicating the request code for displaying the authorization screen)
        //@param data (indicating the authorization result response)

        private void HandleSignInResult(int requestCode, Intent data)
        {
            // Handle only the authorized responses
            if (requestCode != Constants.RequestSignInLogin)
            {
                return;
            }

            // Obtain the authorization response from the intent.
            HuaweiIdAuthResult result = HuaweiIdAuthAPIManager.HuaweiIdAuthAPIService.ParseHuaweiIdFromIntent(data);
            if (result != null)
            {
                Log.Debug(TAG, "HandleSignInResult status = " + result.Status + ", result = " + result.IsSuccess);
                if (result.IsSuccess)
                {
                    Log.Debug(TAG, "Sign in is success");

                    // Obtain the authorization result.
                    HuaweiIdAuthResult authResult = HuaweiIdAuthAPIManager.HuaweiIdAuthAPIService.ParseHuaweiIdFromIntent(data);
                    Log.Debug(TAG, "Sign in is success authResult" + authResult);

                    // after Huawei ID authorization, perform Huawei Health authorization.
                    CheckOrAuthorizeHealth();
                }
            }
        }


        //Method of handling the HAUWEI Health authorization Activity response
        //@param requestCode (indicating the request code for displaying the HUAWEI Health authorization screen)
        private void HandleHealthAuthResult(int requestCode)
        {
            if (requestCode != Constants.RequestHealthAuth)
            {
                return;
            }

            QueryHealthAuthorization();
        }


        //Check HUAWEI Health authorization status.
        //if not, start HUAWEI Health authorization Activity for user authorization.
        public async void CheckOrAuthorizeHealth()
        {
            Log.Debug(TAG, "Begint to checkOrAuthorizeHealth");
            // 1. Build a PopupWindow as progress dialog for time-consuming operation
            popupWindow = InitPopupWindow();

            // 2. Calling SettingController to query HUAWEI Health authorization status.
            // This method is asynchronous Task.
            var AuthTask = MySettingController.GetHealthAppAuthorizationAsync();
            try
            {
                await AuthTask;

                if (AuthTask.IsCompleted)
                {
                    if (AuthTask.Exception == null)
                    {
                        popupWindow.Dismiss();

                        Log.Info(TAG, "CheckOrAuthorizeHealth get result success");

                        // If HUAWEI Health is authorized, build success View.
                        if (AuthTask.Result)
                        {
                            BuildSuccessView();
                        }
                        else
                        {
                            // If not, start HUAWEI Health authorization Activity by schema with User-defined requestCode.
                            Android.Net.Uri healthKitSchemaUri = Android.Net.Uri.Parse(Constants.HEALTH_APP_SETTING_DATA_SHARE_HEALTHKIT_ACTIVITY_SCHEME);
                            Intent intent = new Intent(Intent.ActionView, healthKitSchemaUri);
                            // Before start, Determine whether the HUAWEI health authorization Activity can be opened.
                            if (intent.ResolveActivity(PackageManager) != null)
                            {
                                StartActivityForResult(intent, Constants.RequestHealthAuth);
                            }
                            else
                            {
                                BuildFailView(Constants.AppHealthNotInstalled);
                            }
                        }
                    }
                    else
                    {
                        popupWindow.Dismiss();

                        // The method has encountered an exception. Show exception tips in the View.
                        Log.Info(TAG, "CheckOrAuthorizeHealth has exception");
                        BuildFailView(AuthTask.Exception.Message);
                    }
                }

            }
            catch (System.Exception ex)
            {
                popupWindow.Dismiss();

                Log.Info(TAG, "CheckOrAuthorizeHealth has exception");
                BuildFailView(ex.Message);
            }
        }

        
        //Query Huawei Health authorization result.
        private async void QueryHealthAuthorization()
        {

            Log.Debug(TAG, "Begin to QueryHealthAuthorization");
            // 1. Build a PopupWindow as progress dialog for time-consuming operation
            popupWindow = InitPopupWindow();

            // 2. Calling SettingController to query HUAWEI Health authorization status.
            // This method is asynchronous.
            var QueryTask = MySettingController.GetHealthAppAuthorizationAsync();
            try
            {
                await QueryTask;

                if (QueryTask.IsCompleted)
                {
                    if (QueryTask.Exception == null)
                    {
                        popupWindow.Dismiss();

                        Log.Info(TAG, "QueryHealthAuthorization result is" + QueryTask.Result);
                        
                        // Show authorization result in view.
                        if (QueryTask.Result)
                        {
                            BuildSuccessView();
                        }
                        else
                        {
                            BuildFailView(null);
                        }
                    }
                    else
                    {
                        popupWindow.Dismiss();

                        // The method has encountered an exception. Show exception tips in the View.
                        Log.Info(TAG, "QueryHealthAuthorization has exception");
                        BuildFailView(QueryTask.Exception.Message);
                    }
                }

            }
            catch (System.Exception ex)
            {
                popupWindow.Dismiss();

                Log.Info(TAG, "QueryHealthAuthorization has exception");
                BuildFailView(ex.Message);
            }
        }

        public void BuildFailView(String errorMessage)
        {
            AuthDescTitle.Text = Resources.GetString(Resource.String.health_auth_health_kit_fail);
            AuthFailTips.Visibility = ViewStates.Visible;
            AuthRetry.Visibility = ViewStates.Visible;
            Confirm.Visibility = ViewStates.Gone;

            // Authentication failure message. if error message is not null, displayed based on the error code.
            if (Constants.AppHealthNotInstalled.Equals(errorMessage))
            {
                AuthFailTips.Text = Resources.GetString(Resource.String.health_auth_health_kit_fail_tips_update);
            }
            else
            {
                AuthFailTips.Text = Resources.GetString(Resource.String.health_auth_health_kit_fail_tips_connect);
            }
        }

        public void BuildSuccessView()
        {

            AuthDescTitle.Text = Resources.GetString(Resource.String.health_auth_health_kit_success);
            AuthFailTips.Visibility = ViewStates.Gone;
            AuthRetry.Visibility = ViewStates.Gone;
            Confirm.Visibility = ViewStates.Visible;
        }

        private PopupWindow InitPopupWindow()
        {
            popupWindow = new PopupWindow();
            popupWindow.Height = ViewGroup.LayoutParams.WrapContent;
            popupWindow.Width = ViewGroup.LayoutParams.WrapContent;
            popupWindow.Focusable = true;
            View view = LayoutInflater.From(this).Inflate(Resource.Layout.Popup, null);
            popupWindow.ContentView = view;
            popupWindow.ShowAtLocation(view, GravityFlags.Center, 0, 0);
            AuthDescTitle.Visibility = ViewStates.Visible;
            LoginAuth.Visibility = ViewStates.Gone;

            return popupWindow;

        }
    }

}