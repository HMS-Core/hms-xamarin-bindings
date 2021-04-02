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
using Android.Graphics;
using Android.Icu.Text;
using Android.Net.Http;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Java.Lang;
using Java.Util;
using Org.Json;
using XHms_Drive_Kit_Demo_Project.DriveKit.Hms;
using XHms_Drive_Kit_Demo_Project.DriveKit.Log;
using XHms_Drive_Kit_Demo_Project.DriveKit.Utils;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Views.Activities
{
    [Activity(Label = "WebViewActivity")]
    public class WebViewActivity : HiDiskBaseActivity
    {
        private const string Tag = "WebViewActivity";

        private WebView mWebView;

        private IValueCallback uriCallbacks;

        private static readonly int FileChooserCodeAlbum = 1;

        private static readonly int FileChooserCodeCamera = 2;

        private Android.Net.Uri mCameraUri;

        private Activity mActivity;

        // This is the camera access
        private static string[] CameraPermissions = { Manifest.Permission.Camera };

        // Request ID
        private const int CodeRequestDocPermissionsCamera = 5002;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Logger.Info(Tag, "onCreate");
            InitView();
        }

        private void InitView()
        {
            try
            {
                this.SetContentView(Resource.Layout.docskit_activity_web_view);
                mWebView = (WebView)ViewUtil.FindViewById(this, Resource.Id.external_webview);
                Intent intent = new Intent(Intent);
                string url = intent.GetStringExtra("url");
                // Check the validity of the URL.
                if (IsUrlInValid(url))
                {
                    Logger.Error(Tag, "url is invalid");
                    Finish();
                    return;
                }
                // Setting Web Parameters
                InitWebViewSettings();
                mWebView.LoadUrl(url);
            }
            catch (System.Exception e)
            {
                // The system webView is not installed or is disabled.
                Logger.Error(Tag, "initView error: " + e.Message);
                Finish();
            }
        }

        private bool IsUrlInValid(string url)
        {
            if (TextUtils.IsEmpty(url))
            {
                Logger.Error(Tag, "url is empty");
                return true;
            }
            Android.Net.Uri uri = Android.Net.Uri.Parse(url);
            string scheme = uri.Scheme;
            // check whether HTTPS or HTTP is used.
            if (!"https".Equals(scheme.ToLower()) && !"http".Equals(scheme.ToLower()))
            {
                Logger.Error(Tag, "check url illegal scheme:" + scheme);
                return true;
            }
            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case CodeRequestDocPermissionsCamera:                    
                        if (grantResults != null && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            // Check whether the permission is obtained. If the permission is obtained, the external storage is open and a toast message is displayed
                            string sdCard = Android.OS.Environment.ExternalStorageState;
                            if (Android.OS.Environment.MediaMounted.Equals(sdCard) && mActivity != null)
                            {
                                DoTakePhoto(mActivity);
                            }
                            else
                            {
                                Logger.Warn(Tag, "onRequestPermissionsResult: SD card is not mounted normally.");
                            }
                        }
                        else
                        {
                            if (uriCallbacks != null)
                            {
                                uriCallbacks.OnReceiveValue(null);
                                uriCallbacks = null;
                            }
                            Logger.Warn(Tag, "onRequestPermissionsResult: no camera permission.");
                        }
                    break;
                default:
                    break;
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            this.Intent = intent;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            OnResultAboveLollipop(requestCode, (int)resultCode, data);
        }

        private void OnResultAboveLollipop(int requestCode, int resultCode, Intent intent)
        {
            if (uriCallbacks == null)
            {
                return;
            }
            // check result code
            if (resultCode != (int)Result.Ok)
            {
                uriCallbacks.OnReceiveValue(null);
                uriCallbacks = null;
                return;
            }
            // check request code
            if (requestCode == FileChooserCodeAlbum)
            {
                Intent safeIntent = new Intent(intent);
                Android.Net.Uri albumUri = safeIntent.Data;
                if (albumUri != null)
                {
                    uriCallbacks.OnReceiveValue(new Android.Net.Uri[] { albumUri });
                }
                else
                {
                    uriCallbacks.OnReceiveValue(null);
                }
            }
            else if (requestCode == FileChooserCodeCamera)
            {
                if (mCameraUri != null)
                {
                    uriCallbacks.OnReceiveValue(new Android.Net.Uri[] { mCameraUri });
                }
                else
                {
                    uriCallbacks.OnReceiveValue(null);
                }
            }
            uriCallbacks = null;
        }

        private class ChromeClient : WebChromeClient
        {
            private WebViewActivity mActivity;

            public ChromeClient(WebViewActivity activity)
            {
                mActivity = activity;
            }

            public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
            {
                Logger.Debug(Tag, "onShowFileChooser");
                mActivity.uriCallbacks = filePathCallback;
                Activity activity = mActivity;
                if (activity == null)
                {
                    return false;
                }
                mActivity.ShowChooseDialog(activity);
                return true;
            }
        }

        public void ShowChooseDialog(Activity activity)
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetOnCancelListener(new DialogOnCancelListener(this));
            string[] options = { GetString(Resource.String.menu_open_camera), GetString(Resource.String.menu_album) };
            int MenuCameraIndex = 0;
            int menuAlbumIndex = 1;
            alertDialog.SetItems(options, new DialogInterfaceClickListenerClass(
                    delegate(IDialogInterface dialog, int which)
                    {
                        if (which == MenuCameraIndex)
                        {
                            mActivity = activity;
                            try
                            {
                                if (!RequestDocCameraPermission(activity))
                                {
                                    Logger.Error(Tag, "The user has forbidden to use the camera " + "to take permission or failed to apply for the camera to take permission!");
                                }
                                else
                                {
                                    DoTakePhoto(activity);
                                }
                            }
                            catch (System.Exception e)
                            {
                                Logger.Error(Tag, "onShowFileChooser camera error: " + e.ToString());
                            }
                        }
                        else if (which == menuAlbumIndex)
                        {
                            try
                            {
                                Intent albumIntent = new Intent(Intent.ActionPick);
                                albumIntent.SetType("image/*");
                                ICharSequence charSequence = null;
                                activity.StartActivityForResult(Intent.CreateChooser(albumIntent, charSequence), FileChooserCodeAlbum);
                            }
                            catch (System.Exception e)
                            {
                                Logger.Error(Tag, "onShowFileChooser album error: " + e.ToString());
                            }
                        }

                    }
                ));
            alertDialog.Show();
        }

        private class DialogOnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
        {
            WebViewActivity mActivity;
            public DialogOnCancelListener(WebViewActivity activity)
            {
                this.mActivity = activity;
            }
            public void OnCancel(IDialogInterface dialog)
            {
                if (mActivity.uriCallbacks != null)
                {
                    mActivity.uriCallbacks.OnReceiveValue(null);
                    mActivity.uriCallbacks = null;
                }
            }
        }

        private class DialogInterfaceClickListenerClass : Java.Lang.Object, Android.Content.IDialogInterfaceOnClickListener
        {
            private Action<IDialogInterface, int> actionParameter;
            public DialogInterfaceClickListenerClass(Action<IDialogInterface, int> action)
            {
                this.actionParameter = action;
            }
            public void OnClick(IDialogInterface dialog, int which)
            {
                this.actionParameter(dialog, which);
            }
        }

        private void InitWebViewSettings()
        {
            WebSettings webSettings = mWebView.Settings;
            if (webSettings == null)
            {
                Logger.Error(Tag, "WebSettings is null.");
                Finish();
                return;
            }
            webSettings.JavaScriptEnabled = true;
            webSettings.DomStorageEnabled = true;
            mWebView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            webSettings.LoadWithOverviewMode = true;
            webSettings.UseWideViewPort = true;
            mWebView.SetWebChromeClient(new ChromeClient(this));
            mWebView.AddJavascriptInterface(new WebExternal(), "splash");
            mWebView.SetWebViewClient(new WebViewClientClass(this));
        }

        private void DoTakePhoto(Activity activity)
        {
            SimpleDateFormat timeStampFormat = new SimpleDateFormat("yyyyMMdd_HHmmss");
            string filename = timeStampFormat.Format(new Date());
            ContentValues values = new ContentValues();
            values.Put(MediaStore.Images.ImageColumns.Title, filename);
            mCameraUri = ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);
            Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
            cameraIntent.PutExtra(MediaStore.ExtraOutput, mCameraUri);
            activity.StartActivityForResult(cameraIntent, FileChooserCodeCamera);
        }

        private bool RequestDocCameraPermission(Activity activity)
        {
            if (activity == null)
            {
                return false;
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                Permission cameraPermission = activity.CheckSelfPermission(Manifest.Permission.Camera);
                if (cameraPermission != Permission.Granted)
                {
                    RequestPermissions(CameraPermissions, CodeRequestDocPermissionsCamera);
                    return false;
                }
            }
            return true;
        }

        private class WebExternal : Java.Lang.Object
        {
            public string WPS_GetToken()
            {
                JSONObject jsonObject = new JSONObject();
                try
                {
                    // Send the token to the online preview page.
                    jsonObject.Put("token", HmsProxyImpl.Instance.AccessToken);
                }
                catch (JSONException e)
                {
                    Logger.Error(Tag, "access error");
                }
                return jsonObject.ToString();
            }
        }

        private class WebViewClientClass : WebViewClient
        {
            private WebViewActivity mActivity;
            public WebViewClientClass(WebViewActivity activity)
            {
                this.mActivity = activity;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (url != null || mActivity.IsUrlInValid(url))
                {
                    view.Context.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(url)));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                Logger.Error(Tag, "onReceivedSslError: error");
                handler.Cancel();
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }
        }
    }
}