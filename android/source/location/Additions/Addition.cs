/*      
 *      Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuaweiTask = Huawei.Hmf.Tasks.Task;
using AndroidLocation = global::Android.Locations.Location;

namespace Huawei.Hms.Location
{
    public partial class LocationSettingsStates
    {
        public bool HMSLocationUsable { get => IsHMSLocationUsable; set => SetHmsLocationUsable(value); }
    }
    public partial class ActivityIdentificationService
    {
        public async Task DeleteActivityConversionUpdatesAsync(PendingIntent pendingIntent) => await DeleteActivityConversionUpdates(pendingIntent).CastTask();
        public async Task DeleteActivityIdentificationUpdatesAsync(PendingIntent callbackIntent) => await DeleteActivityIdentificationUpdates(callbackIntent).CastTask();
        public async Task CreateActivityConversionUpdatesAsync(ActivityConversionRequest activityConversionRequest, PendingIntent pendingIntent) => await CreateActivityConversionUpdates(activityConversionRequest, pendingIntent).CastTask();
        public async Task CreateActivityIdentificationUpdatesAsync(long intervalMillis, PendingIntent callbackIntent) => await CreateActivityIdentificationUpdates(intervalMillis, callbackIntent).CastTask();
    }
    public partial class FusedLocationProviderClient
    {
        public async Task FlushLocationsAsync() => await FlushLocations().CastTask();
        public async Task<AndroidLocation> GetLastLocationAsync() => await LastLocation.CastTask<AndroidLocation>();
        public async Task<LocationAvailability> GetLocationAvailabilityAsync() => await LocationAvailability.CastTask<LocationAvailability>();
        public async Task RemoveLocationUpdatesAsync(LocationCallback callback) => await RemoveLocationUpdates(callback).CastTask();
        public async Task RemoveLocationUpdatesAsync(PendingIntent callbackIntent) => await RemoveLocationUpdates(callbackIntent).CastTask();
        public async Task RequestLocationUpdatesAsync(LocationRequest request, LocationCallback callback, Looper looper) => await RequestLocationUpdates(request, callback, looper).CastTask();
        public async Task RequestLocationUpdatesAsync(LocationRequest request, PendingIntent callbackIntent) => await RequestLocationUpdates(request, callbackIntent).CastTask();
        public async Task SetMockModeAsync(bool isMockMode) => await SetMockMode(isMockMode).CastTask();
        public async Task SetMockLocationAsync(AndroidLocation mockLocation) => await SetMockLocation(mockLocation).CastTask();
        public async Task<HWLocation> GetLastLocationWithAddressAsync(LocationRequest request) => await GetLastLocationWithAddress(request).CastTask<HWLocation>();
        public async Task RequestLocationUpdatesExAsync(LocationRequest request, LocationCallback callback, Looper looper) => await RequestLocationUpdatesEx(request, callback, looper).CastTask();
    }
    public partial class GeofenceService
    {
        public async Task CreateGeofenceListAsync(GeofenceRequest geofenceRequest, PendingIntent pendingIntent) => await CreateGeofenceList(geofenceRequest, pendingIntent).CastTask();
        public async Task DeleteGeofenceListAsync(List<string> geofenceRequestIds) => await DeleteGeofenceList(geofenceRequestIds).CastTask();
        public async Task DeleteGeofenceListAsync(PendingIntent pendingIntent) => await DeleteGeofenceList(pendingIntent).CastTask();
    }
    public partial class LocationEnhanceService
    {
        public async Task<NavigationResult> GetNavigationStateAsync(NavigationRequest request) => await GetNavigationState(request).CastTask<NavigationResult>();
    }
    public partial class SettingsClient
    {
        public async Task<LocationSettingsResponse> CheckLocationSettingsAsync(LocationSettingsRequest locationSettingsRequest) => await CheckLocationSettings(locationSettingsRequest).CastTask<LocationSettingsResponse>();
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
        /// <param name="HuaweiTask">Huawei.Hmf.Tasks.Task</param>
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
        /// <param name="HuaweiTask">Huawei.Hmf.Tasks.Task class</param>
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
        /// Modified OnCompleteListener (from Huawei.Hmf.Tasks.Task)
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