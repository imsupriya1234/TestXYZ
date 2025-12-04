using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioClip cardFlipAudio;
    [SerializeField] AudioClip cardMatchingAudio; 
    [SerializeField] AudioClip cardMisMatchedAudio;  
    [SerializeField] AudioClip winAudio;  
    [SerializeField] AudioClip buttonClickAudio;    
    
    [SerializeField] AudioSource audioPlayer;

    public static AudioController Instance; 

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null) // Creating Singleton - AudioController
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
          
    }
   
    // Audio play Func by passing Audio Clip
    private IEnumerator PlayAudioClip(AudioClip clip) 
    {
        if (clip != null)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
            yield return new WaitForSeconds(clip.length + 1f);
        }
    }

    public void PlayCardFlipSound() // Card flip sound
    {
        StartCoroutine(PlayAudioClip(cardFlipAudio));
    }

    public void PlayCardMatchingSound() // Card Match sound
    {
        StartCoroutine(PlayAudioClip(cardMatchingAudio));
    }

    public void PlayCardMisMatchedSound() // Card mismatch sound
    {
        StartCoroutine(PlayAudioClip(cardMisMatchedAudio));
    }

    public void PlayWinPopUpSound() // Win sound
    {
        StartCoroutine(PlayAudioClip(winAudio));
    }

    public void PlayButtonClickSound() // Button click sound
    {
        StartCoroutine(PlayAudioClip(buttonClickAudio));
    }
}
