using UnityEngine;
using TMPro;

public class MenuStats : MonoBehaviour
{
    [SerializeField] private TMP_Text lastRunText;
    [SerializeField] private TMP_Text bestRunText;
    [SerializeField] private string lastFormat = "Last run: {0:0} m";
    [SerializeField] private string bestFormat = "Best run: {0:0} m";

    private void Start()
    {
        if (lastRunText != null) lastRunText.text = string.Format(lastFormat, RunStats.LastRun);
        if (bestRunText != null) bestRunText.text = string.Format(bestFormat, RunStats.BestRun);
    }
}