using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartLevelSequence : MonoBehaviour {

    public float speed = 0.1f;

    private Image image;

    private IEnumerator Start()
    {
        image = GetComponent<Image>();
        image.enabled = true;
        image.color = Color.black;

        while (image.color.a > 0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, 0f, speed));
            yield return new WaitForEndOfFrame();
        }

        image.enabled = false;
        yield return new WaitForEndOfFrame();
    }
}
