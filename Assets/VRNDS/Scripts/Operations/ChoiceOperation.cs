using UnityEngine;
using System.Collections;

public class ChoiceOperation : VisualNovelOperation {

    string[] choices;

    public ChoiceOperation(string input) {
        //Get text
        choices = input.Split('|');
    }

    //Provide resource path for any resource needed by this operation. 
    //This will allow the VisualNovel to extract that resource, so it can be prepare()'d
    public string getResourcePath() {
        return null;
    }

    //Load image, music, etc
    public void prepare() {
        //Do nothing
    }

    //Check if stream is ready
    public bool isReady() {
        return true;
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public bool execute(VisualNovelSystem vns, VisualNovel vn) {
        vns.choice(choices);
        return true;
    }
}
