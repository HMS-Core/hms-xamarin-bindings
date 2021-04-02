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

namespace XamarinHmsWalletDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btnSaveLoyaltyCard, btnSaveGiftCard, btnSaveCouponCard;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            // Init view elements.
            btnSaveLoyaltyCard = FindViewById<Button>(Resource.Id.btnSaveLoyaltyCard);
            btnSaveGiftCard = FindViewById<Button>(Resource.Id.btnSaveGiftCard);
            btnSaveCouponCard = FindViewById<Button>(Resource.Id.btnSaveCouponCard);

            // Click events.
            btnSaveLoyaltyCard.Click += BtnSaveLoyaltyCard_Click;
            btnSaveGiftCard.Click += BtnSaveGiftCard_Click;
            btnSaveCouponCard.Click += BtnSaveCouponCard_Click;
        }

        private void BtnSaveCouponCard_Click(object sender, EventArgs e)
        {
            Intent intentCoupon = new Intent(this, typeof(CouponCardActivity));
            StartActivity(intentCoupon);
        }

        private void BtnSaveGiftCard_Click(object sender, EventArgs e)
        {
            Intent intentGift = new Intent(this, typeof(GiftCardActivity));
            StartActivity(intentGift);
        }

        private void BtnSaveLoyaltyCard_Click(object sender, EventArgs e)
        {
            Intent intentLoyalty = new Intent(this, typeof(PassDataObjectActivity));
            StartActivity(intentLoyalty);
        }
    }
}