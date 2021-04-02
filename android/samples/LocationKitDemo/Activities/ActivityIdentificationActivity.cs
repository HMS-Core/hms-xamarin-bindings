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
using System.Threading.Tasks;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "ActivityIdentificationActivity")]
    public class ActivityIdentificationActivity : Activity
    {
        #region Fields
        Logger log;
        PendingIntent pendingIntent;

        ActivityIdentificationService activityIdentificationService;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_transition_type);
            InitializeViews();
            InitializeFields();
        }

        protected override void OnDestroy()
        {
            OnClickRemoveActivityTransitionUpdate(this, null);
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
            FindViewById<Button>(Resource.Id.requestActivityTransitionUpdate).Click += OnClickRequestActivityTransitionUpdate;
            FindViewById<Button>(Resource.Id.removeActivityTransitionUpdate).Click += OnClickRemoveActivityTransitionUpdate;
        }

        private async void OnClickRequestActivityTransitionUpdate(object sender, EventArgs eventArgs)
        {
            string Tag = "RequestActivityTransitionUpdate";
            long detectionIntervalMillis = 5000;
            pendingIntent = GetPendingIntent();
            LocationBroadcastReceiver.AddIdentificationListener();
            var task = activityIdentificationService.CreateActivityIdentificationUpdatesAsync(detectionIntervalMillis, pendingIntent);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, $"CreateActivityIdentificationUpdates succeeded.");
                else
                    log.Error(Tag, $"CreateActivityIdentificationUpdates failed: {task.Exception.Message}");
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
                log.Error(Tag, $"CreateActivityIdentificationUpdates exception: {e.Message}");
            }
        }

        private async void OnClickRemoveActivityTransitionUpdate(object sender, EventArgs eventArgs)
        {
            string Tag = "RemoveActivityTransitionUpdate";
            LocationBroadcastReceiver.RemoveIdentificationListener();
            var task = activityIdentificationService.DeleteActivityIdentificationUpdatesAsync(pendingIntent);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, $"DeleteActivityIdentificationUpdates succeeded.");
                else
                    log.Error(Tag, $"DeleteActivityIdentificationUpdates failed: {task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"DeleteActivityIdentificationUpdates exception: {e.Message}");
            }
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(LocationBroadcastReceiver));
            intent.SetAction(LocationBroadcastReceiver.ActionProcessLocation);
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}