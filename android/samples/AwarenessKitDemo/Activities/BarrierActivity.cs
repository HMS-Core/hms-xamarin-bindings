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
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwarenessKitDemo.Activities.Barrier;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities
{
    [Activity(Label = "BarrierActivity")]
    public class BarrierActivity : Activity, View.IOnClickListener
    {
        Logger log;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_barrier);

            FindViewById<Button>(Resource.Id.headset_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.location_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.bluetooth_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.behavior_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.time_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.ambientLight_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.beacon_barrier).SetOnClickListener(this);
            FindViewById<Button>(Resource.Id.barrier_combination).SetOnClickListener(this);

            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
        }
        public void OnClick(View v)
        {
            Intent intent;
            switch (v.Id)
            {
                case Resource.Id.headset_barrier:
                    intent = new Intent(this, typeof(HeadsetBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.location_barrier:
                    intent = new Intent(this, typeof(LocationBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.bluetooth_barrier:
                    intent = new Intent(this, typeof(BluetoothBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.behavior_barrier:
                    intent = new Intent(this, typeof(BehaviorBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.time_barrier:
                    intent = new Intent(this, typeof(TimeBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.ambientLight_barrier:
                    intent = new Intent(this, typeof(AmbientLightBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.beacon_barrier:
                    intent = new Intent(this, typeof(BeaconBarrierActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.barrier_combination:
                    intent = new Intent(this, typeof(BarrierCombinationActivity));
                    StartActivity(intent);
                    break;
                default:
                    break;
            }
        }
    }
}