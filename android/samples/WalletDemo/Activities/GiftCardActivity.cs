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
    [Activity(Label = "GiftCardActivity")]
    public class GiftCardActivity : Activity
    {
        private Spinner spinner;
        private List<string> dataList;
        private ArrayAdapter<string> arrayAdapter;
        int index;
        private Spinner barcodeStyleSpinner;
        private List<string> barcodeStyleList;
        private ArrayAdapter<string> barcodeStyleAdapter;
        int barcodeStyleIndex;
        private string updateTime = "";
        private string startTime = "";
        private string endTime = "";
        private Button btnClickSaveGiftData,btnPassBalanceUpateTime, btnSelectStartTime,btnSelectEndTime;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_giftcard);

            //Init views.
            btnPassBalanceUpateTime = FindViewById<Button>(Resource.Id.passBalanceUpdateTime);
            btnSelectStartTime = FindViewById<Button>(Resource.Id.selectStartTime);
            btnSelectEndTime = FindViewById<Button>(Resource.Id.selectEndTime);
            btnClickSaveGiftData = FindViewById<Button>(Resource.Id.clickSaveGiftData);
            spinner = FindViewById<Spinner>(Resource.Id.spinnerGift);
            dataList = new List<string>();
            dataList.Add("ACTIVE");
            dataList.Add("COMPLETED");
            dataList.Add("EXPIRED");
            dataList.Add("INACTIVE");
            arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, dataList);
            arrayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = arrayAdapter;
            spinner.ItemSelected += Spinner_ItemSelected;

            //Barcode Dropdown box
            barcodeStyleSpinner = FindViewById<Spinner>(Resource.Id.gitfBarcodeStyle);
            barcodeStyleList = new List<string>();
            barcodeStyleList.Add("Codabar");
            barcodeStyleList.Add("QrCode");
            barcodeStyleAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, barcodeStyleList);
            barcodeStyleAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            barcodeStyleSpinner.Adapter = barcodeStyleAdapter;

            barcodeStyleSpinner.ItemSelected += BarcodeStyleSpinner_ItemSelected;

            //Click events
            btnClickSaveGiftData.Click += BtnClickSaveGiftData_Click;
            btnPassBalanceUpateTime.Click += BtnPassBalanceUpateTime_Click;
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

        private void BtnPassBalanceUpateTime_Click(object sender, EventArgs e)
        {
            Calendar calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month)+1;
            int day = calendar.Get(CalendarField.DayOfMonth);
            BasisTimesUtils.ShowDatePickerDialog(this, "true", year, month, day, new OnDatePickerListener(this, 3));
        }

        private void BtnClickSaveGiftData_Click(object sender, EventArgs e)
        {
            //Pass style identifier.
            EditText edtPassStyleIdentifierGift = FindViewById<EditText>(Resource.Id.passStyleIdentifierGift);
            string styleId = edtPassStyleIdentifierGift.Text.Trim().ToString();
            if (TextUtils.IsEmpty(styleId))
            {
                Toast.MakeText(this, "Template Id cant be empty.", ToastLength.Long).Show();
                return;
            }

            //Construct gift card data.
            PassObject.Builder passBuilder = PassObject.GetBuilder();

            //Common fields.
            List<CommonField> commonFields = new List<CommonField>();
            //Append fields.
            List<AppendField> appendFields = new List<AppendField>();

            //Background and description
            EditText edtBackgroundColor = FindViewById<EditText>(Resource.Id.giftBackGroundImage);
            EditText edtBackgroundColorDesc = FindViewById<EditText>(Resource.Id.giftBackGroundImageDesc);

            //1 Background image of the card.
            CommonField backgroundImageCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyBackgroundImg)
                .SetLabel(edtBackgroundColorDesc.Text.Trim().ToString())
                .SetValue(edtBackgroundColor.Text.Trim().ToString())
                .Build();
            commonFields.Add(backgroundImageCommonField);

            //2 Logo on the card
            //Logo
            EditText edtGiftCardLogo = FindViewById<EditText>(Resource.Id.giftCardLogo);
            CommonField logoCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyLogo)
                .SetLabel("This is logo label")
                .SetValue(edtGiftCardLogo.Text.Trim().ToString())
                .Build();
            commonFields.Add(logoCommonField);

            //3 Merchant name
            EditText edtMerchantName = FindViewById<EditText>(Resource.Id.giftMerchantName);

            CommonField merchantNameCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyMerchantName)
                .SetLabel("This is Merchant Name label")
                .SetValue(edtMerchantName.Text.Trim().ToString())
                .Build();
            commonFields.Add(merchantNameCommonField);

            //4 Card name
            EditText edtGiftCardname = FindViewById<EditText>(Resource.Id.giftCardname);
            CommonField cardNameCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyName)
                .SetLabel("This is Card name label")
                .SetValue(edtGiftCardname.Text.Trim().ToString())
                .Build();
            commonFields.Add(cardNameCommonField);

            //5 Card number
            EditText edtGiftCardNumber = FindViewById<EditText>(Resource.Id.giftCardNumber);
            string giftCardNumber = edtGiftCardNumber.Text.Trim().ToString();
            if (TextUtils.IsEmpty(giftCardNumber))
            {
                Toast.MakeText(this,"Card number can't be empty.",ToastLength.Long).Show();
                return;
            }
            CommonField cardNumberCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyCardNumber)
                .SetLabel("This is CardNumber label")
                .SetValue(giftCardNumber)
                .Build();
            commonFields.Add(cardNumberCommonField);

            //6 Balance 
            EditText edtGiftBalance = FindViewById<EditText>(Resource.Id.giftBalance);
            CommonField balanceCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyBalance)
                .SetLabel("This is balance label")
                .SetValue(edtGiftBalance.Text.Trim().ToString())
                .Build();
            commonFields.Add(balanceCommonField);

            //Gift currrency
            EditText edtGiftCurrency = FindViewById<EditText>(Resource.Id.giftCurrency);
            passBuilder.SetCurrencyCode(edtGiftCurrency.Text.Trim().ToString());

            //7 Barcode or Qrcode 
            EditText edtPassBarcodeAlternate = FindViewById<EditText>(Resource.Id.passBarcodeAlternateText);
            string barType = BarCode.BarcodeTypeQrCode;
            EditText edtPassBarcodeValue = FindViewById<EditText>(Resource.Id.passBarcodeValue);
            switch (barcodeStyleList[barcodeStyleIndex])
            {
                case "Codabar":
                    barType = BarCode.BarcodeTypeCodabar;
                    break;
                case "QrCode":
                    barType = BarCode.BarcodeTypeQrCode;
                    break;
                default:
                    break;
            }

            BarCode barCode = BarCode.GetBuilder()
                .SetType(barType)
                .SetValue(edtPassBarcodeValue.Text.Trim().ToString())
                .SetText(edtPassBarcodeAlternate.Text.Trim().ToString())
                .Build();
            passBuilder.SetBarCode(barCode);

            //9 Balance update time
            // balance update time
            long update = 0;
            if (TextUtils.IsEmpty(updateTime))
            {
                Toast.MakeText(this, "Please select balance updateTime", ToastLength.Long).Show();
                return;
            }
            else
            {
                update = BasisTimesUtils.GetLongtimeOfYMD(updateTime);
            }

            //Simple date format
            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
            // Create SimpleDateFormat
            string updateTimeValue = format.Format(new Java.Util.Date(update));
            CommonField balanceUpdateTimeAppendfield = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyBalanceRefreshTime)
                .SetLabel("Updated")
                .SetValue(updateTimeValue)
                .Build();
            commonFields.Add(balanceUpdateTimeAppendfield);

            //10 Pin
            EditText edtGiftCardPin = FindViewById<EditText>(Resource.Id.giftCardPin);
            CommonField pinCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyBlancePin)
                .SetLabel("PIN Number")
                .SetValue(edtGiftCardPin.Text.ToString())
                .Build();
            commonFields.Add(pinCommonField);

            //11 Event number
            EditText edtGiftEventNumber = FindViewById<EditText>(Resource.Id.giftEventNumber);
            AppendField eventNumberAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyEventNumber)
                .SetLabel("Event Number")
                .SetValue(edtGiftEventNumber.Text.Trim().ToString())
                .Build();
            appendFields.Add(eventNumberAppendField);

            //12 Message
            //message
            EditText edtMessageHeaderGift = FindViewById<EditText>(Resource.Id.messageHeaderGift);
            EditText edtMessageBodyGift = FindViewById<EditText>(Resource.Id.messageBodyGift);
            EditText edtMessageHeaderGiftOne = FindViewById<EditText>(Resource.Id.messageHeaderGift1);
            EditText edtMessageBodyGiftOne = FindViewById<EditText>(Resource.Id.messageBodyGift1);

            List<AppendField> messageList = new List<AppendField>();
            messageList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(edtMessageHeaderGift.Text.Trim().ToString())
                .SetValue(edtMessageBodyGift.Text.Trim().ToString())
                .Build());
            messageList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(edtMessageHeaderGiftOne.Text.Trim().ToString())
                .SetValue(edtMessageBodyGiftOne.Text.Trim().ToString())
                .Build());
            passBuilder.AddMessageList(messageList);

            //13 Scrolling images
            List<AppendField> imageList = new List<AppendField>();
            EditText edtScrollingImagesOne = FindViewById<EditText>(Resource.Id.giftScrollingImages1);
            EditText edtScrollingImagesDescOne = FindViewById<EditText>(Resource.Id.giftScrollingDesc1);
            EditText edtScrollingImagesTwo = FindViewById<EditText>(Resource.Id.giftScrollingImages2);
            EditText edtScrollingImagesDescTwo = FindViewById<EditText>(Resource.Id.giftScrollingDesc2);

            imageList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(edtScrollingImagesDescOne.Text.Trim().ToString())
                .SetValue(edtScrollingImagesDescTwo.Text.Trim().ToString())
                .Build());
            imageList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(edtScrollingImagesDescTwo.Text.Trim().ToString())
                .SetValue(edtScrollingImagesTwo.Text.Trim().ToString())
                .Build());
            passBuilder.AddImageList(imageList);

            //UrlList
            EditText edtGiftUrlLabel = FindViewById<EditText>(Resource.Id.giftUrlLable);
            EditText edtGiftUrlValue = FindViewById<EditText>(Resource.Id.giftUrlValue);
            EditText edtGiftUrlLabelOne = FindViewById<EditText>(Resource.Id.giftUrlLable1);
            EditText edtGiftUrlValueOne = FindViewById<EditText>(Resource.Id.giftUrlValue1);

            List<AppendField> urlList = new List<AppendField>();
            urlList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(edtGiftUrlLabel.Text.Trim().ToString())
                .SetValue(edtGiftUrlValue.Text.Trim().ToString())
                .Build());
            urlList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(edtGiftUrlLabelOne.Text.Trim().ToString())
                .SetValue(edtGiftUrlValueOne.Text.Trim().ToString())
                .Build());
            passBuilder.AddUrlList(urlList);

            //14 Nearby stores
            EditText edtGiftNearbyStoresUrl = FindViewById<EditText>(Resource.Id.giftNearbyStoresUrl);
            EditText edtGiftNearbyStoresName = FindViewById<EditText>(Resource.Id.giftNearbyStoresName);
            AppendField nearbyAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyNearbyLocations)
                .SetLabel(edtGiftNearbyStoresName.Text.Trim().ToString())
                .SetValue(edtGiftNearbyStoresUrl.Text.Trim().ToString())
                .Build();
            appendFields.Add(nearbyAppendField);

            //15 Main page
            EditText edtGiftMainPageUrl = FindViewById<EditText>(Resource.Id.giftMainPageUrl);
            EditText edtGiftMainPageName = FindViewById<EditText>(Resource.Id.giftMainPageName);

            AppendField mainPageAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyMainpage)
                .SetLabel(edtGiftMainPageName.Text.Trim().ToString())
                .SetValue(edtGiftMainPageUrl.Text.Trim().ToString())
                .Build();
            appendFields.Add(mainPageAppendField);
            //16 Hotline
            EditText edtGiftHotlinePone = FindViewById<EditText>(Resource.Id.giftHotlinePone);
            EditText edtGiftHotlineName = FindViewById<EditText>(Resource.Id.giftHotlineName);
            AppendField hotlineAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyHotline)
                .SetLabel(edtGiftHotlineName.Text.Trim().ToString())
                .SetValue(edtGiftHotlinePone.Text.Trim().ToString())
                .Build();
            //Public constant
            //Time check
            long start = 0;
            long end = 0;
            Date date = new Date();
            if (TextUtils.IsEmpty(startTime))
            {
                Toast.MakeText(this, "Please select Starttime", ToastLength.Long).Show();
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
                Toast.MakeText(this, "Please check （EndTime > StartTime）&& (EndTime > Current Time)", ToastLength.Long).Show();
                return;
            }
            //serial Number
            EditText organizationPassIdGift = FindViewById<EditText>(Resource.Id.organizationPassIdGift);
            string organizationPassId = organizationPassIdGift.Text.Trim().ToString();
            if (TextUtils.IsEmpty(organizationPassId))
            {
                Toast.MakeText(this, "SerialNumber can't be empty.", ToastLength.Long).Show();
                return;
            }

            EditText passTypeId = FindViewById<EditText>(Resource.Id.passTypeIdentifier);
            string typeId = passTypeId.Text.Trim().ToString();
            if (TextUtils.IsEmpty(typeId))
            {
                Toast.MakeText(this, "Pass Type can't be empty", ToastLength.Long).Show();
                return;
            }

            EditText edtIssuerIdGift = FindViewById<EditText>(Resource.Id.issuerIdGift);
            string issuerId = edtIssuerIdGift.Text.Trim().ToString();
            if (TextUtils.IsEmpty(issuerId))
            {
                Toast.MakeText(this, "Issuer id can't be empty.", ToastLength.Long).Show();
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

            appendFields.Add(hotlineAppendField);
            //PassStyleIdentifier
            EditText edtPassStyleIdentifier = FindViewById<EditText>(Resource.Id.passStyleIdentifierGift);
            passBuilder.SetOrganizationPassId(edtGiftCardNumber.Text.Trim().ToString());
            passBuilder.SetPassStyleIdentifier(edtPassStyleIdentifier.Text.Trim().ToString());
            passBuilder.SetPassTypeIdentifier(typeId);
            passBuilder.SetSerialNumber(organizationPassId);
            passBuilder.SetStatus(PassStatus.GetBuilder().SetState(state).SetEffectTime(format.Format(new Date(start))).SetExpireTime(format.Format(new Date(end))).Build());
            passBuilder.AddAppendFields(appendFields);
            passBuilder.AddCommonFields(commonFields);
            PassObject passObject = passBuilder.Build();
            Log.Info("GiftCardActivity", "passObject: " + passObject.ToJson());
            Intent intent = new Intent(this, typeof(PassTestActivity));
            intent.PutExtra("passObject", passObject.ToJson());
            intent.PutExtra("passId", organizationPassId);
            intent.PutExtra("issuerId", issuerId);
            intent.PutExtra("typeId", typeId);
            StartActivity(intent);

        }

        private void BarcodeStyleSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            barcodeStyleIndex = e.Position;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            index = e.Position;
        }

        private class OnDatePickerListener : BasisTimesUtils.IOnDatePickerListener
        {
            GiftCardActivity giftCardActivity;
            int timeType = -1;
            public OnDatePickerListener(GiftCardActivity giftCardActivity, int timeType)
            {
                this.giftCardActivity = giftCardActivity;
                this.timeType = timeType;
            }
            public void OnCancel()
            {
                Log.Info("GiftCardActivity", "Cancel DatePickerDialog");
            }

            public void OnConfirm(int year, int month, int dayOfMonth)
            {
                if (timeType.Equals(1))
                {
                    giftCardActivity.startTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }
                else if (timeType.Equals(2))
                {
                    giftCardActivity.endTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }
                else if (timeType.Equals(3))
                {
                    giftCardActivity.updateTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }
                else
                {
                    Log.Info("PassDataObjectActivity", "Time type is wrong.");
                }

            }
        }
    }
}