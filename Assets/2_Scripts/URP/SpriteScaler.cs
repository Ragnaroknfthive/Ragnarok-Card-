// Code for scaling a sprite to fit the camera view.
// The script resizes the sprite to fit the camera view by adjusting the scale of the SpriteRenderer component.
// The script calculates the scale based on the size of the camera and the original sprite size.
// The script also centers the sprite in the camera view by adjusting its position.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SpriteScaler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer imageResize;
    [SerializeField] private Camera mainCamera;
    private float ajusteX;
    private float ajusteY;


    private void Update()
    {
        //Get the size of the camera in world units
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        //Get the original size of the sprite
        float spriteHeight = imageResize.sprite.bounds.size.y;
        float spriteWidth = imageResize.sprite.bounds.size.x;

        //Calculate the scale needed to fit the sprite to the camera size
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        //Apply the scale to the SpriteRenderer object
        imageResize.transform.localScale = new Vector3(scaleX + ajusteX, scaleY + ajusteY, 1f);

        //Adjust the position to center it in the camera
        Vector3 cameraPosition = mainCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);

    }
}
