using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public GameObject bonusAudio, treasureAudio;

    public void bonusAudioPlay()
    {
        bonusAudio.GetComponent<AudioSource>().Play();
    }

    public void treasureAudioPlay()
    {
        treasureAudio.GetComponent<AudioSource>().Play();
    }
}
