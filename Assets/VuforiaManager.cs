using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public struct TrackTarget
{
    public string targetName { get; set; }
    public Vector3 originPosition { get; set; }
    public Vector3 originEular { get; set; }
}

public class VuforiaManager : MonoBehaviour
{
    /// <summary>
    /// Y軸回転用pivotルート
    /// </summary>
    public GameObject root;


    /// <summary>
    /// ゲーム空間オブジェクト
    /// </summary>
    [SerializeField]
    private GameObject room;

    private static VuforiaManager instance;

    private Text text;
    private Vuforia.ITrackerManager trackerManager;
    private Vuforia.ObjectTracker tracker;
    private Vuforia.StateManager stateManager;
    private Vuforia.VuforiaConfiguration.GenericVuforiaConfiguration config;

    private LinkedList<TrackTarget> trackTargets = new LinkedList<TrackTarget> ();
    private Rigidbody rb;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {

        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(getTrackTargets());
        trackerManager = Vuforia.TrackerManager.Instance;
        stateManager = trackerManager.GetStateManager();
        text = this.gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void runVuforia()
    {
        Vuforia.VuforiaBehaviour.Instance.enabled = true;
        if(trackTargets.Count == 0)
        {
            StartCoroutine(getTrackTargets());
        }
    }

    public void stopVuforia()
    {
        Vuforia.VuforiaBehaviour.Instance.enabled = false;
    }


    public void calcurateWorldPosition()
    {
        foreach (Vuforia.TrackableBehaviour trackableBehavior in stateManager.GetActiveTrackableBehaviours())
        {
            print("===================================");
            var target = trackTargets.Where(trackableTarget => trackableTarget.targetName == trackableBehavior.TrackableName).First();



            var targetPositionW = Camera.main.transform.TransformPoint(target.originPosition);
            float distance = trackableBehavior.transform.position.magnitude;
            Vector3 targetDiffPosition = Camera.main.transform.TransformPoint(trackableBehavior.transform.position);
            Vector3 targetDiffEuler = Camera.main.transform.TransformDirection(trackableBehavior.transform.eulerAngles);
            Vector3 targetDiffEuler_local = Camera.main.transform.TransformDirection(trackableBehavior.transform.localEulerAngles);
            Vector3 targetDiffEuler_room = Camera.main.transform.TransformDirection(target.originEular);

            float calcurated_position_x = target.originPosition.x + trackableBehavior.transform.position.x;
            float calcurated_position_y = room.transform.position.y;
            float calcurated_position_z = target.originPosition.z + trackableBehavior.transform.position.z;

            print("World" + Camera.main.transform.TransformPoint(trackableBehavior.transform.position));
            print("Vuforia" + trackableBehavior.transform.position);

            print("hosei" + Camera.main.transform.TransformPoint(target.originPosition + trackableBehavior.transform.localPosition));
            print("trackableBehavior.transform.localPosition" + trackableBehavior.transform.position);

            float rad = Mathf.Atan2(targetDiffPosition.x, targetDiffPosition.z) * Mathf.Rad2Deg;

            room.transform.localPosition = new Vector3(calcurated_position_x, calcurated_position_y, calcurated_position_z);
            // room.transform.parent = root.transform;
            root.transform.eulerAngles = new Vector3(root.transform.eulerAngles.x, -rad, root.transform.eulerAngles.z);

            print("calcurated_position_x" + calcurated_position_x);
            print("calcurated_position_y" + calcurated_position_y);
            print("calcurated_position_z" + calcurated_position_z);
            print("rad" + rad);
            print("room.transform.position" + room.transform.position);
            print("room.transform.localPosition" + room.transform.localPosition);
            print("root.transform.eulerAngles" + root.transform.eulerAngles);
            // room.transform.parent = null;
            // room.transform.eulerAngles = new Vector3(room.transform.eulerAngles.x, targetDiffEuler_room.y + rad, room.transform.eulerAngles.z);

        }
    }

    private IEnumerator getTrackTargets()
    {
        yield return new WaitForSeconds(2.0f);
        foreach (Vuforia.TrackableBehaviour trackableBehavior in stateManager.GetTrackableBehaviours())
        {
            if(trackableBehavior is Vuforia.ImageTargetBehaviour)
            {
                TrackTarget trackTarget = new TrackTarget();
                trackTarget.targetName = trackableBehavior.TrackableName;
                trackTarget.originPosition = trackableBehavior.transform.localPosition;
                trackTarget.originEular = trackableBehavior.transform.localEulerAngles;
                print(trackableBehavior.TrackableName + trackTarget.originEular);
                trackTargets.AddLast(trackTarget);
            }
        }
    }
}
