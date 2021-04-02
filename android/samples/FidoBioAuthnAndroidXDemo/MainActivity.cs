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
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Security.Keystore;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Fido_BioAuthn_AndroidX.HMSSample;
using Huawei.Hms.Support.Api.Fido.Bioauthn;
using Java.IO;
using Java.Lang;
using Java.Security;
using Java.Security.Cert;
using Javax.Crypto;
using System;

namespace Fido_BioAuthn_AndroidX
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        // In the scenario where BioAuthnPrompt.PromptInfo.Builder.SetDeviceCredentialAllowed(true) is used in EMUI 9.x or
        // earlier, fingerprint authentication may work once only. You can solve this problem in one of the following ways:
        // 1. The activity has only one singleton BioAuthnPrompt object. Do not create the object repeatedly.
        // 2. After the authentication is complete, call the Recreate() method of the activity.
        // 3. Close the activity and open it again.
        private BioAuthnPrompt bioAuthnPrompt;

        Logger log;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SupportRequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_main);
            InitiateViews();
        }
        private void InitiateViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            bioAuthnPrompt = CreateBioAuthnPrompt();
            FindViewById<Button>(Resource.Id.btn_text_face_auth_with_crpObj).Click += OnClickFaceAuthWithCrpObj;
            FindViewById<Button>(Resource.Id.btn_text_finger_auth_with_crpObj).Click += OnClickFingerAuthWithCrpObj;
            FindViewById<Button>(Resource.Id.btn_text_finger_auth_without_crpObj).Click += OnClickFingerAuthWithoutCrpObj;
        }

        /// <summary>
        /// Shows the fingerprint prompt without CryptoObject and allows the user to use the device PIN and password authentication.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFingerAuthWithoutCrpObj(object sender, EventArgs e)
        {
            string Tag = "OnClickFingerAuthWithoutCrpObj";

            // Checks whether fingerprint authentication is available.
            BioAuthnManager bioAuthnManager = new BioAuthnManager(this);
            int errorCode = bioAuthnManager.CanAuth();
            if (errorCode != 0)
            {
                log.Info(Tag, "Can not authenticate. errorCode=" + errorCode);
                return;
            }

            // build prompt info
            BioAuthnPrompt.PromptInfo.Builder builder =
                new BioAuthnPrompt.PromptInfo.Builder().SetTitle("This is the title.")
                    .SetSubtitle("This is the subtitle")
                    .SetDescription("This is the description");

            // The user will first be prompted to authenticate with biometrics, but also given the option to
            // authenticate with their device PIN, pattern, or password. SetNegativeButtonText(CharSequence) should
            // not be set if this is set to true.
            builder.SetDeviceCredentialAllowed(true);

            // Set the text for the negative button. SetDeviceCredentialAllowed(true) should not be set if this button text
            // is set.
            // builder.SetNegativeButtonText("This is the 'Cancel' button.");

            BioAuthnPrompt.PromptInfo info = builder.Build();

            log.Info(Tag, "Start fingerprint authentication without CryptoObject.\nAuthenticating......\n");
            bioAuthnPrompt.Auth(info);
        }

        /// <summary>
        /// Shows the fingerprint prompt with CryptoObject.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFingerAuthWithCrpObj(object sender, EventArgs e)
        {
            string Tag = "FingerAuthWithCrpObj";

            BioAuthnManager bioAuthnManager = new BioAuthnManager(this);
            int errorCode = bioAuthnManager.CanAuth();

            if (errorCode != 0)
            {
                log.Info(Tag, "Can not authenticate. errorCode=" + errorCode);
                return;
            }

            // build prompt info
            BioAuthnPrompt.PromptInfo.Builder builder =
                new BioAuthnPrompt.PromptInfo.Builder().SetTitle("This is the title.")
                    .SetSubtitle("This is the subtitle.")
                    .SetDescription("This is the description.");

            // The user will first be prompted to authenticate with biometrics, but also given the option to
            // authenticate with their device PIN, pattern, or password. SetNegativeButtonText(string) should
            // not be set if this is set to true.
            // builder.SetDeviceCredentialAllowed(true);

            // Set the text for the negative button. SetDeviceCredentialAllowed(true) should not be set if this
            // button text is set.
            builder.SetNegativeButtonText("This is the 'Cancel' button.");

            BioAuthnPrompt.PromptInfo info = builder.Build();

            // Construct CryptoObject.
            Cipher cipher = new HwBioAuthnCipherFactory("hw_test_fingerprint", true).GetCipher();
            if (cipher == null)
            {
                log.Info(Tag, "Failed to create Cipher object.");
                return;
            }
            CryptoObject crypto = new CryptoObject(cipher);

            log.Info(Tag, "Start fingerprint authentication with CryptoObject.\nAuthenticating......\n");

            // When user CryptoObject, BiometricPrompt.PromptInfo.Builder.SetDeviceCredentialAllowed(true) is not allow.
            // Call BiometricPrompt.Authenticate(BiometricPrompt.PromptInfo info) if
            // BiometricPrompt.PromptInfo.Builder.SetDeviceCredentialAllowed(true) is set to true.
            Log.Info("CanAuth",bioAuthnManager.CanAuth().ToString());
            bioAuthnPrompt.Auth(info, crypto);
            Thread.Sleep(10000);
            bioAuthnPrompt.CancelAuth();
        }

        private void OnClickFaceAuthWithCrpObj(object sender, EventArgs e)
        {
            string Tag = "FaceAuthWithCrpObj";
            Android.Content.PM.Permission permissionCheck = Android.Content.PM.Permission.Granted;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                permissionCheck = CheckSelfPermission(Manifest.Permission.Camera);
            }
            if (permissionCheck != Android.Content.PM.Permission.Granted)
            {
                log.Info(Tag, "The camera permission is not enabled. Please enable it.");

                // request camera permissions
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    RequestPermissions(new string[] { Manifest.Permission.Camera }, 1);
                }
                return;
            }
            BioAuthnCallback callback = new FidoBioAuthnCallback();
            // Cancellation Signal
            CancellationSignal cancellationSignal = new CancellationSignal();

            FaceManager faceManager = new FaceManager(this);
            Log.Info("HasEnrolledTemplates", faceManager.HasEnrolledTemplates.ToString());
            Log.Info("IsHardwareDetected", faceManager.IsHardwareDetected.ToString());
            Log.Info("CanAuth", faceManager.CanAuth().ToString());

            log.Info(Tag, $"IsHardwareDetected:{faceManager.IsHardwareDetected}");

            // Checks whether 3D facial authentication can be used.
            int errorCode = faceManager.CanAuth();
            if (errorCode != 0)
            {
                log.Info(Tag, "Can not authenticate. errorCode=" + errorCode);
                return;
            }
            // flags
            int flags = 0;

            // Authentication messsage handler.
            Handler handler = null;

            // Recommended CryptoObject to be set to null. KeyStore is not associated with face authentication in current
            // version. KeyGenParameterSpec.Builder.SetUserAuthenticationRequired() must be set false in this scenario.
            CryptoObject crypto = null;

            log.Info(Tag, "Start face authentication.\nAuthenticating......\n");
            faceManager.Auth(crypto, cancellationSignal, flags, callback, handler);
        }

        private BioAuthnPrompt CreateBioAuthnPrompt()
        {
            BioAuthnCallback callback = new FidoBioAuthnCallback();

            return new BioAuthnPrompt(this, ContextCompat.GetMainExecutor(this), callback);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
    public class FidoBioAuthnCallback : BioAuthnCallback
    {
        public override void OnAuthError(int errMsgId, ICharSequence errString)
        {
            Log.Error("OnAuthError", $"Authentication error. errorCode={errMsgId},errorMessage={errString}");
        }
        public override void OnAuthFailed()
        {
            Log.Error("OnAuthFailed", "Authentication failed.");
        }
        public override void OnAuthSucceeded(BioAuthnResult result)
        {
            if (result.CryptoObject != null)
                Log.Info("OnAuthSucceeded", $"Authentication succeeded.CryptoObject.Cipher.Algorithm = {result.CryptoObject.Cipher.Algorithm}");
            else Log.Info("OnAuthSucceeded", "Authentication succeeded.");
        }
        public override void OnAuthHelp(int helpMsgId, ICharSequence helpString)
        {
            Log.Info("OnAuthHelp", $"Authentication help. helpMsgId={helpMsgId},helpString={helpString}");
        }
    }

    class HwBioAuthnCipherFactory
    {
        private static string Tag = "HwBioAuthnCipherFactory";

        private string storeKey;

        private KeyStore keyStore;

        private KeyGenerator keyGenerator;

        private Cipher defaultCipher;

        private bool isUserAuthenticationRequired;



        /// <summary>
        /// Constructed function
        /// </summary>
        /// <param name="storeKey">story key name</param>
        /// <param name="isUserAuthenticationRequired">Sets whether the key is authorized to be used only if the user has authenticated.</param>
        public HwBioAuthnCipherFactory(string storeKey, bool isUserAuthenticationRequired)
        {
            this.storeKey = storeKey;
            this.isUserAuthenticationRequired = isUserAuthenticationRequired;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                try
                {
                    InitDefaultCipherObject();
                }
                catch (System.Exception e)
                {
                    defaultCipher = null;
                    Log.Error(Tag, "Failed to init Cipher. " + e.Message);
                }
            }
            else
            {
                defaultCipher = null;
                Log.Error(Tag, "Failed to init Cipher.");
            }
        }

        [RequiresApi(Api = (int)BuildVersionCodes.M)]
        private void InitDefaultCipherObject()
        {
            try
            {
                keyStore = KeyStore.GetInstance("AndroidKeyStore");
            }
            catch (KeyStoreException e)
            {
                throw new RuntimeException("Failed to get an instance of KeyStore(AndroidKeyStore). " + e.Message, e);
            }
            try
            {
                keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, "AndroidKeyStore");
            }
            catch (Java.Lang.Exception e)
            {
                throw new RuntimeException("Failed to get an instance of KeyGenerator(AndroidKeyStore)." + e.Message, e);
            }

            CreateSecretKey(storeKey, true);

            try
            {
                defaultCipher = Cipher.GetInstance(KeyProperties.KeyAlgorithmAes + "/" + KeyProperties.BlockModeCbc
                        + "/" + KeyProperties.EncryptionPaddingPkcs7);
            }
            catch (Java.Lang.Exception e)
            {
                throw new RuntimeException("Failed to get an instance of Cipher", e);
            }
            InitCipher(defaultCipher, storeKey);
        }

        private void InitCipher(Cipher cipher, string storeKeyName)
        {
            try
            {
                keyStore.Load(null);
                IKey secretKey = keyStore.GetKey(storeKeyName, null);
                cipher.Init(CipherMode.EncryptMode, secretKey);
            }
            catch (Java.Lang.Exception e)
            {
                if (e is KeyStoreException | e is CertificateException | e is UnrecoverableKeyException | e is IOException
                  | e is NoSuchAlgorithmException | e is InvalidKeyException)
                    throw new RuntimeException("Failed to init Cipher. " + e.Message, e);
            }
        }

        void CreateSecretKey(string storeKeyName, bool isInvalidatedByBiometricEnrollment)
        {
            try
            {
                keyStore.Load(null);
                KeyGenParameterSpec.Builder keyParamBuilder = null;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    keyParamBuilder = new KeyGenParameterSpec.Builder(storeKeyName,
                            KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                            .SetBlockModes(KeyProperties.BlockModeCbc)
                            // This key is authorized to be used only if the user has been authenticated.
                            .SetUserAuthenticationRequired(isUserAuthenticationRequired)
                            .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7);
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    keyParamBuilder.SetInvalidatedByBiometricEnrollment(isInvalidatedByBiometricEnrollment);
                }
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    keyGenerator.Init(keyParamBuilder.Build());
                }
                keyGenerator.GenerateKey();
            }
            catch (Java.Lang.Exception e)
            {
                if (e is NoSuchAlgorithmException || e is InvalidAlgorithmParameterException || e is CertificateException || e is IOException)
                    throw new RuntimeException("Failed to create secret key. " + e.Message, e);
            }
        }

        public Cipher GetCipher()
        {
            return defaultCipher;
        }
    }
}
