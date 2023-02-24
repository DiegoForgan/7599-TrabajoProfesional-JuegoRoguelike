using System;
using UnityEngine;

public class MidLevelBoss : FinalBossEnemy
{

    //Called by the animation event
    protected override void DestroyEntity()
    {
        // Update game progress record
        // Starts with the first level
        GameProgressManager.SetNexLevel(GameProgressManager.GetNextLevel()+1);

        Player player = GameObject.FindWithTag(PLAYER_TAG).GetComponent<Player>();
        GameProgressManager.SetGoldCollected(player.GetGold());
        
        // Stop the stopwatch
        GameManager.Instance.StopStopWatch();
        TimeSpan ts = GameManager.Instance.GetTimeElapsed();
        // Log elapsed time
        GameProgressManager.AddTimeElapsed(ts);

        // Load next level
        GameManager.Instance.LoadNextLevel();

        // Start the stopwatch
        GameManager.Instance.StartStopWatch();

        Destroy(gameObject);
    }
}
