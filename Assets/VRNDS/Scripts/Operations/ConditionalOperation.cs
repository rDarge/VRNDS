using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TODO Handle nested conditionals, see umineko scr files
/// </summary>
public class ConditionalOperation : VisualNovelOperation {

    List<VisualNovelOperation> operations;

    string variable;
    string equalityOperator;
    int value;

    public ConditionalOperation(string input) {
        operations = new List<VisualNovelOperation>();

        string[] tokens = input.Split(' ');

        variable = tokens[0];
        equalityOperator = tokens[1];
        value = int.Parse(tokens[2]);
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

    public void addOperation(VisualNovelOperation o) {
        operations.Add(o);
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public bool execute(VisualNovelSystem vns, VisualNovel vn) {
        bool haltingOperation = false;

        Debug.Log("Attempting to evaluate variable with name " + variable);
        try {
            int storedValue = vns.getVariable(variable);
            bool evaluates = false;
            if (equalityOperator.Equals("==")) {
                evaluates = storedValue == value;
            } else if (equalityOperator.Equals("!=")) {
                evaluates = storedValue != value;
            } //TODO there are probably some more cases that are easy to nix when you get a chance
            if (evaluates) {
                foreach (VisualNovelOperation operation in operations) {
                    operation.prepare();
                    bool eachOperation = operation.execute(vns, vn);

                    //one-way switch for halting operations
                    if (eachOperation == true) {
                        haltingOperation = true;
                    }
                }
            }
        } catch (KeyNotFoundException knfe) {
            Debug.Log("no key found for " + variable + " in dictionary. Assuming false");
        }
        

        return haltingOperation;
    }
}
