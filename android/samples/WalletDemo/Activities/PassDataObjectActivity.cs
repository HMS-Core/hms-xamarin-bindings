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
    [Activity(Label = "PassDataObjectActivity")]
    public class PassDataObjectActivity : Activity
    {
        private Spinner spinner;
        private IList<string> dataList;
        private ArrayAdapter<string> arrayAdapter;
        int index;
        private string startTime = "";
        private string endTime = "";
        Button btnClickSaveData;
        Button btnSelectStartTime, btnSelectEndTime;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_passdataobject);

            //Init views
            btnClickSaveData = FindViewById<Button>(Resource.Id.clickSaveData);
            btnSelectStartTime = FindViewById<Button>(Resource.Id.selectStartTime);
            btnSelectEndTime = FindViewById<Button>(Resource.Id.selectEndTime);
            spinner = FindViewById<Spinner>(Resource.Id.spinner);
            dataList = new List<string>();
            dataList.Add("ACTIVE");
            dataList.Add("COMPLETED");
            dataList.Add("EXPIRED");
            dataList.Add("INACTIVE");

            arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, dataList);
            arrayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = arrayAdapter;

            spinner.ItemSelected += Spinner_ItemSelected;

            btnClickSaveData.Click += BtnClickSaveData_Click;
            btnSelectEndTime.Click += BtnSelectEndTime_Click;
            btnSelectStartTime.Click += BtnSelectStartTime_Click;

        }

        private void BtnSelectStartTime_Click(object sender, EventArgs e)
        {
            Calendar calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month) + 1;
            int day = calendar.Get(CalendarField.DayOfMonth);
            BasisTimesUtils.ShowDatePickerDialog(this, "true", year, month, day, new OnDatePickerListener(this, 1));
        }

        private void BtnSelectEndTime_Click(object sender, EventArgs e)
        {
            Calendar calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month) + 1;
            int day = calendar.Get(CalendarField.DayOfMonth);
            BasisTimesUtils.ShowDatePickerDialog(this, "true", year, month, day, new OnDatePickerListener(this, 2));
        }

        private void BtnClickSaveData_Click(object sender, EventArgs e)
        {
            // Serial number 
            EditText serialNumberLoyalty = FindViewById<EditText>(Resource.Id.serinumberLoyalty);
            string serialNumber = serialNumberLoyalty.Text.ToString().Trim();
            if (TextUtils.IsEmpty(serialNumber))
            {
                Toast.MakeText(this, "Serial number can't be empty.", ToastLength.Long).Show();
                return;
            }

            // Pass type.
            EditText passStyleIdentifier = FindViewById<EditText>(Resource.Id.passStyleIdentifier);
            TextView passTypeId = FindViewById<TextView>(Resource.Id.passTypeId);
            string typeId = passTypeId.Text.ToString().Trim();
            if (TextUtils.IsEmpty(typeId))
            {
                Toast.MakeText(this, "Pass type can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Issuer Id(App ID)
            TextView issuerIdView = FindViewById<TextView>(Resource.Id.issuerId);
            string issuerId = issuerIdView.Text.ToString().Trim();
            if (TextUtils.IsEmpty(issuerId))
            {
                Toast.MakeText(this, "IssuerId can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Card Number.
            EditText edtCardNumber = FindViewById<EditText>(Resource.Id.cardNumberLoyalty);
            string cardNumber = edtCardNumber.Text.ToString().Trim();
            if (TextUtils.IsEmpty(cardNumber))
            {
                Toast.MakeText(this, "Card Number can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Member Name.
            EditText edtMemberName = FindViewById<EditText>(Resource.Id.memberName);
            string memberName = edtMemberName.Text.ToString().Trim();
            if (TextUtils.IsEmpty(memberName))
            {
                Toast.MakeText(this, "Member Name can't be empty.", ToastLength.Long).Show();
                return;
            }

            //Locations
            EditText latitude = FindViewById<EditText>(Resource.Id.latitude);
            EditText longitude = FindViewById<EditText>(Resource.Id.longitude);
            EditText latitude1 = FindViewById<EditText>(Resource.Id.latitude1);
            EditText longitude1 = FindViewById<EditText>(Resource.Id.longitude1);

            //Add location.
            double latitudeValue = 0.0;
            double longitudeValue = 0.0;
            string latitudeStr = latitude.Text.ToString().Trim();
            string longitudeStr = longitude.Text.ToString().Trim();
            string latitude1Str = latitude1.Text.ToString().Trim();
            string longitude1Str = longitude1.Text.ToString().Trim();
            if (TextUtils.IsEmpty(latitudeStr) || TextUtils.IsEmpty(longitudeStr))
            {
                Toast.MakeText(this, "Latitude & Longitude can't be empty.", ToastLength.Long).Show();
                return;
            }
            else
            {
                if(-90.0d <= Convert.ToDouble(latitudeStr) && Convert.ToDouble(latitudeStr) <= 90.0d
                    && -180.0d <= Convert.ToDouble(longitudeStr) && Convert.ToDouble(longitudeStr) <= 180.0d)
                {
                    latitudeValue = Convert.ToDouble(latitudeStr);
                    longitudeValue = Convert.ToDouble(longitudeStr);
                }
                else
                {
                    Toast.MakeText(this, "Latitude or Longitude value is illegal.", ToastLength.Long).Show();
                    return;
                }
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

            //Create PassObject.Builder object.
            PassObject.Builder passBuilder = PassObject.GetBuilder();
            //Create common fields list.
            List<CommonField> commonField = new List<CommonField>();
            //Create append fields list.
            List<AppendField> appendFields = new List<AppendField>();

            //1	Background image of the card
            EditText backgroundImage = FindViewById<EditText>(Resource.Id.backgroundImage);
            CommonField backgroundImageCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyBackgroundImg)
                .SetLabel("backgroundImageLabel")
                .SetValue(backgroundImage.Text.Trim().ToString())
                .Build();
            commonField.Add(backgroundImageCommonField);

            //2	Logo on the card
            EditText logoLoyalty = FindViewById<EditText>(Resource.Id.logoLoyalty);
            CommonField logoCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyLogo)
                .SetLabel("This is logo label")
                .SetValue(logoLoyalty.Text.Trim().ToString())
                .Build();
            commonField.Add(logoCommonField);

            //3 Merchant name
            EditText merchantNameLoyalty = FindViewById<EditText>(Resource.Id.merchantNameLoyalty);
            CommonField merchantNameCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyMerchantName)
                .SetLabel("This is merchantName label.")
                .SetValue(merchantNameLoyalty.Text.Trim().ToString())
                .Build();
            commonField.Add(merchantNameCommonField);

            //4 Card name
            EditText cardNameLoyalty = FindViewById<EditText>(Resource.Id.nameLoyalty);
            CommonField cardNameCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyName)
                .SetLabel("This is cardName label")
                .SetValue(cardNameLoyalty.Text.Trim().ToString())
                .Build();
            commonField.Add(cardNameCommonField);

            //5 Card number
            CommonField cardNumberCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyCardNumber)
                .SetLabel("This is cardNumber label")
                .SetValue(cardNumber)
                .Build();

            //6 Balance 
            EditText balance = FindViewById<EditText>(Resource.Id.balanceLoyalty);
            AppendField balanceCommonField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyBalance)
                .SetLabel("Balance")
                .SetValue(balance.Text.Trim().ToString())
                .Build();
            appendFields.Add(balanceCommonField);

            //7 Number of associated coupons
            EditText relatedPassIdOne = FindViewById<EditText>(Resource.Id.relatedPassId1);
            EditText relatedPassTwo = FindViewById<EditText>(Resource.Id.relatedPassId2);
            EditText relatedPassThree = FindViewById<EditText>(Resource.Id.relatedPassId3);
            EditText relatedPassFour = FindViewById<EditText>(Resource.Id.relatedPassId4);
            List<RelatedPassInfo> relatedPassInfos = new List<RelatedPassInfo>();
            relatedPassInfos.Add(new RelatedPassInfo(relatedPassIdOne.Text.Trim().ToString(), relatedPassTwo.Text.Trim().ToString()));
            relatedPassInfos.Add(new RelatedPassInfo(relatedPassThree.Text.Trim().ToString(), relatedPassFour.Text.Trim().ToString()));
            passBuilder.AddRelatedPassIds(relatedPassInfos);

            //8 Number of loyalty points
            EditText pointsLoyalty = FindViewById<EditText>(Resource.Id.pointsLoyalty);
            AppendField pointsNubAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyPoints)
                .SetLabel("Points")
                .SetValue(pointsLoyalty.Text.Trim().ToString())
                .Build();
            appendFields.Add(pointsNubAppendField);

            //9 Barcode or QR Code
            //10 Remarks
            EditText barcodeText = FindViewById<EditText>(Resource.Id.barcodeTextLoyalty);
            EditText barcodeValue = FindViewById<EditText>(Resource.Id.barcodeValueLoyalty);
            BarCode barCode = BarCode.GetBuilder()
                .SetType(BarCode.BarcodeTypeQrCode)
                .SetValue(barcodeValue.Text.Trim().ToString())
                .SetText(barcodeText.Text.Trim().ToString())
                .Build();
            passBuilder.SetBarCode(barCode);

            //11 Member name
            CommonField memberNameCommonField = CommonField.GetBuilder()
                .SetKey(WalletPassConstant.PassCommonFieldKeyMemberName)
                .SetLabel("Member Name")
                .SetValue(memberName)
                .Build();
            commonField.Add(memberNameCommonField);
            //12	Loyalty card number ->same to 5.Card number

            //13 Loyalty level
            EditText edtLevelLoyalty = FindViewById<EditText>(Resource.Id.levelLoyalty);
            AppendField levelAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyRewardsLevel)
                .SetLabel("Tier")
                .SetValue(edtLevelLoyalty.Text.Trim().ToString())
                .Build();
            appendFields.Add(levelAppendField);

            //14 Message
            EditText messageHeader = FindViewById<EditText>(Resource.Id.messageHeader);
            EditText messageBody = FindViewById<EditText>(Resource.Id.messageBody);
            EditText messageHeaderOne = FindViewById<EditText>(Resource.Id.messageHeader1);
            EditText messageBodyOne = FindViewById<EditText>(Resource.Id.messageBody1);
            List<AppendField> messageList = new List<AppendField>();
            messageList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(messageHeader.Text.Trim().ToString())
                .SetValue(messageBody.Text.Trim().ToString())
                .Build());
            messageList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(messageHeaderOne.Text.Trim().ToString())
                .SetValue(messageBodyOne.Text.Trim().ToString())
                .Build());
            passBuilder.AddMessageList(messageList);

            //15 Scrolling Images
            //ImageUris
            EditText imageModuleDataMainUris = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUris);
            EditText imageModuleDataMainUrisDes = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUrisDes);
            EditText imageModuleDataMainUrisOne = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUris1);
            EditText imageModuleDataMainUrisDesOne = FindViewById<EditText>(Resource.Id.imageModuleDataMainImageUrisDes1);
            List<AppendField> imageList = new List<AppendField>();
            imageList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(imageModuleDataMainUrisDes.Text.Trim().ToString())
                .SetValue(imageModuleDataMainUris.Text.Trim().ToString())
                .Build());
            imageList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(imageModuleDataMainUrisDesOne.Text.Trim().ToString())
                .SetValue(imageModuleDataMainUrisOne.Text.Trim().ToString())
                .Build());
            passBuilder.AddImageList(imageList);

            //UrlList
            EditText loyaltyUrlLabel = FindViewById<EditText>(Resource.Id.loyaltyUrlLable);
            EditText loyaltyUrlValue = FindViewById<EditText>(Resource.Id.loyaltyUrlValue);
            EditText loyaltyUrlLabelOne = FindViewById<EditText>(Resource.Id.loyaltyUrlLable1);
            EditText loyaltyUrlValueOne = FindViewById<EditText>(Resource.Id.loyaltyUrlValue1);
            List<AppendField> urlList = new List<AppendField>();
            urlList.Add(AppendField.GetBuilder()
                .SetKey("1")
                .SetLabel(loyaltyUrlLabel.Text.Trim().ToString())
                .SetValue(loyaltyUrlValue.Text.Trim().ToString())
                .Build());
            urlList.Add(AppendField.GetBuilder()
                .SetKey("2")
                .SetLabel(loyaltyUrlLabelOne.Text.Trim().ToString())
                .SetValue(loyaltyUrlValueOne.Text.Trim().ToString())
                .Build());
            passBuilder.AddUrlList(urlList);

            //16 Nearby stores
            EditText nearbyLocationsLabel = FindViewById<EditText>(Resource.Id.nearbyLocationsLable);
            EditText nearbyLocationsValue = FindViewById<EditText>(Resource.Id.nearbyLocationsValue);
            string nearbyLocationsLabelStr = nearbyLocationsLabel.Text.Trim().ToString();
            string nearbyLocationsValueStr = nearbyLocationsValue.Text.Trim().ToString();
            AppendField nearbyAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyNearbyLocations)
                .SetLabel(nearbyLocationsLabelStr)
                .SetValue(nearbyLocationsValueStr)
                .Build();
            appendFields.Add(nearbyAppendField);

            //17 Main Page
            EditText webSiteLabel = FindViewById<EditText>(Resource.Id.websiteLable);
            EditText webSiteValue = FindViewById<EditText>(Resource.Id.websiteValue);
            string webSiteLabelStr = webSiteLabel.Text.Trim().ToString();
            string webSiteValueStr = webSiteValue.Text.Trim().ToString();
            AppendField mainPageAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyMainpage)
                .SetLabel(webSiteLabelStr)
                .SetValue(webSiteValueStr)
                .Build();
            appendFields.Add(mainPageAppendField);

            //18 Hotline 
            EditText hotLineLabel = FindViewById<EditText>(Resource.Id.hotlineLable);
            EditText hotLineValue = FindViewById<EditText>(Resource.Id.hotlineValue);
            string hotLineLabelStr = hotLineLabel.Text.Trim().ToString();
            string hotLineValueStr = hotLineValue.Text.Trim().ToString();
            AppendField hotLineAppendField = AppendField.GetBuilder()
                .SetKey(WalletPassConstant.PassAppendFieldKeyHotline)
                .SetLabel(hotLineLabelStr)
                .SetValue(hotLineValueStr)
                .Build();
            appendFields.Add(hotLineAppendField);

            //Latlng
            List<Location> locationList = new List<Location>();
            locationList.Add(new Location(longitudeStr, latitudeStr));
            locationList.Add(new Location(longitude1Str, latitude1Str));
            passBuilder.AddLocationList(locationList);

            //Time 
            long start = 0;
            long end = 0;
            Date date = new Date();
            if (TextUtils.IsEmpty(startTime))
            {
                Toast.MakeText(this, "Please select StartTime", ToastLength.Long).Show();
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
                Toast.MakeText(this, "Please check (EndTime > StartTime) && (EndTime > Current time)", ToastLength.Long).Show();
                return;
            }

            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.English);
            passBuilder.SetStatus(PassStatus.GetBuilder()
                .SetState(state)
                .SetEffectTime(format.Format(new Date(start)))
                .SetExpireTime(format.Format(new Date(end)))
                .Build())
                .SetOrganizationPassId(cardNumber)
                .SetPassStyleIdentifier(passStyleIdentifier.Text.Trim().ToString())
                .SetPassTypeIdentifier(typeId)
                .SetSerialNumber(serialNumber)
                .AddAppendFields(appendFields)
                .AddCommonFields(commonField);

            PassObject passObject = passBuilder.Build();
            Intent intent = new Intent(this, typeof(PassTestActivity));
            intent.PutExtra("passObject", passObject.ToJson());
            intent.PutExtra("passId", cardNumber);
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
            PassDataObjectActivity passDataObjectActivity;
            int timeType = -1;
            public OnDatePickerListener(PassDataObjectActivity passDataObjectActivity,int timeType)
            {
                this.passDataObjectActivity = passDataObjectActivity;
                this.timeType = timeType;
            }
            public void OnCancel()
            {
                Log.Info("PassDataObjectActivity", "Cancel DatePickerDialog");
            }

            public void OnConfirm(int year, int month, int dayOfMonth)
            {
                if (timeType.Equals(1))
                {
                    passDataObjectActivity.startTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }else if (timeType.Equals(2))
                {
                    passDataObjectActivity.endTime = year.ToString() + "-" + month.ToString() + "-" + dayOfMonth.ToString();
                }
                else
                {
                    Log.Info("PassDataObjectActivity", "Time type is wrong.");
                }
                
            }
        }
    }
}