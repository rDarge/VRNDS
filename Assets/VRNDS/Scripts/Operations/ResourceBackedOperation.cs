using UnityEngine;
using System.Collections;
using System.IO;
using Ionic.Zip;

public abstract class ResourceBackedOperation : VisualNovelOperation {

    protected string resourcePath;
    protected string novelPath;
    protected WWW resourceStream;

    public ResourceBackedOperation(string resourcePath, string novelPath) {
        this.resourcePath = resourcePath;
        this.novelPath = novelPath;
    }

    //Provide resource path for any resource needed by this operation. 
    //This will allow the VisualNovel to extract that resource, so it can be prepare()'d
    public string getResourcePath() {
        return resourcePath;
    }

    //Load image, music, etc
    public void prepare() {
        if (resourceStream == null) {
            if (!File.Exists(novelPath + "/" + getType() + "/" + resourcePath)) {
                if (!File.Exists(VisualNovel.cacheDirectory + "/" + getType() + "/" + resourcePath)) {
                    using (ZipFile scriptZip = ZipFile.Read(novelPath + "/" + getType() + ".zip")) {
                        string fileName = resourcePath.Substring(resourcePath.LastIndexOf('/') + 1);
                        scriptZip.ExtractSelectedEntries("name = " + fileName, null, VisualNovel.cacheDirectory);
                    }
                }
                resourcePath = "file://" + VisualNovel.cacheDirectory + "/" + getType() + "/" + resourcePath;
            } else {
                resourcePath = "file://" + novelPath + "/" + getType() + "/" + resourcePath;
            }

            //Now that we've extracted the resource, set up a WWW to load it
            resourceStream = new WWW(resourcePath);
        }
    }

    //Override this to return "background" or "sound" etc
    public abstract string getType();

    //Check if stream is ready
    public bool isReady() {
        return resourcePath.Equals("~") || (resourceStream != null && resourceStream.isDone);
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public abstract bool execute(VisualNovelSystem vns, VisualNovel vn);

}
