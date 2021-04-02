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
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Nearby.Discovery;
using static Android.Widget.ExpandableListView;

namespace XamarinHmsNearbyDemo
{
    public class NearbyConnection : Fragment
    {
        public static string TAG = "MainActivity";
        public static ExpandableListView AvalibleDeviceslistview;
        
        private static volatile MainActivity M_SERVICE = null;
        public static Switch switchScan;
        public static Switch switchBroad;
       
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.NearbyConnection, container, false);
            AvalibleDeviceslistview = (ExpandableListView)view.FindViewById(Resource.Id.Nearby_Devices1);
            AvalibleDeviceslistview.SetAdapter(MainActivity.Adapter);
            switchScan = view.FindViewById<Switch>(Resource.Id.switchScan);
            switchBroad = view.FindViewById<Switch>(Resource.Id.switchBroadcast);
            M_SERVICE = (MainActivity)Activity;
            RegisterForContextMenu(AvalibleDeviceslistview);


            switchScan.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (e.IsChecked)
                {
                    Toast.MakeText((Activity), " Starting Scanning ", ToastLength.Short).Show();
                    ScanOption.Builder discBuilder = new ScanOption.Builder();
                    Policy policy = Policy.PolicyStar;
                    discBuilder.SetPolicy(policy);
                    MainActivity.mDiscoveryEngine.StartScan(MainActivity.MyServiceID, new ScanCallBack((MainActivity)Activity), discBuilder.Build()).AddOnSuccessListener(new TaskListener(Activity.ApplicationContext, "StartScanning")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, "StartScanning"));
                    Log.Debug(MainActivity.TAG, "ScanOption.Builder : Policy :" + policy.ToString());
                }
                else
                {
                    Toast.MakeText(Activity.ApplicationContext, " Stopping Scanning ", ToastLength.Short).Show();
                    MainActivity.mDiscoveryEngine.StopScan();

                }
            };

            switchBroad.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (e.IsChecked)
                {
                    Toast.MakeText((Activity), " Starting Broadcasting ", ToastLength.Short).Show();
                    BroadcastOption.Builder advBuilder = new BroadcastOption.Builder();
                    Policy policy = Policy.PolicyStar;
                    advBuilder.SetPolicy(policy);
                    MainActivity.mDiscoveryEngine.StartBroadcasting(MainActivity.MyEndPoint, MainActivity.MyServiceID, new ConnCallBack((MainActivity)Activity), advBuilder.Build()).AddOnSuccessListener(new TaskListener(Activity.ApplicationContext, "StartBroadcasting")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, "StartBroadcasting"));
                    Log.Debug(MainActivity.TAG, "BroadcastOption.Builder : Policy :" + policy.ToString());
                }
                else
                {
                    Toast.MakeText(Activity.ApplicationContext, " Stopping Broadcasting ", ToastLength.Short).Show();
                    MainActivity.mDiscoveryEngine.StopBroadcasting();

                }
            };
            
            AvalibleDeviceslistview.GroupClick += delegate (object sender, ExpandableListView.GroupClickEventArgs e)
            {
                if (!MainActivity.AvaliableDevicesList.ElementAt(e.GroupPosition).Status)
                    MainActivity.InitiateConnection(MainActivity.AvaliableDevicesList.ElementAt(e.GroupPosition));
                else if (AvalibleDeviceslistview.IsGroupExpanded(e.GroupPosition)) AvalibleDeviceslistview.CollapseGroup(e.GroupPosition);
                else AvalibleDeviceslistview.ExpandGroup(e.GroupPosition);
            };

            return view;
        }
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            MenuInflater inflater = new MenuInflater(Activity.ApplicationContext);
            inflater.Inflate(Resource.Menu.ContextMenu, menu);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            ExpandableListContextMenuInfo info = (ExpandableListContextMenuInfo)item.MenuInfo;
            int groupPosition = ExpandableListView.GetPackedPositionGroup(info.PackedPosition);
            if (item.ItemId == Resource.Id.Disconnect)
            {
                MainActivity.DisconnectDevice(MainActivity.AvaliableDevicesList.ElementAt(groupPosition).EndPoint);
            }
            return base.OnContextItemSelected(item);
        }

    }


}
