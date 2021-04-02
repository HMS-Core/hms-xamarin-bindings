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
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using XHms_Drive_Kit_Demo_Project.DriveKit.Views;
using XHms_Drive_Kit_Demo_Project.DriveKit.Adapter;
using XHms_Drive_Kit_Demo_Project.DriveKit.Utils;
using XHms_Drive_Kit_Demo_Project.DriveKit.Fragment;
using Android.Views;
using XHms_Drive_Kit_Demo_Project.DriveKit.Model;
using XHms_Drive_Kit_Demo_Project.DriveKit.Log;

namespace XHms_Drive_Kit_Demo_Project
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : Activity
    {
        private static readonly string Tag = "MainActivity";

        public FileViewPager mViewPager;

        public TabsAdapter mTabsAdapter;

        private Toolbar mHwToolbar;
        public Toolbar Toolbar
        {
            get { return mHwToolbar; }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitActionBar();
            InitMainView();
        }

        protected void InitActionBar()
        {
            mHwToolbar = (Toolbar) ViewUtil.FindViewById(this, Resource.Id.hwtoolbar);
        }

        private void InitMainView()
        {
            mViewPager = (FileViewPager) FindViewById(Resource.Id.view_pager);
            mViewPager.OffscreenPageLimit = 1;

            InitTabsAdapter();
            SetEnableScroll(false);
        }

        private void InitTabsAdapter()
        {
            mTabsAdapter = new TabsAdapter(this, mViewPager, FragmentManager);
            mTabsAdapter.AddTab(typeof(InterfaceFragment), null);
            mTabsAdapter.NotifyDataSetChanged();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);
            if (mViewPager == null)
            {
                Logger.Info(Tag, "onOptionsItemSelected viewpager is null");
                return false;
            }
            if (mTabsAdapter == null)
            {
                Logger.Info(Tag, "onOptionsItemSelected mTabsAdapter is null");
                return false;
            }
            int currentItem = mViewPager.CurrentItem;
            TabInfo tabInfo = mTabsAdapter.GetTabInfo(currentItem);
            Android.App.Fragment fragment = tabInfo.Fragment;

            if (fragment == null)
            {
                Logger.Info(Tag, "onOptionsItemSelected fragment is null");
                return false;
            }

            return true;
        }

        public void SetEnableScroll(bool enableScroll)
        {
            if (mViewPager != null)
            {
                mViewPager.EnableScroll = enableScroll;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (mTabsAdapter == null || null == mViewPager)
            {
                if (keyCode == Keycode.Back)
                {
                    Finish();
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}