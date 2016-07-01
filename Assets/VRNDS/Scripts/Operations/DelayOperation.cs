using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayOperation : VisualNovelOperation {

    private static float convertToSeconds = 1/60f;
    private float wait;
    

    public DelayOperation(string[] tokens) {
        //Get text
        wait = int.Parse(tokens[0]) * convertToSeconds;
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
        vns.wait(wait);
        return true;
    }

    public void close() {
        //nothing
    }
}
