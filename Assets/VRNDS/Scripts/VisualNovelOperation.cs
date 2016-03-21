using UnityEngine;
using System.Collections;

public interface VisualNovelOperation {

    //Provide resource path for any resource needed by this operation. 
    //This will allow the VisualNovel to extract that resource, so it can be prepare()'d
    string getResourcePath();

    //Load image, music, etc
    void prepare();

    //Check if stream is ready
    bool isReady();

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    bool execute(VisualNovelSystem vns, VisualNovel vn);
}
