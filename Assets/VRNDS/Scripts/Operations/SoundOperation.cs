using UnityEngine;

public class SoundOperation : AudioOperation {

    private AudioClip sound;

    //Takes path to sound file
    public SoundOperation(string[] tokens, string novelPath) : base(tokens[0], novelPath) {

    }

    //Perform whatever operation is needed on the VisualNovelSystem/VisualNovel.
    public override bool execute(VisualNovelSystem vns, VisualNovel vn) {

        if (resourcePath.Equals("~")) {
            vns.stopMusic();
        } else {
            Debug.Log("Playing audio " + this.resourcePath);

            sound = resourceStream.GetAudioClip(true);
            sound.LoadAudioData();
            vns.playMusic(sound);
        }

        return false;
    }

}
