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
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.ViewPager.Widget;
using XHms_Drive_Kit_Demo_Project.DriveKit.Model;
using XHms_Drive_Kit_Demo_Project.DriveKit.Views;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Adapter
{
    /// <summary>
    /// Tabs Adapter.
    /// </summary>
    public class TabsAdapter : AndroidX.Legacy.App.FragmentStatePagerAdapter, ActionBar.ITabListener, ViewPager.IOnPageChangeListener
    {
        private static readonly string Tag = "TabsAdapter";

        private static  ViewPager mViewPager;

        private Context mContext;

        public IList<TabInfo> mTabs = new List<TabInfo>();

        public override int Count =>  mTabs == null ? 0 : mTabs.Count;

        public TabsAdapter(Context context, FileViewPager viewPager, Android.App.FragmentManager fm) : base(fm)
        {
            mContext = context;
            mViewPager = viewPager;
            mViewPager.Adapter = (this);
            mViewPager.SetOnPageChangeListener(this);
        }


        public override Android.App.Fragment GetItem(int position)
        {
            TabInfo info = mTabs.ElementAt(position);
            info.Fragment = Android.App.Fragment.Instantiate(mContext,
                    Java.Lang.Class.FromType(info.LoadedClass).Name, info.Args);
            return info.Fragment;
        }

        /// <summary>
        /// Add tab.
        /// </summary>
        /// <param name="_Class">class</param>
        /// <param name="args">other args</param>
        public void AddTab(System.Type _Class, Bundle args)
        {
            TabInfo info = new TabInfo(_Class, args);
            mTabs.Add(info);
        }

        public TabInfo GetTabInfo(int position)
        {
            TabInfo info = mTabs.ElementAt(position);
            if (info == null)
            {
                Android.Util.Log.Warn(Tag, "getTabInfo tabInfo is null");
                return null;
            }

            return info;
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            
        }

        public void OnPageScrollStateChanged(int state)
        {
            
        }

        public void OnPageSelected(int position)
        {
            
        }

        public void OnTabReselected(ActionBar.Tab tab, Android.App.FragmentTransaction ft)
        {
            
        }

        public void OnTabSelected(ActionBar.Tab tab, Android.App.FragmentTransaction ft)
        {
            Object tag = tab.Tag;
            for (int i = 0; i < mTabs.Count; i++)
            {
                if (mTabs.ElementAt(i) == tag)
                {
                    mViewPager.CurrentItem = i;
                }
            }
        }

        public void OnTabUnselected(ActionBar.Tab tab, Android.App.FragmentTransaction ft)
        {
            
        }

    }
}