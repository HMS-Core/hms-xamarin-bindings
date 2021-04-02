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
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Iap.Entity;
using Org.Json;

namespace XamarinHmsIAPDemo
{
    class BillListAdapter : BaseAdapter
    {
        private static  String TAG = "BillListAdapter";

        Context context;
        private IList<String> BillList;


        public BillListAdapter(Context context, IList<String> billList)
        {
            this.context = context;
            this.BillList = billList;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return BillList.ElementAt(position);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            String billInfo = BillList.ElementAt(position);
            var view = convertView;
            BillListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as BillListAdapterViewHolder;

            if (holder == null)
            {
               
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.bill_list_item, parent, false);
                holder = new BillListAdapterViewHolder(view);
                view.Tag = holder;
            }

            
            try
            {
                JSONObject billInformation = new JSONObject(billInfo);
                String productName = billInformation.OptString("productName");
                int productPrice = billInformation.OptInt("price");
                String currency = billInformation.OptString("currency");
                int orderStatus = billInformation.OptInt("purchaseState");
                holder.productName.Text=productName;
                String productPriceNumber = productPrice / 100 + "." + productPrice % 100 + " " + currency;
                holder.productPrice.Text=productPriceNumber;
                switch (orderStatus)
                {
                    case InAppPurchaseData.PurchaseState.Purchased:
                        holder.orderStatus.Text= "Transaction complete";
                        break;
                    case InAppPurchaseData.PurchaseState.Refunded:
                        holder.orderStatus.Text="Transaction refunded";
                        break;
                    case InAppPurchaseData.PurchaseState.Canceled:
                    default:
                        holder.orderStatus.Text= "Transaction cancel";
                        break;
                }
            }
            catch (JSONException e)
            {
                Log.Error(TAG, "Json error occured!"+e.Message);
            }

            return view;
        }

        public override int Count
        {
            get
            {
                return BillList.Count() ;
            }
        }

    }

    class BillListAdapterViewHolder : Java.Lang.Object
    {
        public TextView productName;
        public TextView productPrice;
        public TextView orderStatus;
        public BillListAdapterViewHolder(View view)
        {
            orderStatus = (TextView)view.FindViewById(Resource.Id.bill_status);
            productName = (TextView)view.FindViewById(Resource.Id.bill_product_name);
            productPrice = (TextView)view.FindViewById(Resource.Id.bill_product_price);
        }
    }
}