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


using Android.Util;
using Huawei.Hms.Support.Api.Fido.Fido2;
using Java.IO;
using Java.Lang;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using XFido_Fido2_Demo.FidoUtils.Param;
using XFido_Fido2_Demo.HMSSample;

/**
* Server Utilities
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils
{
    public class ServerUtils
    {
        private const string Tag = "ServerUtils";

        private ServerUtils()
        {
        }

        public static PublicKeyCredentialCreationOptions ConvertToPublicKeyCredentialCreationOptions(IFido2Client fido2Client,
                ServerPublicKeyCredentialCreationOptionsResponse response)
        {
            PublicKeyCredentialCreationOptions.Builder builder = new PublicKeyCredentialCreationOptions.Builder();

            string name = response.Rp.Name;
            PublicKeyCredentialRpEntity entity = new PublicKeyCredentialRpEntity(name, name, null);
            builder.SetRp(entity);

            string id = response.User.Id;
            try
            {
                builder.SetUser(new PublicKeyCredentialUserEntity(id, System.Text.Encoding.UTF8.GetBytes(id)));
            }
            catch (UnsupportedEncodingException e)
            {
                Log.Error(Tag, e.Message, e);
            }

            builder.SetChallenge(ByteUtils.Base64ToByte(response.Challenge));

            if (response.PubKeyCredParams != null)
            {
                List<PublicKeyCredentialParameters> parameters = new List<PublicKeyCredentialParameters>();
                ServerPublicKeyCredentialParameters[] serverPublicKeyCredentialParameters = response.PubKeyCredParams;
                foreach (ServerPublicKeyCredentialParameters param in serverPublicKeyCredentialParameters)
                {
                    try
                    {
                        PublicKeyCredentialParameters parameter = new PublicKeyCredentialParameters(
                            PublicKeyCredentialType.PublicKey, Algorithm.FromCode(param.Alg));
                        parameters.Add(parameter);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error(Tag, e.Message, e);
                    }
                }
                builder.SetPubKeyCredParams(parameters);
            }

            if (response.ExcludeCredentials != null)
            {
                List<PublicKeyCredentialDescriptor> descriptors = new List<PublicKeyCredentialDescriptor>();
                ServerPublicKeyCredentialDescriptor[] serverDescriptors = response.ExcludeCredentials;
                foreach (ServerPublicKeyCredentialDescriptor desc in serverDescriptors)
                {
                    List<AuthenticatorTransport> transports = new List<AuthenticatorTransport>();
                    if (desc.Transports != null)
                    {
                        try
                        {
                            transports.Add(AuthenticatorTransport.FromValue(desc.Transports));
                        }
                        catch (System.Exception e)
                        {
                            Log.Error(Tag, e.Message, e);
                        }
                    }
                    PublicKeyCredentialDescriptor descriptor = new PublicKeyCredentialDescriptor(
                        PublicKeyCredentialType.PublicKey, ByteUtils.Base64ToByte(desc.Id), transports);
                    descriptors.Add(descriptor);
                }
                builder.SetExcludeList(descriptors);
            }

            Attachment attachment = null;
            if (response.AuthenticatorSelection != null)
            {
                ServerAuthenticatorSelectionCriteria selectionCriteria = response.AuthenticatorSelection;
                if (selectionCriteria.AuthenticatorAttachment != null)
                {
                    try
                    {
                        attachment = Attachment.FromValue(selectionCriteria.AuthenticatorAttachment);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error(Tag, e.Message, e);
                    }
                }

                bool residentKey = selectionCriteria.IsRequireResidentKey;

                UserVerificationRequirement requirement = null;
                if (selectionCriteria.UserVerification != null)
                {
                    try
                    {
                        requirement = UserVerificationRequirement.FromValue(selectionCriteria.UserVerification);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error(Tag, e.Message, e);
                    }
                }

                AuthenticatorSelectionCriteria fido2Selection =
                    new AuthenticatorSelectionCriteria(attachment, (Java.Lang.Boolean)residentKey, requirement);
                builder.SetAuthenticatorSelection(fido2Selection);
            }

            // attestation
            if (response.Attestation != null)
            {
                try
                {
                    AttestationConveyancePreference preference =
                        AttestationConveyancePreference.FromValue(response.Attestation);
                    builder.SetAttestation(preference);
                }
                catch (System.Exception e)
                {
                    Log.Error(Tag, e.Message, e);
                }
            }

            Dictionary<string, Java.Lang.Object> extensions = new Dictionary<string, Java.Lang.Object>();
            if (response.Extensions != null)
            {
                extensions.AddRangeOverride(response.Extensions);
            }

            // Specify a platform authenticator and related extension items. You can specify a platform
            // authenticator or not as needed.
            if (Attachment.Platform.Equals(attachment))
            {
                UseSelectedPlatformAuthenticator(fido2Client, extensions);
            }
            builder.SetExtensions(extensions);
            builder.SetTimeoutSeconds((Java.Lang.Long)response.Timeout);
            return builder.Build();
        }

        public static PublicKeyCredentialRequestOptions ConvertToPublicKeyCredentialRequestOptions(
            IFido2Client fido2Client, 
            ServerPublicKeyCredentialCreationOptionsResponse response,
            bool isUseSelectedPlatformAuthenticator)
        {
            PublicKeyCredentialRequestOptions.Builder builder = new PublicKeyCredentialRequestOptions.Builder();

            builder.SetRpId(response.RpId);

            builder.SetChallenge(ByteUtils.Base64ToByte(response.Challenge));

            ServerPublicKeyCredentialDescriptor[] descriptors = response.AllowCredentials;
            if (descriptors != null)
            {
                List<PublicKeyCredentialDescriptor> descriptorList = new List<PublicKeyCredentialDescriptor>();
                foreach (ServerPublicKeyCredentialDescriptor descriptor in descriptors)
                {
                    List<AuthenticatorTransport> transports = new List<AuthenticatorTransport>();
                    if (descriptor.Transports != null)
                    {
                        try
                        {
                            transports.Add(AuthenticatorTransport.FromValue(descriptor.Transports));
                        }
                        catch (System.Exception e)
                        {
                            Log.Error(Tag, e.Message, e);
                        }
                    }
                    PublicKeyCredentialDescriptor desc = new PublicKeyCredentialDescriptor(
                        PublicKeyCredentialType.PublicKey, ByteUtils.Base64ToByte(descriptor.Id), transports);
                    descriptorList.Add(desc);
                }
                builder.SetAllowList(descriptorList);
            }

            Dictionary<string, Java.Lang.Object> extensions = new Dictionary<string, Java.Lang.Object>();
            if (response.Extensions != null)
            {
                extensions.AddRangeOverride(response.Extensions);
            }
            // Specify a platform authenticator and related extension items. You can specify a platform
            // authenticator or not as needed.
            if (isUseSelectedPlatformAuthenticator)
            {
                UseSelectedPlatformAuthenticator(fido2Client, extensions);
            }
            builder.SetExtensions(extensions);
            builder.SetTimeoutSeconds((Java.Lang.Long)response.Timeout);
            return builder.Build();
        }

        public static ServerAttestationResultRequest ConvertToServerAttestationResultRequest(AuthenticatorAttestationResponse authenticatorAttestationResponse)
        {
            ServerAttestationResultRequest request = new ServerAttestationResultRequest();
            ServerAttestationResultResponseRequest attestationResponse = new ServerAttestationResultResponseRequest();
            attestationResponse.AttestationObject = ByteUtils.ByteToBase64(authenticatorAttestationResponse.GetAttestationObject());
            attestationResponse.ClientDataJSON = ByteUtils.ByteToBase64(authenticatorAttestationResponse.GetClientDataJson());
            request.Response = attestationResponse;
            request.Id = ByteUtils.ByteToBase64(authenticatorAttestationResponse.GetCredentialId());
            request.Type = "public-key";
            return request;
        }

        public static ServerAssertionResultRequest ConvertToServerAssertionResultRequest(AuthenticatorAssertionResponse authenticatorAssertation)
        {
            ServerAssertionResultResponseRequest assertionResultResponse = new ServerAssertionResultResponseRequest();
            assertionResultResponse.Signature = ByteUtils.ByteToBase64(authenticatorAssertation.GetSignature());
            assertionResultResponse.ClientDataJSON = ByteUtils.ByteToBase64(authenticatorAssertation.GetClientDataJson());
            assertionResultResponse.AuthenticatorData = ByteUtils.ByteToBase64(authenticatorAssertation.GetAuthenticatorData());

            ServerAssertionResultRequest request = new ServerAssertionResultRequest();
            request.Response = assertionResultResponse;

            request.Id = ByteUtils.ByteToBase64(authenticatorAssertation.GetCredentialId());

            request.Type = "public-key";
            return request;
        }

        // Specify a platform authenticator and related extension items.
        private static void UseSelectedPlatformAuthenticator(IFido2Client fido2Client, Dictionary<string, Java.Lang.Object> extensions)
        {
            if (!fido2Client.HasPlatformAuthenticators)
            {
                return;
            }
            List<string> selectedAuthenticatorList = new List<string>();
            foreach (AuthenticatorMetadata meta in fido2Client.PlatformAuthenticators)
            {
                if (!meta.IsAvailable)
                {
                    continue;
                }
                // Fingerprint authenticator
                if (meta.IsSupportedUvm(AuthenticatorMetadata.UvmFingerprint))
                {
                    selectedAuthenticatorList.Add(meta.Aaguid);

                    if (meta.Extensions.Contains(Fido2Extension.W3cWebauthnUvi.Identifier))
                    {
                        // Indicates whether to verify the fingerprint ID. If the value is true, the
                        // same finger must be used for both registration and verification.
                        extensions[Fido2Extension.W3cWebauthnUvi.Identifier] = true;
                    }

                    if (meta.Extensions.Contains(Fido2Extension.HmsRPaCibbe01.Identifier))
                    {
                        // Indicates whether the authentication credential expires when the biometric
                        // feature changes. If the value is true, the key will expire when the fingerprint
                        // is enrolled. This is valid only for registration.
                        extensions[Fido2Extension.HmsRPaCibbe01.Identifier] = true;
                    }
                }
                // Lock screen 3D face authenticator
                else if (meta.IsSupportedUvm(AuthenticatorMetadata.UvmFaceprint))
                {
                    // selectedAuthenticatorList.add(meta.getAaguid());
                    Log.Info(Tag, "Lock screen 3D face authenticator");
                }
            }
            extensions[Fido2Extension.HmsRaCPacl01.Identifier] = new SelectedAuthenticatorList(selectedAuthenticatorList);
        }
    }
    public class SelectedAuthenticatorList : Java.Lang.Object
    {
        public List<string> List;
        
        public SelectedAuthenticatorList(List<string> list)
        {
            List = list;
        }
    }
}