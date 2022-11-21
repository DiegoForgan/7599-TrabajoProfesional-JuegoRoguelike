public class HighScoreResultsDTO 
{
    public string id;
    public string username;
    public int achieved_level;
    public int difficulty_level;
    public string time_elapsed;
    public int gold_collected;
    public int high_score;
    public string date_created;
    public string date_updated;

    public HighScoreResultsDTO(string id, string username, int achieved_level, int difficulty_level, string time_elapsed, int gold_collected, int high_score, string date_created, string date_updated)
    {
        this.id = id;
        this.username = username;
        this.achieved_level = achieved_level;
        this.difficulty_level = difficulty_level;
        this.time_elapsed = time_elapsed;
        this.gold_collected = gold_collected;
        this.high_score = high_score;
        this.date_created = date_created;
        this.date_updated = date_updated;
    }

    public string getId() { return this.id; }
    public string getUsername() { return this.username; }
    public int getAchievedLevel() { return this.achieved_level; }
    public int getDifficultyLevel() { return this.difficulty_level; }
    public string getTimeElapsed() { return this.time_elapsed; }
    public int getGoldCollected() { return this.gold_collected; }
    public int getHighscore() { return this.high_score; }
    public string getDateCreated() { return this.date_created; }
    public string getDateUpdated() { return this.date_updated; }

    public void setId(string id) { this.id = id; }
    public void setUsername(string username) { this.username = username; }
    public void setAchievedLevel(int achievedLevel) { this.achieved_level = achievedLevel; }
    public void setDifficultyLevel(int difficultyLevel) { this.difficulty_level = difficultyLevel; }
    public void setTimeElapsed(string timeElapsed) { this.time_elapsed = timeElapsed; }
    public void setGoldCollected(int goldCollected) { this.gold_collected = goldCollected; }
    public void setHighscore(int highscore) { this.high_score = highscore; }
    public void setDateCreated(string dateCreated) { this.date_created = dateCreated; }
    public void setDateUpdated(string dateUpdated) { this.date_updated = dateUpdated; }
}
