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
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace XamarinHmsNearbyDemo
{
    public class ExpandableAdapter : BaseExpandableListAdapter
    {

        Context Context;

        public IList<Device> AvaliableDevicesList;

        public ExpandableAdapter(Context context, IList<Device> AvaliableDevicesList)
        {
            this.Context = context;
            this.AvaliableDevicesList = AvaliableDevicesList;
        }


        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return 1;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ExpandableAdapterchildViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ExpandableAdapterchildViewHolder;

            if (holder == null)
            {

                var inflater = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.layoutChild, parent, false);
                holder = new ExpandableAdapterchildViewHolder(view);
                view.Tag = holder;
            }
            Device device = AvaliableDevicesList.ElementAt(groupPosition);
            holder.chat.Click += (object sender, EventArgs e) => {
                if (device.Status)
                {
                    Intent chat = new Intent();
                    chat.SetClass(Context, typeof(ChatActivity));
                    chat.SetFlags(ActivityFlags.ClearTop);
                    chat.PutExtra("DeviceID", groupPosition);
                    Context.StartActivity(chat);
                }
            };

            holder.send.Click += (object sender, EventArgs e) => {
                if (device.Status)
                {
                    Intent intent = new Intent(Intent.ActionOpenDocument);
                    intent.AddCategory(Intent.CategoryOpenable);
                    intent.SetType("*/*");
                    intent.PutExtra("DeviceID", groupPosition);
                    ((Activity)Context).StartActivityForResult(intent, Constants.REQ_CODE_FILE_PICKER);
                }
            };


            return view;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
           return AvaliableDevicesList.ElementAt(groupPosition);
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int position, bool isExpanded, View convertView, ViewGroup parent)
        {
            Device device = AvaliableDevicesList.ElementAt(position);
            var view = convertView;
            ExpandableAdapterViewHolder holder = null;


            if (view != null)
                holder = view.Tag as ExpandableAdapterViewHolder;

            if (holder == null)
            {

                var inflater = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.list_item, parent, false);
                holder = new ExpandableAdapterViewHolder(view);
                view.Tag = holder;
            }

            holder.DeviceName.Text = device.Name;
            holder.EndpointName.Text = device.EndPoint;
            if (device.Status) { holder.Status.Text = "Connected"; }
            else holder.Status.Text = "";
            return view;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return false;
        }


        public override int GroupCount => AvaliableDevicesList.Count();

        public override bool HasStableIds => false;
    }

    class ExpandableAdapterViewHolder : Java.Lang.Object
    {
        public TextView DeviceName;
        public TextView EndpointName;
        public TextView Status;

        public ExpandableAdapterViewHolder(View view)
        {
            DeviceName = (TextView)view.FindViewById(Resource.Id.DeviceName);
            EndpointName = (TextView)view.FindViewById(Resource.Id.Endpoint);
            Status = (TextView)view.FindViewById(Resource.Id.Status);
        }
    }

    class ExpandableAdapterchildViewHolder : Java.Lang.Object
    {
        public Button chat;
        public Button send;

        public ExpandableAdapterchildViewHolder(View view)
        {
            chat = (Button)view.FindViewById(Resource.Id.chat);
            send = (Button)view.FindViewById(Resource.Id.SendFile);
        }
    }
}
