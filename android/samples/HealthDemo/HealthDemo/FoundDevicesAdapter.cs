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
using Huawei.Hms.Hihealth.Data;

namespace XamarinHmsHealthDemo
{
    public class FoundDevicesAdapter : BaseAdapter
    {

        Context context;

        IList<BleDeviceInfo> FoundDevices;

        public FoundDevicesAdapter(Context context, IList<BleDeviceInfo> bleDeviceInfolist)
        {
            this.context = context;
            this.FoundDevices = bleDeviceInfolist;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return FoundDevices.ElementAt(position);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            FoundDevicesAdapterViewHolder holder = null;
            BleDeviceInfo Device = FoundDevices.ElementAt(position);

            if (view != null)
                holder = view.Tag as FoundDevicesAdapterViewHolder;

            if (holder == null)
            {
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.Child_layout, parent, false);
                holder = new FoundDevicesAdapterViewHolder(view);
                view.Tag = holder;
            }

            holder.DeviceName.Text = Device.DeviceName;
            holder.Address.Text = Device.DeviceAddress;

            return view;
        }

        public override int Count
        {
            get
            {
                return FoundDevices.Count;
            }
        }

    }

    class FoundDevicesAdapterViewHolder : Java.Lang.Object
    {
        public TextView DeviceName;
        public TextView Address;
       

        public FoundDevicesAdapterViewHolder(View view)
        {
            DeviceName = (TextView)view.FindViewById(Resource.Id.DeviceName);
            Address = (TextView)view.FindViewById(Resource.Id.Address);
           
        }
    }
}