using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    private AbstractDungeonGenerator randomWalkGenerator;
    private AbstractDungeonGenerator corridorFirstGenerator;
    private AbstractDungeonGenerator roomFirstGenerator;

    private List<AbstractDungeonGenerator> algorithmsList;

    private void Awake() {
        randomWalkGenerator = GetComponent<SimpleRandomWalkDungeonGenerator>();
        corridorFirstGenerator = GetComponent<CorridorFirstDungeonGenerator>();
        roomFirstGenerator = GetComponent<RoomFirstDungeonGenerator>();
        algorithmsList = new List<AbstractDungeonGenerator>();
        algorithmsList.Add(randomWalkGenerator);
        algorithmsList.Add(corridorFirstGenerator);
        algorithmsList.Add(roomFirstGenerator);    
    }

    public void GenerateRandomWalkDungeon()
    {
        randomWalkGenerator.GenerateDungeon();
        ShowGenerationMessage(randomWalkGenerator.GetAlgorithmName());        
    }

    public void GenerateCorridorFirstDungeon()
    {
        corridorFirstGenerator.GenerateDungeon();
        ShowGenerationMessage(corridorFirstGenerator.GetAlgorithmName());
    }

    public void GenerateRoomFirstDungeon()
    {
        roomFirstGenerator.GenerateDungeon();
        ShowGenerationMessage(roomFirstGenerator.GetAlgorithmName());
    }

    private void ShowGenerationMessage(string name){
        Debug.Log("Succesfully created dungeon using: " + name);    
    }
    public void GenerateDungeonUsingRandomAlgorithm()
    {
        int selectedAlgorithm = Random.Range(0,algorithmsList.Count);
        algorithmsList[selectedAlgorithm].GenerateDungeon();
        ShowGenerationMessage(algorithmsList[selectedAlgorithm].GetAlgorithmName());
    }
}