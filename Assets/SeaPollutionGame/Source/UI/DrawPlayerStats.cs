﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPlayerStats : MonoBehaviour
{
    [SerializeField] Text statsText = null;

    int playerID = -1;
    WorldStateManager stateManager = null;

    void Start()
    {
        playerID = GetComponent<Player>().id;
        var stateMgrObj = FindObjectOfType<WorldStateManager>();
        stateManager = stateMgrObj.GetComponent<WorldStateManager>();
    }

    void Update()
    {
        // TODO(Xiaoyue Chen): using event system to update text
        if (!statsText) return;
        float money = stateManager.GetMoney(playerID);
        var playerState = stateManager.GetPlayerState(playerID);
        float incomeThisTurn = playerState.GetIncome();
        float producedPollution = stateManager.GetPollution(playerID, PollutionMapType.PRODUCED);
        float filteredPollution = -stateManager.GetPollution(playerID, PollutionMapType.FILTERED);
        float netPollution = stateManager.GetPollution(playerID, PollutionMapType.NET);

        statsText.text =
            "Player: " + playerID.ToString() + "\n" +
            "Money: " + money.ToString() + "\n" +
            "Income this turn " + incomeThisTurn.ToString() + "\n" +
            "Produced pollution: " + producedPollution.ToString() + "\n" +
            "Filtered pollution: " + filteredPollution.ToString() + "\n" +
            "Pollution into the sea: " + netPollution.ToString() + "\n";
    }
}
