using UnityEngine;
using System.Collections;

public class VariableOperation : VisualNovelOperation {

    string variable;
    int value;
    bool clear = false;

    public VariableOperation(string input) {
        //Split up the input into all it's important parts
        string[] tokens = input.Split(' ');
        
        //Get variable name
        variable = tokens[0];

        //Second token should be the value it'll be set to, if this fails to parse we'll just clear it (it's probably a ~)
        if(tokens[1].Equals("~")) {
            clear = true;
        } else {
            try {
                value = int.Parse(input.Substring(input.IndexOf(' ') + 1));
            } catch (System.Exception e) {
                Debug.Log("Could not parse input " + input + "! This may break the visual novel!");
            }
            
        }        
    }

    //Provide resource path for any resource needed by this operation. 
    //This will allow the VisualNovel to extract that resource, so it can be prepare()'d
    public string getResourcePath() {
        return null;
    }

    //Load image, music, etc
    public void prepare() {
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
}
