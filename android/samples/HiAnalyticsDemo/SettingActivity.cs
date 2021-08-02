/*
*       Copyright 2020-2021. Huawei. Technologies Co., Ltd. All rights reserved.

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

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Util;

using Huawei.Hms.Analytics;

namespace HiAnalyticsXamarinAndroidDemo
{
    [Activity(Label = "SettingActivity")]
    public class SettingActivity : BaseActivity
    {
        public static readonly string TAG = "MainActivity";
        HiAnalyticsInstance instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            instance = HiAnalytics.GetInstance(this);

            FindViewById(Resource.Id.save_setting_button).Click += Submit_Click;
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string occupation = FindViewById<TextView>(Resource.Id.txtOccupation).Text; 
                string age = FindViewById<TextView>(Resource.Id.txtAge).Text; 
                string color = FindViewById<TextView>(Resource.Id.txtFavoriteColor).Text;
                instance.SetUserProfile("Occupation", string.IsNullOrEmpty(occupation) ? "Not specified" : occupation);
                instance.SetUserProfile("Age", string.IsNullOrEmpty(age) ? "Not specified" : age);
                instance.SetUserProfile("FavoriteColor", string.IsNullOrEmpty(color) ? "Not specified" : color);
                DisplayAlert("User profile set successfully");
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SettingActivity Exception:" + ex.Message);
            }
        }

        public override void DisplayAlert(string message)
        {
            base.DisplayAlert(message);
        }

        public override int GetContentViewId()
        {
            return Resource.Layout.settings_view;
        }

        public override int GetNavigationMenuItemId()
        {
            return Resource.Id.navigation_settings;
        }
    }
}