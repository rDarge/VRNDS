using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VisualNovelSystem : MonoBehaviour {

    public Text textWindow;
    public GameObject background;
    public GameObject foregroundImages;
    public GameObject choiceWindow;

    private VisualNovel currentNovel;
    //TODO: Music object

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    //Operations below here are utilized by VisualNovelOperations to make things beautiful
    //In best practice, the logic implemented in this class should relate exclusively to unity gameobjects and UI elements

    /// <summary>
    /// Clears the text on the textWindow;
    /// </summary>
    public void clearText() {
        textWindow.text = "";
    }

    public void clearForeground() {

    }

    public void insertForeGroundImage(GameObject image, int x, int y) {
        //Create a copy of the provided image at the specified coordinates
        Vector3 newPosition = new Vector3(x, y, 5.0f); //TODO get this information from the VisualNovel
        Instantiate(image, newPosition, new Quaternion());
    }
}
