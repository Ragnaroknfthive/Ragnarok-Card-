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
        // Obtiene el tama�o de la c�mara en unidades del mundo
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Obtiene el tama�o original del sprite
        float spriteHeight = imageResize.sprite.bounds.size.y;
        float spriteWidth = imageResize.sprite.bounds.size.x;

        // Calcula la escala necesaria para ajustar el sprite al tama�o de la c�mara
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        // Aplica la escala al objeto SpriteRenderer
        imageResize.transform.localScale = new Vector3(scaleX + ajusteX, scaleY + ajusteY, 1f);

        // Ajusta la posici�n para centrarla en la c�mara
        Vector3 cameraPosition = mainCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);

    }
}
