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
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using SafetyDetectDemo.Fragments;

namespace SafetyDetectDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        //TextView textMessage;
        BottomNavigationView navigation;
        Fragment fgSysIntegrity;
        Fragment fgAppsCheck;
        Fragment fgUrlCheck;
        Fragment fgOthers;
        FragmentManager fManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //textMessage = FindViewById<TextView>(Resource.Id.message);
            navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            fManager = FragmentManager;

            OnNavigationItemSelected(navigation.Menu.GetItem(0));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            bool result;
            FragmentTransaction fTransaction = fManager.BeginTransaction();
            HideAllFragment(fTransaction);
            switch (item.ItemId)
            {
                case Resource.Id.navigation_sysIntegrity:
                    if (fgSysIntegrity == null)
                    {
                        fgSysIntegrity = new SysIntegrityFragment();
                        fTransaction.Add(Resource.Id.ly_content, fgSysIntegrity);
                    }
                    else { fTransaction.Show(fgSysIntegrity); }
                    result = true;
                    break;
                case Resource.Id.navigation_appsCheck:
                    if (fgAppsCheck == null)
                    {
                        fgAppsCheck = new AppsCheckFragment();
                        fTransaction.Add(Resource.Id.ly_content, fgAppsCheck);
                    }
                    else { fTransaction.Show(fgAppsCheck); }
                    result = true;
                    break;
                case Resource.Id.navigation_urlCheck:
                    if (fgUrlCheck == null)
                    {
                        fgUrlCheck = new UrlCheckFragment();
                        fTransaction.Add(Resource.Id.ly_content, fgUrlCheck);
                    }
                    else { fTransaction.Show(fgUrlCheck); }
                    result = true;
                    break;
                case Resource.Id.navigation_others:
                    if (fgOthers == null)
                    {
                        fgOthers = new OthersFragment();
                        fTransaction.Add(Resource.Id.ly_content, fgOthers);
                    }
                    else { fTransaction.Show(fgOthers); }
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            fTransaction.Commit();
            return result;
        }

        private void HideAllFragment(FragmentTransaction fTransaction)
        {
            if (fgSysIntegrity != null) { fTransaction.Remove(fgSysIntegrity); fgSysIntegrity = null; }

            if (fgUrlCheck != null) { fTransaction.Remove(fgUrlCheck); fgUrlCheck = null; }

            if (fgAppsCheck != null) { fTransaction.Remove(fgAppsCheck); fgAppsCheck = null; }

            if (fgOthers != null) { fTransaction.Remove(fgOthers); fgOthers = null; }
        }
    }
}

