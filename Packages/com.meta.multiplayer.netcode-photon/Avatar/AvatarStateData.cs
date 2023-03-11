using System;
using Oculus.Avatar2;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Meta.Multiplayer.Avatar
{
    public struct AvatarStateData : IDisposable, INetworkSerializable
    {
        // FIXME: LOD field is never used by recipient, is it needed?
        public OvrAvatarEntity.StreamLOD lod;
        public int length;
        public NativeArray<byte> bytes;

        public AvatarStateData(int count)
        {
            lod = default;
            bytes = new NativeArray<byte>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            length = count;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref lod);
            serializer.SerializeValue(ref length);

            unsafe
            {
                if (serializer.IsWriter)
                {
                    var writer = serializer.GetFastBufferWriter();

                    if (writer.TryBeginWrite(length))
                    {
                        writer.WriteBytes(bytes.GetPtr(), length);
                    }
                    else
                    {
                        Debug.LogError($"{nameof(FastBufferWriter)} failure.");
                    }
                }
                else if (serializer.IsReader)
                {
                    bytes = new NativeArray<byte>(length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                    var reader = serializer.GetFastBufferReader();

                    if (reader.TryBeginRead(length))
                    {
                        reader.ReadBytes(bytes.GetPtr(), length);
                    }
                    else
                    {
                        Debug.LogError($"{nameof(FastBufferReader)} failure.");
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (bytes.IsCreated)
            {
                bytes.Dispose();
            }

            bytes = default;
            length = -1;
        }
    }
}
