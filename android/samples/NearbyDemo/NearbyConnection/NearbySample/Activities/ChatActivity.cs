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
using Android.Content;
using Android.OS;
using Android.Widget;
using Huawei.Hms.Nearby.Transfer;
using Android.Util;

namespace XamarinHmsNearbyDemo
{
    [Activity(Label = "ChatActivity")]
    public class ChatActivity : Activity
    {
        public static string TAG = "ChatActivity";
        public TextView FriendName;
        public static Device device;
        Button send;
        public string Message;
        EditText input;
        ListView ChatList;
        ChatAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChatLayout);
            FriendName = (TextView)FindViewById(Resource.Id.textView1);
            send = (Button)FindViewById(Resource.Id.btn_send);
            input = (EditText)FindViewById(Resource.Id.chat_text);
            device = MainActivity.AvaliableDevicesList.ElementAt(Intent.GetIntExtra("DeviceID", 0));
            ChatList = (ListView)FindViewById(Resource.Id.listChat);
            if (!FriendName.Text.Equals("Chatting with") && !FriendName.Text.Equals(device.Name))
            {   
                MessageBean message = MainActivity.MsgList.ElementAt(MainActivity.MsgList.Count()- 1); 
                MainActivity.MsgList.Clear();
                MainActivity.MsgList.Add(message);
            }
            FriendName.Text = device.Name;
            adapter = new ChatAdapter(this, MainActivity.MsgList);
            ChatList.Adapter = adapter;
            adapter.NotifyDataSetChanged();
            ChatList.SetSelection(ChatList.Bottom);
            send.Click += delegate
            {
                if (device.Status)
                {
                    if (!input.Text.Equals(""))
                    {
                        Message = input.Text + ":msg input";
                        Data data = Data.FromBytes(System.Text.Encoding.UTF8.GetBytes(Message));
                        Log.Debug(TAG, "myEndpointId " + device.EndPoint);
                        MainActivity.mTransferEngine.SendData(device.EndPoint, data).AddOnSuccessListener(new TaskListener(this, "Send Message ")).AddOnFailureListener(new TaskListener(this, "Send Message"));
                        MessageBean item = new MessageBean();
                        item.MyName = MainActivity.MyEndPoint;
                        item.FriendName = device.Name;
                        item.Msg = Message.Split(":")[0];
                        item.Type = (MessageBean.TYPE_SEND_TEXT);
                        MainActivity.MsgList.Add(item);
                        adapter.NotifyDataSetChanged();
                        input.Text = "";
                        ChatList.SetSelection(ChatList.Bottom);
                    }
                    else Toast.MakeText(this, "Enter a Message", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "The device is not connected, try again", ToastLength.Long).Show();
                    MainActivity.InitiateConnection(device);
                }
            };
        }


    }
}