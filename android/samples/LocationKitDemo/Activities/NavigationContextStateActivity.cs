using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HMSLocationKit;
using HMSSample;
using Huawei.Hms.Common;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "NavigationContextStateActivity")]
    public class NavigationContextStateActivity : Activity
    {
        #region Fields
        Logger log;

        LocationEnhanceService locationEnhanceService;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_navigation_context_state);
            InitializeViews();
            InitializeFields();
        }
        private void InitializeFields()
        {
            locationEnhanceService = LocationServices.GetLocationEnhanceService(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            FindViewById<Button>(Resource.Id.getNavigationContextState).Click += OnClickGetNavigationContextState;
        }

        private async void OnClickGetNavigationContextState(object sender, EventArgs eventArgs)
        {
            string Tag = "NavigationContextState";
            int requestType;
            if (int.TryParse(FindViewById<EditText>(Resource.Id.typeText).Text, out requestType))
            {
                NavigationRequest request = new NavigationRequest(requestType);
                var navigationResultTask = locationEnhanceService.GetNavigationStateAsync(request);
                try
                {
                    await navigationResultTask;
                    if (navigationResultTask.IsCompleted && navigationResultTask.Result != null)
                    {
                        NavigationResult result = navigationResultTask.Result;
                        log.Info(Tag, $"Get {Tag} succeeded, state: {result.State}, possibility: {result.Possibility}");
                    }
                    else log.Error(Tag, $"Get {Tag} failed: {navigationResultTask.Exception.Message}");
                }
                catch (Exception e)
                {
                    if (e is ApiException apiException)
                    {
                        if (apiException.StatusCode == 10803)
                        {
                            //PermissionDenied
                            Utils.RequestLocationPermission(this);
                        }
                    }
                    log.Error(Tag, $"Get {Tag} exception: {e.Message}");
                }
            }
        }
    }
}