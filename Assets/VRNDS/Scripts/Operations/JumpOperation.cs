using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Ionic.Zip;

public class JumpOperation : VisualNovelOperation {

    protected string resourcePath;
    protected string novelPath;

    //Takes path to sound file
    public JumpOperation(string path, string novelPath) {
        this.resourcePath = path;
        this.novelPath = novelPath;
    }

    public string getResourcePath() {
        return resourcePath;
    }

    public void prepare() {
        if (!File.Exists(novelPath + "/script/" + resourcePath)) {
            using (ZipFile scriptZip = ZipFile.Read(novelPath + "/script.zip")) {
                scriptZip.ExtractSelectedEntries("name = " + resourcePath, null, VisualNovel.cacheDirectory);
            }
            resourcePath = VisualNovel.cacheDirectory + "/script/" + resourcePath;
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
}
