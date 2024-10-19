using UnityEngine;
using System.Collections;


/// <summary>
/// Read version and print text version on screen (attached to a Gameobject with TMPRO.TextMesh 
/// </summary>
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class DisplayVersion : MonoBehaviour {

	// Use this for initialization
	void Start () {    
            this.GetComponent<TMPro.TextMeshProUGUI>().text = "version "+Application.version;
        }
}