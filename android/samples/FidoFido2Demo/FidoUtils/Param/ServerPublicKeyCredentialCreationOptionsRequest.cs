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
* Server Public Key Credential Creation Options Request
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils.Param
{
    public class ServerPublicKeyCredentialCreationOptionsRequest
    {
        private string username;

        public string Username
        {
            get { return username; }
            set { this.username = value; }
        }

        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { this.displayName = value; }
        }

        private string userVerification;

        public string UserVerification
        {
            get { return userVerification; }
            set { this.userVerification = value; }
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

        private Dictionary<string, object> extensions;

        public Dictionary<string, object> Extensions
        {
            get { return extensions; }
            set { this.extensions = value; }
        }

    }

}
