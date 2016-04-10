using UnityEngine;
using System.Collections;

public class TextOperation : VisualNovelOperation {

    string text;

    public TextOperation(string input) {
        //Get text
        text = input;
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
        if(text.Equals("~")) {
            vns.clearText();
        } else {
            vns.setText(text);
        }
        return true;
    }
}
