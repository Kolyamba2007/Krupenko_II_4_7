using System;
using System.Collections.Generic;

public struct DamageArgs
{
    public readonly uint Value;
    public readonly PlayerScript Source;
    public DamageArgs(uint value, PlayerScript source)
    {
        Value = value;
        Source = source;
    }
}

struct PlayerData
{
    public int ID;
    public uint Health;
    public float PositionX;
    public float PositionZ;
    public float RotationY;

    public static byte[] Serialize(object data)
    {
        PlayerData playerData = (PlayerData)data;
        List<byte> array = new List<byte>();
        array.AddRange(BitConverter.GetBytes(playerData.ID));
        array.AddRange(BitConverter.GetBytes(playerData.Health));
        array.AddRange(BitConverter.GetBytes(playerData.PositionX));
        array.AddRange(BitConverter.GetBytes(playerData.PositionZ));
        array.AddRange(BitConverter.GetBytes(playerData.RotationY));
        return array.ToArray();
    }
    public static object Deserialize(byte[] data)
    {
        return new PlayerData
        {
            ID = BitConverter.ToInt32(data, 0),
            Health = BitConverter.ToUInt32(data, 4),
            PositionX = BitConverter.ToSingle(data, 8),
            PositionZ = BitConverter.ToSingle(data, 12),
            RotationY = BitConverter.ToSingle(data, 16),
        };
    }
}
