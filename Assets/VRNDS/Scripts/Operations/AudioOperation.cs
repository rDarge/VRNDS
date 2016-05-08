using NAudio.Wave;
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
        if(resourcePath.EndsWith(".mp3")) {
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


    //This function blows up. Not sure how best to address it now, getting memory access errors.
    private void convertAACtoWAV() {
        string newResourcePath = resourcePath.Replace(".aac", ".wav");

        try {
            if (resourcePath != null) {
                using (MediaFoundationReader reader = new MediaFoundationReader(resourcePath)) {

                    // resample the file to PCM with same sample rate, channels and bits per sample
                    WaveFormat format = new WaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, reader.WaveFormat.Channels);
                    using (ResamplerDmoStream resampledReader = new ResamplerDmoStream(reader, format)) {

                        // create WAVe file
                        using (WaveFileWriter waveWriter = new WaveFileWriter(newResourcePath, resampledReader.WaveFormat)) {
                            // copy samples
                            // try to read 10 ms;
                            int bytesToRead = resampledReader.WaveFormat.AverageBytesPerSecond / 100;
                            byte[] buffer = new byte[bytesToRead];
                            int count;
                            int total = 0;
                            do {
                                count = resampledReader.Read(buffer, 0, bytesToRead);
                                waveWriter.Write(buffer, 0, count);
                                total += count;
                                //Assert.AreEqual(count, bytesToRead, "Bytes Read");
                            } while (count > 0);
                        }
                    }
                }
            }
        } catch (System.Exception e) {
            Debug.Log("Bad time");
        }
        

        resourcePath = newResourcePath;
        resourceStream = new WWW(resourcePath);
    }
}
