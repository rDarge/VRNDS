using System.Collections.Generic;
using UnityEngine;

public abstract class AudioOperation : ResourceBackedOperation {

    protected AudioClip music;

    protected AudioOperation(string resourcePath, string novelPath) : base(resourcePath, novelPath) {
        //Don't really need to do anything here, just pass on the goods
    }

    public override string getType() {
        return "sound";
    }

    public override void prepare(Dictionary<string, int> variables) {
        if(resourcePath.EndsWith(".mp3") || resourcePath.EndsWith(".ogg")) {
            base.prepare(variables);
        } else {
            Debug.Log("Probably doesn't support " + resourcePath + ", not extracting it.");
            resourcePath = "~";
        }
        

        //Convert audio to supported format (but it's busted now)
        //if (resourcePath.EndsWith("aac")) {
        //    resourceStream.Dispose();
        //    resourceStream = null;
        //    convertAACtoWAV();
        //}
    }

    public override bool isReady() {
        return base.isReady(); //&& !resourcePath.EndsWith(".aac");
    }
}
