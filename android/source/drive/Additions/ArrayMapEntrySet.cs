/*
*       Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Runtime;

namespace Huawei.Cloud.Base.Util
{
    public partial class ArrayMap
    {
		// Metadata.xml XPath method reference: path="/api/package[@name='com.huawei.cloud.base.util']/class[@name='ArrayMap']/method[@name='entrySet' and count(parameter)=0]"
		[Register("entrySet", "()Ljava/util/Set;", "")]
		public override unsafe global::System.Collections.ICollection EntrySet()
		{
			const string __id = "entrySet.()Ljava/util/Set;";
			try
			{
				var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod(__id, this, null);
				return (System.Collections.ICollection)global::Android.Runtime.JavaSet<global::Java.Util.IMapEntry>.FromJniHandle(__rm.Handle, JniHandleOwnership.TransferLocalRef);
			}
			finally
			{
			}
		}
	}
}