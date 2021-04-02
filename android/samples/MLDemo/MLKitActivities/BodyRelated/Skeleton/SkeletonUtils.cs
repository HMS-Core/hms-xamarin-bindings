/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Skeleton;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Skeleton
{
    public class SkeletonUtils
    {
        private static readonly float MinJointScore = 0.2f;

        public static List<MLSkeleton> GetValidSkeletons(List<MLSkeleton> results)
        {
            if (results == null || results.Count == 0)
            {
                return results;
            }
            List<MLSkeleton> mSkeletons = new List<MLSkeleton>();
            foreach (MLSkeleton skeleton in results)
            {
                List<MLJoint> mJoints = new List<MLJoint>();
                IList<MLJoint> joints = skeleton.Joints;
                foreach (MLJoint joint in joints)
                {
                    // Remove invalid point.
                    if (!(Math.Abs(joint.PointX - 0f) == 0 && Math.Abs(joint.PointY - 0f) == 0)
                            && joint.Score >= MinJointScore)
                    {
                        mJoints.Add(joint);
                    }
                    else
                    {
                        mJoints.Add(new MLJoint(0f, 0f, joint.Type, 0f));
                    }
                }
                MLSkeleton mSkeleton = new MLSkeleton(mJoints);
                mSkeletons.Add(mSkeleton);
            }
            return mSkeletons;
        }
    }
}