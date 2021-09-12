﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersTable
{
    private Dictionary<int, PlayerScript> _playersList { set; get; } = new Dictionary<int, PlayerScript>();
    public IReadOnlyDictionary<int, PlayerScript> PlayersList => _playersList;

    public void AddPlayer(PlayerScript player)
    {
        if (!PlayersList.ContainsKey(player.ID)) _playersList.Add(player.ID, player);            
    }
    public void RemovePlayer(PlayerScript player)
    {
        if (PlayersList.ContainsKey(player.ID)) _playersList.Remove(player.ID);
    }
}
