using InventorySystem.Items.Firearms;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PluginAPI.Core;
using RelativePositioning;
using Respawning;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System;
using UnityEngine;
using System.Text;
using MapGeneration;

namespace Mistaken.API.Extensions;

public static class MirrorExtensions
{
    public static MethodInfo SetDirtyBitsMethodInfo
        => setDirtyBitsMethodInfoValue ??= typeof(NetworkBehaviour).GetMethod("SetDirtyBit");

    public static MethodInfo SendSpawnMessageMethodInfo
        => sendSpawnMessageMethodInfoValue ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Static | BindingFlags.NonPublic);

    public static void PlayBeepSound(this Player player)
        => SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), "RpcPlaySound", 7);

    public static void SetPlayerInfoForTargetOnly(this Player player, Player target, string info)
        => SendFakeSyncVar(player, target.ReferenceHub.networkIdentity, typeof(NicknameSync), "Network_customPlayerInfoString", info);

    public static void PlayGunSound(this Player player, Vector3 position, ItemType itemType, byte volume, byte audioClipId = 0)
    {
        GunAudioMessage msg = default;
        msg.Weapon = itemType;
        msg.AudioClipId = audioClipId;
        msg.MaxDistance = volume;
        msg.ShooterHub = player.ReferenceHub;
        msg.ShooterPosition = new RelativePosition(position);
        player.Connection.Send(msg);
    }

    public static void SetRoomColorForTargetOnly(this RoomIdentifier room, Player target, Color color)
    {
        SendFakeSyncVar(target, room.GetComponentInChildren<FlickerableLightController>().netIdentity, typeof(FlickerableLightController), "Network_warheadLightColor", color);
        SendFakeSyncVar(target, room.GetComponentInChildren<FlickerableLightController>().netIdentity, typeof(FlickerableLightController), "Network_warheadLightOverride", true);
    }

    public static void SetRoomLightIntensityForTargetOnly(this RoomIdentifier room, Player target, float multiplier)
        => SendFakeSyncVar(target, room.GetComponentInChildren<FlickerableLightController>().netIdentity, typeof(FlickerableLightController), "Network_lightIntensityMultiplier", multiplier);

    public static void ChangeAppearance(this Player player, RoleTypeId type)
    {
        foreach (Player item in Player.GetPlayers().Where(x => x != player))
            item.Connection.Send(new RoleSyncInfo(player.ReferenceHub, type, item.ReferenceHub));
    }

    public static void PlayCassieAnnouncement(this Player player, string words, bool makeHold = false, bool makeNoise = true, bool isSubtitles = false)
    {
        foreach (RespawnEffectsController allController in RespawnEffectsController.AllControllers)
        {
            if (allController != null)
                SendFakeTargetRpc(player, allController.netIdentity, typeof(RespawnEffectsController), "RpcCassieAnnouncement", words, makeHold, makeNoise, isSubtitles);
        }
    }

    public static void MessageTranslated(this Player player, string words, string translation, bool makeHold = false, bool makeNoise = true, bool isSubtitles = true)
    {
        StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
        string[] array = words.Split('\n');
        string[] array2 = translation.Split('\n');
        for (int i = 0; i < array.Length; i++)
            stringBuilder.Append(array2[i] + "<size=0> " + array[i].Replace(' ', '\u2005') + " </size><split>");

        foreach (RespawnEffectsController allController in RespawnEffectsController.AllControllers)
        {
            if (allController != null)
                SendFakeTargetRpc(player, allController.netIdentity, typeof(RespawnEffectsController), "RpcCassieAnnouncement", stringBuilder, makeHold, makeNoise, isSubtitles);
        }

        StringBuilderPool.Shared.Return(stringBuilder);
    }

    public static void SendFakeSyncVar(this Player target, NetworkIdentity behaviorOwner, Type targetType, string propertyName, object value)
    {
        PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
        PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
        MakeCustomSyncWriter(behaviorOwner, targetType, null, CustomSyncVarGenerator, writer, writer2);
        target.ReferenceHub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage()
        {
            netId = behaviorOwner.netId,
            payload = writer.ToArraySegment()
        });

        NetworkWriterPool.Recycle(writer);
        NetworkWriterPool.Recycle(writer2);
        void CustomSyncVarGenerator(NetworkWriter targetWriter)
        {
            targetWriter.WriteUInt64(SyncVarDirtyBits[propertyName]);
            WriterExtensions[value.GetType()]?.Invoke(null, new object[2] { targetWriter, value });
        }
    }

    public static void ResyncSyncVar(NetworkIdentity behaviorOwner, Type targetType, string propertyName)
        => SetDirtyBitsMethodInfo.Invoke(behaviorOwner.gameObject.GetComponent(targetType), new object[1] { SyncVarDirtyBits[propertyName ?? ""] });

    public static void SendFakeTargetRpc(Player target, NetworkIdentity behaviorOwner, Type targetType, string rpcName, params object[] values)
    {
        PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
        foreach (object obj in values)
            WriterExtensions[obj.GetType()].Invoke(null, new object[2] { writer, obj });

        RpcMessage msg = default;
        msg.netId = behaviorOwner.netId;
        msg.componentIndex = GetComponentIndex(behaviorOwner, targetType);
        msg.functionHash = targetType.FullName.GetStableHashCode() * 503 + rpcName.GetStableHashCode();
        msg.payload = writer.ToArraySegment();
        target.Connection.Send(msg);
        NetworkWriterPool.Recycle(writer);
    }

    public static void SendFakeSyncObject(Player target, NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customAction)
    {
        PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
        PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
        MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
        target.ReferenceHub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage()
        {
            netId = behaviorOwner.netId,
            payload = writer.ToArraySegment()
        });

        NetworkWriterPool.Recycle(writer);
        NetworkWriterPool.Recycle(writer2);
    }

    public static void EditNetworkObject(NetworkIdentity identity, Action<NetworkIdentity> customAction)
    {
        customAction(identity);
        ObjectDestroyMessage msg = default;
        msg.netId = identity.netId;

        foreach (Player item in Player.GetPlayers())
        {
            item.Connection.Send(msg);
            SendSpawnMessageMethodInfo.Invoke(null, new object[2] { identity, item.Connection });
        }
    }

    private static int GetComponentIndex(NetworkIdentity identity, Type type)
        =>  Array.FindIndex(identity.NetworkBehaviours, x => x.GetType() == type);

    private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
    {
        byte value = 0;
        NetworkBehaviour networkBehaviour = null;
        for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
        {
            if (behaviorOwner.NetworkBehaviours[i].GetType() == targetType)
            {
                networkBehaviour = behaviorOwner.NetworkBehaviours[i];
                value = (byte)i;
                break;
            }
        }

        owner.WriteByte(value);
        int position = owner.Position;
        owner.WriteInt32(0);
        int position2 = owner.Position;
        if (customSyncObject != null)
            customSyncObject(owner);
        else
            networkBehaviour.SerializeObjectsDelta(owner);

        customSyncVar?.Invoke(owner);
        int position3 = owner.Position;
        owner.Position = position;
        owner.WriteInt32(position3 - position2);
        owner.Position = position3;
        if (networkBehaviour.syncMode != 0)
            observer.WriteBytes(owner.ToArraySegment().Array, position, owner.Position - position);
    }

    public static ReadOnlyDictionary<Type, MethodInfo> WriterExtensions
    {
        get
        {
            if (WriterExtensionsValue.Count == 0)
            {
                foreach (MethodInfo item in typeof(NetworkWriterExtensions).GetMethods().Where(x =>
                {
                    if (!x.IsGenericMethod)
                    {
                        ParameterInfo[] parameters2 = x.GetParameters();
                        if (parameters2 == null)
                            return false;

                        return parameters2.Length == 2;
                    }

                    return false;
                }))
                {
                    WriterExtensionsValue.Add(item.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, item);
                }

                foreach (MethodInfo item2 in typeof(GeneratedNetworkCode).GetMethods().Where(x =>
                {
                    if (!x.IsGenericMethod)
                    {
                        ParameterInfo[] parameters = x.GetParameters();
                        if (parameters is not null && parameters.Length == 2)
                            return x.ReturnType == typeof(void);
                    }

                    return false;
                }))
                {
                    WriterExtensionsValue.Add(item2.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, item2);
                }

                foreach (Type item3 in from x in typeof(ServerConsole).Assembly.GetTypes()
                                       where x.Name.EndsWith("Serializer")
                                       select x)
                {
                    foreach (MethodInfo item4 in from x in item3.GetMethods()
                                                 where x.ReturnType == typeof(void) && x.Name.StartsWith("Write")
                                                 select x)
                    {
                        WriterExtensionsValue.Add(item4.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, item4);
                    }
                }
            }

            return ReadOnlyWriterExtensionsValue;
        }
    }

    public static ReadOnlyDictionary<string, ulong> SyncVarDirtyBits
    {
        get
        {
            if (SyncVarDirtyBitsValue.Count == 0)
            {
                foreach (PropertyInfo item in from m in typeof(ServerConsole).Assembly.GetTypes().SelectMany((Type x) => x.GetProperties())
                                              where m.Name.StartsWith("Network")
                                              select m)
                {
                    MethodInfo setMethod = item.GetSetMethod();
                    if (setMethod is null)
                        continue;

                    MethodBody methodBody = setMethod.GetMethodBody();
                    if (methodBody is not null)
                    {
                        byte[] iLAsByteArray = methodBody.GetILAsByteArray();
                        if (!SyncVarDirtyBitsValue.ContainsKey(item.Name ?? ""))
                            SyncVarDirtyBitsValue.Add(item.Name ?? "", iLAsByteArray[iLAsByteArray.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }
            }

            return ReadOnlySyncVarDirtyBitsValue;
        }
    }

    private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new();

    private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new();

    private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue = new(WriterExtensionsValue);

    private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue = new(SyncVarDirtyBitsValue);

    private static MethodInfo setDirtyBitsMethodInfoValue;

    private static MethodInfo sendSpawnMessageMethodInfoValue;
}
