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
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinHmsNearbyDemo
{
    class ChatAdapter : BaseAdapter
    {

        Context Context;
        private List<MessageBean> MsgList;
        public ChatAdapter(Context mContext, List<MessageBean> msgList)
        {
            this.Context = mContext;
            this.MsgList = msgList;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return MsgList.ElementAt(position);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

    public override int GetItemViewType(int position)
        {
            return MsgList.ElementAt(position).Type;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MessageBean item = MsgList.ElementAt(position);
            int type = item.Type;
            SendTextViewHolder sendTextViewHolder = null;
            ReceiveTextViewHolder receiveTextViewHolder = null;
            switch (type)
            {
                case MessageBean.TYPE_SEND_TEXT:
                    sendTextViewHolder = new SendTextViewHolder();
                    var inflater1 = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                    convertView = inflater1.Inflate(Resource.Layout.send_message_item, parent, false);
                    sendTextViewHolder.SendTextView = (TextView)convertView.FindViewById(Resource.Id.tv_send);
                    sendTextViewHolder.SendTextView.Text = item.Msg;
                    convertView.Tag = sendTextViewHolder;
                    break;
                case MessageBean.TYPE_RECEIVE_TEXT:
                    receiveTextViewHolder = new ReceiveTextViewHolder();
                    var inflater2 = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                    convertView = inflater2.Inflate(Resource.Layout.receive_message_item, parent, false);
                    receiveTextViewHolder.ReceiveTextView = (TextView)convertView.FindViewById(Resource.Id.tv_receive);
                    receiveTextViewHolder.ReceiveTextView.Text = item.Msg;
                    convertView.Tag = receiveTextViewHolder;
                    break;
                default:
                    break;
            }           

            return convertView;
        }

       
        public override int Count
        {
            get
            {
                return MsgList.Count();
            }
        }

    }
    class SendTextViewHolder : Java.Lang.Object
    {
        public TextView SendTextView;
    }

    class ReceiveTextViewHolder : Java.Lang.Object
    {
        public TextView ReceiveTextView;
    }
}