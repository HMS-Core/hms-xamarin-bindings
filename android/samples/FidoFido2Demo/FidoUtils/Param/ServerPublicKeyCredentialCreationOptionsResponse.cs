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




using System.Collections.Generic;
/**
* Server Public Key Credential Creation Options Response
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils.Param
{
    public class ServerPublicKeyCredentialCreationOptionsResponse : ServerResponse
    {
        private ServerPublicKeyCredentialRpEntity rp;

        public ServerPublicKeyCredentialRpEntity Rp
        {
            get { return rp; }
            set { this.rp = value; }
        }

        private ServerPublicKeyCredentialUserEntity user;

        public ServerPublicKeyCredentialUserEntity User
        {
            get { return user; }
            set { this.user = value; }
        }

        private string challenge;

        public string Challenge
        {
            get { return challenge; }
            set { this.challenge = value; }
        }

        private ServerPublicKeyCredentialParameters[] pubKeyCredParams;

        public ServerPublicKeyCredentialParameters[] PubKeyCredParams
        {
            get { return pubKeyCredParams; }
            set { this.pubKeyCredParams = value; }
        }

        private long timeout;

        public long Timeout
        {
            get { return timeout; }
            set { this.timeout = value; }
        }

        private ServerPublicKeyCredentialDescriptor[] excludeCredentials;

        public ServerPublicKeyCredentialDescriptor[] ExcludeCredentials
        {
            get { return excludeCredentials; }
            set { this.excludeCredentials = value; }
        }

        private ServerAuthenticatorSelectionCriteria authenticatorSelection;

        public ServerAuthenticatorSelectionCriteria AuthenticatorSelection
        {
            get { return authenticatorSelection; }
            set { this.authenticatorSelection = value; }
        }

        private string attestation;

        public string Attestation
        {
            get { return attestation; }
            set { this.attestation = value; }
        }

        private Dictionary<string, Java.Lang.Object> extensions;

        public Dictionary<string, Java.Lang.Object> Extensions
        {
            get { return extensions; }
            set { this.extensions = value; }
        }

        private string rpId;

        public string RpId
        {
            get { return rpId; }
            set { this.rpId = value; }
        }

        private ServerPublicKeyCredentialDescriptor[] allowCredentials;

        public ServerPublicKeyCredentialDescriptor[] AllowCredentials
        {
            get { return allowCredentials; }
            set { this.allowCredentials = value; }
        }

        private string userVerification;

        public string UserVerification
        {
            get { return userVerification; }
            set { this.userVerification = value; }
        }

    }

}
