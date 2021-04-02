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
using Android.Content;
using Android.OS;
using Android.Runtime;
using Huawei.Hms.Hihealth;
using Android.Util;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Hihealth.Options;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Java.Lang;
using Android.Graphics;
using Huawei.Hms.Support.Hwid.Result;
using Huawei.Hms.Support.Hwid;

namespace XamarinHmsHealthDemo
{
    [Service(Exported = true, Name = "com.xamarin.healthkitdemo.PersistService")]
    [IntentFilter(new string[] { "HealthKitService" })]
    class PersistService : Service
    {
        private static string TAG = "PersistService";

        // HMS Health AutoRecorderController
        private AutoRecorderController MyAutoRecorderController;

        public static Context context;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            context = this;
            InitAutoRecorderController();
            Log.Info (TAG, "service is created");
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // Invoke the real-time callback interface of the HealthKit.
            GetRemoteService();

            // Binding a notification bar
            GetNotification();

            return base.OnStartCommand(intent, flags, startId);
        }
        private void InitAutoRecorderController()
        {
            HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
            AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
            MyAutoRecorderController = HuaweiHiHealth.GetAutoRecorderController(this, signInHuaweiId);
        }

        private async void GetRemoteService()
        {
            if (MyAutoRecorderController == null)
            {
                InitAutoRecorderController();
            }
            // Start recording real-time steps.
            Task StartTask= MyAutoRecorderController.StartRecordAsync(Huawei.Hms.Hihealth.Data.DataType.DtContinuousStepsTotal,  new MySamplePointListener());
           
            try
            {
                await StartTask;

                if (StartTask.IsCompleted)
                {
                    if (StartTask.Exception == null)
                    {
                        Log.Info(TAG, "record steps success... ");
                    }
                    else
                    {
                        Log.Info(TAG, "report steps failed... " + StartTask.Exception);
                    }
                }

            }
            catch (System.Exception ex)
            {
                Log.Info(TAG, "report steps failed... " + ex.Message);
            }
        }

        private void GetNotification()
        {
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            Notification notification = new NotificationCompat.Builder(this, "1")
                .SetContentTitle("Real-time step counting")
                .SetContentText("Real-time step counting...")
                .SetWhen(JavaSystem.CurrentTimeMillis())
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Mipmap.ic_launcher))
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetContentIntent(PendingIntent.GetActivity(this, 0, new Intent(this, typeof(AutoRecorderController)), 0))
                .Build();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel("1", "subscribeName", NotificationImportance.Default);
                channel.Description = "description";
                notificationManager.CreateNotificationChannel(channel);
            }
            notification.Flags = NotificationFlags.OngoingEvent;
            StartForeground(1, notification);
        }

       
    }
    public class MySamplePointListener : Java.Lang.Object, IOnSamplePointListener
    {

        public void OnSamplePoint(SamplePoint samplePoint)
        {
            // The step count, time, and type data reported by the pedometer is called back to the app through
            // samplePoint.
            Intent intent = new Intent();
            intent.PutExtra("SamplePoint", samplePoint);
            intent.SetAction("HealthKitService");
            // Transmits service data to activities through broadcast.
            PersistService.context.SendBroadcast(intent);
        }

    }

}