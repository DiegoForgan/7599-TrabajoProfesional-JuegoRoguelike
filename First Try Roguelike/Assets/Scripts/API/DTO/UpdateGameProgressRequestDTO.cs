public class UpdateGameProgressRequestDTO
{
    public int next_level;
    public int difficulty_level;
    public int gold_collected;
    public string time_elapsed;

    public UpdateGameProgressRequestDTO(int new_next_level, int new_difficulty_level, int new_gold_collected, string new_time_elapsed)
    {
        this.next_level = new_next_level;
        this.difficulty_level = new_difficulty_level;
        this.gold_collected = new_gold_collected;
        this.time_elapsed = new_time_elapsed;
    }
}
