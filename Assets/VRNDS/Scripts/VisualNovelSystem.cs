using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class VisualNovelSystem : MonoBehaviour {

    public static string LIBRARY_DIRECTORY = "/storage/extSdCard/vnds/";

    public Text textWindow;
    private string text;

    public GameObject background;
    public GameObject foreground;
    public GameObject novelSelector;
    public GameObject choiceWindow;

    public GameObject novelIconPrefab;

    private VisualNovel currentNovel;

    private List<VisualNovel> novels;
    private List<Thread> scanThreads;

    private Dictionary<string, int> variables; //State object related to choice operations
    private string prepareToJump;
    private WWW nextBackgroundImage;

    // Use this for initialization
    void Start() {
        buildListOfNovels();
    }

    // Update is called once per frame
    void Update() {
        if (text != null && textWindow.text.Length < text.Length) {
            textWindow.text += text[textWindow.text.Length];
        }

        if(prepareToJump != null) {
            currentNovel.loadScript(prepareToJump);
            prepareToJump = null;
        }
        
        if(nextBackgroundImage != null && nextBackgroundImage.isDone) {
            loadNextBackgroundImage();
        }
    }


    void buildListOfNovels() {
        scanThreads = new List<Thread>();
        novels = new List<VisualNovel>();
        variables = new Dictionary<string, int>();

        string[] novelFolders = Directory.GetDirectories(LIBRARY_DIRECTORY);

        //Scan all the novels
        Vector3 novelPosition = new Vector3(0, 0, 0);
        Vector3 incrementPositionX = new Vector3(105, 0, 0);
        Vector3 incrementPositionY = new Vector3(0, 80, 0);

        foreach (string folder in novelFolders) {
            //Create the visual novel entity
            VisualNovel novel = new VisualNovel(folder);

            //Create an accompanying novel icon 
            GameObject newNovelIcon = (GameObject)Instantiate(novelIconPrefab, novelPosition, gameObject.transform.rotation);
            newNovelIcon.transform.parent = novelSelector.gameObject.transform;
            newNovelIcon.transform.localScale = new Vector3(1, 1, 1);
            newNovelIcon.transform.localPosition = novelPosition;

            //Update the button's icon and text to that of the novel
            Button novelButton = newNovelIcon.GetComponent<Button>();
            novelButton.image.sprite = Sprite.Create(novel.getThumbnail(), new Rect(0, 0, 100, 75), new Vector2(0, 0), .01f);
            Text novelText = newNovelIcon.GetComponentInChildren<Text>();
            novelText.text = novel.getTitle();
            StartNovel theNovelStarter = newNovelIcon.GetComponent<StartNovel>();
            theNovelStarter.novel = novel;
            theNovelStarter.system = this;

            //Increment the position that the next icon will be created at
            novelPosition += incrementPositionX;
        }
    }

    public void startNovel(VisualNovel novel) {
        currentNovel = novel;

        //Hide the things we don't want to show anymore
        novelSelector.SetActive(false);

        //Show the main screen
        foreground.SetActive(true);
        background.SetActive(true);

        novel.start();
    }

    public void step() {
        if (currentNovel != null && !choiceWindow.activeSelf) {
            currentNovel.step(this);
        }
    }



    //Operations below here are utilized by VisualNovelOperations to make things beautiful
    //In best practice, the logic implemented in this class should relate exclusively to unity gameobjects and UI elements

    /// <summary>
    /// Clears the text on the textWindow;
    /// </summary>
    public void clearText() {
        textWindow.text = "";
    }

    public void setText(string text) {
        clearText();
        this.text = text;
    }

    public void clearForeground() {

    }

    public void choice(string[] options) {
        choiceWindow.SetActive(true);
        ChoiceRig choiceRig = choiceWindow.GetComponent<ChoiceRig>();
        choiceRig.setOptions(options);

    }

    public void choose(int value) {
        variables.Remove("selected");
        variables.Add("selected", value);
        choiceWindow.SetActive(false);
    }

    public int getVariable(string key) {
        int value = 0;
        if(variables.ContainsKey(key)) {
            value = variables[key];
        }
        return value;
    }

    public void insertForeGroundImage(GameObject image, int x, int y) {
        //Create a copy of the provided image at the specified coordinates
        Vector3 newPosition = new Vector3(x, y, 5.0f); //TODO get this information from the VisualNovel
        Instantiate(image, newPosition, new Quaternion());
    }

    //Call this from the backgroundOperation to queue up a new background image
    public void prepareBackgroundImage(WWW nextBackgroundImage) {
        this.nextBackgroundImage = nextBackgroundImage;
    }

    //Part 2 of the above, this is called once the WWW clears
    private void loadNextBackgroundImage() {
        Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
        nextBackgroundImage.LoadImageIntoTexture(texture);
        nextBackgroundImage.Dispose();
        nextBackgroundImage = null;

        clearForeground();
        background.transform.GetComponent<Renderer>().material.mainTexture = texture;
    }

    public void jump(string newScriptPath) {
        //Do this async so we don't blow up the rest of the potential operation evaluations this tic
        prepareToJump = newScriptPath;
    }
}
