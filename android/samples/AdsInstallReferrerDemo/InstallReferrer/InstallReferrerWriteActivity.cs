/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Org.Json;

namespace XamarinAdsInstallReferrerDemo.InstallReferrer
{
    [Activity(Label = "InstallReferrerWriteActivity")]
    public class InstallReferrerWriteActivity : BaseActivity,View.IOnClickListener
    {
        private static readonly string TAG = "InstallReferrerWrite";
        private EditText packageNameEt;
        private EditText installReferrerEt;
        private Button deleteBtn;
        private Button saveBtn;
      

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_install_referrer_write);
            Init();

        }

        protected void Init()
        {
            base.Init();
            // Create the "package_name" EditText. User can write service package name.
            packageNameEt = FindViewById<EditText>(Resource.Id.package_name_et);
            // Create the "install_referrer" EditText. User can write install referrer info.
            installReferrerEt = FindViewById<EditText>(Resource.Id.install_referrer_et);
            // Create the "delete" button, which tries to delete existed install referrer according package name.
            deleteBtn = FindViewById<Button>(Resource.Id.delete_btn);
            deleteBtn.SetOnClickListener(this);
            // Create the "save" button, which tries to save what just typed.
            saveBtn = FindViewById<Button>(Resource.Id.save_btn);
            saveBtn.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {

            switch (v.Id)
            {
                case Resource.Id.delete_btn:
                    DeleteInstallReferrer();
                    break;
                case Resource.Id.save_btn:
                    SaveInstallReferrer();
                    break;
                default:
                    break;
            }
        }
        private bool IsInvalid(EditText editText)
        {
            if(null == editText.Text || TextUtils.IsEmpty(editText.Text.ToString()))
            {
                return true;
            }
            return false;
        }
        private void SaveInstallReferrer()
        {
            if (IsInvalid(packageNameEt))
            {
                Log.Error(TAG, "invalid package name");
                Toast.MakeText(this, Resource.String.invalid_package_name, ToastLength.Short).Show();
                return;
            }
            if (IsInvalid(installReferrerEt))
            {
                Log.Error(TAG, "invalid install referrer");
                Toast.MakeText(this, Resource.String.invalid_install_referrer, ToastLength.Short).Show();
                return;
            }
            string pkgName = packageNameEt.Text.ToString();
            string installReferrer = installReferrerEt.Text.ToString();
            SaveOrDelete(pkgName, installReferrer, false);
        }

        private void SaveOrDelete(string pkgName, string installReferrer, bool isDelete)
        {
            Log.Info(TAG, "saveOrDelete isDelete=" + isDelete);
            ISharedPreferences sharedPreferences = GetSharedPreferences(Constants.InstallReferrerFile, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            if(null != editor)
            {
                if (isDelete)
                {
                    // Delete existed install referrer according package name
                    editor.Remove(pkgName);
                    editor.Commit();
                    editor.Apply();
                    Toast.MakeText(this, Resource.String.delete_install_referrer_success, ToastLength.Short).Show();
                }
                else
                {
                    // Save the typed install referrer.
                    JSONObject jsonObject = new JSONObject();
                    try
                    {
                        jsonObject.Put("channelInfo", installReferrer);
                        jsonObject.Put("clickTimestamp", Java.Lang.JavaSystem.CurrentTimeMillis() - 123456L);
                        jsonObject.Put("installTimestamp", Java.Lang.JavaSystem.CurrentTimeMillis());
                        editor.PutString(pkgName, jsonObject.ToString());
                        editor.Commit();
                        editor.Apply();
                        Toast.MakeText(this, Resource.String.save_install_referrer_success, ToastLength.Short).Show();
                    }
                    catch (JSONException e)
                    {
                        Log.Error(TAG, "saveOrDelete JSONException: "+ e.Message);
                        Toast.MakeText(this, Resource.String.save_install_referrer_success, ToastLength.Short).Show();
                        throw;
                    }
                }
            }
        }

        private void DeleteInstallReferrer()
        {
            if (IsInvalid(packageNameEt))
            {
                Log.Error(TAG, "invalid package name");
                Toast.MakeText(this, Resource.String.invalid_package_name, ToastLength.Short).Show();
                return;
            }
            string pkgName = packageNameEt.Text.ToString();
            SaveOrDelete(pkgName, "", true);
        }
    }
}