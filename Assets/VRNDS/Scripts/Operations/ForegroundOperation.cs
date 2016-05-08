using UnityEngine;
using System.Collections;
using System;

public class ForegroundOperation : ResourceBackedOperation {

    private int xPosition;
    private int yPosition;

    //Takes path to sound file
    public ForegroundOperation(string[] tokens, string novelPath) : base(tokens[0], novelPath) {
        xPosition = int.Parse(tokens[1]);
        yPosition = int.Parse(tokens[2]);
    }

    public override string getType() {
        return "foreground";
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public override bool execute(VisualNovelSystem vns, VisualNovel vn) {
        vns.prepareForegroundImage(this);
        return false;
    }

    public int getX() {
        return xPosition;
    }

    public int getY() {
        return yPosition;
    }
}
