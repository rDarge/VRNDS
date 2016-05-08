using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChoiceRig : MonoBehaviour {

    public Button[] buttons;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setOptions(List<string> options) {
        //Enable these buttons and fill them with the choices
        for(int x = 0; x < options.Count; x++) {
            if(buttons.Length > x) {
                buttons[x].gameObject.SetActive(true);
                buttons[x].GetComponentInChildren<Text>().text = options[x];
            }
        }
       
        //Disable the other buttons
        for(int x = options.Count; x < buttons.Length; x++) {
            buttons[x].gameObject.SetActive(false);
        }
    }
}
