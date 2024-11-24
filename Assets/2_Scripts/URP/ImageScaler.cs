// Code for scaling an image to fit the camera's size.
// The image is a child of the object that has this script attached.
// The script will adjust the image's scale to fit the camera's size.
// The script also adjusts the image's position to center it in the camera.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ImageScaler : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image imageResize;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float ajusteX;
    [SerializeField] private float ajusteY;

    private void Update()
    {
        //Get the camera size in world units
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        //Calculate the scale needed to adjust the image to the camera size
        float scaleX = cameraWidth / imageResize.rectTransform.sizeDelta.x;
        float scaleY = cameraHeight / imageResize.rectTransform.sizeDelta.y;

        //Apply the scale to the Image object
        imageResize.rectTransform.localScale = new Vector3(scaleX + ajusteX, scaleY + ajusteY, 1f);

        //Adjust the position to center it in the camera
        Vector3 cameraPosition = mainCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
    }
}
