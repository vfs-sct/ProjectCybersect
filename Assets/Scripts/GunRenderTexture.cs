using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRenderTexture : MonoBehaviour
{
    // Private Members
    private RectTransform imageTransform;
    private Camera mainCamera;
    private Camera gunCamera;
    private RenderTexture renderTexture;

    private void Start()
    {
        imageTransform = GameObject.Find("gunRender").GetComponent<RectTransform>();
        gunCamera = GameObject.Find("gunCamera").GetComponent<Camera>();
        mainCamera = GameObject.Find("mainCamera").GetComponent<Camera>();
        renderTexture = gunCamera.targetTexture;

        lastWidth = mainCamera.pixelWidth;
        lastHeight = mainCamera.pixelHeight;
    }

    int lastWidth;
    int lastHeight;
    private void OnPreRender()
    {
        if (lastWidth != mainCamera.pixelWidth || lastHeight != mainCamera.pixelHeight)
        {
            renderTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 16);
            gunCamera.targetTexture = renderTexture;
        }

        lastWidth = mainCamera.pixelWidth;
        lastHeight = mainCamera.pixelHeight;
    }
}
