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
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Wallet.Constant;
using Huawei.Hms.Wallet.Pass;
using Java.Text;
using Java.Util;
using XamarinHmsWalletDemo.Utils;

namespace XamarinHmsWalletDemo
{
    [Activity(Label = "CouponCardActivity")]
    public class CouponCardActivity : Activity
    {
        private Spinner spinner;
        private List<string> dataList;
        private ArrayAdapter<string> arrayAdapter;
        int index;
        private string startTime = "";
        private string endTime = "";
        Button btnClickSaveCouponData, btnSelectStartTime, btnSelectEndTime;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_couponcard);

            //Init views.
            spinner = FindViewById<Spinner>(Resource.Id.spinnerCoupon);
            //state list
            dataList = new List<string>();
            dataList.Add("ACTIVE");
            dataList.Add("COMPLETED");
            dataList.Add("EXPIRED");
            dataList.Add("INACTIVE");
            arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, dataList);
            spinner.Adapter = arrayAdapter;

            spinner.ItemSelected += Spinner_ItemSelected;

            btnClickSaveCouponData = FindViewById<Button>(Resource.Id.clickSaveCouponData);
            btnSelectStartTime = FindViewById<Button>(Resource.Id.selectStartTime);
            btnSelectEndTime = FindViewById<Button>(Resource.Id.selectEndTime);
            //Click events
            btnClickSaveCouponData.Click += BtnClickSaveCouponData_Click;
            btnSelectStartTime.Click += BtnSelectStartTime_Click;
            btnSelectEndTime.Click += BtnSelectEndTime_Click;
        }

        private void BtnSelectEndTime_Click(object sender, EventArgs e)
        {
            Calendar calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month) + 1;
            int day = calendar.Get(CalendarField.DayOfMonth);
            BasisTimesUtils.ShowDatePickerDialog(this, "true", year, month, day, new OnDatePickerListener(this, 2));
        }

        private void BtnSelectStartTime_Click(object sender, EventArgs e)
        {
            Calendar calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month) + 1;
            int day = calendar.Get(CalendarField.DayOfMonth);
            BasisTimesUtils.ShowDatePickerDialog(this, "true", year, month, day, new OnDatePickerListener(this, 1));
        }

        private void BtnClickSaveCouponData_Click(object sender, EventArgs e)
        {
            //Serial number
            EditText edtSerialNumberCoupon = FindViewById<EditText>(Resource.Id.serinumberCoupon);
            string serialNumber = edtSerialNumberCoupon.Text.Trim().ToString();
            if (TextUtils.IsEmpty(serialNumber))
            {
                Toast.MakeText(this, "Serialnumber can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Organization id
            EditText edtOrganizationIdCoupon = FindViewById<EditText>(Resource.Id.cardNumberCoupon);
            string organizationId = edtOrganizationIdCoupon.Text.Trim().ToString();
            if (TextUtils.IsEmpty(organizationId))
            {
                Toast.MakeText(this, "Cardnumber can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Style id
            EditText edtPassStyleIdentifierCoupon = FindViewById<EditText>(Resource.Id.passStyleIdentifierCoupon);
            string styleId = edtPassStyleIdentifierCoupon.Text.Trim().ToString();
            if (TextUtils.IsEmpty(styleId))
            {
                Toast.MakeText(this, "Template ID can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Pass type id
            EditText edtPassType = FindViewById<EditText>(Resource.Id.typeIdentifierCoupon);
            string typeId = edtPassType.Text.Trim().ToString();
            if (TextUtils.IsEmpty(typeId))
            {
                Toast.MakeText(this, "Pass type can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Issuer id.
            EditText edtIssuerId = FindViewById<EditText>(Resource.Id.issuerIdCoupon);
            string issuerId = edtIssuerId.Text.Trim().ToString();
            if (TextUtils.IsEmpty(issuerId))
            {
                Toast.MakeText(this, "Issuer id can't be empty.", ToastLength.Long).Show();
                return;
            }

            EditText edtBackgroundColor = FindViewById<EditText>(Resource.Id.backgroundColorCoupon);
            EditText edtLogoCoupon = FindViewById<EditText>(Resource.Id.logoCoupon);

            //Merchant name
            EditText edtMerchantNameCoupon = FindViewById<EditText>(Resource.Id.merchantNameCoupon);
            string merchantName = edtMerchantNameCoupon.Text.Trim().ToString();
            if (TextUtils.IsEmpty(merchantName))
            {
                Toast.MakeText(this, "Merchant name can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Coupon title
            EditText edtNameCoupon = FindViewById<EditText>(Resource.Id.nameCoupon);
            string name = edtNameCoupon.Text.Trim().ToString();
            if (TextUtils.IsEmpty(name))
            {
                Toast.MakeText(this, "Coupon title can't be empty.", ToastLength.Long).Show();
                return;
            }

            //State
            string state = WalletPassConstant.PassStateActive;
            switch (dataList[index])
            {
                case "ACTIVE":
                    state = WalletPassConstant.PassStateActive;
                    break;
                case "COMPLETED":
                    state = WalletPassConstant.PassStateCompleted;
                    break;
                case "EXPIRED":
                    state = WalletPassConstant.PassStateExpired;
                    break;
                case "INACTIVE":
                    state = WalletPassConstant.PassStateInactive;
                    break;
                default:
                    break;
            }

            //Time check
            long start = 0;
            long end = 0;
            Date date = new Date();
            if (TextUtils.IsEmpty(startTime))
            {
                Toast.MakeText(this, "Please select StartTime.", ToastLength.Long).Show();
                return;
            }
            else
            {
                start = BasisTimesUtils.GetLongtimeOfYMD(startTime);
            }

            if (TextUtils.IsEmpty(endTime))
            {
                Toast.MakeText(this, "Please select EndTime", ToastLength.Long).Show();
                return;
            }
            else
            {
                end = BasisTimesUtils.GetLongtimeOfYMD(endTime);
            }

            if(end <= start || end <= date.Time)
            {
                Toast.MakeText(this, "Please check (EndTime > StartTime) && (EndTime > CurrentTime)", ToastLength.Long).Show();
                return;
            }

            //Provider name
            EditText edtMerchantProviderCoupon = FindViewById<EditText>(Resource.Id.merchantProvidesCoupon);
            string providesCoupon = edtMerchantProviderCoupon.Text.ToString();
            if (TextUtils.IsEmpty(providesCoupon))
            {
                Toast.MakeText(this, "ProvidesName can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Bar code
            EditText edtPassBarcode = FindViewById<EditText>(Resource.Id.barcodeTextCoupon);
            EditText edtPassBarcodeValue = FindViewById<EditText>(Resource.Id.barcodeValueCoupon);

            //Details 
            EditText edtDetailsCoupon = FindViewById<EditText>(Resource.Id.DetailsCoupon);

            //ImageUris
            EditText edtImageModuleDataMainImageUrisCoupon = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUrisCoupon);
            EditText edtImageModuleDataMainImageUrisDescCoupon = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUrisDesCoupon);
            EditText edtImageModuleDataMainImageUrisCouponOne = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUrisCoupon1);
            EditText edtImageModuleDataMainImageUrisDescCouponOne = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUrisDesCoupon1);

            string imageUris = edtImageModuleDataMainImageUrisCoupon.Text.Trim().ToString();
            string imageDesc = edtImageModuleDataMainImageUrisDescCoupon.Text.Trim().ToString();
            string imageUrisOne = edtImageModuleDataMainImageUrisCouponOne.Text.Trim().ToString();
            string imageDescOne = edtImageModuleDataMainImageUrisDescCouponOne.Text.Trim().ToString();

            //DisclaimerCoupon
            EditText edtDisclaimerCoupon = FindViewById<EditText>(Resource.Id.disclaimerCoupon);

            //Message
            EditText edtMessageHeaderCoupon = FindViewById<EditText>(Resource.Id.messageHeaderCoupon);
            EditText edtMessageBodyCoupon = FindViewById<EditText>(Resource.Id.messageBodyCoupon);
            EditText edtMessageHeaderCouponOne = FindViewById<EditText>(Resource.Id.messageHeaderCoupon1);
            EditText edtMessageBodyCouponOne = FindViewById<EditText>(Resource.Id.messageBodyCoupon1);

            string messageHeader = edtMessageHeaderCoupon.Text.Trim().ToString();
            string messageBody = edtMessageBodyCoupon.Text.Trim().ToString();
            string messageHeaderOne = edtMessageHeaderCouponOne.Text.Trim().ToString();
            string messageBodyOne = edtMessageBodyCouponOne.Text.Trim().ToString();

            PassObject.Builder passBuilder = PassObject.GetBuilder();
            //Common fields
            List<CommonField> commonFields = new List<CommonField>();
            //Append fields
            List<AppendField> appendFields = new List<AppendField>();

            //1 Background color of the outer frame.
            CommonField backgroundColorCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyBackgroundColor)
                .SetLabel("Background color label")
                .SetValue(edtBackgroundColor.Text.Trim().ToString())
                .Build();
            commonFields.Add(backgroundColorCommonField);

            //3 Logo on the coupon.
            CommonField logoCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyLogo)
                .SetLabel("This is logo label")
                .SetValue(edtLogoCoupon.Text.Trim().ToString())
                .Build();
            commonFields.Add(logoCommonField);

            //4 Merchant name
            CommonField merchantNameCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyMerchantName)
                .SetLabel("This is merchant name label")
                .SetValue(merchantName)
                .Build();
            commonFields.Add(merchantNameCommonField);

            //5 Coupon title
            CommonField couponTitleCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyName)
                .SetLabel("This is Coupon title label")
                .SetValue(name)
                .Build();
            commonFields.Add(couponTitleCommonField);

            //6 Expiration time
            //Simple date format.
            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
            passBuilder.SetStatus(PassStatus.GetBuilder().SetState(state).SetEffectTime(format.Format(new Date(start))).SetExpireTime(format.Format(new Date(end))).Build());

            //7 Merchant that provides the coupon
            AppendField merchantProvidesAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyProviderName)
                .SetLabel("This is merchant provider label")
                .SetValue(providesCoupon)
                .Build();
            appendFields.Add(merchantProvidesAppendField);

            //8 Barcode or QR code
            //9 Remarks
            BarCode barCode = BarCode.GetBuilder()
                .SetType(BarCode.BarcodeTypeQrCode)
                .SetValue(edtPassBarcodeValue.Text.Trim().ToString())
                .SetText(edtPassBarcode.Text.Trim().ToString())
                .Build();
            passBuilder.SetBarCode(barCode);

            //10 Details
            AppendField detailsAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyDetails)
                .SetLabel("This is details label")
                .SetValue(edtDetailsCoupon.Text.Trim().ToString())
                .Build();
            appendFields.Add(detailsAppendField);

            //11 Scrolling images
            List<AppendField> imageList = new List<AppendField>();
            imageList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(imageDesc)
                .SetValue(imageUris)
                .Build());
            imageList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(imageDescOne)
                .SetValue(imageUrisOne)
                .Build());
            passBuilder.AddImageList(imageList);

            //Urllist
            EditText edtCouponUrlLabel = FindViewById<EditText>(Resource.Id.couponUrlLable);
            EditText edtCouponUrlValue = FindViewById<EditText>(Resource.Id.couponUrlValue);
            EditText edtCouponUrlLabelOne = FindViewById<EditText>(Resource.Id.couponUrlLable1);
            EditText edtCouponUrlValueOne = FindViewById<EditText>(Resource.Id.couponUrlValue1);

            List<AppendField> urlList = new List<AppendField>();
            urlList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(edtCouponUrlLabel.Text.Trim().ToString())
                .SetValue(edtCouponUrlValue.Text.Trim().ToString())
                .Build());
            urlList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(edtCouponUrlLabelOne.Text.Trim().ToString())
                .SetValue(edtCouponUrlValueOne.Text.Trim().ToString())
                .Build());
            passBuilder.AddUrlList(urlList);

            //12 Disclaimer
            AppendField disclaimerAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyDisclaimer)
                .SetLabel("This is Disclaimer label")
                .SetValue(edtDisclaimerCoupon.Text.Trim().ToString())
                .Build();
            appendFields.Add(disclaimerAppendField);

            //13 Message
            List<AppendField> messageList = new List<AppendField>();
            messageList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(messageHeader)
                .SetValue(messageBody)
                .Build());
            messageList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(messageHeaderOne)
                .SetValue(messageBodyOne)
                .Build());
            passBuilder.AddMessageList(messageList);

            passBuilder.SetOrganizationPassId(organizationId);
            passBuilder.SetPassStyleIdentifier(styleId);
            passBuilder.SetPassTypeIdentifier(typeId);
            passBuilder.SetSerialNumber(serialNumber);
            passBuilder.AddAppendFields(appendFields);
            passBuilder.AddCommonFields(commonFields);

            PassObject passObject = passBuilder.Build();
            Intent intent = new Intent(this, typeof(PassTestActivity));
            intent.PutExtra("passObject", passObject.ToJson());
            intent.PutExtra("passId", organizationId);
            intent.PutExtra("issuerId", issuerId);
            intent.PutExtra("typeId", typeId);
            StartActivity(intent);
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            index = e.Position;
        }

        private class OnDatePickerListener : BasisTimesUtils.IOnDatePickerListener
        {
            CouponCardActivity couponCardActivity;
            int timeType = -1;
            public OnDatePickerListener(CouponCardActivity couponCardActivity, int timeType)
            {
                this.couponCardActivity = couponCardActivity;
                this.timeType = timeType;
            }
            public void OnCancel()
            {
                Log.Info("CouponCardActivity", "Cancel DatePickerDialog");
            }

            public void OnConfirm(int year, int month, int dayOfMonth)
            {
                if (timeType.Equals(1))
                {
                    couponCardActivity.startTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }
                else if (timeType.Equals(2))
                {
                    couponCardActivity.endTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }

                else
                {
                    Log.Info("CouponCardActivity", "Time type is wrong.");
                }

            }
        }
    }
}