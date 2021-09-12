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
    public int Health;
    public float PositionX;
    public float PositionZ;
    public float RotationY;

    public static PlayerData Parse(PlayerScript player)
    {
        return new PlayerData
        {
            Health = player.Health,
            PositionX = player.transform.position.x,
            PositionZ = player.transform.position.z,
            RotationY = player.transform.eulerAngles.y
        };
    }
    public static byte[] Serialize(object data)
    {
        PlayerData playerData = (PlayerData)data;
        List<byte> array = new List<byte>();
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
            Health = BitConverter.ToInt32(data, 0),
            PositionX = BitConverter.ToSingle(data, 4),
            PositionZ = BitConverter.ToSingle(data, 8),
            RotationY = BitConverter.ToSingle(data, 12),
        };
    }
} 

struct ProjectileData
{
    public float PositionX;
    public float PositionZ;
    public float RotationY;
    public static ProjectileData Parse(ProjectileScript projectile)
    {
        return new ProjectileData
        {
            PositionX = projectile.transform.position.x,
            PositionZ = projectile.transform.position.z,
            RotationY = projectile.transform.eulerAngles.y
        };
    }
    public static byte[] Serialize(object data)
    {
        ProjectileData projectileData = (ProjectileData)data;
        List<byte> array = new List<byte>();
        array.AddRange(BitConverter.GetBytes(projectileData.PositionX));
        array.AddRange(BitConverter.GetBytes(projectileData.PositionZ));
        array.AddRange(BitConverter.GetBytes(projectileData.RotationY));
        return array.ToArray();
    }
    public static object Deserialize(byte[] data)
    {
        return new ProjectileData
        {
            PositionX = BitConverter.ToSingle(data, 0),
            PositionZ = BitConverter.ToSingle(data, 4),
            RotationY = BitConverter.ToSingle(data, 8),
        };
    }
}
