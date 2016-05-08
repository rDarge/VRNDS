using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ChoiceOperation : VisualNovelOperation {

    List<string> choices;

    public ChoiceOperation(string[] tokens) {
        
        //Unity's C# environment is a load of horseapples that can't execute basic String functions,
        //So we're stuck doing this.
        //TODO Abstract this please
        StringBuilder sb = new StringBuilder();
        foreach (string token in tokens) {
            sb.Append(token);
            sb.Append(" ");
        }

        choices = new List<string>();
        choices.AddRange(sb.ToString().Split('|'));
        
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
        vns.choice(choices);
        return true;
    }

    public void close() {
        //nothing
    }
}
