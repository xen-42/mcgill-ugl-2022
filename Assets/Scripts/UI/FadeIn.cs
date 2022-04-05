using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] public Image image;
    [SerializeField] public float fadeTime = 1f;
    
    private float _timer = 0f;
    private bool _isFading = false;

    private void Awake()
    {
        _timer = fadeTime;

        EventManager.AddListener("FadeIn", StartFade);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("FadeIn", StartFade);
    }

    public void StartFade()
    {
        _isFading = true;
    }

    private void Update()
    {
        if(_isFading)
        {
            _timer -= Time.deltaTime;
            if(_timer <= 0f)
            {
                _timer = 0f;
                _isFading = false;
                gameObject.SetActive(false);
            }

            var alpha = Mathf.Lerp(0f, 1f, _timer / fadeTime);
            image.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
