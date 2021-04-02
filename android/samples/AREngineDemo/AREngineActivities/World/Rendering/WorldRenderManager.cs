/*
*       Copyright 2020-2021 Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;
using Java.Util.Concurrent;
using Javax.Microedition.Khronos.Opengles;
using XamarinAREngineDemo.Common;
using static XamarinAREngineDemo.Common.TextDisplay;

namespace XamarinAREngineDemo.AREngineActivities.World.Rendering
{
    /// <summary>
    /// This class provides rendering management related to the world scene, including
    /// label rendering and virtual object rendering management.
    /// </summary>
    public class WorldRenderManager : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private const string TAG = "WorldRenderManager";

        private static readonly int PROJ_MATRIX_OFFSET = 0;

        private static readonly float PROJ_MATRIX_NEAR = 0.1f;

        private static readonly float PROJ_MATRIX_FAR = 100.0f;

        private static readonly float MATRIX_SCALE_SX = -1.0f;

        private static readonly float MATRIX_SCALE_SY = -1.0f;

        private static readonly float[] BLUE_COLORS = new float[] { 66.0f, 133.0f, 244.0f, 255.0f };

        private static readonly float[] GREEN_COLORS = new float[] { 66.0f, 133.0f, 244.0f, 255.0f };

        private ARSession mSession;

        private Activity mActivity;

        private Context mContext;

        private TextView mTextView;

        private TextView mSearchingTextView;

        private int frames = 0;

        private long lastInterval;

        private float fps;

        private TextureDisplay mTextureDisplay = new TextureDisplay();

        private TextDisplay mTextDisplay = new TextDisplay();

        private LabelDisplay mLabelDisplay = new LabelDisplay();

        private ObjectDisplay mObjectDisplay = new ObjectDisplay();

        private DisplayRotationManager mDisplayRotationManager;

        private ArrayBlockingQueue mQueuedSingleTaps;

        private List<VirtualObject> mVirtualObjects = new List<VirtualObject>();

        private VirtualObject mSelectedObj = null;

        /// <summary>
        /// The constructor passes context and activity.
        /// This method will be called when Activity's OnCreate}.
        /// </summary>
        /// <param name="activity">Activity</param>
        /// <param name="context">Context</param>
        public WorldRenderManager(Activity activity, Context context)
        {
            mActivity = activity;
            mContext = context;
            mTextView = (TextView)activity.FindViewById(Resource.Id.wordTextView);
            mSearchingTextView = (TextView)activity.FindViewById(Resource.Id.searchingTextView);
        }

        /// <summary>
        /// Set ARSession, which will update and obtain the latest data in OnDrawFrame.
        /// </summary>
        /// <param name="arSession">ARSession.</param>
        public void SetArSession(ARSession arSession)
        {
            if (arSession == null)
            {
                Log.Info(TAG, "setSession error, arSession is null!");
                return;
            }
            mSession = arSession;
        }

        /// <summary>
        /// Set a gesture type queue.
        /// </summary>
        /// <param name="queuedSingleTaps">Gesture type queue.</param>
        public void SetQueuedSingleTaps(ArrayBlockingQueue queuedSingleTaps)
        {
            if (queuedSingleTaps == null)
            {
                Log.Info(TAG, "setSession error, arSession is null!");
                return;
            }
            mQueuedSingleTaps = queuedSingleTaps;
        }

        /// <summary>
        /// Set the DisplayRotationManager object, 
        /// which will be used in OnSurfaceChanged and OnDrawFrame.
        /// </summary>
        /// <param name="displayRotationManager">DisplayRotationManager is a customized object.</param>
        public void SetDisplayRotationManager(DisplayRotationManager displayRotationManager)
        {
            if (displayRotationManager == null)
            {
                Log.Info(TAG, "SetDisplayRotationManage error, displayRotationManage is null!");
                return;
            }
            mDisplayRotationManager = displayRotationManager;
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            // Set the window color.
            GLES20.GlClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            mTextureDisplay.Init();
            mTextDisplay.SetListener(new WorldOnTextInfoChangeListenerClass(this));

            mLabelDisplay.Init(GetPlaneBitmaps());

            mObjectDisplay.Init(mContext);

        }

        /// <summary>
        /// Create a thread for text display in the UI thread. This thread will be called back in TextureDisplay.
        /// </summary>
        /// <param name="text">Gesture information displayed on the screen</param>
        /// <param name="positionX">The left padding in pixels.</param>
        /// <param name="positionY">The right padding in pixels.</param>
        public void ShowWorldTypeTextView(string text, float positionX, float positionY)
        {
            mActivity.RunOnUiThread(() => {
                mTextView.SetTextColor(Color.White);

                // Set the font size.
                mTextView.TextSize = 10f;
                if (text != null)
                {
                    mTextView.Text = text;
                    mTextView.SetPadding((int)positionX, (int)positionY, 0, 0);
                }
                else
                {
                    mTextView.Text = ("");
                }
            });
        }

        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            mTextureDisplay.OnSurfaceChanged(width, height);
            GLES20.GlViewport(0, 0, width, height);
            mDisplayRotationManager.UpdateViewportRotation(width, height);
            mObjectDisplay.SetSize(width, height);
        }


        public void OnDrawFrame(IGL10 gl)
        {

            
            GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

            if (mSession == null) {
                return;
            }
            if (mDisplayRotationManager.GetDeviceRotation()) {
                mDisplayRotationManager.UpdateArSessionDisplayGeometry(mSession);
            }

            try {
                mSession.SetCameraTextureName(mTextureDisplay.GetExternalTextureId());
                ARFrame arFrame = mSession.Update();
                ARCamera arCamera = arFrame.Camera;

                // The size of the projection matrix is 4 * 4.
                float[] projectionMatrix = new float[16];

                arCamera.GetProjectionMatrix(projectionMatrix, PROJ_MATRIX_OFFSET, PROJ_MATRIX_NEAR, PROJ_MATRIX_FAR);
                mTextureDisplay.OnDrawFrame(arFrame);
                StringBuilder sb = new StringBuilder();
                UpdateMessageData(sb);
                mTextDisplay.OnDrawFrame(sb);

                // The size of ViewMatrix is 4 * 4.
                float[] viewMatrix = new float[16];
                arCamera.GetViewMatrix(viewMatrix, 0);
                var allTrackables = mSession.GetAllTrackables(Java.Lang.Class.FromType(typeof(ARPlane)));

                foreach (ARPlane plane in allTrackables) {
                    if (plane.Type != ARPlane.PlaneType.UnknownFacing
                        && plane.TrackingState == ARTrackableTrackingState.Tracking) {
                        HideLoadingMessage();
                        break;
                    }
                }
                mLabelDisplay.OnDrawFrame(allTrackables, arCamera.DisplayOrientedPose,
                    projectionMatrix);
                HandleGestureEvent(arFrame, arCamera, projectionMatrix, viewMatrix);
                ARLightEstimate lightEstimate = arFrame.LightEstimate;
                float lightPixelIntensity = 1;
                if (lightEstimate.GetState() != ARLightEstimate.State.NotValid) {
                    lightPixelIntensity = lightEstimate.PixelIntensity;
                }
                DrawAllObjects(projectionMatrix, viewMatrix, lightPixelIntensity);
            } catch (ArDemoRuntimeException e) {
                Log.Info(TAG, "Exception on the ArDemoRuntimeException!");
            } catch (Exception t) {
                // This prevents the app from crashing due to unhandled exceptions.
                Log.Info(TAG, "Exception on the OpenGL thread: " + t.Message);
            } 
        }


        private void DrawAllObjects(float[] projectionMatrix, float[] viewMatrix, float lightPixelIntensity)
        {
            foreach (VirtualObject obj in mVirtualObjects)
            {
                if (obj.GetAnchor().TrackingState == ARTrackableTrackingState.Stopped)
                {
                    mVirtualObjects.Remove(obj);
                }
                if (obj.GetAnchor().TrackingState == ARTrackableTrackingState.Tracking)
                {
                    mObjectDisplay.OnDrawFrame(viewMatrix, projectionMatrix, lightPixelIntensity, obj);
                }
            }
        }

        private List<Bitmap> GetPlaneBitmaps()
        {
            List<Bitmap> bitmaps = new List<Bitmap>();
            bitmaps.Add(GetPlaneBitmap(Resource.Id.plane_other));
            bitmaps.Add(GetPlaneBitmap(Resource.Id.plane_wall));
            bitmaps.Add(GetPlaneBitmap(Resource.Id.plane_floor));
            bitmaps.Add(GetPlaneBitmap(Resource.Id.plane_seat));
            bitmaps.Add(GetPlaneBitmap(Resource.Id.plane_table));
            bitmaps.Add(GetPlaneBitmap(Resource.Id.plane_ceiling));
            return bitmaps;
        }
         
        private Bitmap GetPlaneBitmap(int id)
        {
            TextView view = (TextView)mActivity.FindViewById(id);
            view.DrawingCacheEnabled = true;
            view.Measure(View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
                View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
            Bitmap bitmap = view.DrawingCache;
            Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
            matrix.SetScale(MATRIX_SCALE_SX, MATRIX_SCALE_SY);
            if (bitmap != null)
            {
                bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
            }
            return bitmap;
        }

        /// <summary>
        /// Update the information to be displayed on the screen.
        /// </summary>
        /// <param name="sb">String buffer.</param>
        private void UpdateMessageData(StringBuilder sb)
        {
            float fpsResult = DoFpsCalculate();
            sb.Append("FPS=").Append(fpsResult).Append(System.Environment.NewLine);
        }

        private float DoFpsCalculate()
        {
            ++frames;
            long timeNow = Java.Lang.JavaSystem.CurrentTimeMillis();

            // Convert millisecond to second.
            if (((timeNow - lastInterval) / 1000.0f) > 0.5f)
            {
                fps = frames / ((timeNow - lastInterval) / 1000.0f);
                frames = 0;
                lastInterval = timeNow;
            }
            return fps;
        }

        private void HideLoadingMessage()
        {
            mActivity.RunOnUiThread(() => {
                if (mSearchingTextView != null)
                {
                    mSearchingTextView.Visibility = ViewStates.Gone;
                    mSearchingTextView = null;
                }
            });
        }

        private void HandleGestureEvent(ARFrame arFrame, ARCamera arCamera, float[] projectionMatrix, float[] viewMatrix)
        {
            GestureEvent mEvent = mQueuedSingleTaps.Poll().JavaCast<GestureEvent>();
            if (mEvent == null) 
            {
                return;
            }

            // Do not perform anything when the object is not tracked.
            if (arCamera.TrackingState != ARTrackableTrackingState.Tracking)
            {
                return;
            }
            int eventType = mEvent.GetType();
            switch (eventType)
            {
                case GestureEvent.GESTURE_EVENT_TYPE_DOUBLETAP:
                    DoWhenEventTypeDoubleTap(viewMatrix, projectionMatrix, mEvent);
                    break;
                case GestureEvent.GESTURE_EVENT_TYPE_SCROLL:
                    {
                        if (mSelectedObj == null)
                        {
                            break;
                        }
                        ARHitResult hitResult = HitTest4Result(arFrame, arCamera, mEvent.GetEventSecond());
                        if (hitResult != null)
                        {
                            mSelectedObj.SetAnchor(hitResult.CreateAnchor());
                        }
                        break;
                    }
                case GestureEvent.GESTURE_EVENT_TYPE_SINGLETAPCONFIRMED:
                    {
                        // Do not perform anything when an object is selected.
                        if (mSelectedObj != null)
                        {
                            mSelectedObj.SetIsSelected(false);
                            mSelectedObj = null;
                        }
                        MotionEvent tap = mEvent.GetEventFirst();
                        ARHitResult hitResult = null;

                        hitResult = HitTest4Result(arFrame, arCamera, tap);

                        if (hitResult == null)
                        {
                            break;
                        }
                        DoWhenEventTypeSingleTap(hitResult);
                        break;
                    }
                default:
                    Log.Info(TAG, "Unknown motion event type, and do nothing.");
                    break;
            }

        }

        private void DoWhenEventTypeDoubleTap(float[] viewMatrix, float[] projectionMatrix, GestureEvent mEvent)
        {
            if (mSelectedObj != null)
            {
                mSelectedObj.SetIsSelected(false);
                mSelectedObj = null;
            }
            foreach (VirtualObject obj in mVirtualObjects)
            {
                if (mObjectDisplay.HitTest(viewMatrix, projectionMatrix, obj, mEvent.GetEventFirst())) 
                {
                obj.SetIsSelected(true);
                mSelectedObj = obj;
                break;
                }
            }
        }

        private void DoWhenEventTypeSingleTap(ARHitResult hitResult)
        {
            // The hit results are sorted by distance. Only the nearest hit point is valid.
            // Set the number of stored objects to 10 to avoid the overload of rendering and AR Engine.
            if (mVirtualObjects.Count >= 16)
            {
                mVirtualObjects.ElementAt(0).GetAnchor().Detach();
                mVirtualObjects.RemoveAt(0);
            }

            IARTrackable currentTrackable = hitResult.Trackable;
            Java.Lang.Object PointOrPlane = null;
            bool isPlanHitJudge = false;
            bool isPointHitJudge = false;
            try
            {
                PointOrPlane = currentTrackable.JavaCast<ARPlane>();
                isPlanHitJudge = PointOrPlane.GetType() == typeof(ARPlane) ;
            }
            catch (Exception e)
            {
                PointOrPlane = currentTrackable.JavaCast<ARPoint>();
                isPointHitJudge = PointOrPlane.GetType() == typeof(ARPoint);
            };
            if (isPointHitJudge) {
                mVirtualObjects.Add(new VirtualObject(hitResult.CreateAnchor(), BLUE_COLORS));
            } else if (isPlanHitJudge) {
                mVirtualObjects.Add(new VirtualObject(hitResult.CreateAnchor(), GREEN_COLORS));
            } else
            {
                Log.Info(TAG, "Hit result is not plane or point.");
            }
        }

        private ARHitResult HitTest4Result(ARFrame frame, ARCamera camera, MotionEvent mEvent) 
        {
                ARHitResult hitResult = null;
                IList<ARHitResult> hitTestResults = frame.HitTest(mEvent);

                for (int i = 0; i<hitTestResults.Count; i++) {
                // Determine whether the hit point is within the plane polygon.
                ARHitResult hitResultTemp = hitTestResults.ElementAt(i);
                if (hitResultTemp == null) 
                {
                    continue;
                }
                IARTrackable trackable = hitResultTemp.Trackable;
                
                Java.Lang.Object PointOrPlane = null;
                bool isPlanHitJudge = false;
                bool isPointHitJudge = false;

                try 
                {
                    PointOrPlane = trackable.JavaCast<ARPlane>();
                    isPlanHitJudge =
                        PointOrPlane.GetType() == typeof(ARPlane) && ((ARPlane)PointOrPlane).IsPoseInPolygon(hitResultTemp.HitPose)
                            && (CalculateDistanceToPlane(hitResultTemp.HitPose, camera.Pose) > 0);
                } 
                catch(Exception e) 
                {
                    PointOrPlane = trackable.JavaCast<ARPoint>();
                    isPointHitJudge = PointOrPlane.GetType() == typeof(ARPoint)
                        && ((ARPoint)PointOrPlane).GetOrientationMode() == ARPoint.OrientationMode.EstimatedSurfaceNormal;
                };

                // Determine whether the point cloud is clicked and whether the point faces the camera.


                // Select points on the plane preferentially.
                if (isPlanHitJudge) {
                    hitResult = hitResultTemp;
                }
            }
            return hitResult;
        }

        /// <summary>
        ///  Calculate the distance between a point in a space and a plane. This method is used
        ///  to calculate the distance between a camera in a space and a specified plane.
        /// </summary>
        /// <param name="planePose">ARPose of a plane.</param>
        /// <param name="cameraPose">ARPose of a camera.</param>
        /// <returns>Calculation results.</returns>
        private static float CalculateDistanceToPlane(ARPose planePose, ARPose cameraPose)
        {
            // The dimension of the direction vector is 3.
            float[] normals = new float[3];

            // Obtain the unit coordinate vector of a normal vector of a plane.
            planePose.GetTransformedAxis(1, 1.0f, normals, 0);

            // Calculate the distance based on projection.
            return (cameraPose.Tx() - planePose.Tx()) * normals[0] // 0:x
                + (cameraPose.Ty() - planePose.Ty()) * normals[1] // 1:y
                + (cameraPose.Tz() - planePose.Tz()) * normals[2]; // 2:z
        }

    }

    /// <summary>
    /// TextInfo change listener class.
    /// </summary>
    public class WorldOnTextInfoChangeListenerClass : OnTextInfoChangeListener
    {
        WorldRenderManager mRenderManager;

        public WorldOnTextInfoChangeListenerClass(WorldRenderManager mRenderManager)
        {
            this.mRenderManager = mRenderManager;
        }
        public void TextInfoChanged(string text, float positionX, float positionY)
        {
            mRenderManager.ShowWorldTypeTextView(text, positionX, positionY);
        }
    }
}