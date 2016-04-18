using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;

public class VisualNovelSystem : MonoBehaviour {

    public static string LIBRARY_DIRECTORY = "/storage/sdcard0/vnds/";

    public Text textWindow;
    public Slider loadingSlider;
    private string text;

    public GameObject background;
    public GameObject foreground;
    public GameObject novelSelector;
    public GameObject choiceWindow;

    public GameObject novelIconPrefab;
    public GameObject foregroundSpritePrefab;

    private VisualNovel currentNovel;

    private List<VisualNovel> novels;
    private List<Thread> scanThreads;

    private Dictionary<string, int> variables; //State object related to choice operations
    private string prepareToJump;
    private WWW nextBackgroundImage;
    private List<WWW> nextForegroundImages;

    // Use this for initialization
    void Start() {
        buildListOfNovels();
        nextForegroundImages = new List<WWW>();
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

        if (currentNovel != null && !currentNovel.ready()) {
            updateLoadingBar();
        }
    }


    void buildListOfNovels() {
        scanThreads = new List<Thread>();
        novels = new List<VisualNovel>();
        variables = new Dictionary<string, int>();

        string[] novelFolders = Directory.GetDirectories(LIBRARY_DIRECTORY);

        //Scan all the novels
        Vector3 novelPosition = new Vector3(-300, 200, 0);
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
        if (currentNovel != null && !choiceWindow.activeSelf && currentNovel.ready()) {
            currentNovel.step(this);
            updateLoadingBar();
        }
    }



    //Operations below here are utilized by VisualNovelOperations to make things beautiful
    //In best practice, the logic implemented in this class should relate exclusively to unity gameobjects and UI elements

    public void updateLoadingBar() {
        loadingSlider.value = currentNovel.getProgress();
    }

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

    //Call this from the backgroundOperation to queue up a new background image
    public void prepareBackgroundImage(WWW nextBackgroundImage) {
        this.nextBackgroundImage = nextBackgroundImage;
    }

    //Call this from the backgroundOperation to queue up a new background image
    public void prepareForegroundImage(WWW nextForegroundImage) {
        this.nextForegroundImages.Add(nextForegroundImage);
        //TODO figured out the coordinate system, now create a shell class to hold the coordinates and then instantiate and apply texture to new sprite
    }

    //Part 2 of the above, this is called once the WWW clears
    private void loadNextBackgroundImage() {
        try {
            Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
            nextBackgroundImage.LoadImageIntoTexture(texture);
            nextBackgroundImage.Dispose();
            nextBackgroundImage = null;

            clearForeground();
            background.transform.GetComponent<Renderer>().material.mainTexture = texture;
        } catch (System.Exception e) {
            Debug.Log("Could not load texture from " + nextBackgroundImage.url + " + into a texture, skipping operation!");
        }
        
    }

    private void loadReadyForegroundImages() {
        if(nextForegroundImages.Count > 0) {
            WWW nextImage = nextForegroundImages[0];
            try {
                Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
                nextImage.LoadImageIntoTexture(texture);
                nextImage.Dispose();
                nextImage = null;

                
                background.transform.GetComponent<Renderer>().material.mainTexture = texture;
            } catch (System.Exception e) {
                Debug.Log("Could not load texture from " + nextBackgroundImage.url + " + into a texture, skipping operation!");
            }
        }
    }

    public void jump(string newScriptPath) {
        //Do this async so we don't blow up the rest of the potential operation evaluations this tic
        prepareToJump = newScriptPath;
    }
}
