using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using UnityEngine.XR;
using Valve.VR;
using System.Linq;
using UnityEngine.UIElements;

public class RosHeadRotationPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "head_rot";
    public float publishMessageFrequency = 0.05f;
    private float timeElapsed;

    private Vector3 headPosition;
    private Quaternion headRotation;
    private List<XRNodeState> nodeStates = new List<XRNodeState>();

    // Start is called before the first frame update
    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Vector3Msg>(topicName);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > publishMessageFrequency)
        {
            InputTracking.GetNodeStates(nodeStates);
            var headState = nodeStates.FirstOrDefault(node => node.nodeType == XRNode.Head);
            headState.TryGetRotation(out headRotation);
            Vector3 angles = headRotation.eulerAngles;
            Vector3Msg headRotMsg = new Vector3Msg(angles.x, angles.y, angles.z);
            ros.Send(topicName, headRotMsg);
            timeElapsed = 0;
        }
    }
}
