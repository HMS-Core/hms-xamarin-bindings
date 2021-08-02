/*
*       Copyright 2020-2021. Huawei. Technologies Co., Ltd. All rights reserved.

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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Analytics;
using Huawei.Hms.Analytics.Type;

namespace HiAnalyticsXamarinAndroidDemo
{
    public class OrderFragment : Fragment
    {
        Button createOrderBtn;
        EditText productId, productName, productFeature, productPrice;
        HiAnalyticsInstance instance;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_order, container, false);

            createOrderBtn = view.FindViewById<Button>(Resource.Id.CreateOrderBtn);
            productId = view.FindViewById<EditText>(Resource.Id.ProductIdValue);
            productName = view.FindViewById<EditText>(Resource.Id.ProductNameValue);
            productFeature = view.FindViewById<EditText>(Resource.Id.ProductFeatureValue);
            productPrice = view.FindViewById<EditText>(Resource.Id.PriceValueTV);

            createOrderBtn.Click += CreateOrderBtn_Click;
            productId.Text = CreateRandomId();
            return view;
        }
        public override void OnStart()
        {
            base.OnStart();
            //Create Context
            Context context = Android.App.Application.Context;
            //Generate the Analytics Instance
            instance = HiAnalytics.GetInstance(context);
            //Enable collection capability
            instance.SetAnalyticsEnabled(true);
            //Customizes a page start event.
            instance.PageStart("Order Page", "OrderFragment");
        }

        private void CreateOrderBtn_Click(object sender, EventArgs e)
        {
            bool isPassed = false;
            double price;
            if (ValidationInputs(productId.Text.Trim(), productName.Text.Trim(), productFeature.Text.Trim(), productPrice.Text.Trim()))
            {
                isPassed = double.TryParse(productPrice.Text.Trim(), out price);
                if (isPassed)
                {
                    Bundle bundle = new Bundle();
                    //Initiate Parameters
                    bundle.PutString(HAParamType.Productid, productId.Text.Trim());
                    bundle.PutString(HAParamType.Productname, productName.Text.Trim());
                    bundle.PutString(HAParamType.Productfeature, productFeature.Text.Trim());

                    bundle.PutDouble(HAParamType.Price, price);

                    //Report a preddefined Event
                    instance.OnEvent(HAEventType.Completeorder, bundle);

                    Bundle orderBundle = new Bundle();
                    orderBundle.PutString(HAParamType.Orderid, CreateRandomId());
                    instance.OnEvent(HAEventType.Completeorder, orderBundle);

                    Toast.MakeText(Activity, "Order was created. OrderId: " + orderBundle.GetString(HAParamType.Orderid),ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(Activity, "Please enter valid price.", ToastLength.Short).Show();
                }

            }
            else
            {
                Toast.MakeText(Activity, "Please fill all inputs.", ToastLength.Short).Show();
            }
        }

        private string CreateRandomId()
        {
            return Guid.NewGuid().ToString();
        }
        private bool ValidationInputs(string productid,string productname,string productfeature,string productprice)
        {
            bool isValidate = false;
            if(!TextUtils.IsEmpty(productid)&& !TextUtils.IsEmpty(productname)&&!TextUtils.IsEmpty(productfeature) && !TextUtils.IsEmpty(productprice))
            {
                isValidate = true;
            }
            return isValidate;
        }
       
        public override void OnDestroy()
        {
            base.OnDestroy();
            //Customizes a page end event.
            instance.PageEnd("Order Page");
        }
    }
}