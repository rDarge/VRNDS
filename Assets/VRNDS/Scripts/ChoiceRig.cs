using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChoiceRig : MonoBehaviour {

    public Button[] buttons;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setOptions(string[] options) {
        //Enable these buttons and fill them with the choices
        for(int x = 0; x < options.Length; x++) {
            if(buttons.Length > x) {
                buttons[x].gameObject.SetActive(true);
                buttons[x].GetComponentInChildren<Text>().text = options[x];
            }
        }
       
        //Disable the other buttons
        for(int x = options.Length; x < buttons.Length; x++) {
            buttons[x].gameObject.SetActive(false);
        }
    }
}
