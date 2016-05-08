using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayOperation : VisualNovelOperation {

    int wait;

    public DelayOperation(string[] tokens) {
        //Get text
        wait = int.Parse(tokens[0]);
    }

    //Provide resource path for any resource needed by this operation. 
    //This will allow the VisualNovel to extract that resource, so it can be prepare()'d
    public string getResourcePath() {
        return null;
    }

    //Load image, music, etc
    public void prepare(Dictionary<string, int> variables) {
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

    public void close() {
        //nothing
    }
}
