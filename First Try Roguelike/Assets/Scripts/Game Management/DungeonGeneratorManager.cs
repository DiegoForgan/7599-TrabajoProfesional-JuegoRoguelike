using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGeneratorManager : MonoBehaviour
{
    [SerializeField] private Dungeon dungeon;
    private AbstractDungeonGenerator randomWalkGenerator;
    private AbstractDungeonGenerator corridorFirstGenerator;
    private AbstractDungeonGenerator roomFirstGenerator;
    private string algorithmName;
    private List<AbstractDungeonGenerator> algorithmsList;

    private void Awake() {
        randomWalkGenerator = GetComponent<SimpleRandomWalkDungeonGenerator>();
        corridorFirstGenerator = GetComponent<CorridorFirstDungeonGenerator>();
        roomFirstGenerator = GetComponent<RoomFirstDungeonGenerator>();
        algorithmsList = new List<AbstractDungeonGenerator>();
        //We won't be using pure random walk levels, as they affect playability
        //algorithmsList.Add(randomWalkGenerator);
        algorithmsList.Add(corridorFirstGenerator);
        algorithmsList.Add(roomFirstGenerator);    
    }

    public void GenerateRandomWalkDungeon()
    {
        randomWalkGenerator.GenerateDungeon();
        algorithmName = randomWalkGenerator.GetAlgorithmName();
        ShowGenerationMessage(algorithmName);        
    }

    public void GenerateCorridorFirstDungeon()
    {
        corridorFirstGenerator.GenerateDungeon();
        algorithmName = corridorFirstGenerator.GetAlgorithmName();
        ShowGenerationMessage(algorithmName);
    }

    public void GenerateRoomFirstDungeon()
    {
        roomFirstGenerator.GenerateDungeon();
        algorithmName = roomFirstGenerator.GetAlgorithmName();
        ShowGenerationMessage(algorithmName);
    }

    private void ShowGenerationMessage(string nameToShow){
        Debug.Log("Succesfully created dungeon using: " + nameToShow);
    }

    // Exports the level map to a file, in the
    // same directory as the main game files
    public void DumpLevelToFile() {

        Debug.Log("Dumping level to file...");

        DateTime now = DateTime.Now;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        string filePath = "./" +
                          "Escape.from.NODNOL.v" +
                          Application.version +
                          ".level.dump." +
                          now.ToFileTime() + 
                          ".txt";
        StreamWriter writer = new StreamWriter(filePath, false);
        writer.Write("Escape from NODNOL - v" + Application.version + "\n");
        writer.Write("Level dump" + "\n");
        writer.Write("" + "\n");
        writer.Write("Date generated: " + now.ToLocalTime() + "\n");
        writer.Write("Algorithm used: " + algorithmName + "\n");
        writer.Write(dungeon.ExportDungeon());
        writer.Write("\n");
        writer.Close();

        Debug.Log("Done!");
    }

    public void GenerateDungeonUsingRandomAlgorithm()
    {
        int selectedAlgorithm = Random.Range(0,algorithmsList.Count);
        algorithmsList[selectedAlgorithm].GenerateDungeon();
        algorithmName = algorithmsList[selectedAlgorithm].GetAlgorithmName();
        ShowGenerationMessage(algorithmsList[selectedAlgorithm].GetAlgorithmName());
    }

    internal Vector2Int GetRandomFloorPositionOnDungeon()
    {
        return dungeon.GetRandomFloorPosition();
    }

    internal Dungeon GetDungeon()
    {
        return dungeon;
    }
}
