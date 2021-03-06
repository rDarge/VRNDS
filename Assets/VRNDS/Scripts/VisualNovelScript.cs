﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
    Thread extractionThread;
    int currentOperation;
    float parsingProgress;
    string pathToScript;
    string novelPath;
    bool halt;

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
        extractionThread = new Thread(() => parseScript());
        extractionThread.Start();

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
        string[] rawOperations = script.Split('\n');
        operations = new List<VisualNovelOperation>();
        parsingProgress = 0;
        int parsedLines = 0;
        foreach(string line in rawOperations) {
            try {
                builder.addLine(line);
                if (builder.isReady()) {
                    operations.Add(builder.getOperation());
                }
                //Calculte parsing progress
                parsingProgress = parsedLines++ / (float)rawOperations.Length;
                Debug.Log("Parsing is " + parsingProgress + " percent complete!");
            } catch (System.Exception e) {
                Debug.Log("Couldn't parse script line: " + line + "\n\nBecause of " + e.Message);
            }
            
        }
        parsingProgress = 1;

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
        bool returnState = extractionThread != null && !extractionThread.IsAlive && operations != null;

        if(returnState && currentOperation == 0 && operations.Count > 0 && !operations[0].isReady()) {
            operations[0].prepare(null);
        }

        return returnState; //TODO multithread this please
    }

    public float getProgress() {
        return parsingProgress;
    }

    /// <summary>
    /// Iterates through the list of operations, incrementing currentOperation 
    /// and passing the VisualNovelSystem to each operation to perform the corresponding action
    /// </summary>
    public int step() {
        if(this.ready() && halt) {
            halt = false;
        }

        return currentOperation;
    }

    public void executeReadyOperation(VisualNovelSystem vns, VisualNovel vn) {
        if (this.ready() && currentOperation < operations.Count && operations[currentOperation].isReady() && !halt) {
            Debug.Log("Attempting to execute operation " + currentOperation + " which is a " + operations[currentOperation].GetType() + " pointing to " + operations[currentOperation].getResourcePath());
            try {
                halt = operations[currentOperation].execute(vns, vn);
            } catch (System.Exception e) {
                Debug.LogError("Could not execute operation " + currentOperation + " in " + this.pathToScript);
            }

            currentOperation++;

            //Prepare the next operation
            if (currentOperation < operations.Count) {
                operations[currentOperation].prepare(vns.getVariables());
            }
        }
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
        foreach (VisualNovelOperation vno in operations) {
            vno.close();
        }
    }

}
