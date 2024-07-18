using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenAdaptation : MonoBehaviour {

	public enum resolutions
    {
        _4x3 = 0,
        _1x1,
        _16x9,
        _user // beta
    };
    public resolutions currentResolution = resolutions._4x3;

    public bool enable = false;
    public bool saveQuality = true;

    public float targetQuality = 480f;

    public RawImage postRenderer;

    private float targetAspect;
    private float currentAspect = (float)Screen.width/(float)Screen.height;
    private float scaleHeight = 0f;
    private float ratio = 0f;

    private int _targetWidth = 0;
    private int _targetHeight = 0;

    private void Start()
    {
        switch (currentResolution)
        {
            case resolutions._4x3:
                {
                    targetAspect = 4f / 3f;
                    break;
                }
            case resolutions._1x1:
                {
                    targetAspect = 1f / 1f;
                    break;
                }
            case resolutions._16x9:
                {
                    targetAspect = 16f / 9f;
                    break;
                }
            case resolutions._user:
                {
                    targetAspect = Screen.width / Screen.height;
                    break;
                }
        }

        if (enable && postRenderer)
        {
            if (saveQuality)
            {
                _targetWidth = Screen.width;
                _targetHeight = Screen.height;
            } else
            {
                ratio = (float)Screen.height / targetQuality;
                _targetWidth = Mathf.CeilToInt(Screen.width / ratio);
                _targetHeight = (int)targetQuality;
            }

            RenderTexture newRender = new RenderTexture(_targetWidth, _targetHeight, 24);
            newRender.name = "Postrenderer Texture";
            postRenderer.texture = newRender;
            GetComponent<Camera>().targetTexture = newRender;
        }
        else
        {
            if (!enable)
                Debug.Log("<color=red>SCREEN ADAPTATION DISABLED</color>");
            else
                Debug.Log("No postRenderer texture");
            //ErrorLogger.Log("No postRenderer texture");
        }
        scaleHeight = currentAspect / targetAspect;

        if (scaleHeight < 1.0f)
            GetComponent<Camera>().rect = new Rect(new Vector2(0f, (1f - scaleHeight) / 2f), new Vector2(1f, scaleHeight));
        else
        {
            float scaleWidth = 1f / scaleHeight;
            GetComponent<Camera>().rect = new Rect(new Vector2((1f - scaleWidth) / 2f, 0f), new Vector2(scaleWidth, 1f));
        }
    }
}
