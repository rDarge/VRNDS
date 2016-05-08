using UnityEngine;
using System.Collections;

public class GazeInputButtonScript : MonoBehaviour {

    public float waitTimeInSeconds = 2;
    protected bool selected;

    // Use this for initialization
    void Start() {
        selected = false;
    }

    // Update is called once per frame
    void Update() {

    }

    public void select() {
        Debug.Log("Selecting a gazeInput");
        selected = true;
    }

    public void deselect() {
        Debug.Log("Deselecting a gazeInput");
        selected = false;
    }
}
