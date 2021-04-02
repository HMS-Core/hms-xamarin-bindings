/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinAdsLiteDemo.Utils;

namespace XamarinAdsLiteDemo.Adapters
{
    public class AdSampleAdapter : BaseAdapter<AdFormat>
    {

        private readonly Context context;
        private List<AdFormat> adFormats;
        public AdSampleAdapter(Context context, List<AdFormat> formatList)
        {
            this.context = context;
            this.adFormats = formatList;
        }

        public override AdFormat this[int position] => adFormats[position];

        public override int Count => adFormats.Count;


        public override long GetItemId(int position)
        {
            return position;
        }

        public AdFormat GetItem(int position)
        {
            return adFormats[position];
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AdFormat adFormat = adFormats[position];
            View view = LayoutInflater.From(context).Inflate(Resource.Layout.item_ad_format, parent, false);

            TextView title = view.FindViewById<TextView>(Resource.Id.itemAdFormat);
            title.Text = adFormat.GetTitle();


            return view;
        }
    }
}