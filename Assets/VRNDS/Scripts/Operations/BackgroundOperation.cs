using UnityEngine;
using System.Collections;
using System.IO;
using Ionic.Zip;

public class BackgroundOperation : ResourceBackedOperation {

    //Takes path to sound file
    public BackgroundOperation(string path, string novelPath) : base(path, novelPath) {

    }

    public override string getType() {
        return "background";
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public override bool execute(VisualNovelSystem vns, VisualNovel vn) {
        vns.prepareBackgroundImage(resourceStream);
        return false;
    }
}
