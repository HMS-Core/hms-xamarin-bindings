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

namespace XamarinHmsNearbyDemoMessage
{
    public class MessageAdapter : BaseAdapter
    {

        Context Context;
        public List<string> MsgList;

        public MessageAdapter(Context context,List<string> msglist)
        {
            this.Context = context;
            MsgList = msglist;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return MsgList.ElementAt(position);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            string Message = MsgList.ElementAt(position);
            var view = convertView;
            MessageAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as MessageAdapterViewHolder;

            if (holder == null)
            {
               
                var inflater = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.message, parent, false);
                holder = new MessageAdapterViewHolder(view);
                view.Tag = holder;
            }


            holder.Message.Text = Message;

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return MsgList.Count();
            }
        }

    }

   public class MessageAdapterViewHolder : Java.Lang.Object
    {
        public TextView Message;


        public MessageAdapterViewHolder(View view)
        {
            Message = (TextView)view.FindViewById(Resource.Id.message);
          
        }
    }
}