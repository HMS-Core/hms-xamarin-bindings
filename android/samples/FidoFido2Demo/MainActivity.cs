/*
 * Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Huawei.Hms.Support.Api.Fido.Fido2;
using Java.Security;
using System.Collections.Generic;
using Java.IO;
using System.Linq;
using Java.Lang;
using System;
using Android.Content;
using Android.Util;
using XFido_Fido2_Demo.HMSSample;
using Android.Views;
using XFido_Fido2_Demo.FidoUtils;
using XFido_Fido2_Demo.FidoUtils.Param;
using Android.Text;

namespace XFido_Fido2_Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.FullSensor)]
    public class MainActivity : AppCompatActivity
    {
        #region Fields & Variables
        Logger log;

        private const string UserName = "FidoTestUser";

        private const string Tag = "Fido2DemoMainActivity";

        static readonly string rpId = "com.huawei.hms.fido2.test";

        static readonly string user = "fidoCp";

        private static readonly string[] UserRequiredLevelList = { "null", "required", "preferred", "discouraged" };

        private static readonly string[] AuthAttachMode = { "null", "platform", "cross-platform" };

        private static readonly string[] AttestConveyancePreference = { "null", "none", "indirect", "direct" };

        private static readonly string[] ResidentKeyType = { "null", "true", "false" };

        private ArrayAdapter userVerificationLevelAdapter;

        private ArrayAdapter authAttachModeAdapter;

        private ArrayAdapter attestConveyancePreferenceAdapter;

        private ArrayAdapter residentKeyTypeAdapter;

        private Spinner userVerificationSp;

        private Spinner attachmentSp;

        private Spinner attestationSp;

        private Spinner residentKeySp;

        IFido2Client fido2Client;

        private byte[] regCredentialId = null;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_fido2_demo_main);

            InitiateViews();

            fido2Client = Fido2.GetFido2Client(this);
        }
        
        private void InitiateViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            FindViewById<Button>(Resource.Id.btn_net_register).Click += OnClickRegistration;
            FindViewById<Button>(Resource.Id.btn_net_auth).Click += OnClickAuthentication;
            FindViewById<Button>(Resource.Id.btn_net_get_register_info).Click += OnClickRegInfo;
            FindViewById<Button>(Resource.Id.btn_net_deregister_all).Click += OnClickDeregistration;
            userVerificationSp = FindViewById<Spinner>(Resource.Id.sp_user_required_level);
            attachmentSp = FindViewById<Spinner>(Resource.Id.sp_auth_attach_mode);
            attestationSp = FindViewById<Spinner>(Resource.Id.sp_attest_conveyance_preference);
            residentKeySp = FindViewById<Spinner>(Resource.Id.sp_residentkey_type);
            SupportActionBar.Hide();
            userVerificationLevelAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, UserRequiredLevelList);
            userVerificationSp.Adapter = userVerificationLevelAdapter;

            authAttachModeAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, AuthAttachMode);
            attachmentSp.Adapter = authAttachModeAdapter;

            attestConveyancePreferenceAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, AttestConveyancePreference);
            attestationSp.Adapter = attestConveyancePreferenceAdapter;

            residentKeyTypeAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, ResidentKeyType);
            residentKeySp.Adapter = residentKeyTypeAdapter;
        }

        private void OnClickDeregistration(object sender, EventArgs e)
        {
            string Tag = "OnClickDeregistration";
            IFidoServer fidoServer = new FidoServerSimulator();
            if (fidoServer == null)
            {
                log.Error(Tag, GetString(Resource.String.connect_server_err));
                return;
            }
            string userName = UserName;
            if (userName == null)
            {
                return;
            }
            ServerRegDeleteRequest request = new ServerRegDeleteRequest();
            request.Username = userName;

            ServerResponse response = fidoServer.Delete(request);
            if (!ServerStatus.Ok.Equals(response.GetStatus()))
            {
                log.Error(Tag, GetString(Resource.String.delete_register_info_fail) + response.GetErrorMessage());
                return;
            }
            log.Error(Tag, GetString(Resource.String.delete_register_info_success));
        }

        private void OnClickRegInfo(object sender, EventArgs e)
        {
            string Tag = "OnClickRegInfo";
            IFidoServer fidoServer = new FidoServerSimulator();
            if (fidoServer == null)
            {
                log.Error(Tag, GetString(Resource.String.connect_server_err));
                return;
            }
            GetRegInfo(fidoServer);
        }

        private void OnClickAuthentication(object sender, EventArgs e)
        {
            string Tag = "OnClickAuthentication";
            if (!fido2Client.IsSupported)
            {
                log.Info(Tag, "FIDO2 is not supported.");
                return;
            }

            IFidoServer fidoServer = new FidoServerSimulator();
            if (fidoServer == null)
            {
                log.Error(Tag, GetString(Resource.String.connect_server_err));
                return;
            }
            ServerPublicKeyCredentialCreationOptionsRequest request = GetAuthnServerPublicKeyCredentialCreationOptionsRequest();
            if (request == null)
            {
                return;
            }

            // Obtain the challenge value and related policy from the FIDO server, and initiate a Fido2AuthenticationRequest
            // request.
            ServerPublicKeyCredentialCreationOptionsResponse response = fidoServer.GetAssertionOptions(request);
            if (!ServerStatus.Ok.Equals(response.GetStatus()))
            {
                log.Error(Tag, GetString(Resource.String.authn_fail) + response.GetErrorMessage());
                return;
            }

            string attachmentMode = GetSpinnerSelect(attachmentSp.SelectedItem);
            bool isUseSelectedPlatformAuthenticator = Attachment.Platform.Value.Equals(attachmentMode);

            PublicKeyCredentialRequestOptions publicKeyCredentialCreationOptions = ServerUtils.ConvertToPublicKeyCredentialRequestOptions(fido2Client, response, isUseSelectedPlatformAuthenticator);

            AuthenticateToFido2Client(publicKeyCredentialCreationOptions);
        }

        private void OnClickRegistration(object sender, EventArgs e)
        {
            string Tag = "OnClickRegistration";
            if (!fido2Client.IsSupported)
            {
                log.Info(Tag, "FIDO2 is not supported.");
                return;
            }

            IFidoServer fidoServer = new FidoServerSimulator();
            if (fidoServer == null)
            {
                log.Error(Tag, GetString(Resource.String.connect_server_err));
                return;
            }
            ServerPublicKeyCredentialCreationOptionsRequest request = GetRegServerPublicKeyCredentialCreationOptionsRequest();
            if (request == null)
            {
                return;
            }
            // Obtain the challenge value and related policy from the FIDO server, and initiate a Fido2RegistrationRequest
            // request.
            ServerPublicKeyCredentialCreationOptionsResponse response = fidoServer.GetAttestationOptions(request);
            if (!ServerStatus.Ok.Equals(response.GetStatus()))
            {
                log.Error(Tag, GetString(Resource.String.reg_fail) + response.GetErrorMessage());
            }
            PublicKeyCredentialCreationOptions publicKeyCredentialCreationOptions =
                    ServerUtils.ConvertToPublicKeyCredentialCreationOptions(fido2Client, response);
            RegisterToFido2Client(publicKeyCredentialCreationOptions);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok)
            {
                ShowToast("Unknown error.");
                return;
            }
            switch (requestCode)
            {
                case Fido2ClientCommon.RegistrationRequest:
                    {
                        Fido2RegistrationResponse fido2RegistrationResponse = fido2Client.GetFido2RegistrationResponse(data);
                        RegisterToServer(fido2RegistrationResponse);
                        log.Info(Tag, "fido2RegistrationResponse", fido2RegistrationResponse);
                    }
                    break;
                case Fido2ClientCommon.AuthenticationRequest:
                    {
                        Fido2AuthenticationResponse fido2AuthenticationResponse = fido2Client.GetFido2AuthenticationResponse(data);
                        if (fido2AuthenticationResponse.AuthenticatorAssertionResponse == null || fido2AuthenticationResponse.AuthenticatorAssertionResponse.Signature == null) break;
                        AuthenticateToServer(fido2AuthenticationResponse);
                        log.Info(Tag, "fido2AuthenticationResponse", fido2AuthenticationResponse);

                    }
                    break;
                default:
                    break;
            }
        }

        private void RegisterToFido2Client(PublicKeyCredentialCreationOptions publicKeyCredentialCreationOptions)
        {
            string Tag = "RegisterToFido2Client";
            NativeFido2RegistrationOptions registrationOptions = NativeFido2RegistrationOptions.DefaultOptions;
            Fido2RegistrationRequest registrationRequest = new Fido2RegistrationRequest(publicKeyCredentialCreationOptions, null);

            // Call IFido2Client.GetRegistrationIntent to obtain a IFido2Intent instance and start the FIDO client
            // registration process.
            fido2Client.GetRegistrationIntent(registrationRequest, registrationOptions, new Fido2IntentCallback
            {
                OnSuccessAction = (fido2Intent) =>
                {
                    // Start the FIDO client registration process through Fido2ClientCommon.RegistrationRequest.
                    fido2Intent.LaunchFido2Activity(this, Fido2ClientCommon.RegistrationRequest);
                },
                OnFailureAction = (errorCode, errString) =>
                {
                    log.Error(Tag, $"{GetString(Resource.String.reg_fail)} {errorCode}={errString}");
                }

            });
        }

        private ServerPublicKeyCredentialCreationOptionsRequest GetAuthnServerPublicKeyCredentialCreationOptionsRequest()
        {
            ServerPublicKeyCredentialCreationOptionsRequest request = new ServerPublicKeyCredentialCreationOptionsRequest();
            string userName = UserName;
            if (userName == null)
            {
                return null;
            }
            request.Username = userName;
            request.DisplayName = userName;

            return request;
        }

        private void AuthenticateToFido2Client(PublicKeyCredentialRequestOptions publicKeyCredentialCreationOptions)
        {
            NativeFido2AuthenticationOptions authenticationOptions = NativeFido2AuthenticationOptions.DefaultOptions;
            Fido2AuthenticationRequest authenticationRequest =
                    new Fido2AuthenticationRequest(publicKeyCredentialCreationOptions, null);

            // Call IFido2Client.GetAuthenticationIntent to obtain a Fido2Intent instance and start the FIDO client
            // authentication process.
            fido2Client.GetAuthenticationIntent(authenticationRequest, authenticationOptions, new Fido2IntentCallback(
                (fido2Intent) =>
                {
                    // Start the FIDO client authentication process through Fido2ClientCommon.AuthenticationRequest.
                    fido2Intent.LaunchFido2Activity(this, Fido2ClientCommon.AuthenticationRequest);
                },
                (errorCode, errString) =>
                {
                    log.Error(Tag, GetString(Resource.String.authn_fail) + errorCode + "=" + errString);
                }
                ));
        }

        private ServerPublicKeyCredentialCreationOptionsRequest GetRegServerPublicKeyCredentialCreationOptionsRequest()
        {
            ServerPublicKeyCredentialCreationOptionsRequest request = new ServerPublicKeyCredentialCreationOptionsRequest();

            string userName = UserName;
            if (userName == null)
            {
                return null;
            }
            request.Username = userName;
            request.DisplayName = userName;

            string userVeriLevel = GetSpinnerSelect(userVerificationSp.SelectedItem);
            string attachmentMode = GetSpinnerSelect(attachmentSp.SelectedItem);

            bool residentKey = false;
            if (residentKeySp.SelectedItem != null)
            {
                string residentKeyString = GetSpinnerSelect(residentKeySp.SelectedItem);
                if (TextUtils.IsEmpty(residentKeyString))
                {
                    residentKey = false;
                }
                else if ("false".Equals(residentKeyString))
                {
                    residentKey = false;
                }
                else if ("true".Equals(residentKeyString))
                {
                    residentKey = true;
                }
            }
            string attestConveyancePreference = GetSpinnerSelect(attestationSp.SelectedItem);

            ServerAuthenticatorSelectionCriteria selection = GetAuthenticatorSelectionCriteria(userVeriLevel, attachmentMode, residentKey);
            request.AuthenticatorSelection = selection;

            request.Attestation = attestConveyancePreference;
            return request;
        }

        private ServerAuthenticatorSelectionCriteria GetAuthenticatorSelectionCriteria(string userVerificationLevel, string attachmentMode, bool residentKey)
        {
            ServerAuthenticatorSelectionCriteria selectionCriteria = new ServerAuthenticatorSelectionCriteria();

            if (!TextUtils.IsEmpty(userVerificationLevel))
            {
                selectionCriteria.UserVerification = userVerificationLevel;
            }
            else
            {
                selectionCriteria.UserVerification = null;
            }

            if (!TextUtils.IsEmpty(attachmentMode))
            {
                selectionCriteria.AuthenticatorAttachment = attachmentMode;
            }
            else
            {
                selectionCriteria.AuthenticatorAttachment = null;
            }

            selectionCriteria.IsRequireResidentKey = residentKey;
            return selectionCriteria;
        }

        private static string GetSpinnerSelect(object select)
        {
            string data = select.ToString();
            if (select == null || TextUtils.IsEmpty(data) || "null".Equals(data))
            {
                return null;
            }
            return data;
        }

        private void AuthenticateToServer(Fido2AuthenticationResponse fido2AuthenticationResponse)
        {
            string Tag = "AuthenticateToServer";
            if (!fido2AuthenticationResponse.IsSuccess)
            {
                log.Error(Tag, GetString(Resource.String.authn_fail), fido2AuthenticationResponse);
                return;
            }

            IFidoServer fidoServer = new FidoServerSimulator();
            if (fidoServer == null)
            {
                log.Error(Tag, GetString(Resource.String.connect_server_err));
                return;
            }

            ServerAssertionResultRequest request = ServerUtils.ConvertToServerAssertionResultRequest(fido2AuthenticationResponse.AuthenticatorAssertionResponse);

            ServerResponse response = fidoServer.GetAssertionResult(request);
            if (!ServerStatus.Ok.Equals(response.GetStatus()))
            {
                log.Error(Tag, GetString(Resource.String.authn_fail) + response.GetErrorMessage());
                return;
            }
            Log.Info(Tag, GetString(Resource.String.authn_success));
        }
        
        private void RegisterToServer(Fido2RegistrationResponse fido2RegistrationResponse)
        {
            string Tag = "RegisterToServer";
            if (!fido2RegistrationResponse.IsSuccess)
            {
                log.Error(Tag, GetString(Resource.String.reg_fail), fido2RegistrationResponse);
                return;
            }
            IFidoServer fidoServer = new FidoServerSimulator();
            if (fidoServer == null)
            {
                log.Error(Tag, GetString(Resource.String.connect_server_err));
                return;
            }

            ServerAttestationResultRequest request = ServerUtils.ConvertToServerAttestationResultRequest(fido2RegistrationResponse.AuthenticatorAttestationResponse);

            ServerResponse response = fidoServer.GetAttestationResult(request);
            if (!ServerStatus.Ok.Equals(response.GetStatus()))
            {
                log.Error(Tag, GetString(Resource.String.reg_fail) + response.GetErrorMessage());
                return;
            }
            GetRegInfo(fidoServer);
            log.Info(Tag, GetString(Resource.String.reg_success));
        }

        private void GetRegInfo(IFidoServer fidoServer)
        {
            ServerRegInfoRequest request = new ServerRegInfoRequest();
            string username = UserName;
            if (username == null)
            {
                return;
            }
            request.Username = username;

            ServerRegInfoResponse response = fidoServer.GetRegInfo(request);
            if (!ServerStatus.Ok.Equals(response.GetStatus()))
            {
                log.Error(Tag, GetString(Resource.String.get_register_info_fail) + response.GetErrorMessage());
                return;
            }
            ShowRegInfo(response.Infos);
        }

        private void ShowRegInfo(List<ServerRegInfo> regInfos)
        {
            string Tag = "RegistrationInfo";
            string message = string.Empty;
            if (regInfos != null)
            {
                int i = 0;
                foreach (ServerRegInfo regInfo in regInfos)
                {
                    message += $"\n{++i}.{GetString(Resource.String.credential_id)}{regInfo.CredentialId}";
                }
            }
            log.Info(Tag, message);
        }

        public void ShowToast(string msg)
        {
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }
    }
    public class Fido2IntentCallback : Java.Lang.Object, IFido2IntentCallback
    {
        public Action<IFido2Intent> OnSuccessAction;
        public Action<int, ICharSequence> OnFailureAction;
        public Fido2IntentCallback()
        {

        }
        public Fido2IntentCallback(Action<IFido2Intent> OnSuccessAction, Action<int, ICharSequence> OnFailureAction)
        {
            this.OnSuccessAction = OnSuccessAction;
            this.OnFailureAction = OnFailureAction;
        }
        public void OnFailure(int p0, ICharSequence p1)
        {
            OnFailureAction(p0, p1);
        }

        public void OnSuccess(IFido2Intent p0)
        {
            OnSuccessAction(p0);
        }
    }
}