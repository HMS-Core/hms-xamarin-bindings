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




using Java.Util;
using System.Collections.Generic;
using System.IO;
using XFido_Fido2_Demo.FidoUtils.Param;
/**
* Server Assertion Result Request
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils.Param
{
    public class ServerAssertionResultRequest
    {

        private string id;

        public string Id
        {
            get { return id; }
            set { this.id = value; }
        }

        private ServerAssertionResultResponseRequest response;

        public ServerAssertionResultResponseRequest Response
        {
            get { return response; }
            set { this.response = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { this.type = value; }
        }

        private Dictionary<string, object> extensions;

        public Dictionary<string, object> Extensions
        {
            get { return extensions; }
            set { this.extensions = value; }
        }

    }

}
