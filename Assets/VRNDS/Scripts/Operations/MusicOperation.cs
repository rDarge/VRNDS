using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Ionic.Zip;

public class MusicOperation : ResourceBackedOperation {

    //Takes path to sound file
    public MusicOperation(string path, string novelPath) : base(path, novelPath) {

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
