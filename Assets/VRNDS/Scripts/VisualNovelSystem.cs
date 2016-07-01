using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;

public class VisualNovelSystem : MonoBehaviour {

    public static string LIBRARY_DIRECTORY = "/storage/sdcard0/vnds/novels/";
    public static int BACKGROUND_HEIGHT = 192;
    public static int BACKGROUND_WIDTH = 256;

    public Text textWindow;
    public Text textLog;
    public Slider loadingSlider;
    public AudioSource soundPlayer;
    public AudioSource musicPlayer;
    private string text;

    public GameObject background;
    public GameObject foreground;
    public GameObject novelSelector;
    public GameObject choiceWindow;

    public GameObject novelIconPrefab;
    public GameObject foregroundSpritePrefab;

    public GameObject[] turnOnWithNovel;

    private VisualNovel currentNovel;
    private List<VisualNovel> novels;
    private List<Thread> scanThreads;

    private float waitTime;

    private Dictionary<string, int> variables; //State object related to choice operations
    private string prepareToJump;
    private WWW nextBackgroundImage;
    private Queue<ForegroundOperation> nextForegroundImages;
    private List<GameObject> currentForegroundImages;

    private int messageLogOffset;
    private StringBuilder messageLog;
    private TextGenerator proofReader;
    private TextGenerationSettings proofReaderSettings;

    // Use this for initialization
    void Start() {
        hideNovel();
        buildListOfNovels();
        nextForegroundImages = new Queue<ForegroundOperation>();
        currentForegroundImages = new List<GameObject>();

        //Set up the text generator
        messageLogOffset = 0;
        messageLog = new StringBuilder();
        proofReader = new TextGenerator();
        proofReaderSettings = textLog.GetGenerationSettings(new Vector2(textLog.rectTransform.rect.width, textLog.rectTransform.rect.height));
    }

    // Update is called once per frame
    void Update() {

        if(currentNovel != null) {
            currentNovel.update(this);
        }

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

        if(nextForegroundImages.Count > 0) {
            loadReadyForegroundImages();
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
        Vector3 novelPosition = new Vector3(-270, 250, 0);
        Vector3 incrementPositionX = new Vector3(105, 0, 0);
        Vector3 incrementPositionY = new Vector3(0, -115, 0);

        foreach (string folder in novelFolders) {

            try {
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

                if(novelPosition.x > 270) {
                    novelPosition.x = -270;
                    novelPosition += incrementPositionY;
                }

            } catch (System.Exception e) {
                Debug.Log("Could not parse directory " + folder + "\n\n because of " + e.Message);
            }
        }
    }

    public void startNovel(VisualNovel novel) {
        currentNovel = novel;

        showNovel();

        //Pass the background dimensions to the novel so it can scale properly
        Vector3 bgScale = background.transform.localScale;
        novel.scaleToBackground(bgScale.x, bgScale.y);

        novel.start();
    }

    public void close() {
        this.currentNovel.close();
        this.messageLog = new StringBuilder();
        cancelChoice();
        hideNovel();
    }

    public void showNovel() {
        novelSelector.SetActive(false);

        //foreach (GameObject obj in turnOnWithNovel) {
        //    obj.SetActive(true);
        //}
    }

    public void hideNovel() {
        //foreach (GameObject obj in turnOnWithNovel) {
        //    obj.SetActive(false);
        //}

        novelSelector.SetActive(true);
        this.textWindow.text = "Please select a novel";
    }

    public void step() {
        if (text.Length != textWindow.text.Length) {
            textWindow.text = text;
            wait(1);
        } else if (currentNovel != null && !choiceWindow.activeSelf && currentNovel.ready() && Time.realtimeSinceStartup > waitTime) {
            currentNovel.step();
            updateLoadingBar();
        }
    }

    //Operations below here are utilized by VisualNovelOperations to make things beautiful
    //In best practice, the logic implemented in this class should relate exclusively to unity gameobjects and UI elements

    public void updateLoadingBar() {
        loadingSlider.value = currentNovel.getProgress();
    }

    public void wait(float seconds) {
        waitTime = Time.realtimeSinceStartup + seconds;
    }

    private void updateMessageLog() {
        if (textLog != null) {
            string logSnippet = messageLog.ToString().Substring(messageLogOffset);
            proofReader.Populate(logSnippet, proofReaderSettings);
            while (logSnippet.Length > proofReader.characterCountVisible) {
                logSnippet = logSnippet.Substring(textLog.text.IndexOf('\n') + 1);
            }
            messageLogOffset = messageLog.Length - logSnippet.Length;
            textLog.text = logSnippet;
        }
    }

    /// <summary>
    /// Clears the text on the textWindow;
    /// </summary>
    public void clearText() {
        if(textLog != null) {
            messageLog.Append("\n\n");
            messageLog.Append(textWindow.text);

            updateMessageLog();
        }
        textWindow.text = "";
    }

    public void setText(string text) {
        clearText();
        this.text = text;
    }

    public string getText() {
        return this.text;
    }

    public void clearForeground() {
        while(currentForegroundImages.Count > 0) {
            Destroy(currentForegroundImages[0]);
            currentForegroundImages.RemoveAt(0);
        }
    }

    public void playSound(AudioClip sound) {
        soundPlayer.clip = sound;
        soundPlayer.Play();
    }

    public void playMusic(AudioClip music) {
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    public void stopMusic() {
        musicPlayer.Stop();
    }

    public void choice(List<string> options) {
        choiceWindow.SetActive(true);
        ChoiceRig choiceRig = choiceWindow.GetComponent<ChoiceRig>();
        choiceRig.setOptions(options);

    }

    public void cancelChoice() {
        choiceWindow.SetActive(false);
    }

    public void choose(int value) {
        variables.Remove("selected");
        variables.Add("selected", value);
        choiceWindow.SetActive(false);
        step();
    }

    public int getVariable(string key) {
        int value = 0;
        if(variables.ContainsKey(key)) {
            value = variables[key];
        }
        return value;
    }

    public void setVariable(string key, int value) {
        if(variables.ContainsKey(key)) {
            variables.Remove(key);
        }
        variables.Add(key, value);
    }

    public Dictionary<string, int> getVariables() {
        return variables;
    }

    //Call this from the backgroundOperation to queue up a new background image
    public void prepareBackgroundImage(WWW nextBackgroundImage) {
        this.nextBackgroundImage = nextBackgroundImage;
    }

    //Call this from the backgroundOperation to queue up a new background image
    public void prepareForegroundImage(ForegroundOperation nextForegroundImage) {
        this.nextForegroundImages.Enqueue(nextForegroundImage);
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
        string debugName = "ERROR";
        if(nextForegroundImages.Count > 0 && nextForegroundImages.Peek().isReady()) {
            ForegroundOperation nextImageOp = nextForegroundImages.Dequeue();
            debugName = nextImageOp.getResourcePath();
            WWW nextImage = nextImageOp.getStream();
            try {
                Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
                nextImage.LoadImageIntoTexture(texture);
                nextImage.Dispose();
                nextImage = null;

                GameObject newSprite = GameObject.Instantiate<GameObject>(foregroundSpritePrefab);
                newSprite.SetActive(true);
                newSprite.transform.parent = foreground.transform;
                newSprite.transform.localScale = currentNovel.getForegroundScale();

                float newX = nextImageOp.getX();
                float newY = - (currentNovel.getForegroundHeight(texture) + nextImageOp.getY());
                newSprite.transform.localPosition = new Vector3(newX, newY, 0);
                newSprite.transform.localRotation = new Quaternion();
                SpriteRenderer renderer = newSprite.GetComponent<SpriteRenderer>();
                renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());

                //Keep track of the current foreground images
                currentForegroundImages.Add(newSprite);

                //background.transform.GetComponent<Renderer>().material.mainTexture = texture;
            } catch (System.Exception e) {
                Debug.Log("Could not load texture from " + debugName + " into a texture, skipping operation! " + e.Message);
            }
        }
    }

    public void jump(string newScriptPath) {
        //Do this async so we don't blow up the rest of the potential operation evaluations this tic
        prepareToJump = newScriptPath;
    }
}
