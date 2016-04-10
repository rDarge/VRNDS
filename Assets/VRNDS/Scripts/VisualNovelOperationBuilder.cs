using UnityEngine;
using System.Collections;

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
        string[] tokens = input.Split(' ');
        string key = tokens[0];
        Debug.Log("Parsing token " + key);

        //There will always be a token unless we hit a "fi"
        string token = null;
        if(tokens.Length > 1) {
            token = input.Substring(key.Length + 1).Trim();
        }

        if (key.Equals("sound")) {
            parseSound(token);
        } else if (key.Equals("music")) {
            parseMusic(token);
        } else if (key.Equals("setvar")) {
            parseVar(token);
        } else if (key.Equals("text")) {
            parseText(token);
        } else if (key.Equals("clearText !")) {
            parseText("~"); //Another way to clear the text
        } else if (key.Equals("bgload")) {
            parseBackground(token);
        } else if (key.Equals("setimg")) {
            parseForeground(token);
        } else if (key.Equals("delay")) {
            parseDelay(token);
        } else if (key.Equals("choice")) {
            parseChoice(token);
        } else if (key.Equals("if")) {
            parseIf(token);
        } else if (key.Equals("jump")) {
            parseJump(token);
        } else if (key.Equals("fi")) {
            parseFi();
        }
    }

    public void parseSound(string token) {
        operation = new SoundOperation(token, novelPath);
    }

    public void parseMusic(string token) {
        operation = new MusicOperation(token, novelPath);
    }

    public void parseVar(string token) {
        operation = new VariableOperation(token);
    }

    public void parseText(string token) {
        operation = new TextOperation(token);
    }

    public void parseBackground(string token) {
        operation = new BackgroundOperation(token, novelPath);
    }

    public void parseForeground(string token) {
        operation = new ForegroundOperation(token, novelPath);
    }

    public void parseDelay(string token) {
        operation = new DelayOperation(token);
    }

    public void parseChoice(string token) {
        operation = new ChoiceOperation(token);
    }
    
    public void parseIf(string token) {
        multiline = true;
        conditionalOperation = new ConditionalOperation(token);
    }

    public void parseFi() {
        multiline = false;
        conditionalOperation.addOperation(operation);
    }

    public void parseJump(string token) {
        operation = new JumpOperation(token, novelPath);
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
