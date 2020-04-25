
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The simplest SFX System
/// </summary>
public class SFXSystem : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    public struct PlayingSFX
    {
        public AudioClip sfx;
        public float startingTime;
    }

    private List<PlayingSFX> playingSFX = new List<PlayingSFX>();

    public void PlayUniqueSFX(AudioClip sfx)
    {
        if (playingSFX.Exists((x) => x.sfx == sfx))
            return;

        audioSource.PlayOneShot(sfx);
        playingSFX.Add(new PlayingSFX()
        {
            sfx = sfx,
            startingTime = Time.realtimeSinceStartup,
        });
    }

    public void PlaySFX(AudioClip sfx)
    {
        audioSource.PlayOneShot(sfx);
    }


    public void Update()
    {
        for (int i = playingSFX.Count -1; i >=0; i--)
        {
            PlayingSFX p = playingSFX[i];
            if (p.startingTime < Time.realtimeSinceStartup + p.sfx.length)
                playingSFX.RemoveAt(i);
        }
    }

}
