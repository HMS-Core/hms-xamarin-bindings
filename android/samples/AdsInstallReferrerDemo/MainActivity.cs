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
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System;
using Android.Content;
using Android.Util;
using XamarinAdsInstallReferrerDemo.InstallReferrer;

namespace XamarinAdsInstallReferrerDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity,View.IOnClickListener,RadioGroup.IOnCheckedChangeListener
    {
        private static readonly string TAG = "MainActivity";
        private RelativeLayout installReferrerRl;
        private RelativeLayout writeInstallReferrerRl;
        private RadioGroup modeGroup;
        private int callMode = CallMode.Sdk;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            InitView();
        }

        private void InitView()
        {
            // Create the "install_referrer" TextView, which tries to show the obtained install referrer.
            installReferrerRl = FindViewById<RelativeLayout>(Resource.Id.enter_install_referrer_rl);
            installReferrerRl.SetOnClickListener(this);
            // Create the "write_install_referrer" view, which tries to enter the page where user can set service package name and install referer.
            writeInstallReferrerRl = FindViewById<RelativeLayout>(Resource.Id.write_install_referrer_rl);
            writeInstallReferrerRl.SetOnClickListener(this);

            modeGroup = FindViewById<RadioGroup>(Resource.Id.call_mode_rg);
            modeGroup.SetOnCheckedChangeListener(this);
        }

        public void OnClick(View v)
        {
            int id = v.Id;
            if (id == Resource.Id.enter_install_referrer_rl)
            {
                StartActivityIntent(typeof(InstallReferrerActivity));
            }else if(id == Resource.Id.write_install_referrer_rl)
            {
                StartActivityIntent(typeof(InstallReferrerWriteActivity));
            }
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            GetCallMode(checkedId);
        }

        private void GetCallMode(int checkedId)
        {
            if (Resource.Id.mode_sdk_rb == checkedId)
            {
                callMode = CallMode.Sdk;
            }else if(Resource.Id.mode_aidl_rb == checkedId)
            {
                callMode = CallMode.Aidl;
            }
        }


        private void StartActivityIntent(Type activity)
        {
            try
            {
                Intent intent = new Intent(this, activity);
                intent.PutExtra("mode", callMode);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "StartActivity Exception: " + ex.ToString());
                
            }
        }
    }
}