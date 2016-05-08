using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Ionic.Zip;
using System.Collections.Generic;

public class JumpOperation : VisualNovelOperation {

    protected string resourcePath;
    protected string novelPath;

    //Takes path to sound file
    public JumpOperation(string[] tokens, string novelPath) {
        this.resourcePath = tokens[0];
        this.novelPath = novelPath;
    }

    public string getResourcePath() {
        return resourcePath;
    }

    public void prepare(Dictionary<string, int> variables) {
        
        //Attempt to replace variables
        if(resourcePath.IndexOf('{') > 0) {
            int varStart = resourcePath.IndexOf('$') + 1;
            int varLength = resourcePath.IndexOf('}') - varStart;
            string variableName = resourcePath.Substring(varStart, varLength);
            string substituteString = "{$" + variableName + "}";
           
            Debug.Log("Replacing variable " + variableName + " in " + resourcePath + " with " + variables[variableName]);

            if (variables.ContainsKey(variableName)) {
                resourcePath = resourcePath.Replace(substituteString, variables[variableName].ToString());
            }
        }

        Debug.Log("Attempting to jump to script file at " + resourcePath);
        if (!File.Exists(novelPath + "/script/" + resourcePath)) {
            using (ZipFile scriptZip = ZipFile.Read(novelPath + "/script.zip")) {
                string fileName = resourcePath.Substring(resourcePath.LastIndexOf('/') + 1);
                resourcePath = ArchiveUtil.extractFromZipFile(scriptZip, fileName, VisualNovel.cacheDirectory);
            }
            //resourcePath = VisualNovel.cacheDirectory + "/script/" + resourcePath;
        } else {
            resourcePath = novelPath + "/script/" + resourcePath;
        }
    }

    public bool isReady() {
        return true;
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public bool execute(VisualNovelSystem vns, VisualNovel vn) {
        vns.jump(resourcePath);
        return false;
    }

    public void close() {
        //nothing
    }
}
