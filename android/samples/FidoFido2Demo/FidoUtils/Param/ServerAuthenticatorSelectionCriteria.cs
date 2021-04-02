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




using Java.Lang;
/**
* Server Authenticator Selection Criteria
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils.Param
{
    public class ServerAuthenticatorSelectionCriteria
    {
        private string authenticatorAttachment;

        public string AuthenticatorAttachment
        {
            get { return authenticatorAttachment; }
            set { this.authenticatorAttachment = value; }
        }

        private bool requireResidentKey;

        public bool IsRequireResidentKey
        {
            get { return requireResidentKey; }
            set { this.requireResidentKey = value; }
        }

        private string userVerification;

        public string UserVerification
        {
            get { return userVerification; }
            set { this.userVerification = value; }
        }


    }

}
