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
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Nio;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;

namespace XamarinAREngineDemo.AREngineActivities.World
{
    /// <summary>
    /// This class contains methods to obtain buffer objects
    /// from parsed .obj file.
    /// </summary>
    public class ObjDataBufferHelper
    {
        /// <summary>
        /// Obtain Face object count from .obj file
        /// </summary>
        public static int GetFaceCount(IList<Group> obj)
        {
            int count = 0;
            foreach (var group in obj)
            {
                foreach (var faceList in group.Faces)
                {
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Obtain Face vertices buffer.
        /// </summary>
        /// <param name="obj">Group list from .obj file</param>
        /// <param name="numVerticesPerFace">number of vertices per face</param>
        /// <returns>IntBuffer</returns>
        public static IntBuffer GetFaceVerticesIntBuffer(IList<Group> obj, int numVerticesPerFace)
        {
            var faceCount = GetFaceCount(obj);
            IntBuffer buffer = CreateDirectIntBuffer(
                                faceCount * numVerticesPerFace);
            GetFaceVertexIndices(obj, buffer , numVerticesPerFace);
            buffer.Position(0);
            return buffer;
        }

        /// <summary>
        /// Add Face vertices to target buffer.
        /// </summary>
        public static void GetFaceVertexIndices(IList<Group> obj, IntBuffer target , int numVerticesPerFace)
        {
            foreach(var group in obj)
            {
                foreach (var face in group.Faces)
                {
                    for(int i = 0; i<numVerticesPerFace; i++)
                    {
                        target.Put(face[i].VertexIndex-1);
                    }                  
                }
            }
        }

        /// <summary>
        /// Obtain Vertices buffer.
        /// </summary>
        /// <param name="obj">Vertex list from .obj file</param>
        /// <returns>FloatBuffer</returns>
        public static FloatBuffer GetVerticesFloatBuffer(IList<Vertex> obj)
        {
            FloatBuffer buffer = CreateDirectFloatBuffer(obj.Count * 3);
            GetVertices(obj, buffer);
            buffer.Position(0);
            return buffer;
        }

        /// <summary>
        /// Add Vertex to target buffer.
        /// </summary>
        public static void GetVertices(IList<Vertex> obj, FloatBuffer target)
        {
            foreach (var item in obj)
            {
                target.Put(item.X);
                target.Put(item.Y);
                target.Put(item.Z);
            }
        }

        /// <summary>
        /// Obtain Texture buffer.
        /// </summary>
        /// <param name="obj">Texture list from .obj file</param>
        /// <param name="dimensions">Dimension of Texture</param>
        /// <returns>FloatBuffer</returns>
        public static FloatBuffer GetTexturesFloatBuffer(IList<Texture> obj, int dimensions)
        {
            FloatBuffer buffer = CreateDirectFloatBuffer(obj.Count * dimensions);
            GetTextures(obj, buffer);
            buffer.Position(0);
            return buffer;
        }

        /// <summary>
        /// Add Textures to target buffer.
        /// </summary>
        public static void GetTextures(IList<Texture> obj , FloatBuffer target)
        {
            foreach (var item in obj)
            {
                target.Put(item.X);
                target.Put(item.Y);
            }
        }

        /// <summary>
        /// Obtain Normal buffer.
        /// </summary>
        /// <param name="obj">Normal list from .obj file</param>
        /// <returns>FloatBuffer</returns>
        public static FloatBuffer GetNormalsFloatBuffer(IList<Normal> obj)
        {
            FloatBuffer buffer = CreateDirectFloatBuffer(obj.Count * 3);
            GetNormals(obj, buffer);
            buffer.Position(0);
            return buffer;
        }

        /// <summary>
        /// Add Normals to target buffer.
        /// </summary>
        public static void GetNormals(IList<Normal> obj, FloatBuffer target)
        {
            foreach(var item in obj)
            {
                target.Put(item.X);
                target.Put(item.Y);
                target.Put(item.Z);
            }
        }

        /// <summary>
        /// Convert Int buffer to ShortBuffer.
        /// </summary>
        /// <param name="intBuffer">source buffer</param>
        /// <returns>ShortBuffer</returns>
        public static ShortBuffer ConvertToShortBuffer(IntBuffer intBuffer)
        {
            ShortBuffer shortBuffer = CreateDirectShortBuffer(intBuffer.Capacity());
            for (int i = 0; i < intBuffer.Capacity(); i++)
            {
                shortBuffer.Put(i, (short)intBuffer.Get());
            }
            return shortBuffer;
        }

        /// <summary>
        /// Create IntBuffer.
        /// </summary>
        /// <param name="size">Size of buffer.</param>
        /// <returns></returns>
        private static IntBuffer CreateDirectIntBuffer(int size)
        {
            return ByteBuffer.AllocateDirect(size * 4)
                .Order(ByteOrder.NativeOrder())
                .AsIntBuffer();
        }

        /// <summary>
        /// Create ShortBuffer.
        /// </summary>
        /// <param name="size">Size of buffer.</param>
        /// <returns></returns>
        private static ShortBuffer CreateDirectShortBuffer(int size)
        {
            return ByteBuffer.AllocateDirect(size * 2)
                .Order(ByteOrder.NativeOrder())
                .AsShortBuffer();
        }

        /// <summary>
        /// Create FloatBuffer.
        /// </summary>
        /// <param name="size">Size of buffer.</param>
        /// <returns></returns>
        private static FloatBuffer CreateDirectFloatBuffer(int size)
        {
            return ByteBuffer.AllocateDirect(size * 4)
                .Order(ByteOrder.NativeOrder())
                .AsFloatBuffer();
        }

    }
}