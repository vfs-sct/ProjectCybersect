using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGrappleArrow : MonoBehaviour
{
    private FPSLook look;
    private Grapple grapple;
    private Camera mainCamera;
    private RawImage image;

    private void Hide()
    {
        Color color = image.color;
        color.a = 0f;
        image.color = color;
    }

    private void Start()
    {
        GameObject player = GameObject.Find("player");
        look = player.GetComponent<FPSLook>();
        grapple = player.GetComponent<Grapple>();
        mainCamera = GameObject.Find("mainCamera").GetComponent<Camera>();
        image = GetComponent<RawImage>();

        Hide();
    }

    GrappleState lastGrappleState = GrappleState.INACTIVE;
    private void Update()
    {
        if (grapple.state != lastGrappleState)
        {
            if (grapple.state == GrappleState.INACTIVE)
                Hide();
        }

        if (grapple.state != GrappleState.ENGAGED)
            return;

        if (look.grappleLookDir == Vector2.zero)
            return;

        Color color = image.color;
        color.a = look.grappleLookDir.magnitude;
        image.color = color;

        Vector2 pos = new Vector2(mainCamera.pixelWidth/2f, mainCamera.pixelHeight/2f) + look.grappleLookDir*50f;
        transform.position = new Vector3(pos.x, pos.y, 0f);

        float theta = Mathf.Atan2(look.grappleLookDir.y, look.grappleLookDir.x) + Mathf.PI/2f; 
        transform.localRotation = Quaternion.Euler(0f, 0f, theta*Mathf.Rad2Deg);

        lastGrappleState = grapple.state;
    }
}
