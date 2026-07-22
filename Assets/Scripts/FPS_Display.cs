using UnityEngine;
using TMPro;

public class FPS_Display : MonoBehaviour
{
    public TextMeshProUGUI FpsText;
    public TextMeshProUGUI FrameTime;
    private float pollingTime = 1.0f;
    private float time;
    private int frameCount;

    void Start()
    {
        // Make the game run as fast as possible
        Application.targetFrameRate = -1;
        // Limit the framerate to 60
        //Application.targetFrameRate = 480;
    }

    void Update()
    {
        time += Time.deltaTime;
        frameCount++;
        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            float frameTime = (time / frameCount) * 1000;
            FpsText.text = frameRate.ToString() + " fps";
            FrameTime.text = frameTime.ToString() + " ms";
            time -= pollingTime;
            frameCount = 0;
        }
    }
}
