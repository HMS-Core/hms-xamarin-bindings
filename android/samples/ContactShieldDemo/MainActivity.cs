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
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Huawei.Hms.Contactshield;
using Android.Views;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using Huawei.Hmf.Tasks;
using Java.Lang;
using Android.Util;
using File = Java.IO.File;
using System.Collections.Generic;
using Android.Content;
using XamarinHmsContactShieldDemo.Utils;
using Huawei.Agconnect.Config;
using Android.Hardware;
using System;
using XamarinHmsContactShieldDemo.Fragments;
using XamarinHmsContactShieldDemo.Interfaces;

namespace XamarinHmsContactShieldDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, CompoundButton.IOnCheckedChangeListener, IOnItemSelectedListener
    {
        private const string TAG = "ContactShield_MainActivity";
        // token
        private static string token = "TOKEN_TEST";

        private ContactShieldEngine mEngine;

        private Switch statusSwitch;
        private Button reportButton;
        private Button checkButton;
        private Button summaryButton;
        private Button detailButton;
        private Button windowModeButton;
        private Button clearAllDataButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (ActivityCompat.CheckSelfPermission(this,
                Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessFineLocation },
                        940);
            }
            if (ActivityCompat.CheckSelfPermission(this,
                    Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation },
                        940);
            }
            if (ActivityCompat.CheckSelfPermission(this,
                    Manifest.Permission.Bluetooth) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Bluetooth },
                        940);
            }
            if (ActivityCompat.CheckSelfPermission(this,
                    Manifest.Permission.BluetoothAdmin) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.BluetoothAdmin },
                        940);
            }
            if (ActivityCompat.CheckSelfPermission(this,
                    Manifest.Permission.Internet) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Internet },
                        940);
            }

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            InitView();
            //Gets ContactShieldEngine.
            mEngine = ContactShield.GetContactShieldEngine(this);

            //Run CheckContactShieldRunning task method.
            System.Threading.Tasks.Task.Run(() => CheckContactShieldRunning());


        }
        private void CheckContactShieldRunning()
        {
            System.Threading.Tasks.Task.Run(async () =>
            {
                var resultIsRunning = mEngine.IsContactShieldRunningAsync();
                bool isContactShieldRun = await resultIsRunning;
                if (resultIsRunning.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                {
                    Log.Debug(TAG, "IsContactShieldRunning: " + isContactShieldRun.ToString());
                }
                RunOnUiThread(() =>
                {

                });
            }).Wait();
            Log.Debug(TAG, "Done w/ CheckContactShieldRunning");
        }
        private void InitView()
        {
            statusSwitch = FindViewById<Switch>(Resource.Id.swContactShieldStatus);
            statusSwitch.SetOnCheckedChangeListener(this);

            reportButton = FindViewById<Button>(Resource.Id.btnReport);
            reportButton.Click += ReportButton_Click;

            checkButton = FindViewById<Button>(Resource.Id.btnCheck);
            checkButton.Click += CheckButton_Click;

            summaryButton = FindViewById<Button>(Resource.Id.btnSummary);
            summaryButton.Click += SummaryButton_Click;

            detailButton = FindViewById<Button>(Resource.Id.btnDetail);
            detailButton.Click += DetailButton_Click;

            windowModeButton = FindViewById<Button>(Resource.Id.btnWindowMode);
            windowModeButton.Click += WindowModeButton_Click;

            clearAllDataButton = FindViewById<Button>(Resource.Id.btnClearAllData);
            clearAllDataButton.Click += ClearAllDataButton_Click;
        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            StartContactControl(isChecked);
        }
        private void ClearAllDataButton_Click(object sender, EventArgs e)
        {
            ClearAllData();
        }

        private void WindowModeButton_Click(object sender, EventArgs e)
        {
            GetWindowModeDetails();
        }

        private void DetailButton_Click(object sender, EventArgs e)
        {
            GetDetailButtonOnClick();
        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {
            GetSketchButtonOnClick();
        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            PutKeysButtonOnClick();
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportPeriodicKeys();
        }
        private async void StartContactControl(bool onOff)
        {
            if (onOff)
            {
                BottomDialogFragment bottomDialogFragment = BottomDialogFragment.newInstance();
                bottomDialogFragment.Show(SupportFragmentManager, "bottom_dialog_fragment");
            }
            else
            {
                //Stop Contact Shield asynchronously.
                var resultStopContact = mEngine.StopContactShieldAsync();
                await resultStopContact;
                if (resultStopContact.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                {
                    Log.Debug(TAG, "ContactShield stopped.");
                }
            }
        }

        public async void ReportPeriodicKeys()
        {
            //Gets periodic keys.
            var resultPeriodicKey = mEngine.GetPeriodicKeyAsync();
            IList<PeriodicKey> periodicKeys = await resultPeriodicKey;
            if (resultPeriodicKey.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
            {
                Log.Debug(TAG, periodicKeys.Count.ToString());
                foreach (var items in periodicKeys)
                {
                    PeriodicKey periodicKey = items;
                    Log.Debug(TAG, periodicKey.PeriodicKeyLifeTime.ToString());
                }
            }
        }

        public async void PutKeysButtonOnClick()
        {
            File directory;
            // Shared key list file, Please configure according to the actual situation
            if (Android.OS.Environment.ExternalStorageState == "mounted")
            {
                //create new file directory object
                directory = FilesDir;

                if (directory.Exists())
                {
                    Toast.MakeText(this, "File exist.", ToastLength.Short).Show();
                }
                string fileName = directory.ToString() + "/putkeysfile.zip";

                File file = new Java.IO.File(fileName);
                if (!file.Exists())
                {
                    file.CreateNewFile();
                }
                else
                {
                    List<File> putList = new List<File>();
                    putList.Add(file);
                    DiagnosisConfiguration diagnosisConfiguration = new DiagnosisConfiguration.Builder().Build();

                    PendingIntent pendingIntent = PendingIntent.GetService(this, 0, new Intent(this, typeof(BackgroundContactCheckingIntentService)), PendingIntentFlags.UpdateCurrent);

                    //Put shared key files to diagnosisconfiguration.
                    var resultPutSharedKeys = mEngine.PutSharedKeyFilesAsync(pendingIntent, putList, diagnosisConfiguration, token);
                    await resultPutSharedKeys;
                    if (resultPutSharedKeys.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                    {
                        Log.Debug(TAG, "PutSharedKeyFiles succeeded.");
                    }
                }
            }
        }
        private async void ClearAllData()
        {
            //Clear asynchronously all data.
            var resultClearData = mEngine.ClearDataAsync();
            await resultClearData;
            if (resultClearData.IsCompleted)
            {
                Log.Debug(TAG, "All data has been cleared.");
            }
        }
        private async void GetWindowModeDetails()
        {
            switch (token)
            {
                case "TOKEN_WINDOW_MODE":
                    //Gets Contact details in Window Mode.
                    var resultContactWindow = mEngine.GetContactWindowAsync(token);
                    IList<ContactWindow> contactWindow = await resultContactWindow;
                    if (resultContactWindow.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                    {
                        Log.Debug(TAG, "ContactWindow: " + contactWindow.ToString());
                    }
                    break;
                default:
                    //Gets Contact Shield Summary.
                    var resultSketch = mEngine.GetContactSketchAsync(token);
                    ContactSketch contactSketch = await resultSketch;
                    if (resultSketch.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                    {
                        Log.Debug(TAG, "ContactSketch : " + contactSketch.ToString());
                    }
                    break;
            }
        }

        public async void GetSketchButtonOnClick()
        {
            //Gets Contact Shield Summary.
            var resultSketch = mEngine.GetContactSketchAsync(token);
            ContactSketch contactSketch = await resultSketch;
            if (resultSketch.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
            {
                Log.Debug(TAG, "ContactSketch : " + contactSketch.ToString());
            }
        }

        public async void GetDetailButtonOnClick()
        {
            //Gets Contact details.
            var resultGetContactDetail = mEngine.GetContactDetailAsync(token);
            IList<ContactDetail> contactDetails = await resultGetContactDetail;
            if (resultGetContactDetail.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
            {
                foreach (var items in contactDetails)
                {
                    Log.Debug(TAG, "ContactDetail: " + items.TotalRiskValue);
                }
            }
        }

        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(context);
            AGConnectServicesConfig config = AGConnectServicesConfig.FromContext(context);
            config.OverlayWith(new HmsLazyInputStream(context));
        }

        public async void OnItemSelected(int itemId)
        {
            switch (itemId)
            {
                case Resource.Id.btnStartNormalContactShield:
                    //Start Contact Shield in default settings.
                    var resultStartContactShield = mEngine.StartContactShieldAsync(ContactShieldSetting.Default);
                    try
                    {
                        await resultStartContactShield;
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        throw;
                    }
                    
                    if (resultStartContactShield.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                    {
                        Log.Debug(TAG, "StartContactShield succeed.");
                    }
                    break;
                case Resource.Id.btnStartNoPersistentContactShield:
                    //Start Contact Shield with no persistent.
                    var resultStartNoPersistentConShield = mEngine.StartContactShieldNoPersistentAsync(ContactShieldSetting.Default);
                    await resultStartNoPersistentConShield;
                    if (resultStartNoPersistentConShield.Status.Equals(System.Threading.Tasks.TaskStatus.RanToCompletion))
                    {
                        Log.Debug(TAG, "StartContactShieldNoPersistent succeed.");
                    }
                    break;
                case Resource.Id.btnDismissDialog:
                    statusSwitch.Checked = false;
                    break;
                default:
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}