using System;
using System.Collections.Generic;
using System.Text;

public class TextOperation : VisualNovelOperation {

    string text;
    bool halt = true;

    public TextOperation(string[] tokens) {
        //Get text

        //Unity's C# environment is a load of horseapples that can't execute basic String functions,
        //So we're stuck doing this.
        //TODO Abstract this please
        StringBuilder sb = new StringBuilder();
        foreach(string token in tokens) {
            sb.Append(token);
            sb.Append(" ");
        }

        text = sb.ToString();

        if (text.StartsWith("@")) {
            halt = false;
        }
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
        if(text.Equals("~")) {
            vns.clearText();
        } else {
            vns.setText(text);
        }
        return halt;
    }

    public void close() {
        //nothing
    }
}
