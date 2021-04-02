using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using HMSLocationKit;
using HMSSample;
using Huawei.Hms.Common;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "ActivityConversionActivity")]
    public class ActivityConversionActivity : Activity
    {
        #region Fields
        Logger log;
        PendingIntent pendingIntent;

        ActivityIdentificationService activityIdentificationService;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_transition);
            InitializeViews();
            InitializeFields();
        }

        protected override void OnDestroy()
        {
            OnClickRemoveActivityTransition(this, null);
            base.OnDestroy();
        }

        private void InitializeFields()
        {
            activityIdentificationService = ActivityIdentification.GetService(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            if (Logger.Instance != log)
                log.Initialize();
            FindViewById<Button>(Resource.Id.btn_request_activity_transition).Click += OnClickRequestActivityTransition;
            FindViewById<Button>(Resource.Id.btn_remove_activity_transition).Click += OnClickRemoveActivityTransition;
        }

        private async void OnClickRequestActivityTransition(object sender, EventArgs eventArgs)
        {
            string Tag = "CreateActivityConversionUpdates";
            List<ActivityConversionInfo> transitions = GetTransitions();

            LocationBroadcastReceiver.AddConversionListener();
            pendingIntent = GetPendingIntent();
            ActivityConversionRequest activityTransitionRequest = new ActivityConversionRequest(transitions);
            var task = activityIdentificationService.CreateActivityConversionUpdatesAsync(activityTransitionRequest, pendingIntent);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, $"CreateActivityConversionUpdates succeeded.");
                else
                    log.Error(Tag, $"CreateActivityConversionUpdates failed: {task.Exception.Message}");
            }
            catch (Exception e)
            {
                if (e is ApiException apiException)
                {
                    if (apiException.StatusCode == 10803)
                    {
                        //PermissionDenied
                        Utils.RequestActivityTransitionPermission(this);
                    }
                }
                log.Error(Tag, $"CreateActivityConversionUpdates exception: {e.Message}");
            }
        }


        private async void OnClickRemoveActivityTransition(object sender, EventArgs eventArgs)
        {
            string Tag = "RemoveActivityTransitionUpdate";
            LocationBroadcastReceiver.RemoveConversionListener();
            var task = activityIdentificationService.DeleteActivityConversionUpdatesAsync(pendingIntent);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, $"DeleteActivityConversionUpdates succeeded.");
                else
                    log.Error(Tag, $"DeleteActivityConversionUpdates failed: {task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"DeleteActivityConversionUpdates exception: {e.Message}");
            }
        }

        private List<ActivityConversionInfo> GetTransitions()
        {
            List<ActivityConversionInfo> result = new List<ActivityConversionInfo>();

            ActivityConversionInfo.Builder builder = new ActivityConversionInfo.Builder();

            if (FindViewById<CheckBox>(Resource.Id.IN_VEHICLE_IN).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Vehicle)
                    .SetConversionType(ActivityConversionInfo.EnterActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.IN_VEHICLE_OUT).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Vehicle)
                    .SetConversionType(ActivityConversionInfo.ExitActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.ON_BICYCLE_IN).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Bike)
                    .SetConversionType(ActivityConversionInfo.EnterActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.ON_BICYCLE_OUT).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Bike)
                    .SetConversionType(ActivityConversionInfo.ExitActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.ON_FOOT_IN).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Foot)
                    .SetConversionType(ActivityConversionInfo.EnterActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.ON_FOOT_OUT).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Foot)
                    .SetConversionType(ActivityConversionInfo.ExitActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.STILL_IN).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Still)
                    .SetConversionType(ActivityConversionInfo.EnterActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.STILL_OUT).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Still)
                    .SetConversionType(ActivityConversionInfo.ExitActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.WALKING_IN).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Walking)
                    .SetConversionType(ActivityConversionInfo.EnterActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.WALKING_OUT).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Walking)
                    .SetConversionType(ActivityConversionInfo.ExitActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.RUNNING_IN).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Running)
                    .SetConversionType(ActivityConversionInfo.EnterActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            if (FindViewById<CheckBox>(Resource.Id.RUNNING_OUT).Checked)
            {
                builder
                    .SetActivityType(ActivityIdentificationData.Running)
                    .SetConversionType(ActivityConversionInfo.ExitActivityConversion);
                ActivityConversionInfo activityConversionInfo = builder.Build();
                result.Add(activityConversionInfo);
            }
            return result;
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(LocationBroadcastReceiver));
            intent.SetAction(LocationBroadcastReceiver.ActionProcessLocation);
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}