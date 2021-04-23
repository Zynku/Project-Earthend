using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ropescript : MonoBehaviour
{
    //FUCK THIS SCRIPT
    private LineRenderer LineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLength = 0.25f;
    public Vector2 ropeStartPoint;
    private int segmentLength = 35;
    private float lineWidth = 0.1f;
    public Vector3 startRopeHere;

    // Start is called before the first frame update
    void Start()
    {
        this.LineRenderer = this.GetComponent<LineRenderer>();

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLength;

            ropeStartPoint = startRopeHere;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        LineRenderer.startWidth = lineWidth;
        LineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }
        LineRenderer.positionCount = ropePositions.Length;
        LineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}
