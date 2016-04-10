using UnityEngine;
using System.Collections;

public class DelayOperation : VisualNovelOperation {

    int wait;

    public DelayOperation(string input) {
        //Get text
        wait = int.Parse(input);
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
        Debug.Log("This operation is not implemented yet");
        return false;
    }
}
