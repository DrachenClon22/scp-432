using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroSequence : MonoBehaviour {

    public VideoClip[] introClips;
    public VideoPlayer player;

    public RawImage image;

    private IEnumerator Start()
    {
        image.color = Color.black;
        yield return new WaitForSeconds(3);

        for (int i = 0; i < introClips.Length; i++)
        {
            player.clip = introClips[i];
            player.Prepare();

            while (!player.isPrepared)
            {
                yield return new WaitForEndOfFrame();
            }

            image.texture = player.texture;
            image.color = Color.white;
            player.Play();

            while (player.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            
        }

        image.enabled = false;
    }
}
