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
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Kit.Awareness;
using Huawei.Hms.Kit.Awareness.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AwarenessKitDemo.HMSSample
{
    public static class BarrierUtils
    {
        public async static void AddBarrier(Context context, string label, AwarenessBarrier barrier, PendingIntent pendingIntent)
        {
            string MethodName = "UpdateBarriers";
            BarrierUpdateRequest.Builder builder = new BarrierUpdateRequest.Builder();
            // When the status of the registered barrier changes, pendingIntent is triggered.
            // label is used to uniquely identify the barrier. You can query a barrier by label and delete it.
            BarrierUpdateRequest request = builder.AddBarrier(label, barrier, pendingIntent).Build();
            var barrierTask = Awareness.GetBarrierClient(context).UpdateBarriersAsync(request);
            try
            {
                await barrierTask;

                if (barrierTask.IsCompleted)
                {
                    string logMessage = "Add barrier success";
                    ShowToast(context, logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    string logMessage = "Add barrier failed";
                    ShowToast(context, logMessage);
                    Log.Error(MethodName, logMessage, barrierTask.Exception);
                }
            }
            catch (Exception ex)
            {
                string logMessage = "Add barrier failed";
                ShowToast(context, logMessage);
                Log.Error(MethodName, logMessage, ex);
            }
        }

        public async static void BatchBarrier(Context context, BarrierUpdateRequest.Builder builder)
        {
            string MethodName = "UpdateBarriers";
            var barrierTask = Awareness.GetBarrierClient(context).UpdateBarriersAsync(builder.Build());
            try
            {
                await barrierTask;

                if (barrierTask.IsCompleted)
                {
                    string logMessage = "Batch barrier success";
                    ShowToast(context, logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    string logMessage = "Batch barrier failed";
                    ShowToast(context, logMessage);
                    Log.Error(MethodName, logMessage, barrierTask.Exception);
                }
            }
            catch (Exception ex)
            {
                string logMessage = "Batch barrier failed";
                ShowToast(context, logMessage);
                Log.Error(MethodName, logMessage, ex);
            }
        }

        public async static void DeleteBarrier(Context context, params PendingIntent[] pendingIntents)
        {
            string MethodName = "UpdateBarriers";
            BarrierUpdateRequest.Builder builder = new BarrierUpdateRequest.Builder();
            foreach (PendingIntent pendingIntent in pendingIntents)
            {
                builder.DeleteBarrier(pendingIntent);
            }
            var barrierTask = Awareness.GetBarrierClient(context).UpdateBarriersAsync(builder.Build());
            try
            {
                await barrierTask;

                if (barrierTask.IsCompleted)
                {
                    string logMessage = "Delete barrier success";
                    ShowToast(context, logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    string logMessage = "Delete barrier failed";
                    ShowToast(context, logMessage);
                    Log.Error(MethodName, logMessage, barrierTask.Exception);
                }
            }
            catch (Exception ex)
            {
                string logMessage = "Delete barrier failed";
                ShowToast(context, logMessage);
                Log.Error(MethodName, logMessage, ex);
            }
        }

        public async static void DeleteBarrier(Context context, params string[] labels)
        {
            string MethodName = "UpdateBarriers";
            BarrierUpdateRequest.Builder builder = new BarrierUpdateRequest.Builder();
            foreach (string label in labels)
            {
                builder.DeleteBarrier(label);
            }
            var barrierTask = Awareness.GetBarrierClient(context).UpdateBarriersAsync(builder.Build());
            try
            {
                await barrierTask;

                if (barrierTask.IsCompleted)
                {
                    string logMessage = "Delete barrier success";
                    ShowToast(context, logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    string logMessage = "Delete barrier failed";
                    ShowToast(context, logMessage);
                    Log.Error(MethodName, logMessage, barrierTask.Exception);
                }
            }
            catch (Exception ex)
            {
                string logMessage = "Delete barrier failed";
                ShowToast(context, logMessage);
                Log.Error(MethodName, logMessage, ex);
            }
        }

        public async static void QueryBarrier(Context context, params string[] labels)
        {
            string MethodName = "QueryBarrier";
            BarrierQueryRequest request = BarrierQueryRequest.ForBarriers(labels);
            var barrierTask = Awareness.GetBarrierClient(context).QueryBarriersAsync(request);
            try
            {
                await barrierTask;

                if (barrierTask.IsCompleted)
                {
                    IBarrierStatusMap statusMap = barrierTask.Result.BarrierStatusMap;
                    string barrierLabel = "target barrier label";
                    BarrierStatus barrierStatus = statusMap.GetBarrierStatus(barrierLabel);
                    if (barrierStatus != null)
                    {
                        string logMessage = $"Target barrier status is {barrierStatus.PresentStatus}";
                        ShowToast(context, logMessage);
                        Log.Info(MethodName, logMessage);
                    }
                    else Log.Info(MethodName, $"{MethodName} is success but there is no barrier.");
                }
                else
                {
                    string logMessage = "Query barrier failed";
                    ShowToast(context, logMessage);
                    Log.Error(MethodName, logMessage, barrierTask.Exception);
                }
            }
            catch (Exception ex)
            {
                string logMessage = "Query barrier failed";
                ShowToast(context, logMessage);
                Log.Error(MethodName, logMessage, ex.Message);
            }
        }

        public static void ShowToast(Context context, string msg)
        {
            Toast.MakeText(context, msg, ToastLength.Short).Show();
        }
    }
}