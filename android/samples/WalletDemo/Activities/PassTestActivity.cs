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
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Wallet;
using Huawei.Hms.Wallet.Constant;
using XamarinHmsWalletDemo.Utils;

namespace XamarinHmsWalletDemo
{
    [Activity(Label = "PassTestActivity")]
    public class PassTestActivity : Activity
    {
        private static readonly string TAG = "PassTestActivity";
        public const int SaveToAndroid = 888;
        public const int NoOwner = 20;
        public const int HmsVersionCode = 907135001;
        private WalletPassClient walletPassClient;
        private string issuerId;
        private string passObject;
        private Button btnSaveToWallet, btnAddPassOne, btnAddPassTwo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_passtest);
            //Get data with intent.
            Intent intent = this.Intent;
            passObject = intent.GetStringExtra("passObject");
            Log.Info(TAG, "passObject: " + passObject);
            issuerId = intent.GetStringExtra("issuerId");
            Log.Info(TAG, "issuerId: " + issuerId);

            //Init views
            btnSaveToWallet = FindViewById<Button>(Resource.Id.btnSaveToWallet);
            btnAddPassOne = FindViewById<Button>(Resource.Id.btnAddPassOne);
            btnAddPassTwo = FindViewById<Button>(Resource.Id.btnAddPassTwo);

            //Click events
            btnSaveToWallet.Click += BtnSaveToWallet_Click;
            btnAddPassOne.Click += BtnAddPassOne_Click;
            btnAddPassTwo.Click += BtnAddPassTwo_Click;
        }

        //add by wallet app or browser
        private void BtnAddPassTwo_Click(object sender, EventArgs e)
        {
            
            string jweStr = GetJweFromAppServer(passObject);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse("wallet://com.huawei.wallet/walletkit/consumer/pass/save?content=" + Android.Net.Uri.Encode(jweStr)));
            try
            {
                StartActivity(intent);
            }
            catch (ActivityNotFoundException ex)
            {
                Log.Error(TAG, "HMS error:ActivityNotFoundException :" + ex.Message);
                throw;
            }
        }

        private void BtnAddPassOne_Click(object sender, EventArgs e)
        {
            string jweStr = GetJweFromAppServer(passObject);
            string uriStr = "hms://www.huawei.com/payapp/{" + jweStr + "}";
            
            Intent intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(uriStr));
            try
            {
                StartActivityForResult(intent, SaveToAndroid);
            }
            catch (ActivityNotFoundException ex)
            {
                Log.Error(TAG, "HMS error:ActivityNotFoundException"+ ex.Message);
                throw;
            }
        }
        //add by wallet kit sdk
        private void BtnSaveToWallet_Click(object sender, EventArgs e)
        {
            string jweStr = GetJweFromAppServer(passObject);
            
            CreateWalletPassRequest request = CreateWalletPassRequest.GetBuilder()
                .SetContent(jweStr)
                .Build();
            Log.Info(TAG, "GetWalletObjectClient");
            walletPassClient = Wallet.GetWalletPassClient(this);
            Task task = walletPassClient.CreateWalletPass(request);
            ResolveTaskHelper.ExecuteTask(task, this, SaveToAndroid);
        }
        
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            Log.Info(TAG, "requestCode: " + requestCode + "resultCode" + resultCode);
            switch (requestCode)
            {
                case SaveToAndroid:
                    switch (resultCode)
                    {
                        case Result.Ok:
                            Toast.MakeText(this, "Save Success", ToastLength.Long).Show();
                            break;
                        case Result.Canceled:
                            Toast.MakeText(this, "(Reason, 1：cancel by user 2：HMS not install or register or updated)", ToastLength.Long).Show();
                            break;
                        
                        default:
                            if(data != null)
                            {
                                int errorCode = data.GetIntExtra(WalletPassConstant.ExtraErrorCode, -1);
                                Toast.MakeText(this, "fail, [" + errorCode + "]:" + AnalyzerErrorCode(errorCode), ToastLength.Long).Show();
                            }
                            else
                            {
                                Toast.MakeText(this, "fail: data is null ", ToastLength.Long).Show();
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private string AnalyzerErrorCode(int errorCode)
        {
            string tips = "";
            switch (errorCode)
            {
                case WalletPassConstant.ErrorCodeServiceUnavailable:
                    tips = "server unavailable (net error)";
                    break;
                case WalletPassConstant.ErrorCodeInternalError:
                    tips = "internal error";
                    break;
                case WalletPassConstant.ErrorCodeInvalidParameters:
                    tips = "invalid parameters or card is added";
                    break;
                case WalletPassConstant.ErrorCodeMerchantAccountError:
                    tips = "JWE verify fail";
                    break;
                case WalletPassConstant.ErrorCodeUserAccountError:
                    tips = "hms account error（invalidity or Authentication failed）";
                    break;
                case WalletPassConstant.ErrorCodeUnsupportedApiRequest:
                    tips = "unSupport API";
                    break;
                case WalletPassConstant.ErrorCodeOthers:
                default:
                    tips = "unknown Error";
                    break;
            }
            return tips;
        }

        private string GetJweFromAppServer(string passObject)
        {
            string jweStr = "";
            try
            {
                jweStr = JweUtil.GenerateJwe(issuerId, passObject);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "fail ：jwe trans error: "+ ex.Message, ToastLength.Long).Show();
                Log.Info(TAG, "jwe trans error");
                throw;
            }
            Log.Info(TAG, "jweStr:" + jweStr);
            return jweStr;
        }
    }
}