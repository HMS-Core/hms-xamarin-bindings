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
using System.Linq;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Nearby.Message;

using static Android.Widget.AdapterView;
using Message = Huawei.Hms.Nearby.Message.Message;

namespace XamarinHmsNearbyDemoMessage
{
    public class PublishFragment : Fragment
    {
        private Button Publish;
        public static EditText Message;
        ListView PublishedMsg;
        public static MessageAdapter MsglistArrayAdapter;
        public static List<string> MsgList = new List<string>();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.publish_layout, container, false);
            Publish = (Button) view.FindViewById(Resource.Id.publishBtn);
            Message = (EditText)view.FindViewById(Resource.Id.MessageInput);
            PublishedMsg = (ListView)view.FindViewById(Resource.Id.listView2);
            MsglistArrayAdapter = new MessageAdapter(Activity.ApplicationContext, MsgList);
            PublishedMsg.Adapter = MsglistArrayAdapter;
            MsglistArrayAdapter.NotifyDataSetChanged();
            RegisterForContextMenu(PublishedMsg);
            
            Publish.Click += delegate
            {
                if (Message.Text.Length != 0) { 
                Log.Info(MainActivity.TAG, "HMS start publish...");
                Policy policy = new Policy.Builder().SetTtlSeconds(Policy.PolicyTtlSecondsDefault).Build();
                PutOption putOption = new PutOption.Builder().SetPolicy(policy).SetCallback(new MyPutCallback()).Build();
                MainActivity.sMessageEngine.Put(new Message(System.Text.Encoding.UTF8.GetBytes(Message.Text)), putOption).AddOnCompleteListener(new TaskListener(Activity.ApplicationContext, " Put Message")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, " Put Message"));
                Log.Debug(MainActivity.TAG, "PutOption: Policy= " + putOption.Policy.ToString());
                }
                else Toast.MakeText(Activity.ApplicationContext, "Please enter a Message", ToastLength.Long).Show();
            };

            return view;
        }
        void Unpublish(Message msg)
        {
            MainActivity.sMessageEngine.Unput(msg).AddOnCompleteListener(new TaskListener(Activity.ApplicationContext, " Unput Message")).AddOnFailureListener(new TaskListener(Activity.ApplicationContext, " unput Message"));

        }
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            MenuInflater inflater = new MenuInflater(Activity.ApplicationContext);
            inflater.Inflate(Resource.Menu.ContextMenu, menu);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            AdapterContextMenuInfo info = (AdapterContextMenuInfo)item.MenuInfo;
            if (item.ItemId == Resource.Id.Unput)
            {
                Message msg = new Message(System.Text.Encoding.UTF8.GetBytes(MsgList.ElementAt(info.Position)));
                Unpublish(msg);
                MsgList.RemoveAt(info.Position);
                MsglistArrayAdapter.NotifyDataSetChanged();
            }
            return base.OnContextItemSelected(item);
        }

    }
}