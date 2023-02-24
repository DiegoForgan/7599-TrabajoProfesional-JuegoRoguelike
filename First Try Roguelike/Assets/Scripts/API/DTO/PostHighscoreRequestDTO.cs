public class PostHighscoreRequestDTO
{
    public int achieved_level;
    public int difficulty_level;
    public int gold_collected;
    public string time_elapsed;
    public int high_score;

    public PostHighscoreRequestDTO(int new_achieved_level, int new_difficulty_level, int new_gold_collected, string new_time_elapsed)
    {
        this.achieved_level = new_achieved_level;
        this.difficulty_level = new_difficulty_level;
        this.gold_collected = new_gold_collected;
        this.time_elapsed = new_time_elapsed;
        // The highscore is not calculated in this version
        // Instead, all other properties are used to rank players
        this.high_score = 0;
    }
}
