using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualNovel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneFader _sceneFader;

    [Header("Settings")]
    [SerializeField] private float _timeUntilNextPicture;
    private WaitForSeconds _waitUntilNextPicture;
    [SerializeField] private int _oneSlideCapasity;
    private int _currectFullness = 0;

    [Header("Images")]
    [SerializeField] private List<Image> _images;
    private Queue<Image> _imagesQueue = new Queue<Image>();
    private List<Image> _activatedImages = new List<Image>();

    Image nextImage;

    private void Start()
    {
        _waitUntilNextPicture = new WaitForSeconds(_timeUntilNextPicture);

        foreach (Image pic in _images)
        {
            pic.gameObject.SetActive(false);
            _imagesQueue.Enqueue(pic);
        }
        StartCoroutine(PictureCarousel());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Skip");
            _sceneFader.FadeIn();
        }
    }

    private IEnumerator PictureCarousel() 
    {
        while (true)
        {

            if (_imagesQueue.Count == 0)
            {
                Debug.Log("Visual novel is done!");
                _sceneFader.FadeIn();
                break;
            }

            if (_currectFullness < _oneSlideCapasity)
            {
                Debug.Log(_currectFullness);
                nextImage = _imagesQueue.Dequeue();
                nextImage.gameObject.SetActive(true);
                _activatedImages.Add(nextImage);
                _currectFullness++;
            }
            else
            {
                foreach (Image image in _activatedImages)
                {
                    image.gameObject.SetActive(false);
                }
                _activatedImages.Clear();
                _currectFullness = 1;

                nextImage = _imagesQueue.Dequeue();
                nextImage.gameObject.SetActive(true);
                _activatedImages.Add(nextImage);
            }

            yield return _waitUntilNextPicture;
        }
        
    }
}
