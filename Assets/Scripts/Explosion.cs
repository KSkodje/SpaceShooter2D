using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionSound = null;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (!_audioSource)
        {
            Debug.Log("AudioSource on Explosion is Null");
        }
        else
        {
            _audioSource.clip = _explosionSound;
            _audioSource.volume = 0.6f;
            _audioSource.pitch = 1.5f;
            _audioSource.Play();
        }
        Destroy(this.gameObject, 3f);
    }
}
