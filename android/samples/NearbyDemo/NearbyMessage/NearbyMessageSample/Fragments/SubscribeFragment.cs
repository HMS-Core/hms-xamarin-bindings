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

using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Nearby.Message;
using Message = Huawei.Hms.Nearby.Message.Message;

namespace XamarinHmsNearbyDemoMessage
{
    public class SubscribeFragment : Fragment
    {
        private Button Subscribe;
        public MessageHandler mMessageHandler;
        ListView CapturedMsg;
        public static MessageAdapter MsglistArrayAdapter;
        public static List<string> MsgList = new List<string>();
        public MsgHandlerImple msgHandlerImple = new MsgHandlerImple();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.subscribe_layout, container, false);
            Subscribe = (Button)view.FindViewById(Resource.Id.subscribeBtn);
            CapturedMsg = (ListView)view.FindViewById(Resource.Id.listView1);
            MsglistArrayAdapter = new MessageAdapter(Activity.ApplicationContext, MsgList);
            CapturedMsg.Adapter = MsglistArrayAdapter;
            MsglistArrayAdapter.NotifyDataSetChanged();
            Subscribe.Click += delegate
            {
                if (Subscribe.Text == "Subscribe")
                {
                    Log.Info(MainActivity.TAG, "HMS start Subscribe...");
                    Subscribe.Text = "Subscribed";
                    Policy policy = new Policy.Builder().SetTtlSeconds(Policy.PolicyTtlSecondsInfinite).Build();
                    GetOption GetOption = new GetOption.Builder().SetPolicy(policy).SetCallback(new MyGetCallback()).Build();
                    MainActivity.sMessageEngine.Get(msgHandlerImple, GetOption).AddOnCompleteListener(new TaskListener(Activity.ApplicationContext, " Get Message")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, " Get Message"));
                    Log.Debug(MainActivity.TAG, "GetOption: Policy= " + GetOption.Policy.ToString());
                }
               else  {
                    Log.Info(MainActivity.TAG, "HMS start Unsubscribe...");
                    Subscribe.Text = "Subscribe";
                    MainActivity.sMessageEngine.Unget(msgHandlerImple).AddOnCompleteListener(new TaskListener(Activity.ApplicationContext, " Unget Message")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, " Unget Message"));
                }
            };

            return view;
        }
        public static void UpdateAdapterList()
        {

            MsglistArrayAdapter.NotifyDataSetChanged();
        }
    }
    public class MsgHandlerImple : MessageHandler
    {
        public override void OnFound(Message msg)
        {
            Log.Info(MainActivity.TAG, "Message Recevied " + System.Text.Encoding.UTF8.GetString(msg.GetContent()));
            SubscribeFragment.MsgList.Add(System.Text.Encoding.UTF8.GetString(msg.GetContent()));
            SubscribeFragment.UpdateAdapterList();

        }

        public override void OnLost(Message msg)
        {
            Log.Info(MainActivity.TAG, "Message Lost"+ System.Text.Encoding.UTF8.GetString(msg.GetContent()));
            SubscribeFragment.MsgList.Remove(System.Text.Encoding.UTF8.GetString(msg.GetContent()));
            SubscribeFragment.UpdateAdapterList();

        }


    }

}