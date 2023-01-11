public class GameProgressResponseDTO 
{
    public string id;
    public string username;
    public int next_level;
    public int difficulty_level;
    public string time_elapsed;
    public int gold_collected;
    public string date_created;
    public string date_updated;

    public GameProgressResponseDTO(string id, string username, int next_level, int difficulty_level, string time_elapsed, int gold_collected, string date_created, string date_updated)
    {
        this.id = id;
        this.username = username;
        this.next_level = next_level;
        this.difficulty_level = difficulty_level;
        this.time_elapsed = time_elapsed;
        this.gold_collected = gold_collected;
        this.date_created = date_created;
        this.date_updated = date_updated;
    }

    public int getNextLevel() { return this.next_level; }
    public int getDifficultyLevel() { return this.difficulty_level; }
    public string getTimeElapsed() { return this.time_elapsed; }
    public int getGoldCollected() { return this.gold_collected; }
}
