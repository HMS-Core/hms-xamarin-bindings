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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Skeleton;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Skeleton
{
    public class SkeletonGraphic : Graphic
    {
        private IList<MLSkeleton> skeletons;
        private Paint circlePaint;
        private Paint linePaint;

        public SkeletonGraphic(GraphicOverlay overlay, IList<MLSkeleton> skeletons) : base(overlay)
        {
            this.skeletons = skeletons;
            circlePaint = new Paint();
            circlePaint.Color = Color.Red;
            circlePaint.SetStyle(Paint.Style.Fill);
            circlePaint.AntiAlias = true;

            linePaint = new Paint();
            linePaint.Color = Color.Green;
            linePaint.SetStyle(Paint.Style.Stroke);
            linePaint.StrokeWidth = 10f;
            linePaint.AntiAlias = true;
        }

        public override void Draw(Canvas canvas)
        {
            for (int i = 0; i < skeletons.Count; i++)
            {
                MLSkeleton skeleton = skeletons.ElementAt(i);
                if (skeleton.Joints == null || skeleton.Joints.Count < 14)
                {
                    continue;
                }
                List<Path> paths = new List<Path>();
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeHeadTop), skeleton.GetJointPoint(MLJoint.TypeNeck)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeNeck), skeleton.GetJointPoint(MLJoint.TypeLeftShoulder)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeLeftShoulder), skeleton.GetJointPoint(MLJoint.TypeLeftElbow)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeLeftElbow), skeleton.GetJointPoint(MLJoint.TypeLeftWrist)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeNeck), skeleton.GetJointPoint(MLJoint.TypeLeftHip)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeLeftHip), skeleton.GetJointPoint(MLJoint.TypeLeftKnee)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeLeftKnee), skeleton.GetJointPoint(MLJoint.TypeLeftAnkle)));

                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeNeck), skeleton.GetJointPoint(MLJoint.TypeRightShoulder)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeRightShoulder), skeleton.GetJointPoint(MLJoint.TypeRightElbow)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeRightElbow), skeleton.GetJointPoint(MLJoint.TypeRightWrist)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeNeck), skeleton.GetJointPoint(MLJoint.TypeRightHip)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeRightHip), skeleton.GetJointPoint(MLJoint.TypeRightKnee)));
                paths.Add(GetPath(skeleton.GetJointPoint(MLJoint.TypeRightKnee), skeleton.GetJointPoint(MLJoint.TypeRightAnkle)));

                for (int j = 0; j < paths.Count; j++)
                {
                    if (paths.ElementAt(j) != null)
                    {
                        canvas.DrawPath(paths.ElementAt(j), linePaint);
                    }
                }

                foreach (MLJoint joint in skeleton.Joints)
                {
                    if (!(Math.Abs(joint.PointX - 0f) == 0 && Math.Abs(joint.PointY - 0f) == 0))
                    {
                        canvas.DrawCircle(TranslateX(joint.PointX),
                                TranslateY(joint.PointY), 24f, circlePaint);
                    }
                }
            }
        }

        private Path GetPath(MLJoint point1, MLJoint point2)
        {
            if (point1 == null || point2 == null)
            {
                return null;
            }
            if (point1.PointX == 0f && point1.PointY == 0f)
            {
                return null;
            }
            if (point2.PointX == 0f && point2.PointY == 0f)
            {
                return null;
            }
            Path path = new Path();
            path.MoveTo(TranslateX(point1.PointX),
                    TranslateY(point1.PointY));
            path.LineTo(TranslateX(point2.PointX), TranslateY(point2.PointY));

            return path;
        }
    }
}