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

using Java.Security;
using System.Collections.Generic;
using XFido_Fido2_Demo.FidoUtils;
using XFido_Fido2_Demo.FidoUtils.Param;
/**
* Simulating a Fido Server
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils
{
    public class FidoServerSimulator : IFidoServer
    {
        private static List<ServerRegInfo> regInfos = new List<ServerRegInfo>();

        ServerPublicKeyCredentialCreationOptionsResponse IFidoServer.GetAttestationOptions(ServerPublicKeyCredentialCreationOptionsRequest request)
        {
            ServerPublicKeyCredentialCreationOptionsResponse response =
                new ServerPublicKeyCredentialCreationOptionsResponse();
            response.Attestation = request.Attestation;

            ServerAuthenticatorSelectionCriteria selectionCriteria = request.AuthenticatorSelection;
            if (selectionCriteria != null)
            {
                response.AuthenticatorSelection = selectionCriteria;
            }

            response.Challenge = ByteUtils.ByteToBase64(GetChallege());

            List<ServerPublicKeyCredentialDescriptor> excludeCredentialList = new List<ServerPublicKeyCredentialDescriptor>();
            foreach (ServerRegInfo info in regInfos)
            {
                ServerPublicKeyCredentialDescriptor desc = new ServerPublicKeyCredentialDescriptor();
                desc.Id = info.CredentialId;
                desc.Type = "public-key";
                excludeCredentialList.Add(desc);
            }
            response.ExcludeCredentials = excludeCredentialList.ToArray();

            List<ServerPublicKeyCredentialParameters> pubKeyCredParamList = new List<ServerPublicKeyCredentialParameters>();
            ServerPublicKeyCredentialParameters cp = new ServerPublicKeyCredentialParameters();
            cp.Alg = -7;
            cp.Type = "public-key";
            pubKeyCredParamList.Add(cp);
            cp = new ServerPublicKeyCredentialParameters();
            cp.Alg = -257;
            cp.Type = "public-key";
            pubKeyCredParamList.Add(cp);
            response.PubKeyCredParams = pubKeyCredParamList.ToArray();

            ServerPublicKeyCredentialRpEntity rpEntity = new ServerPublicKeyCredentialRpEntity();
            rpEntity.Name = "www.huawei.fidodemo";
            response.Rp = rpEntity;

            response.RpId = "www.huawei.fidodemo";

            response.Timeout = 60L;
            ServerPublicKeyCredentialUserEntity user = new ServerPublicKeyCredentialUserEntity();
            user.Id = request.Username;
            user.DisplayName = request.DisplayName;
            response.User = user;
            return response;
        }

        ServerResponse IFidoServer.GetAttestationResult(ServerAttestationResultRequest attestationResultRequest)
        {
            ServerResponse response = new ServerResponse();
            ServerRegInfo info = new ServerRegInfo();
            info.CredentialId = attestationResultRequest.Id;
            regInfos.Add(info);
            return response;
        }

        ServerPublicKeyCredentialCreationOptionsResponse IFidoServer.GetAssertionOptions(
            ServerPublicKeyCredentialCreationOptionsRequest serverPublicKeyCredentialCreationOptionsRequest)
        {
            ServerPublicKeyCredentialCreationOptionsResponse response =
               new ServerPublicKeyCredentialCreationOptionsResponse();

            List<ServerPublicKeyCredentialDescriptor> allowCredentials = new List<ServerPublicKeyCredentialDescriptor>();
            foreach (ServerRegInfo info in regInfos)
            {
                ServerPublicKeyCredentialDescriptor desc = new ServerPublicKeyCredentialDescriptor();
                desc.Id = info.CredentialId;
                desc.Type = "public-key";
                allowCredentials.Add(desc);
            }
            response.AllowCredentials = allowCredentials.ToArray();

            response.Challenge = ByteUtils.ByteToBase64(GetChallege());

            response.RpId = "www.huawei.fidodemo";

            response.Timeout = 60L;

            return response;
        }

        ServerResponse IFidoServer.GetAssertionResult(ServerAssertionResultRequest assertionResultRequest)
        {
            ServerResponse response = new ServerResponse();
            return response;
        }

        ServerRegInfoResponse IFidoServer.GetRegInfo(ServerRegInfoRequest regInfoRequest)
        {
            ServerRegInfoResponse response = new ServerRegInfoResponse();
            List<ServerRegInfo> infos = new List<ServerRegInfo>();
            foreach (ServerRegInfo regInfo in regInfos)
            {
                ServerRegInfo info = new ServerRegInfo();
                info.CredentialId = regInfo.CredentialId;
                infos.Add(info);
            }
            response.Infos = infos;
            return response;
        }

        ServerResponse IFidoServer.Delete(ServerRegDeleteRequest regDeleteRequest)
        {
            ServerResponse response = new ServerResponse();
            regInfos.Clear();
            return response;
        }

        private static byte[] GetChallege()
        {
            return SecureRandom.GetSeed(16);
        }
    }

}