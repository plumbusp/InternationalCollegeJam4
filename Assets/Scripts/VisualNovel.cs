using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualNovel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _timeUntilNextPicture;
    private WaitForSeconds _waitUntilNextPicture;
    [SerializeField] private int _oneSlideCapasity;
    private int _currectFullness = 0;

    [Header("Images")]
    [SerializeField] private List<Image> _images;

    private Queue<Image> _imagesQueue = new Queue<Image>();

    private Image[] _activatedImages;
    Image nextImage;

    private void Start()
    {
        _activatedImages = new Image[_oneSlideCapasity];
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
        }
    }

    private IEnumerator PictureCarousel() 
    {
        while (true)
        {
            if (_currectFullness < _oneSlideCapasity)
            {
                nextImage = _imagesQueue.Dequeue();
                _activatedImages[_currectFullness] = nextImage;
                nextImage.gameObject.SetActive(true);
            }
            else
            {
                foreach (Image image in _activatedImages)
                {
                    Debug.Log("Deactivates");
                    image.gameObject.SetActive(false);
                }
                Array.Clear(_activatedImages, 0, _activatedImages.Length);

                _currectFullness = 0;
                nextImage = _imagesQueue.Dequeue();
                _activatedImages[_currectFullness] = nextImage;
                nextImage.gameObject.SetActive(true);
            }
            yield return _waitUntilNextPicture;
            if(_imagesQueue.Peek() == null)
            {
                Debug.Log("Visual novel is done!");
                break;
            }
        }
        
    }
}
