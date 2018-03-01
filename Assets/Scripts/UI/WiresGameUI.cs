using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WiresGame))]
public class WiresGameUI : MonoBehaviour
{

    public string Tag;

    private GameObject _userInterface;

    private void Awake()
    {
        Debug.Log("Awake WiresGameUI");
        GetComponent<WiresGame>().GeneratedInstructions += OnGeneratedInstructions;
        _userInterface = GameObject.FindGameObjectWithTag(Tag);
    }

    private void OnGeneratedInstructions(List<WiresGame.Instruction> instructions)
    {
        Debug.Log("Update Wires UI");
        _userInterface.BroadcastMessage("UpdateInstructions", instructions);
    }

    private void OnDisable()
    {
        GetComponent<WiresGame>().GeneratedInstructions -= OnGeneratedInstructions;
    }
}
