/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Huawei.Android.Hms.Ppskit;
using Org.Json;

namespace XamarinAdsInstallReferrerDemo.InstallReferrer
{
    /// <summary>
    /// get install referrer by AIDL mode
    /// </summary>
    public class InstallReferrerAidUtil
    {
        private static readonly string TAG = "InstallReferrerAidlUtil";
        private Context context;
        private IServiceConnection serviceConnection;
        private IPPSChannelInfoService service;
        private InstallReferrerCallback callback;

        public InstallReferrerAidUtil(Context context)
        {
            this.context = context;
        }
        private void UnBindService()
        {
            Log.Info(TAG, "unbindService");
            if (null == context)
            {
                Log.Error(TAG, "context is null");
                return;
            }
            if (null != serviceConnection)
            {
                // Unbind HUAWEI Ads kit
                context.UnbindService(serviceConnection);
                service = null;
                context = null;
                callback = null;
            }
        }
        private bool BindService()
        {
            Log.Info(TAG, "bindService");
            if (context == null)
            {
                Log.Error(TAG, "context is null");
                return false;
            }
            serviceConnection = new InstallReferrerServiceConnection(this);
            Intent intent = new Intent(Constants.ServiceAction);
            intent.SetPackage(Constants.TestServicePackageName);
            // Bind HUAWEI Ads kit
            bool result = context.BindService(intent, serviceConnection, Bind.AutoCreate);
            Log.Info(TAG, "BindService result: " + result);
            return result;
        }


        public void GetInstallReferrer(InstallReferrerCallback callback)
        {
            this.callback = callback;
            BindService();
        }

        private class InstallReferrerServiceConnection : Java.Lang.Object, IServiceConnection
        {
            InstallReferrerAidUtil referrerAidUtil;
            private InstallReferrerServiceConnection()
            {

            }
            public InstallReferrerServiceConnection(InstallReferrerAidUtil referrerAidUtil)
            {
                this.referrerAidUtil = referrerAidUtil;
            }
            public bool IsConnected { get; private set; }
            public PpsChannelInfoBinder Binder { get; private set; }
            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                Log.Info(TAG, "onServiceConnected");
               
                    referrerAidUtil.service = IPPSChannelInfoServiceStub.AsInterface(service);
                    if (referrerAidUtil.service != null)
                    {
                        try
                        {
                            // Get channel info（Json format）
                            string channelJson = referrerAidUtil.service.GetChannelInfo();
                            Log.Info(TAG, "IPPSChannelInfoService.GetChannelInfo: " + channelJson);
                            // Parser
                            JSONObject jsonObject = new JSONObject(channelJson);
                            // Get install referrer
                            string installReferrer = jsonObject.OptString("channelInfo");
                            long clickTimestamp = jsonObject.OptLong("clickTimestamp", 0);
                            long installTimestamp = jsonObject.OptLong("installTimestamp", 0);
                            if (null != referrerAidUtil.callback)
                            {
                                // Update install referrer details.
                                referrerAidUtil.callback.OnSuccess(installReferrer, clickTimestamp, installTimestamp);
                            }
                            else
                            {
                                referrerAidUtil.callback.OnFail("install referrer is empty.");
                            }
                        }
                        catch (RemoteException remoteEx)
                        {
                            Log.Error(TAG, "ChannelInfo RemoteException");
                            referrerAidUtil.callback.OnFail(remoteEx.Message);
                        }
                        catch (Exception e)
                        {
                            Log.Error(TAG, "ChannelInfo Exception");
                            referrerAidUtil.callback.OnFail(e.Message);

                        }
                        finally
                        {
                            referrerAidUtil.UnBindService();
                        }
                    }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                Log.Info(TAG, "OnServiceDisconnected");
                IsConnected = false;
                Binder = null;
                referrerAidUtil.service = null;
            }
        }
    }
}