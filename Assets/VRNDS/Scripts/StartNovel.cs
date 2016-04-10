using UnityEngine;
using System.Collections;

public class StartNovel : MonoBehaviour {
    
    //Link these two together
    public VisualNovel novel;
    public VisualNovelSystem system;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void startNovel() {
        system.startNovel(novel);
    }
}
