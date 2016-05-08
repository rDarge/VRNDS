using UnityEngine;
using System.Collections;
using Ionic.Zip;

public class ArchiveUtil {
    
    public static string extractFromZipFile(ZipFile zip, string target, string destination) {
        string returnValue = null;

        foreach (ZipEntry e in zip) {
            if (e.FileName.EndsWith(target)) {
                e.Extract(destination);
                returnValue = destination + "/" + e.FileName;
                Debug.Log("EXTRACTED " + returnValue);
                break;
            }
        }

        return returnValue;
    }
}
