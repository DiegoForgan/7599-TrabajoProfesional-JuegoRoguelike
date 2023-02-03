using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class UserSessionData{
    public string username;   
    public string user_role;
    public string session_token;
    public string expires;
    public string date_created;
    public string id;
}*/

/*public class UserRecoveryData{
    public string username;
    public string email;   
    public string recovery_key;
    public string expires;
    public string date_created;
    public string id;
}*/

public class ErrorAPIResponse{
    public int code; 
    public string message;
    public string data;
}

/*public class PaginatedHighScoreResponse{
    public int total;
    public int limit;
    public string next;
    public string previous;
    // WARNING!!! Fix this issue to correct display the highscores
    public List<HighScoresResponse> results;
}*/

/*public class HighScoresResponse{
    public string id;
    public string username;
    public int achieved_level;
    public int difficulty_level;
    public string time_elapsed;
    public int gold_collected;
    public int high_score;
    public string date_created;
    public string date_updated;
}*/

public class GameProgressResponse{
    public string id;
    public string username;
    public int next_level;
    public int difficulty_level;
    public string time_elapsed;
    public int gold_collected;
    public string date_created;
    public string date_updated;
}


