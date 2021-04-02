/*
        Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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

using Android.App;
using Android.Text;
using Huawei.Cloud.Services.Drive;
using Huawei.Hms.Support.Api.Entity.Auth;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Request;
using Huawei.Hms.Support.Hwid.Result;
using Huawei.Hms.Support.Hwid.Service;
using Java.Util.Concurrent;
using Java.Util.Concurrent.Locks;
using XHms_Drive_Kit_Demo_Project.DriveKit.Common;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Hms
{
    /// <summary>
    /// HmsProxyImpl encapsulates the entry class. 
    /// Provides a encapsulation of the HMS SDK functionality that enables developers to focus more on business processing.
    /// </summary>
    public class HmsProxyImpl
    {
        private static string Tag = "HmsProxyImpl";

        // Get AT Lock
        private ReentrantLock getATLock = new ReentrantLock();

        // Login successful result
        public static readonly int REQUEST_SIGN_IN_LOGIN = 1002;

        // HuaweiIdAuthService  object
        private IHuaweiIdAuthService service;

        // HuaweiIdAuthParams  object
        private HuaweiIdAuthParams authParams;

        // HmsProxyImpl instance
        private static HmsProxyImpl instance = new HmsProxyImpl();

        public static HmsProxyImpl Instance
        {
            get { return instance; }
        }

        // AccessToken
        private string accessToken;
        public string AccessToken
        {
            get { return accessToken; }
        }

        // UnionId
        private string unionId;
        public string UnionId
        {
            get { return unionId; }
            set { unionId = value; }
        }

        // DeviceId
        private string deviceId;
        public string DeviceId
        {
            get { return deviceId; }
        }

        // DisplayName
        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
        }

        // Status
        private int status;
        public int Status
        {
            get { return status; }
        }

        // Gender
        private int gender;
        public int Gender
        {
            get { return gender; }
        }

        // GrantedScopes
        ICollection<Scope> grantedScopes = new List<Scope>();

        // ServiceCountryCode
        private string serviceCountryCode;
        public string ServiceCountryCode
        {
            get { return serviceCountryCode; }
        }

        private HmsProxyImpl()
        {
            authParams = InitData();
        }

        /// <summary>
        /// Initialize and return the HuaweiIdSignInOptions object
        /// </summary>
        private HuaweiIdAuthParams InitData()
        {
            IList<Scope> scopeList = new List<Scope>();
            scopeList.Add(HuaweiIdAuthAPIManager.HuaweiidBaseScope);
            scopeList.Add(new Scope(DriveScopes.ScopeDrive));
            scopeList.Add(new Scope(DriveScopes.ScopeDriveFile));
            scopeList.Add(new Scope(DriveScopes.ScopeDriveMetadata));
            scopeList.Add(new Scope(DriveScopes.ScopeDriveMetadataReadonly));
            scopeList.Add(new Scope(DriveScopes.ScopeDriveReadonly));
            scopeList.Add(new Scope(DriveScopes.ScopeDriveAppdata));

            HuaweiIdAuthParams mParams = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DefaultAuthRequestParam).SetAccessToken().SetIdToken().SetScopeList(scopeList).CreateParams();

            return mParams;
        }

        /// <summary>
        /// Huawei Api Client authorized login method
        /// </summary>
        /// <param name="activity">Call the Activity page handle of the singIn interface</param>
        public void SingIn(Activity activity)
        {
            if (authParams == null)
            {
                authParams = InitData();
            }
            service = HuaweiIdAuthManager.GetService(activity, authParams);
            activity.StartActivityForResult(service.SignInIntent, REQUEST_SIGN_IN_LOGIN);
        }

        /// <summary>
        /// Synchronously acquire access token, must be called in non-main thread
        /// </summary>
        /// <returns>accessToken or null</returns>
        public string RefreshAccessToken()
        {
            Log.Logger.Info(Tag, "refreshAccessToken begin");
            try
            {
                if (service != null)
                {
                    getATLock.Lock();
                    try
                    {
                        GetAccessToken();
                    }
                    finally
                    {
                        getATLock.Unlock();
                    }
                    Log.Logger.Debug(Tag, "refreshAccessToken return new");
                }
                else
                {
                    Log.Logger.Error(Tag, "refreshAccessToken client is null, return null");
                }

            }
            catch (Exception e)
            {
                Log.Logger.Error(Tag, "refreshAccessToken exception, return null");
            }

            Log.Logger.Info(Tag, "refreshAccessToken end");
            return accessToken;
        }

        /// <summary>
        /// Get accessToken
        /// </summary>
        private void GetAccessToken()
        {
            for (int retry = 0; retry < 2; retry++)
            {
                Log.Logger.Info(Tag, "signInBackend times: " + retry);
                if (SignInBackend())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Sign in Background
        /// </summary>
        private bool SignInBackend()
        {
            Log.Logger.Info(Tag, "signInBackend");
            ClearAccountInfo();

            if (service == null)
            {
                return false;
            }

            CountDownLatch countDownLatch = new CountDownLatch(1);
            Huawei.Hmf.Tasks.Task task = service.SilentSignIn();
            task.AddOnSuccessListener(new OnSuccessListener(delegate (Java.Lang.Object authHuaweiId)
                {
                    Log.Logger.Info(Tag, "silentSignIn success");
                    DealSignInResult((AuthHuaweiId)authHuaweiId);
                    countDownLatch.CountDown();
                }
            ));

            task.AddOnFailureListener(new OnFailureListener(delegate (Java.Lang.Exception authHuaweiId)
                {
                    Log.Logger.Info(Tag, "silentSignIn error");
                    countDownLatch.CountDown();
                }
            ));

            try
            {
                countDownLatch.Await(15, TimeUnit.Seconds);
            }
            catch (Java.Lang.InterruptedException e)
            {
                Log.Logger.Info(Tag, "signInBackend catch InterruptedException");
                countDownLatch.CountDown();
            }

            if (TextUtils.IsEmpty(AccessToken))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Process certification results
        /// </summary>
        /// <param name="huaweiAccount">Huawei account</param>
        public void DealSignInResult(AuthHuaweiId huaweiAccount)
        {
            string tempAt = huaweiAccount.AccessToken;
            if (String.IsNullOrEmpty(tempAt))
            {
                Log.Logger.Error(Tag, "dealSignInResult get accessToken is null.");
                return;
            }

            Log.Logger.Info(Tag, "dealSignInResult signInBackend get new AT successfully");
            SaveAccountInfo(huaweiAccount);
        }

        /// <summary>
        /// Save account info
        /// </summary>
        private void SaveAccountInfo(AuthHuaweiId signInHuaweiId)
        {
            if (signInHuaweiId == null)
            {
                return;
            }
            unionId = signInHuaweiId.UnionId;
            deviceId = signInHuaweiId.OpenId;
            displayName = signInHuaweiId.DisplayName;
            status = signInHuaweiId.Status;
            gender = signInHuaweiId.Gender;
            grantedScopes = signInHuaweiId.AuthorizedScopes;
            serviceCountryCode = signInHuaweiId.ServiceCountryCode;
            accessToken = signInHuaweiId.AccessToken;
        }

        /// <summary>
        /// Clear account info.
        /// </summary>
        private void ClearAccountInfo()
        {
            unionId = null;
            deviceId = null;
            displayName = null;
            status = 0;
            gender = 0;
            grantedScopes = null;
            serviceCountryCode = null;
            accessToken = null;
        }

    }
}