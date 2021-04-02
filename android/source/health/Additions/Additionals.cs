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

using Android.Runtime;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Hihealth.Options;
using Huawei.Hms.Hihealth.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuaweiTask = Huawei.Hmf.Tasks.Task;



namespace Huawei.Hms.Hihealth
{
    public partial class AutoRecorderController
    {

        public async Task StartRecordAsync(DataType dataType, IOnSamplePointListener listener) => 
            await StartRecord(dataType,listener).CastTask();

        public async Task StopRecordAsync(DataType dataType, IOnSamplePointListener listener) => 
            await StopRecord(dataType, listener).CastTask();

    }

    public partial class ActivityRecordsController
    {

        public async Task BeginActivityRecordAsync(ActivityRecord activityRecord) =>
            await BeginActivityRecord(activityRecord).CastTask();

        public async Task AddActivityRecordAsync(ActivityRecordInsertOptions activityRecordInsertOptions) =>
            await AddActivityRecord(activityRecordInsertOptions).CastTask();

        public async Task<IList<ActivityRecord>> EndActivityRecordAsync(string activityRecordId) =>
            await EndActivityRecord(activityRecordId).CastTask<JavaList<ActivityRecord>>();

        public async Task<ActivityRecordReply> GetActivityRecordAsync(ActivityRecordReadOptions activityRecordReadOptions) =>
           await GetActivityRecord(activityRecordReadOptions).CastTask<ActivityRecordReply>();
    }

    public partial class BleController
    {
        public async Task<bool> EndScanAsync(BleScanCallback scanCallback) =>
            (bool)await EndScan(scanCallback).CastTask<Java.Lang.Boolean>();
        public async Task<IList<BleDeviceInfo>> GetSavedDevicesAsync() =>
            await SavedDevices.CastTask<JavaList<BleDeviceInfo>>();
        public async Task BeginScanAsync(IList<DataType> dataTypes, int timeoutSecs, BleScanCallback callback) => 
            await BeginScan(dataTypes, timeoutSecs, callback).CastTask();
        public async Task DeleteDeviceAsync(BleDeviceInfo bleDeviceInfo) =>
            await DeleteDevice(bleDeviceInfo).CastTask();
        public async Task DeleteDeviceAsync(string deviceAddress) =>
            await DeleteDevice(deviceAddress).CastTask();
        public async Task SaveDeviceAsync(BleDeviceInfo bleDeviceInfo) =>
            await SaveDevice(bleDeviceInfo).CastTask();
        public async Task SaveDeviceAsync(string deviceAddress) =>
            await SaveDevice(deviceAddress).CastTask();
    }

    public partial class ConsentsController
    {
      
        public async Task<ScopeLangItem> GetAsync(string lang, string appId) =>
            await Get(lang, appId).CastTask<ScopeLangItem>();

        public async Task RevokeAsync(string appId) => 
            await Revoke(appId).CastTask();

        public async Task RevokeAsync(string appId, IList<string> scopes) => 
            await Revoke(appId, scopes).CastTask();
    }

    public partial class SettingController
    {
       
        public async Task<bool> GetHealthAppAuthorizationAsync() => 
            (bool) await HealthAppAuthorisation.CastTask<Java.Lang.Boolean>();

        public async Task<DataType> AddDataTypeAsync(DataTypeAddOptions options) =>  
            await AddDataType(options).CastTask<DataType>();

        public async Task<DataType> ReadDataTypeAsync(string dataTypeName) => 
            await ReadDataType(dataTypeName).CastTask<DataType>();

        public async Task DisableHiHealthAsync() => 
            await DisableHiHealth().CastTask();

        public async Task CheckHealthAppAuthorizationAsync() => 
            await CheckHealthAppAuthorization().CastTask();
    }

    public partial class DataController
    {

        public async Task InsertAsync(SampleSet sampleSet) => 
            await Insert(sampleSet).CastTask();

        public async Task DeleteAsync(DeleteOptions deleteOptions) => 
            await Delete(deleteOptions).CastTask();

        public async Task<ReadReply> ReadAsync(ReadOptions readOptions) =>  
            await Read(readOptions).CastTask<ReadReply>();

        public async Task<SampleSet> ReadDailySummationAsync(DataType dataType, int startTime, int endTime) =>
            await ReadDailySummation(dataType,startTime,endTime).CastTask<SampleSet>();

        public async Task<SampleSet> ReadTodaySummationAsync(DataType dataType) => 
            await ReadTodaySummation(dataType).CastTask<SampleSet>();

        public async Task UpdateAsync(UpdateOptions updateOptions) =>
            await Update(updateOptions).CastTask();

        public async Task ClearAllAsync() => 
            await ClearAll().CastTask();

       
    }


    #region Task Extensions
    /// <summary>
    /// Task Extension Class for convert HuaweiTask to System.Threading.Task
    /// </summary>
    internal static class HuaweiTaskExtensions
    {
        /// <summary>
        /// Convert HuaweiTask to System.Threading.Task without return type
        /// </summary>
        /// <param name="HuaweiTask">Com.Huawei.Hmf.Tasks.Task</param>
        /// <returns>System.Threading.Task</returns>
        public static Task CastTask(this HuaweiTask HuaweiTask)
        {
            var tcs = new TaskCompletionSource<bool>();

            HuaweiTask.AddOnCompleteListener(new HuaweiTaskCompleteListener(
                t =>
                {
                    if (t.Exception == null)
                        tcs.TrySetResult(false);
                    else
                        tcs.TrySetException(t.Exception);
                }
            ));

            return tcs.Task;
        }


        /// <summary>
        /// Convert HuaweiTask to System.Threading.Task with Generic return type
        /// </summary>
        /// <typeparam name="TResult">Return type,a Java object</typeparam>
        /// <param name="HuaweiTask">Com.Huawei.Hmf.Tasks.Task class</param>
        /// <returns>System.Threading.Task with wrapped a generic type</returns>
        public static Task<TResult> CastTask<TResult>(this HuaweiTask HuaweiTask) where TResult : Java.Lang.Object
        {
            var tcs = new TaskCompletionSource<TResult>();

            HuaweiTask.AddOnCompleteListener(new HuaweiTaskCompleteListener(
                t =>
                {
                    if (t.Exception == null)
                        tcs.TrySetResult(t.Result.JavaCast<TResult>());
                    else
                        tcs.TrySetException(t.Exception);
                }));

            return tcs.Task;
        }


        /// <summary>
        /// Modified OnCompleteListener (from Com.Huawei.Hmf.Tasks.Task)
        /// Invoke the given action
        /// </summary>
        class HuaweiTaskCompleteListener : Java.Lang.Object, Huawei.Hmf.Tasks.IOnCompleteListener
        {
            public HuaweiTaskCompleteListener(Action<HuaweiTask> onComplete)
                => OnCompleteHandler = onComplete;

            public Action<HuaweiTask> OnCompleteHandler { get; }

            public void OnComplete(HuaweiTask task)
                => OnCompleteHandler?.Invoke(task);
        }
    }
    #endregion
}