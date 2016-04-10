using UnityEngine;
using System.Collections;

public class SoundOperation : ResourceBackedOperation {

    //Takes path to sound file
    public SoundOperation(string path, string novelPath) : base(path, novelPath) {

    }

    public override string getType() {
        return "sound";
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public override bool execute(VisualNovelSystem vns, VisualNovel vn) {
        Debug.Log("This operation is not implemented yet");
        return false;
    }

}
