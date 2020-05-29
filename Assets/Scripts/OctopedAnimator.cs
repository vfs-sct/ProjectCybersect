using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Leg
{
    public Transform origin;
    public float originLength;
    public Transform joint;
    public float jointLength;
    public Transform end;
    public Transform restPoint;

    public Vector3 lastContact;
    public Vector3 targetContact;
}

public struct Pair
{
    public const int RIGHT = 0;
    public const int LEFT = 1;
    public const int NONE = 2;

    public Leg[] legs;

    public int activeLeg;
    public float time;
}

public class OctopedAnimator : MonoBehaviour
{
    private Transform root;
    private Transform legArmature;
    private Pair[] pairs = new Pair[4];

    private Rigidbody rb;

    private float LawOfCosinesC(float a, float b, float c)
    {
        return (c*c - a*a - b*b)/(-2f*a*b);
    }

    private void ApplyIK(ref Pair pair)
    {
        foreach (Leg leg in pair.legs)
        {
            Vector3 originToContact = leg.end.position - leg.origin.position;
            float c = leg.jointLength;
            float a = leg.originLength;
            float b = originToContact.magnitude;

            if (b >= c + a)
                return;

            float theta = Mathf.Acos(LawOfCosinesC(a, b, c));

            Vector3 rotationAxis = Vector3.Cross(originToContact, Vector3.up);
            Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg*theta, rotationAxis);
            leg.joint.position = rotation*originToContact;

            leg.joint.position = leg.joint.position.normalized*leg.originLength;
            leg.joint.position += leg.origin.position;
        }
    }

    private void PerPairPre(ref Pair pair)
    {
        pair.legs = new Leg[2];
        pair.time = 1f;
        pair.activeLeg = Pair.NONE;
    }

    private void PerLegPost(ref Leg leg)
    {
        leg.joint = leg.origin.GetChild(0);
        leg.end = leg.joint.GetChild(0);
        leg.joint.parent = legArmature;
        leg.end.parent = legArmature;

        leg.originLength = Vector3.Distance(leg.origin.position, 
                                            leg.joint.position);
        leg.jointLength = Vector3.Distance(leg.joint.position, 
                                           leg.end.position);
    }

    private void InitializeLegs()
    {
        Transform restPoints = transform.Find("restPoints");

        for (uint i = 0; i < 4; ++i)
            PerPairPre(ref pairs[i]);

        pairs[0].legs[Pair.RIGHT].origin = root.Find("fr");
        pairs[0].legs[Pair.LEFT].origin = root.Find("fl");
        pairs[0].legs[Pair.RIGHT].restPoint = restPoints.Find("fr");
        pairs[0].legs[Pair.LEFT].restPoint = restPoints.Find("fl");

        pairs[1].legs[Pair.RIGHT].origin = root.Find("fmr");
        pairs[1].legs[Pair.LEFT].origin = root.Find("fml");
        pairs[1].legs[Pair.RIGHT].restPoint = restPoints.Find("fmr");
        pairs[1].legs[Pair.LEFT].restPoint = restPoints.Find("fml");

        pairs[2].legs[Pair.RIGHT].origin = root.Find("bmr");
        pairs[2].legs[Pair.LEFT].origin = root.Find("bml");
        pairs[2].legs[Pair.RIGHT].restPoint = restPoints.Find("bmr");
        pairs[2].legs[Pair.LEFT].restPoint = restPoints.Find("bml");

        pairs[3].legs[Pair.RIGHT].origin = root.Find("br");
        pairs[3].legs[Pair.LEFT].origin = root.Find("bl");
        pairs[3].legs[Pair.RIGHT].restPoint = restPoints.Find("br");
        pairs[3].legs[Pair.LEFT].restPoint = restPoints.Find("bl");

        for (uint i = 0; i < 4; ++i)
        {
            PerLegPost(ref pairs[i].legs[Pair.RIGHT]);
            PerLegPost(ref pairs[i].legs[Pair.LEFT]);
        }

        for (uint i = 0; i < 4; ++i)
        {
            for (uint j = 0; j < 2; ++j)
            {
                Vector3 origin = pairs[i].legs[j].restPoint.position;
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit))
                    pairs[i].legs[j].end.position = hit.point;
            }

            ApplyIK(ref pairs[i]);
        }
    }

    private void Start()
    {
        root = transform.Find("Armature").GetChild(0);
        legArmature = GameObject.Find("legArmature").transform;
        InitializeLegs();

        rb = GetComponent<Rigidbody>();
    }

    float radius = 0.4f;
    private void NewTarget(ref Leg leg)
    {
        Vector3 rayOrigin = leg.restPoint.position + rb.velocity.normalized*radius;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit))
        {
            leg.lastContact = leg.end.position;
            leg.targetContact = hit.point;
        }
    }

    private void Alternate(ref Pair pair)
    {
        if (pair.activeLeg != Pair.NONE)
            pair.legs[pair.activeLeg].end.position = pair.legs[pair.activeLeg].targetContact;

        if (pair.activeLeg == Pair.NONE)
            pair.activeLeg = Pair.RIGHT;
        else if (pair.activeLeg == Pair.RIGHT)
            pair.activeLeg = Pair.LEFT;
        else
            pair.activeLeg = Pair.RIGHT;

        NewTarget(ref pair.legs[pair.activeLeg]);
        pair.time = 0f;
    }

    private void Mirror(ref Pair subject, ref Pair pair, bool inverse)
    {
        pair.time = subject.time;

        int activeLeg;
        if (inverse)
        {
            if (subject.activeLeg == Pair.RIGHT)
                activeLeg = Pair.LEFT;
            else
                activeLeg = Pair.RIGHT;
        }
        else
        {
            activeLeg = subject.activeLeg;
        }

        pair.activeLeg = activeLeg;
        NewTarget(ref pair.legs[activeLeg]);
    }

    private float HeightFunction(float t)
    {
        return Mathf.Sin(t*Mathf.PI);
    }
    
    private void StepPair(ref Pair pair)
    {
        ref Leg currentLeg = ref pair.legs[pair.activeLeg];
        currentLeg.end.position = Vector3.Lerp(currentLeg.lastContact, currentLeg.targetContact, pair.time) + Vector3.up*HeightFunction(pair.time)*0.4f;
    }

    private void LateUpdate()
    {
        if (rb.velocity.magnitude > 0.001f)
        {
            for (uint i = 0; i < 4; ++i)
                pairs[i].time += Time.deltaTime*rb.velocity.magnitude*3f;

            if (pairs[0].time >= 1f)
            {
                Alternate(ref pairs[0]);
                Mirror(ref pairs[0], ref pairs[1], true);
                Mirror(ref pairs[0], ref pairs[2], false);
                Mirror(ref pairs[0], ref pairs[3], true);
            }

            ref Leg firstActiveLeg = ref pairs[0].legs[pairs[0].activeLeg];
            if (Vector3.Dot(firstActiveLeg.targetContact - firstActiveLeg.lastContact, rb.velocity) < -0.1f)
            {
                for (int i = 0; i < 4; ++i)
                {
                    ref Leg activeLeg = ref pairs[i].legs[pairs[i].activeLeg];
                    Vector3 last = activeLeg.lastContact;
                    activeLeg.lastContact = activeLeg.targetContact;
                    activeLeg.targetContact = last;
                    pairs[i].time = 1 - pairs[i].time;
                }
            }

            for (uint i = 0; i < 4; ++i)
                StepPair(ref pairs[i]);
        }

        for (uint i = 0; i < 4; ++i)
            ApplyIK(ref pairs[i]);


        if (Input.GetKey(KeyCode.D))
            rb.velocity += Vector3.right*Time.deltaTime*2;
        if (Input.GetKey(KeyCode.A))
            rb.velocity -= Vector3.right*Time.deltaTime*2;
        if (Input.GetKey(KeyCode.W))
            rb.velocity += Vector3.forward*Time.deltaTime*2;
        if (Input.GetKey(KeyCode.S))
            rb.velocity -= Vector3.forward*Time.deltaTime*2;
    }
}
