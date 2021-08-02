/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Ml.Scan;
using XamarinHmsScanKitDemo.Utils;

namespace XamarinHmsScanKitDemo.Activities
{
    [Activity(Label = "DisplayMuActivity")]
    public class DisplayMuActivity : Activity
    {
        private ImageView backBtn;
        private TextView codeFormat;
        private TextView resultType;
        private TextView rawResult;
        private LinearLayout scrollView;
        private View view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_display);
            backBtn = FindViewById<ImageView>(Resource.Id.result_back_img_in);
            backBtn.Click += (s, e) =>
            {
                Finish();
            };

            scrollView = FindViewById<LinearLayout>(Resource.Id.scroll_item);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window window = Window;
                window.DecorView.SystemUiVisibility = ((StatusBarVisibility)(SystemUiFlags.LayoutFullscreen | SystemUiFlags.LightStatusBar));
                //Set noTitleBar.
                if (window != null)
                {
                    window.AddFlags(WindowManagerFlags.TranslucentStatus | WindowManagerFlags.TranslucentNavigation);
                }
                RelativeLayout relativeLayout = FindViewById<RelativeLayout>(Resource.Id.result_title);
                if (relativeLayout != null)
                {
                    RelativeLayout.LayoutParams lp = new RelativeLayout.LayoutParams(relativeLayout.LayoutParameters.Width, relativeLayout.LayoutParameters.Height);
                    lp.SetMargins(0, GetStatusBarHeight(), 0, 0);
                    relativeLayout.LayoutParameters = lp;
                }
            }
            Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
            vibrator.Vibrate(300);
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            //Obtain the scanning result.
            IParcelable[] obj = Intent.GetParcelableArrayExtra(Constants.Result);
            if (obj != null)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    if (!string.IsNullOrEmpty(((HmsScan)obj[i]).OriginalValue))
                    {
                        view = layoutInflater.Inflate(Resource.Layout.activity_display_item, null);
                        scrollView.AddView(view);
                        ValueFillIn((HmsScan)obj[i], view);
                    }
                }
            }
            view.FindViewById(Resource.Id.line).Visibility = ViewStates.Gone;
        }


        private void ValueFillIn(HmsScan hmsScan, View view)
        {
            codeFormat = view.FindViewById<TextView>(Resource.Id.barcode_type);

            resultType = view.FindViewById<TextView>(Resource.Id.barcode_type_mon);
            rawResult = view.FindViewById<TextView>(Resource.Id.barcode_rawValue);

            rawResult.Text = hmsScan.OriginalValue;
            if (hmsScan.ScanType == HmsScan.QrcodeScanType)
            {
                codeFormat.Text = "QR code";
            }
            else if (hmsScan.ScanType == HmsScan.AztecScanType)
            {
                codeFormat.Text = "AZTEC code";
            }
            else if (hmsScan.ScanType == HmsScan.DatamatrixScanType)
            {
                codeFormat.Text = "DATAMATRIX code";
            }
            else if (hmsScan.ScanType == HmsScan.Pdf417ScanType)
            {
                codeFormat.Text = "PDF417 code";
            }
            else if (hmsScan.ScanType == HmsScan.Code93ScanType)
            {
                codeFormat.Text = "CODE93";
            }
            else if (hmsScan.ScanType == HmsScan.Code39ScanType)
            {
                codeFormat.Text = "CODE39";
            }
            else if (hmsScan.ScanType == HmsScan.Code128ScanType)
            {
                codeFormat.Text = "CODE128";

            }
            else if (hmsScan.ScanType == HmsScan.Ean13ScanType)
            {
                codeFormat.Text = "EAN13 code";

            }
            else if (hmsScan.ScanType == HmsScan.Ean8ScanType)
            {
                codeFormat.Text = "EAN8 code";

            }
            else if (hmsScan.ScanType == HmsScan.Itf14ScanType)
            {
                codeFormat.Text = "ITF14 code";

            }
            else if (hmsScan.ScanType == HmsScan.UpccodeAScanType)
            {
                codeFormat.Text = "UPCCODE_A";

            }
            else if (hmsScan.ScanType == HmsScan.UpccodeEScanType)
            {
                codeFormat.Text = "UPCCODE_E";

            }
            else if (hmsScan.ScanType == HmsScan.CodabarScanType)
            {
                codeFormat.Text = "CODABAR";
            }
            if (hmsScan.ScanType == HmsScan.QrcodeScanType)
            {
                if (hmsScan.ScanTypeForm == HmsScan.PureTextForm)
                {
                    resultType.Text = "Text";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.EventInfoForm)
                {
                    resultType.Text = "Event";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.ContactDetailForm)
                {
                    resultType.Text = "Contact";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.DriverInfoForm)
                {
                    resultType.Text = "License";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.EmailContentForm)
                {
                    resultType.Text = "Email";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.TelPhoneNumberForm)
                {
                    resultType.Text = "Tel";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.SmsForm)
                {
                    resultType.Text = "SMS";

                }
                else if (hmsScan.ScanTypeForm == HmsScan.WifiConnectInfoForm)
                {
                    resultType.Text = "Wi-Fi";

                }
                else if (hmsScan.ScanTypeForm == HmsScan.UrlForm)
                {
                    resultType.Text = "WebSite";

                }
                else if (hmsScan.ScanTypeForm == HmsScan.ArticleNumberForm)
                {
                    resultType.Text = "Product";

                }
                else
                {
                    resultType.Text = "Text";
                }
            }
            else if (hmsScan.ScanType == HmsScan.Ean13ScanType)
            {
                if (hmsScan.ScanTypeForm == HmsScan.IsbnNumberForm)
                {
                    resultType.Text = "ISBN";
                }
                else if (hmsScan.ScanTypeForm == HmsScan.ArticleNumberForm)
                {
                    resultType.Text = "Product";
                }
                else
                {
                    resultType.Text = "Text";
                }
            }
            else if (hmsScan.ScanType == HmsScan.Ean8ScanType || hmsScan.ScanType == HmsScan.UpccodeAScanType
                  || hmsScan.ScanType == HmsScan.UpccodeEScanType)
            {
                if (hmsScan.ScanTypeForm == HmsScan.ArticleNumberForm)
                {
                    resultType.Text = "Product";
                }
                else
                {
                    resultType.Text = "Text";
                }
            }
            else
            {
                resultType.Text = "Text";
            }
        }

        protected int GetStatusBarHeight()
        {
            int result = 0;
            // Obtain the ID.
            if (Resources != null)
            {
                int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
                if (resourceId > 0)
                {
                    result = Resources.GetDimensionPixelSize(resourceId);
                }
            }
            return result;
        }
    }
}