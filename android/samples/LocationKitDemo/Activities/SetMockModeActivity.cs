using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HMSSample;
using Huawei.Hms.Common;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "SetMockModeActivity")]
    public class SetMockModeActivity : Activity
    {
        #region Fields
        Logger log;
        RadioGroup radioGroup;

        bool mockFlag;
        FusedLocationProviderClient fusedLocationProviderClient;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_set_mock_mode);
            InitializeViews();
            InitializeFields();
        }

        private void InitializeFields()
        {
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            if (Logger.Instance != log)
                log.Initialize();
            FindViewById<Button>(Resource.Id.location_setMockMode).Click += OnClickSetMockMode;
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup_mockMode);
            radioGroup.CheckedChange += OnCheckedChangeRadioGroup;
        }

        private void OnCheckedChangeRadioGroup(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            //If you do not need to simulate a location, set mode to false. Otherwise, other applications cannot use the positioning function of Huawei location service.
            RadioButton radioButton = radioGroup.FindViewById<RadioButton>(e.CheckedId);
            if (radioButton.Text == "True")
                mockFlag = true;
            else mockFlag = false;
        }

        private async void OnClickSetMockMode(object sender, EventArgs eventArgs)
        {
            string Tag = "SetMockMode";
            // Note: To enable the mock function, enable the android.permission.ACCESS_MOCK_LOCATION permission in the AndroidManifest.xml file,
            // and set the application to the mock location app in the device setting.
            Task task = fusedLocationProviderClient.SetMockModeAsync(mockFlag);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, $"{Tag} is succeeded");
                else
                    log.Error(Tag, $"{Tag} failed: {task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"{Tag} exception: {e.Message}");
                if (e is ApiException apiException)
                {
                    int statuesCode = apiException.StatusCode;
                    if (statuesCode == Huawei.Hms.Common.Api.CommonStatusCodes.ResolutionRequired)
                        try
                        {
                            //When the startResolutionForResult is invoked, a dialog box is displayed, asking you to open the corresponding permission.
                            ResolvableApiException resolvableApiException = (ResolvableApiException)e;
                            resolvableApiException.StartResolutionForResult(this, 0);
                        }
                        catch (IntentSender.SendIntentException sendIntentException)
                        {
                            log.Error(Tag, "Unable to start resolution.");
                        }
                }
            }
        }
    }
}