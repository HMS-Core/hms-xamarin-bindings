/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Bumptech.Glide;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ProductVisionSearch
{
    public class BottomSheetAdapter : BaseAdapter
    {
        private IList<RealProductBean> mlProducts;
        private Context context;

        public BottomSheetAdapter(IList<RealProductBean> mlProducts, Context context) 
        {
            this.mlProducts = mlProducts;
            this.context = context;
        }

        public override int Count =>  mlProducts == null ? 0 : mlProducts.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return mlProducts.ElementAt(position);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = View.Inflate(context, Resource.Layout.adapter_item_product, null);
            }
            
            Glide.With(context).Load(mlProducts.ElementAt(position).ImgUrl).Into((ImageView)convertView.FindViewById(Resource.Id.img));
            ((TextView)convertView.FindViewById(Resource.Id.tv)).Text = (mlProducts.ElementAt(position).Name);
            return convertView;
        }
    }
}