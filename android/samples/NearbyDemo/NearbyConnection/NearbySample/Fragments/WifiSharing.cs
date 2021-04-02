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

using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Nearby.Wifishare;

namespace XamarinHmsNearbyDemo
{
    public class WifiSharing : Fragment
    {
        public static string TAG = "MainActivity";
        private ExpandableListView AvalibleDeviceslistview;
        public static Switch switchWifiShare;
        public static Switch switchWifiSet;
        private static volatile MainActivity M_SERVICE = null;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.WifiSharing, container, false);
            AvalibleDeviceslistview = (ExpandableListView)view.FindViewById(Resource.Id.Nearby_Devices2);
            AvalibleDeviceslistview.SetAdapter(MainActivity.Adapter);
            switchWifiShare = view.FindViewById<Switch>(Resource.Id.switchShareWIFI);
            switchWifiSet = view.FindViewById<Switch>(Resource.Id.switchSetWIFI);
            M_SERVICE = (MainActivity)Activity;


            switchWifiShare.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (e.IsChecked)
                {
                    Toast.MakeText(Activity.ApplicationContext, " Starting Scanning Nearby Devices", ToastLength.Short).Show();

                    MainActivity.mWifiShareEngine.StartWifiShare(new WifiCallBackImp((MainActivity)Activity), WifiSharePolicy.PolicyShare).AddOnSuccessListener(new TaskListener(Activity.ApplicationContext, "StartScanningWiFiForShare")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, "StartScanningWiFiForShare")); 
                }
                else
                {
                    Toast.MakeText(Activity.ApplicationContext, " Stopping Scanning ", ToastLength.Short).Show();
                    MainActivity.mWifiShareEngine.StopWifiShare().AddOnSuccessListener(new TaskListener(Activity.ApplicationContext, "StopWifi Sharing")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, "StopWifi Sharing"));

                }
            };

            switchWifiSet.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (e.IsChecked)
                {
                    Toast.MakeText(Activity.ApplicationContext, " Starting Scanning Nearby Wifi Sharing Devices", ToastLength.Short).Show();

                    MainActivity.mWifiShareEngine.StartWifiShare(new WifiCallBackImp((MainActivity)Activity), WifiSharePolicy.PolicySet).AddOnSuccessListener(new TaskListener(Activity.ApplicationContext, "StartScanningWiFiSharing")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, "StartScanningWiFiSharing")); ;
                }
                else
                {
                    Toast.MakeText(Activity.ApplicationContext, " Stopping Scanning ", ToastLength.Short).Show();
                    MainActivity.mWifiShareEngine.StopWifiShare();

                }
            };

            AvalibleDeviceslistview.GroupClick += delegate (object sender, ExpandableListView.GroupClickEventArgs e)
            {
                MainActivity.mWifiShareEngine.ShareWifiConfig(MainActivity.AvaliableDevicesList.ElementAt(e.GroupPosition).EndPoint).AddOnSuccessListener(new TaskListener(Activity.ApplicationContext, "ShareWIFIConfig")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, "ShareWIFIConfig"));

            };

            return view;
        }


    }

}