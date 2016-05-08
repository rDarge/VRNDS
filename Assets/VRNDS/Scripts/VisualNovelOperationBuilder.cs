using UnityEngine;
using System.Collections;
using System;

public class VisualNovelOperationBuilder {


    VisualNovelOperation operation;
    ConditionalOperation conditionalOperation;

    string novelPath;

    //Building state objects
    bool clear;
    string text;
    string resourcePath;
    string variableName;
    int value;

    //Additional flags for multi-line operations
    bool multiline;
    


	public VisualNovelOperationBuilder(string novelPath) {
        this.novelPath = novelPath;
        operation = null;
        conditionalOperation = null;
    }

    public void addLine(string input) {
        input = input.Trim();

        string[] command = input.Split(' ');

        //Generate key and tokens (if provided)
        string key = command[0];
        string[] tokens = new string[command.Length - 1];
        if(command.Length > 1) {
            Array.Copy(command, 1, tokens, 0, command.Length - 1);
        }
        
        Debug.Log("Parsing token " + key);

        if (key.Equals("sound")) {
            parseSound(tokens);
        } else if (key.Equals("music")) {
            parseMusic(tokens);
        } else if (key.Equals("setvar")) {
            parseVar(tokens);
        } else if (key.Equals("text") || key.Equals("clearText")) {
            parseText(tokens);
        } else if (key.Equals("bgload")) {
            parseBackground(tokens);
        } else if (key.Equals("setimg")) {
            parseForeground(tokens);
        } else if (key.Equals("delay")) {
            parseDelay(tokens);
        } else if (key.Equals("choice")) {
            parseChoice(tokens);
        } else if (key.Equals("if")) {
            parseIf(tokens);
        } else if (key.Equals("jump")) {
            parseJump(tokens);
        } else if (key.Equals("fi")) {
            parseFi();
        }
    }

    public void parseSound(string[] tokens) {
        operation = new SoundOperation(tokens, novelPath);
    }

    public void parseMusic(string[] tokens) {
        operation = new MusicOperation(tokens, novelPath);
    }

    public void parseVar(string[] tokens) {
        operation = new VariableOperation(tokens);
    }

    public void parseText(string[] tokens) {
        operation = new TextOperation(tokens);
    }

    public void parseBackground(string[] tokens) {
        operation = new BackgroundOperation(tokens, novelPath);
    }

    public void parseForeground(string[] tokens) {
        operation = new ForegroundOperation(tokens, novelPath);
    }

    public void parseDelay(string[] tokens) {
        operation = new DelayOperation(tokens);
    }

    public void parseChoice(string[] tokens) {
        operation = new ChoiceOperation(tokens);
    }
    
    public void parseIf(string[] tokens) {
        multiline = true;
        conditionalOperation = new ConditionalOperation(tokens);
    }

    public void parseJump(string[] tokens) {
        operation = new JumpOperation(tokens, novelPath);
    }

    public void parseFi() {
        multiline = false;
        conditionalOperation.addOperation(operation);
    }

    public bool isReady() {
        return operation != null && multiline == false;
    }

    public VisualNovelOperation getOperation() {
        VisualNovelOperation newOperation = conditionalOperation != null ? conditionalOperation : operation;
        operation = null;
        conditionalOperation = null;
        return newOperation;
        
    }
}
