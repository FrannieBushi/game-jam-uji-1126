using UnityEngine;

public static class RunStats
{
    const string LastKey = "LastRunDistance";
    const string BestKey = "BestRunDistance";

    public static float LastRun => PlayerPrefs.GetFloat(LastKey, 0f);
    public static float BestRun => PlayerPrefs.GetFloat(BestKey, 0f);

    public static void SaveRun(float distance)
    {
        PlayerPrefs.SetFloat(LastKey, distance);
        if (distance > BestRun) PlayerPrefs.SetFloat(BestKey, distance);
        PlayerPrefs.Save();
    }
}