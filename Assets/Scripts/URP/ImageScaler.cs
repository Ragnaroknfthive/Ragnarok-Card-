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
        // Obtiene el tama�o de la c�mara en unidades del mundo
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calcula la escala necesaria para ajustar la imagen al tama�o de la c�mara
        float scaleX = cameraWidth / imageResize.rectTransform.sizeDelta.x;
        float scaleY = cameraHeight / imageResize.rectTransform.sizeDelta.y;

        // Aplica la escala al objeto Image
        imageResize.rectTransform.localScale = new Vector3(scaleX + ajusteX, scaleY + ajusteY, 1f);

        // Ajusta la posici�n para centrarla en la c�mara
        Vector3 cameraPosition = mainCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
    }
}
