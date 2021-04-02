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
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Iap;
using Huawei.Hms.Iap.Entity;


namespace XamarinHmsIAPDemo
{
    class ProductListAdapter : BaseAdapter
    {

        Context context;
        private IList<ProductInfo> productInfos;

        public ProductListAdapter(Context context, IList<ProductInfo> productInfos)
        {
            this.context = context;
            this.productInfos = productInfos;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            if (productInfos != null && productInfos.Count() > 0)
            {
                return productInfos.ElementAt(position);
            }
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ProductInfo productInfo = productInfos.ElementAt(position);
            var view = convertView;
            ProductListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ProductListAdapterViewHolder;

            if (holder == null)
            {
               
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.item_layout, parent, false);
                holder = new ProductListAdapterViewHolder(view);
                view.Tag = holder;
            }

            holder.productName.Text=productInfo.ProductName;
            holder.productPrice.Text=productInfo.Price;
           

            return view;
        }

      
        public override int Count
        {
            get
            {
                return productInfos.Count();
            }
        }

    }

    class ProductListAdapterViewHolder : Java.Lang.Object
    {
        public TextView productName;
        public TextView productPrice;
        public ImageView imageView;

        public ProductListAdapterViewHolder(View view)
        {
            productName = (TextView)view.FindViewById(Resource.Id.item_name);
            productPrice = (TextView)view.FindViewById(Resource.Id.item_price);
            imageView = (ImageView)view.FindViewById(Resource.Id.item_image);
        }
    }
}

