using UnityEngine;
using NAudio.Wave;


public class MusicOperation : AudioOperation {

    //Takes path to sound file
    public MusicOperation(string[] tokens, string novelPath) : base(tokens[0], novelPath) {

    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public override bool execute(VisualNovelSystem vns, VisualNovel vn) {

        if (resourcePath.Equals("~")) {
            vns.stopMusic();
        } else {
            Debug.Log("Playing music " + this.resourcePath);

            music = resourceStream.GetAudioClip(true);
            music.LoadAudioData();
            vns.playMusic(music);
        }

        return false;
    }

}
