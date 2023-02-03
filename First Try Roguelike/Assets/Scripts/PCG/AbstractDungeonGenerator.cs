using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected new string name;

    [SerializeField] private bool shouldGenerate = false;

    public void GenerateDungeon(){
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }

    private void Start() {
        if(shouldGenerate) GenerateDungeon();
    }

    public string GetAlgorithmName(){
        return name;
    }

    //This method will excute the desired algorithm in order to generate a new Dungeon
    protected abstract void RunProceduralGeneration();
}
