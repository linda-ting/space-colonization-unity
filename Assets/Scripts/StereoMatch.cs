using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class StereoMatch : MonoBehaviour
{
    private StereoSGBM _stereo;

    // Start is called before the first frame update
    void Start()
    {
        int windowSize = 3;
        int minDisp = 16;
        int numDisp = 112 - minDisp;
        int blockSize = 16;
        int p1 = 8 * 3 * windowSize ^ 2;
        int p2 = 32 * 3 * windowSize ^ 2;

        _stereo = StereoSGBM.Create(minDisp, numDisp, blockSize, p1, p2, 1, 0, 10, 100, 32);
    }

    private void OpenImages(string leftFilename, string rightFilename)
    {
        Mat leftImg = Cv2.ImRead(leftFilename);
        Mat rightImg = Cv2.ImRead(rightFilename);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
