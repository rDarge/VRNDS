﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class VisualNovelScript {

    
    public enum OPERATION_TYPES {
        label,   //UI label, typically followed by a CHOICE
        bgload,  //Load background, clear foreground images
        setimg,  //Load additional image into foreground
        choice,  //Conditional branch, usually followed by a conditional
        setvar,  //Sets a numeric value to a variable. May accept arithmetic operations.
        text,    //Text to be output directly to the screen. Interrupts flow.
        cleartext, //Clears the any text on the screen
        music,   //Background music. Does not stop between background loads.
        sound,   //Short audio sound. Can be stopped by sound ~ (I think)
        delay,   //Wait before the next execution, for dramatic effect?
        conditional, //bracketed by "if" and "fi", takes option from CHOICE and typically calls a JUMP
        jump,    //Like a "GOTO", this operation can jump to a LABEL or to a new SCRIPT. 
                 //Variables set from setvar can be referenced using {$varname} syntax. May reference scripts in folders.
                 //May provide a label after the script path. Should jump to that label if provided.
        _goto,   //This operation jumps to a label identified in the same script.
    }
    
    public enum RESOURCE_TYPE {
        background,
        foreground,
        sound,
        text
    }

    List<VisualNovelOperation> operations; //List of operations
    int currentOperation;
    string pathToScript;
    string novelPath;

    public VisualNovelScript(string pathToScript, string novelPath) {
        this.pathToScript = pathToScript;
        this.novelPath = novelPath;
        load();
    }

    /// <summary>
    /// Set the path to the script, and start a thread to parse out all the important bits
    /// </summary>
    /// <param name="pathToScript"></param>
    /// <param name="novel">novel object, pass this in to automatically start the script when you're ready</param>
    public void load() {

        //Build out the set of operations
        parseScript();

        //Initialize currentOperation
        currentOperation = 0;
    }

    /// <summary>
    /// Execute this in another thread to asynchronously parse the script and populate the operations list
    /// </summary>
    public void parseScript() {
        //Build each operation as you parse through lines
        string script = null;
        using (StreamReader sr = new StreamReader(pathToScript)) {
            // Read the stream to a string, and write the string to the console.
            while (!sr.EndOfStream) {
                script = sr.ReadToEnd();
            }
        }

        VisualNovelOperationBuilder builder = new VisualNovelOperationBuilder(novelPath);
        operations = new List<VisualNovelOperation>();
        foreach(string line in script.Split('\n')) {
            builder.addLine(line);
            if(builder.isReady()) {
                operations.Add(builder.getOperation());
            }
        }

        //Extract each resource file now, so we will have it when it's needed

    }

    public void extractResource(RESOURCE_TYPE type, string path) {
        //Extract each resource type so that it will be available when the VisualNovelOperation executes
    }

    /// <summary>
    /// Returns true when the parse() thread terminates
    /// </summary>
    /// <returns>True when the script is done parsing, false otherwise</returns>
    public bool ready() {
        return true; //TODO multithread this please
    }

    /// <summary>
    /// Iterates through the list of operations, incrementing currentOperation 
    /// and passing the VisualNovelSystem to each operation to perform the corresponding action
    /// </summary>
    public int step(VisualNovelSystem vns, VisualNovel vn) {
        bool halt = false;
        while (operations[currentOperation + 1].isReady() && !halt && currentOperation < operations.Count) {
            currentOperation++;
            Debug.Log("Attempting to execute operation " + currentOperation + " which is a " + operations[currentOperation].GetType() + " pointing to " + operations[currentOperation].getResourcePath());
            halt = operations[currentOperation].execute(vns, vn);

            //Prepare the next operation
            if(currentOperation < operations.Count) {
                operations[currentOperation + 1].prepare();
            }
            
        }
        return currentOperation;
    }


    /// <summary>
    /// Jumps to a specific index in operations, changing currentOperation accordingly. 
    /// This typically should be used for loading save games.
    /// </summary>
    /// <param name="index"></param>
    public void jumpToIndex(int index) {

    }

    public void close() {
        //Clean up resources here.
    }

}