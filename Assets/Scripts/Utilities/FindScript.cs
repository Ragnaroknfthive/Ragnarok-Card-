using System.Collections.Generic;
using UnityEngine;

public class FindScript : MonoBehaviour
{
    public string scriptName = "SpellCardDisplay";

    void Update()
    {
        List<MonoBehaviour> scripts = new List<MonoBehaviour>();

        foreach (var script in FindObjectsOfType<MonoBehaviour>())
        {
            if (script.GetType().Name == scriptName)
            {
                scripts.Add(script);
            }
        }

        if (scripts.Count == 0)
        {
            //Debug.Log($"No se encontró ningún objeto con el script '{scriptName}' en la escena.");
        }
        else
        {
            foreach (var script in scripts)
            {
                //Debug.Log("El script está asignado a: " + script.gameObject.name);
            }
        }
    }
}
