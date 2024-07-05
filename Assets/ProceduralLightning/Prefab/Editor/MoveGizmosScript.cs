using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning.Editor
{
    [InitializeOnLoad]
    public class MoveGizmosScript
    {
        static MoveGizmosScript()
        {
            try
            {
                var destinationPath = Path.Combine(Application.dataPath, "Gizmos");
                Directory.CreateDirectory(destinationPath);
                var pngFiles = Directory.GetFiles(Application.dataPath, "LightningPath*.png",
                    SearchOption.AllDirectories);
                foreach (var gizmo in pngFiles)
                {
                    var fileName = Path.GetFileName(gizmo);
                    if (fileName.Equals("LightningPathStart.png", StringComparison.OrdinalIgnoreCase) ||
                        fileName.Equals("LightningPathNext.png", StringComparison.OrdinalIgnoreCase))
                    {
                        var destFile = Path.Combine(destinationPath, fileName);
                        var srcInfo = new FileInfo(gizmo);
                        var dstInfo = new FileInfo(destFile);
                        if (!dstInfo.Exists || srcInfo.LastWriteTimeUtc > dstInfo.LastWriteTimeUtc)
                            srcInfo.CopyTo(dstInfo.FullName, true);
                    }
                }
            }
            catch
            {
            }
        }
    }
}