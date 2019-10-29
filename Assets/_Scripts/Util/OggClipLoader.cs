using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class OggClipLoader
{
    public static AudioClip LoadClip(string audioFilePath) {
        var path = "file:///" + audioFilePath;

        var handler = new DownloadHandlerAudioClip(path, AudioType.OGGVORBIS);
        handler.compressed = false;
        handler.streamAudio = true;

        var wr = new UnityWebRequest(path, UnityWebRequest.kHttpVerbGET, handler, null);
        wr.SendWebRequest();

        while (!wr.isDone) { }

        return handler.audioClip;
    }
}