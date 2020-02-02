using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button)), RequireComponent(typeof(AudioSource))]
public class AudioButton : MonoBehaviour
{
    AudioSource source;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        source = GetComponent<AudioSource>();
    }

    private void OnClick()
    {
        source.Play();
    }
}
