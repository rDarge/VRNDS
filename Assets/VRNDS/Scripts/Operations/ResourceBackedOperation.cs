using UnityEngine;
using System.Collections;
using System.IO;
using Ionic.Zip;
using System.Collections.Generic;

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
    public virtual void prepare(Dictionary<string, int> variables) {
        if (!resourcePath.Equals("~") && resourceStream == null) {
            if (!File.Exists(novelPath + "/" + getType() + "/" + resourcePath)) {
                if (!File.Exists(VisualNovel.cacheDirectory + "/" + getType() + "/" + resourcePath)) {
                    Debug.Log("Couldn't find asset " + resourcePath + " outside of archive, checking " + getType() + ".zip");
                    if (File.Exists(novelPath + "/" + getType() + ".zip")) {
                        using (ZipFile scriptZip = ZipFile.Read(novelPath + "/" + getType() + ".zip")) {
                            string fileName = resourcePath.Substring(resourcePath.LastIndexOf('/') + 1);
                            resourcePath = "file://" + ArchiveUtil.extractFromZipFile(scriptZip, fileName, VisualNovel.cacheDirectory);

                            //
                            //try {
                            //    scriptZip.ExtractSelectedEntries("name = " + fileName, null, VisualNovel.cacheDirectory);
                            //} catch (System.Exception e) {
                            //    Debug.Log("Could not find an entry in the " + getType() + " archive for resource " + resourcePath + ". Please ensure this file exists.");
                            //}
                        }
                    } else {
                        Debug.Log("Could not find " + resourcePath + " in folder or archive. Please ensure this file exists.");
                        resourcePath = "~";
                    }
                } else {
                    resourcePath = "file://" + VisualNovel.cacheDirectory + "/" + getType() + "/" + resourcePath;
                }
                //This doesn't report accurately?
                //if (!File.Exists(resourcePath)) {
                //    Debug.Log("Was not able to extract the " + getType() + " at " + resourcePath);
                //}
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
    public virtual bool isReady() {
        return resourcePath.Equals("~") || (resourceStream != null && resourceStream.isDone);
    }

    public WWW getStream() {
        if(isReady()) {
            return resourceStream;
        } else {
            throw new System.Exception("Stream not ready to be consumed! Check isReady() before fetching content!");
        }
    }

    public virtual void close() {
        if(this.resourceStream != null) {
            this.resourceStream.Dispose();
        }
    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public abstract bool execute(VisualNovelSystem vns, VisualNovel vn);

}
