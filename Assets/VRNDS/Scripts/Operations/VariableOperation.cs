using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VariableOperation : VisualNovelOperation {

    string[] tokens;
    string outputVariable;
    string inputVariable;
    string operation;
    int value;
    bool clear = false;

    public VariableOperation(string[] tokens) {
        //Get variable name
        this.tokens = tokens;
        outputVariable = tokens[0];
        operation = tokens[1];

        //Second token should be the value it'll be set to, if this fails to parse we'll just clear it (it's probably a ~)
        if(tokens[1].Equals("~")) {
            clear = true;
        } else {
            try {
                value = int.Parse(tokens[2]);
            } catch (System.Exception e) {
                inputVariable = tokens[2];
            }
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
        if(inputVariable != null) {
            value = vns.getVariable(inputVariable);
        }

        if (operation.Equals("=")) {
            Debug.Log("Setting " + outputVariable + " to " + value);
            vns.setVariable(outputVariable, value);
        } else if (operation.Equals("+")) {
            Debug.Log("Adding " + value + " to the current value of " + outputVariable);
            vns.setVariable(outputVariable, vns.getVariable(outputVariable) + value);
        }

        
        return false;
    }

    public void close() {
        //nothing
    }
}
