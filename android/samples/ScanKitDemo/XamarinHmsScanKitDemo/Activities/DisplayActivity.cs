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
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Hmsscankit;
using Huawei.Hms.Ml.Scan;
using System;
using XamarinHmsScanKitDemo.Action;
using Uri = Android.Net.Uri;

namespace XamarinHmsScanKitDemo.Activities
{
    [Activity(Label = "DisplayActivity")]
    public class DisplayActivity : Activity
    {
        private ImageView backBtn;
        private Button copyButton;
        private TextView codeFormat;
        private TextView resultType;
        private TextView rawResult;
        private ImageView icon;
        private TextView iconText;
        private TextView resultTypeTitle;
        private HmsScan.WiFiConnectionInfo wiFiConnectionInfo;
        const int CalendarEvent = 0x3300;
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

            codeFormat = FindViewById<TextView>(Resource.Id.barcode_type);
            icon = FindViewById<ImageView>(Resource.Id.diplay_icon);
            iconText = FindViewById<TextView>(Resource.Id.diplay_text);
            resultType = FindViewById<TextView>(Resource.Id.barcode_type_mon);
            rawResult = FindViewById<TextView>(Resource.Id.barcode_rawValue);
            resultTypeTitle = FindViewById<TextView>(Resource.Id.barcode_type_mon_t);
            copyButton = FindViewById<Button>(Resource.Id.button_operate);


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
                //Obtain the scanning result.
                HmsScan obj = (HmsScan)Intent.GetParcelableExtra(ScanUtil.Result);
                try
                {
                    ValueFillIn(obj);
                }
                catch (Java.Lang.Exception e)
                {

                }
            }
            Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
            vibrator.Vibrate(300);

        }

        private void ValueFillIn(HmsScan hmsScan)
        {
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

            //Show the barcode result.
            if (hmsScan.ScanType == HmsScan.QrcodeScanType)
            {
                resultType.Visibility = ViewStates.Visible;
                resultTypeTitle.Visibility = ViewStates.Visible;
                if (hmsScan.ScanTypeForm == HmsScan.PureTextForm)
                {
                    icon.SetImageResource(Resource.Drawable.text);
                    iconText.Text = "Text";
                    resultType.Text = "Text";
                    copyButton.Text = GetText(Resource.String.copy);
                    copyButton.Click += (s, e) =>
                    {
                        if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                        {
                            ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                            ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                            cm.PrimaryClip = mClipData;
                            Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                        }
                    };
                }
                else if (hmsScan.ScanType == HmsScan.EventInfoForm)
                {
                    icon.SetImageResource(Resource.Drawable.@event);
                    iconText.Text = "Event";
                    resultType.Text = "Event";
                    copyButton.Text = GetText(Resource.String.add_calendar);
                    copyButton.Click += (s, e) =>
                    {
                        {
                            StartActivity(CalendarEventAction.GetCalendarEventIntent(hmsScan.GetEventInfo()));
                            Finish();
                        }
                    };
                }
                else if (hmsScan.ScanType == HmsScan.ContactDetailForm)
                {
                    icon.SetImageResource(Resource.Drawable.contact);
                    iconText.Text = "Contact";
                    resultType.Text = "Contact";
                    copyButton.Text = GetText(Resource.String.add_contact);
                    copyButton.Click += (s, e) =>
                    {
                        StartActivity(ContactInfoAction.GetContactInfoIntent(hmsScan.GetContactDetail()));
                        Finish();
                    };
                }
                else if (hmsScan.ScanType == HmsScan.DriverInfoForm)
                {
                    icon.SetImageResource(Resource.Drawable.text);
                    iconText.Text = "Text";
                    resultType.Text = "License";
                    copyButton.Text = GetText(Resource.String.copy);
                    copyButton.Click += (s, e) =>
                    {
                        if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                        {
                            ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                            ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                            cm.PrimaryClip = mClipData;
                            Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                        }

                    };
                }
                else if (hmsScan.ScanType == HmsScan.EmailContentForm)
                {
                    icon.SetImageResource(Resource.Drawable.email);
                    iconText.Text = "Email";
                    resultType.Text = "Email";
                    copyButton.Text = GetText(Resource.String.send_email);
                    copyButton.Click += (s, e) =>
                    {
                        StartActivity(Intent.CreateChooser(EmailAction.GetEmailInfo(hmsScan.GetEmailContent()), "Select email application."));
                        Finish();
                    };
                }
                else if (hmsScan.ScanType == HmsScan.LocationCoordinateForm)
                {
                    icon.SetImageResource(Resource.Drawable.location);
                    iconText.Text = "Location";
                    resultType.Text = "Location";
                    if (LocationAction.CheckMapAppExist(this))
                    {
                        copyButton.Text = GetText(Resource.String.nivigation);
                        copyButton.Click += (s, e) =>
                        {
                            try
                            {
                                StartActivity(LocationAction.GetLocationInfo(hmsScan.GetLocationCoordinate()));
                                Finish();
                            }
                            catch (Java.Lang.Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        };
                    }
                    else
                    {
                        copyButton.Text = GetText(Resource.String.copy);
                        copyButton.Click += (s, e) =>
                        {
                            if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                            {
                                ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                                ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                                cm.PrimaryClip = mClipData;
                                Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                            }
                        };
                    }
                }
                else if (hmsScan.ScanType == HmsScan.TelPhoneNumberForm)
                {
                    icon.SetImageResource(Resource.Drawable.tel);
                    iconText.Text = "Tel";
                    resultType.Text = "Tel";
                    copyButton.Text = GetText(Resource.String.call);
                    copyButton.Click += (s, e) =>
                    {
                        try
                        {
                            StartActivity(DialAction.getDialIntent(hmsScan.GetTelPhoneNumber()));
                            Finish();
                        }
                        catch (Java.Lang.Exception ex)
                        {
                           Console.WriteLine(ex);
                        }
                    };

                }
                else if (hmsScan.ScanType == HmsScan.SmsForm)
                {
                    icon.SetImageResource(Resource.Drawable.sms);
                    iconText.Text = "SMS";
                    resultType.Text = "SMS";
                    copyButton.Text = GetText(Resource.String.send_sms);
                    copyButton.Click += (s, e) =>
                    {
                        StartActivity(SmsAction.GetSMSInfo(hmsScan.GetSmsContent()));
                        Finish();
                    };
                }
                else if (hmsScan.ScanType == HmsScan.WifiConnectInfoForm)
                {
                    icon.SetImageResource(Resource.Drawable.wifi);
                    iconText.Text = "Wi-Fi";
                    resultType.Text = "Wi-Fi";
                    copyButton.Text = GetText(Resource.String.connect_network);
                    wiFiConnectionInfo = hmsScan.WifiConnectionInfo;
                    copyButton.Click += (s, e) =>
                    {
                        string permissionWifi = Manifest.Permission.AccessWifiState;
                        string permissionWifi2 = Manifest.Permission.ChangeWifiState;
                        string[] permission = new string[] { permissionWifi, permissionWifi2 };
                        ActivityCompat.RequestPermissions(this, permission, CalendarEvent);
                    };

                }
                else if (hmsScan.ScanType == HmsScan.UrlForm)
                {
                    icon.SetImageResource(Resource.Drawable.website);
                    iconText.Text = "WebSite";
                    resultType.Text = "WebSite";
                    copyButton.Text = GetText(Resource.String.open_browser);
                    copyButton.Click += (s, e) =>
                    {
                        Uri webpage = Uri.Parse(hmsScan.OriginalValue);
                        Intent intent = new Intent(Intent.ActionView, webpage);
                        if (intent.ResolveActivity(PackageManager) != null)
                        {
                            StartActivity(intent);
                        }
                    };
                    resultType.Text = "WebSite";
                }
                else
                {
                    icon.SetImageResource(Resource.Drawable.text);
                    iconText.Text = "Text";
                    resultType.Text = "Text";
                    copyButton.Text = GetText(Resource.String.copy);
                    copyButton.Click += (s, e) =>
                    {
                        if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                        {
                            ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                            ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                            cm.PrimaryClip = mClipData;
                            Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                        }
                    };
                }
            }
            else if (hmsScan.ScanType == HmsScan.Ean13ScanType)
            {
                if (hmsScan.ScanType == HmsScan.IsbnNumberForm)
                {
                    icon.SetImageResource(Resource.Drawable.isbn);
                    iconText.Text = "ISBN";
                    resultType.Visibility = ViewStates.Visible;
                    resultTypeTitle.Visibility = ViewStates.Visible;
                    resultType.Text = "ISBN";
                }
                else if (hmsScan.ScanType == HmsScan.ArticleNumberForm)
                {
                    icon.SetImageResource(Resource.Drawable.product);
                    iconText.Text = "Product";
                    resultType.Visibility = ViewStates.Visible;
                    resultTypeTitle.Visibility = ViewStates.Visible;
                    resultType.Text = "Product";
                }
                else
                {
                    icon.SetImageResource(Resource.Drawable.text);
                    iconText.Text = "Text";
                    resultType.Visibility = ViewStates.Gone;
                    resultTypeTitle.Visibility = ViewStates.Gone;
                }
                copyButton.Text = GetText(Resource.String.copy);
                copyButton.Click += (s, e) =>
                {
                    if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                    {
                        //Obtain the clipboard manager.
                        ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                        ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                        cm.PrimaryClip = mClipData;
                        Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                    }
                };
            }
            else if (hmsScan.ScanType == HmsScan.Ean8ScanType || hmsScan.ScanType == HmsScan.UpccodeAScanType
                  || hmsScan.ScanType == HmsScan.UpccodeEScanType)
            {
                if (hmsScan.ScanType == HmsScan.ArticleNumberForm)
                {
                    icon.SetImageResource(Resource.Drawable.product);
                    iconText.Text = "Product";
                    resultType.Visibility = ViewStates.Visible;
                    resultTypeTitle.Visibility = ViewStates.Visible;
                    resultType.Text = "Product";
                }
                else
                {
                    icon.SetImageResource(Resource.Drawable.text);
                    iconText.Text = "Text";
                    resultType.Visibility = ViewStates.Gone;
                    resultTypeTitle.Visibility = ViewStates.Gone;
                }
                copyButton.Text = GetText(Resource.String.copy);
                copyButton.Click += (s, e) =>
                {
                    if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                    {
                        ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                        ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                        cm.PrimaryClip = mClipData;
                        Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                    }
                };
            }
            else
            {
                icon.SetImageResource(Resource.Drawable.text);
                iconText.Text = "Text";
                copyButton.Text = GetText(Resource.String.copy);
                copyButton.Click += (s, e) =>
                {
                    if (rawResult != null && !string.IsNullOrEmpty(rawResult.Text))
                    {
                        ClipboardManager cm = (ClipboardManager)GetSystemService(Context.ClipboardService);
                        ClipData mClipData = ClipData.NewPlainText("Label", rawResult.Text);
                        cm.PrimaryClip = mClipData;
                        Toast.MakeText(this, GetText(Resource.String.copy_toast), ToastLength.Short).Show();
                    }
                };
                resultType.Visibility = ViewStates.Gone;
                resultTypeTitle.Visibility = ViewStates.Gone;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case CalendarEvent:
                    {
                        if (grantResults.Length > 1 && grantResults[0] == Android.Content.PM.Permission.Granted &&
                                grantResults[1] == Android.Content.PM.Permission.Granted)
                        {
                            if (wiFiConnectionInfo != null)
                            {
                                new WifiAdmin(this).Connect(wiFiConnectionInfo.SsidNumber,
                                        wiFiConnectionInfo.Password, wiFiConnectionInfo.CipherMode);
                                StartActivity(new Intent(Settings.ActionWifiSettings));
                                Finish();
                            }
                        }
                    }
                    break;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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