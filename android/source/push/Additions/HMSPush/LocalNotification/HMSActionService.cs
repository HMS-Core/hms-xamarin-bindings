using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huawei.Hms.Push.LocalNotification
{
    [Service]
    public class HMSPushActionIntentService : IntentService
    {
        readonly string Tag = "HMSPushActionIntentService";
        public HMSPushActionIntentService() : base("HMSPushActionIntentService")
        {
            Log.Info(Tag, $"{Tag} has started");
        }
        protected override void OnHandleIntent(Intent intent)
        {
            Log.Info(Tag, intent.ToString());
        }
    }
    public class HMSActionService : Service
    {
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            OnClickAction(intent);
            return base.OnStartCommand(intent, flags, startId);
        }

        public virtual void OnClickAction(Intent notification) { }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}