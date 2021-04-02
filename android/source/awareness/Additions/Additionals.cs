/*
    Copyright 2020.Huawei Technologies Co., Ltd.All rights reserved.

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
using Android.Locations;
using Android.Runtime;
using Huawei.Hms.Kit.Awareness.Barrier;
using Huawei.Hms.Kit.Awareness.Capture;
using Huawei.Hms.Kit.Awareness.Status;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuaweiTask = Huawei.Hmf.Tasks.Task;

namespace Huawei.Hms.Kit.Awareness.Base
{
    public partial class AwarenessClient
    {
        public async Task<ApplicationStatusResponse> GetApplicationStatusAsync(string packageName) => await GetApplicationStatus(packageName).CastTask<ApplicationStatusResponse>();
        public async Task<BeaconStatusResponse> GetBeaconStatusAsync(ICollection<BeaconStatusFilter> filters) => await GetBeaconStatus(filters).CastTask<BeaconStatusResponse>();
        public async Task<BeaconStatusResponse> GetBeaconStatusAsync(params BeaconStatusFilter[] filters) => await GetBeaconStatus(filters).CastTask<BeaconStatusResponse>();
        public async Task<BluetoothStatusResponse> GetBluetoothStatusAsync(int deviceType) => await GetBluetoothStatus(deviceType).CastTask<BluetoothStatusResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesByCountryCodeAsync(string countryCode) => await GetTimeCategoriesByCountryCode(countryCode).CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesByUserAsync(double latitude, double longitude) => await GetTimeCategoriesByUser(latitude, longitude).CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesForFutureAsync(long futureTimestamp) => await GetTimeCategoriesForFuture(futureTimestamp).CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesByIPAsync() => await TimeCategoriesByIP.CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesAsync() => await TimeCategories.CastTask<TimeCategoriesResponse>();
        public async Task<BehaviorResponse> GetBehaviorAsync() => await Behavior.CastTask<BehaviorResponse>();
        public async Task<HeadsetStatusResponse> GetHeadsetStatusAsync() => await HeadsetStatus.CastTask<HeadsetStatusResponse>();
        public async Task<LocationResponse> GetLocationAsync() => await Location.CastTask<LocationResponse>();
        public async Task<LocationResponse> GetCurrentLocationAsync() => await CurrentLocation.CastTask<LocationResponse>();
        public async Task<AmbientLightResponse> GetLightIntensityAsync() => await LightIntensity.CastTask<AmbientLightResponse>();
        public async Task<WeatherStatusResponse> GetWeatherByDeviceAsync() => await WeatherByDevice.CastTask<WeatherStatusResponse>();
        public async Task<WeatherStatusResponse> GetWeatherByIPAsync() => await WeatherByIP.CastTask<WeatherStatusResponse>();
        public async Task<WeatherStatusResponse> GetWeatherByPositionAsync(WeatherPosition weatherPosition) => await GetWeatherByPosition(weatherPosition).CastTask<WeatherStatusResponse>();
        public async Task<CapabilityResponse> QuerySupportingCapabilitiesAsync() => await QuerySupportingCapabilities().CastTask<CapabilityResponse>();
        public async Task<ScreenStatusResponse> GetScreenStatusAsync() => await ScreenStatus.CastTask<ScreenStatusResponse>();
        public async Task<WifiStatusResponse> GetWifiStatusAsync() => await WifiStatus.CastTask<WifiStatusResponse>();
        public async Task<DarkModeStatusResponse> GetDarkModeStatusAsync() => await DarkModeStatus.CastTask<DarkModeStatusResponse>();
        public async Task<BarrierQueryResponse> QueryBarriersAsync(BarrierQueryRequest request) => await QueryBarriers(request).CastTask<BarrierQueryResponse>();
        public async Task UpdateBarriersAsync(BarrierUpdateRequest request) => await UpdateBarriers(request).CastTask();
        public async Task UpdateBarriersAsync(BarrierUpdateRequest request, bool autoRemove) => await UpdateBarriers(request, autoRemove).CastTask();

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
namespace Huawei.Hms.Kit.Awareness
{
    public partial interface ICaptureClient
    {
        public async Task<BeaconStatusResponse> GetBeaconStatusAsync(params BeaconStatusFilter[] filters) => await GetBeaconStatus(filters).CastTask<BeaconStatusResponse>();
        public async Task<BeaconStatusResponse> GetBeaconStatusAsync(ICollection<BeaconStatusFilter> filters) => await GetBeaconStatus(filters).CastTask<BeaconStatusResponse>();
        public async Task<BehaviorResponse> GetBehaviorAsync() => await Behavior.CastTask<BehaviorResponse>();
        public async Task<HeadsetStatusResponse> GetHeadsetStatusAsync() => await HeadsetStatus.CastTask<HeadsetStatusResponse>();
        public async Task<LocationResponse> GetLocationAsync() => await Location.CastTask<LocationResponse>();
        public async Task<LocationResponse> GetCurrentLocationAsync() => await CurrentLocation.CastTask<LocationResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesAsync() => await TimeCategories.CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesByUserAsync(double latitude, double longitude) => await GetTimeCategoriesByUser(latitude, longitude).CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesByCountryCodeAsync(string countryCode) => await GetTimeCategoriesByCountryCode(countryCode).CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesByIPAsync() => await TimeCategoriesByIP.CastTask<TimeCategoriesResponse>();
        public async Task<TimeCategoriesResponse> GetTimeCategoriesForFutureAsync(long futureTimestamp) => await GetTimeCategoriesForFuture(futureTimestamp).CastTask<TimeCategoriesResponse>();
        public async Task<AmbientLightResponse> GetLightIntensityAsync() => await LightIntensity.CastTask<AmbientLightResponse>();
        public async Task<WeatherStatusResponse> GetWeatherByDeviceAsync() => await WeatherByDevice.CastTask<WeatherStatusResponse>();
        public async Task<WeatherStatusResponse> GetWeatherByIPAsync() => await WeatherByIP.CastTask<WeatherStatusResponse>();
        public async Task<WeatherStatusResponse> GetWeatherByPositionAsync(WeatherPosition weatherPosition) => await GetWeatherByPosition(weatherPosition).CastTask<WeatherStatusResponse>();
        public async Task<BluetoothStatusResponse> GetBluetoothStatusAsync(int deviceType) => await GetBluetoothStatus(deviceType).CastTask<BluetoothStatusResponse>();
        public async Task<CapabilityResponse> QuerySupportingCapabilitiesAsync() => await QuerySupportingCapabilities().CastTask<CapabilityResponse>();
        public async Task<ScreenStatusResponse> GetScreenStatusAsync() => await ScreenStatus.CastTask<ScreenStatusResponse>();
        public async Task<WifiStatusResponse> GetWifiStatusAsync() => await WifiStatus.CastTask<WifiStatusResponse>();
        public async Task<ApplicationStatusResponse> GetApplicationStatusAsync(string packageName) => await GetApplicationStatus(packageName).CastTask<ApplicationStatusResponse>();
        public async Task<DarkModeStatusResponse> GetDarkModeStatusAsync() => await DarkModeStatus.CastTask<DarkModeStatusResponse>();
    }

    public partial interface IBarrierClient
    {
        public async Task<BarrierQueryResponse> QueryBarriersAsync(BarrierQueryRequest request) => await QueryBarriers(request).CastTask<BarrierQueryResponse>();
        public async Task UpdateBarriersAsync(BarrierUpdateRequest request) => await UpdateBarriers(request).CastTask();
        public async Task UpdateBarriersAsync(BarrierUpdateRequest request, bool autoRemove) => await UpdateBarriers(request, autoRemove).CastTask();

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