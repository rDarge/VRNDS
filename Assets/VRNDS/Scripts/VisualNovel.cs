using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualNovel {

    //Dictionary<string,VisualNovelScript> scripts; //Table of scripts, mapped by filename. Load the first one (alphabetically).
    VisualNovelScript currentScript;    //Current script for convenience's sake
    string novelDirectory;
    string title;           //Title of the novel. 
    string currentScriptName;   //Name of the current loaded script. Start with the first alphabetic entry in /scripts. Start executing that script immediately upon loading.
    int currentIndex;    //Name of the currently loaded script's index. Starts at 0.

    int height;             //Not sure if we'll need these yet
    int width;              

    //TODO: Some type of save functionality
    
    public void setup(string novelDirectory) {
        this.novelDirectory = novelDirectory;
        
        //Get title

        //Extract scripts

        //Load first script

    }

    public void step(VisualNovelSystem vns) {
        currentIndex = currentScript.step(vns, this);
    }

    public void loadScript(string scriptName) {
        currentScript.close();                //Todo make sure this isn't blocking if it's a large cleanup.
        currentScriptName = scriptName;
        currentIndex = 0;
        currentScript.load(scriptName, this); //Todo is this the best way to do this? Look up listener patterns
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
