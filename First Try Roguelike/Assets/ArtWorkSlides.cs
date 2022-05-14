using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtWorkSlides : MonoBehaviour
{
    [SerializeField] Sprite[] artworkImages;
    private Image _imageComponent;
    private int currentArtWorkDisplayed;
    
    private void Awake() {
        _imageComponent = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("number of artworks: "+ artworkImages.Length);
        if (artworkImages.Length > 0){
            _imageComponent.sprite = artworkImages[0];
            currentArtWorkDisplayed = 0;
            InvokeRepeating("ChangeArtworkImage", 6f, 6f);
        }
    }

    void ChangeArtworkImage(){
        
        if (currentArtWorkDisplayed == artworkImages.Length - 1){
            currentArtWorkDisplayed = 0;
        }
        else currentArtWorkDisplayed += 1;
        _imageComponent.sprite = artworkImages[currentArtWorkDisplayed];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
