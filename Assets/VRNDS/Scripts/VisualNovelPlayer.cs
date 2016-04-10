using UnityEngine;
using System.Collections;

public class VisualNovelPlayer : MonoBehaviour {

    public VisualNovelSystem vns;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("Button A")) {
            vns.step();
        }
	}
}
