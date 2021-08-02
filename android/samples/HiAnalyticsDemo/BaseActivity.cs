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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace HiAnalyticsXamarinAndroidDemo
{
    [Activity(Label = "BaseActivity")]
    public abstract class BaseActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private BottomNavigationView navigationView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetContentViewId());
            navigationView = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigationView.SetOnNavigationItemSelectedListener(this);
        }


        protected override void OnStart()
        {
            base.OnStart();
            UpdateNavigationBarState();
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);


        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    Intent mainIntent = new Intent(this, typeof(MainActivity));
                    StartActivity(mainIntent);
                    return true;
                case Resource.Id.navigation_settings:
                    Intent settingsIntent = new Intent(this, typeof(SettingActivity));
                    StartActivity(settingsIntent);
                    return true;
            }
            return false;
        }
        private void UpdateNavigationBarState()
        {
            int actionId = GetNavigationMenuItemId();
            SelectBottomNavigationBarItem(actionId);
        }

        void SelectBottomNavigationBarItem(int itemId)
        {
            IMenuItem item = navigationView.Menu.FindItem(itemId);
            item.SetChecked(true);
        }
        public abstract int GetContentViewId();

        public abstract int GetNavigationMenuItemId();

        public virtual void DisplayAlert(string message)
        {
            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("");
            alertDialog.SetMessage(message);
            alertDialog.SetPositiveButton("OK", delegate
            {
                alertDialog.Dispose();
                StartActivity(typeof(MainActivity));
            });
            alertDialog.Show();
        } 
    }
}