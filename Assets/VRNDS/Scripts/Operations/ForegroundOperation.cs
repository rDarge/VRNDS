using UnityEngine;
using System.Collections;
using System;

public class ForegroundOperation : ResourceBackedOperation {

    private int xPosition;
    private int yPosition;

    //Takes path to sound file
    public ForegroundOperation(string path, string novelPath) : base(path.Substring(0, path.IndexOf(' ')), novelPath) {
        string[] splitPath = path.Split(' ');
        xPosition = int.Parse(splitPath[1]);
        yPosition = int.Parse(splitPath[2]);
    }

    public override string getType() {
        return "foreground";
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public override bool execute(VisualNovelSystem vns, VisualNovel vn) {
        Debug.Log("This operation is not implemented yet");
        return false;
    }
}
