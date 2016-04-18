using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

public class VisualNovel {

    //Dictionary<string,VisualNovelScript> scripts; //Table of scripts, mapped by filename. Load the first one (alphabetically).
    VisualNovelScript currentScript;    //Current script for convenience's sake
    public static string cacheDirectory;
    string novelDirectory;
    string title;           //Title of the novel. 
    string currentScriptName;   //Name of the current loaded script. Start with the first alphabetic entry in /scripts. Start executing that script immediately upon loading.
    int currentIndex;    //Name of the currently loaded script's index. Starts at 0.

    int height;             //Not sure if we'll need these yet
    int width;

    private WWW iconStream;
    private WWW thumbStream;
    private Texture2D icon;
    private Texture2D thumbnail;
    private Dictionary<string, string> infoMap;

    //TODO: Some type of save functionality

    public VisualNovel(string novelDirectory) {
        this.novelDirectory = novelDirectory;
        cacheDirectory = Application.persistentDataPath + "/temp";
        setup();
    }
    
    public void setup() {
        //Populate info map
        infoMap = new Dictionary<string, string>();

        //Typically this contains the "title" parameter
        using (StreamReader sr = new StreamReader(novelDirectory + "/info.txt")) {
            // Read the stream to a string, and write the string to the console.
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                string key = line.Substring(0, line.IndexOf('='));
                infoMap.Add(key, line.Substring(key.Length + 1));
            }
        }

        //Typically this will contain the height/width parameters for the novel
        string imageInitializer = novelDirectory + "/img.ini";
        if (File.Exists(imageInitializer)) {
            using (StreamReader sr = new StreamReader(imageInitializer)) {
                // Read the stream to a string, and write the string to the console.
                while (!sr.EndOfStream) {
                    string line = sr.ReadLine();
                    string key = line.Substring(0, line.IndexOf('='));
                    infoMap.Add(key, line.Substring(key.Length + 1));
                }
            }
        } else {
            infoMap.Add("height", "dynamic");
            infoMap.Add("width", "dynamic");
        }
        

        setTitleFromInfoMap();
        loadThumbnail();
        loadIcon();
        Debug.Log("Loaded novel " + title);
    }

    public void setTitleFromInfoMap() {
        //Get the title from the information map, assuming it's been parsed out
        if (infoMap != null && infoMap.ContainsKey("title")) {
            title = infoMap["title"];
        } else {
            title = novelDirectory;
        }
    }

    public void loadThumbnail() {
        //Load it! (we assume this is pretty quick, since it's hard to multithread)
        thumbStream = new WWW("file://" + novelDirectory + "/thumbnail.png");
        //Store it!
        thumbnail = new Texture2D(4, 4, TextureFormat.DXT1, false);
        thumbStream.LoadImageIntoTexture(thumbnail);
        thumbStream.Dispose();
        thumbStream = null;
    }

    public void loadIcon() {
        //Load it! (we assume this is pretty quick, since it's hard to multithread)
        iconStream = new WWW("file://" + novelDirectory + "/icon.png");
        //Store it!
        icon = new Texture2D(4, 4, TextureFormat.DXT1, false);
        iconStream.LoadImageIntoTexture(icon);
        iconStream.Dispose();
        iconStream = null;
    }

    public Texture2D getThumbnail() {
        return thumbnail;
    }

    public Texture2D getIcon() {
        return icon;
    }

    public string getTitle() {
        return title;
    }

    public void start() {
        Debug.Log("About to parse novel in " + novelDirectory);
        string[] subFolders = Directory.GetDirectories(novelDirectory);
        string[] topLevelFiles = Directory.GetFiles(novelDirectory);

        //Find out where our scripts live
        Debug.Log(cacheDirectory);
        if (Directory.Exists(cacheDirectory)) {
            try {
                Directory.Delete(cacheDirectory, true);
            } catch (System.Exception e) {
                Debug.Log("Couldn't delete stuff! IDK Why!");
            }
        }

        //Extract the main script file
        string scriptPath = novelDirectory + "/script/main.scr"; //By default assume it's in the normal script folder
        string scriptZipPath = novelDirectory + "/script.zip";
        if (File.Exists(scriptZipPath)) {
            //Extract main.scr to start with

            using (ZipFile scriptZip = ZipFile.Read(scriptZipPath)) {

                try {
                    scriptZip.ExtractSelectedEntries("name = main.scr", null, cacheDirectory);
                    scriptPath = cacheDirectory + "/script/main.scr"; //If we find it in the zip, update the path
                    //foreach (ZipEntry e in scriptZip.Entries) {
                    //    e.Extract(cacheDirectory);
                    //}
                } catch (System.Exception e) {
                    Debug.Log("Error extracting main.scr: " + e.Message);
                }       
            }
        }
        
        //Read the main script file
        currentScript = new VisualNovelScript(scriptPath, novelDirectory);




        //Extract scripts
        //Load first script?
    }

    public void step(VisualNovelSystem vns) {
        currentIndex = currentScript.step(vns, this);
    }

    public void loadScript(string scriptPath) {
        //Todo make sure this isn't blocking if it's a large cleanup.
        currentScript.close();                
        
        currentIndex = 0;
        currentScriptName = scriptPath;
        currentScript = new VisualNovelScript(scriptPath, novelDirectory);
    }

    public bool ready() {
        return currentScript != null && currentScript.ready();
    }

    public float getProgress() {
        return currentScript.getProgress();
    }

    public void save(int saveNumber) {
        //Do saving things here, write script name/index to file here
    }

    public void load(int saveNumber) {
        //Read from save states, jump to script name/index.
    }

    public void close() {
        //Clean everything up.
    }
}
