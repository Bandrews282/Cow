using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visitor : MonoBehaviour
{
    #region Definitions, public and private variables

    public enum MovementType
    {
        MoveTowards,
        LerpTowards
    }

    public MovementType Type = MovementType.MoveTowards;  // Movement type used
    public PathMovement MyPath;  // Reference to movement type used
    public float VisitorSpeed = 3;  // how fast the visitor object moves
    public float MaxDistanceToGoal = 0.1f;  // how close to the path point has to be achieved
    //public float MaxTimeAttentionSpan = 6.0f;  // maximum time panda has visitor attention
    //public float CurrentAttentionUsed = 0.0f;  // time used of current attention span
    //public float AttentionResetTime = 12.0f;  // Amount of time before the attention is reset

    private IEnumerator<Transform> pointInPath;  // reference points returned from MyPath.GetNextPathPoint
    private GameObject visitor;  // The normal visitor body
    private GameObject excitedChild;  // An excited child (probably not too many of these
    private GameObject player;  // The Pander main character object
    private bool IsPlayerTheFocus = false;  // Switch to swap from panda focus to walking path
    private float PandaInReactionRange = 6f;  // View range for trigger of excited child
    private Animator anim;

    #endregion  // Definitions, public and private variables

    #region Main Methods
    private void Awake()
    {
        visitor = GameObject.FindGameObjectWithTag("Visitor");
        excitedChild = GameObject.FindGameObjectWithTag("VisitorExcitedChild");
        anim = excitedChild.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check that there is a path assigned
        if (MyPath == null)
        {
            Debug.LogError("Movement Path cannot be null - enter path to follow.", gameObject);
            return;
        }

        pointInPath = MyPath.GetNextPathPoint();  // reference to instance of coroutine GetNextPathPoint

        pointInPath.MoveNext();  // Get the next point on the path to move to (Gets default 1st value)

        // Check for valid point to move to
        if (pointInPath.Current == null)
        {
            Debug.LogError("A path must have points in it to follow", gameObject);
            return;
        }

        transform.position = pointInPath.Current.position;  // set the postion of object to starting point

    } // End Start

    // Update is called once per frame
    void Update()
    {
    
        /// <remarks>  Code not implemented - will probably need a re-think
        /// // The attention span continues to gain attention to enable a reset after a set period
        /// if(CurrentAttentionUsed > MaxTimeAttentionSpan)
        /// {
        ///    CurrentAttentionUsed = CurrentAttentionUsed + Time.d
        ///    if(CurrentAttentionUsed > AttentionResetTime)
        ///        CurrentAttentionUsed = 0.0f;
        ///} 
        /// if (IsPlayerTheFocus)
        /// {
        ///     VisitorBehaviourOnPandaFocus();
        /// }
        /// </remarks>

        // Check for a valid path with a point in it and if so, follow it
        if (pointInPath != null || pointInPath.Current != null)  
        {
            FollowPath();
        }

        // WIP - behaviour for Excited child
        // Turn the child to face the panda
        if(excitedChild == null)
        {
            Debug.LogError("Can't find the excited child", gameObject);
        }
        excitedChild.transform.LookAt(new Vector3(player.transform.position.x,
            transform.position.y, player.transform.position.z));
        // Get the distance from the child
        Vector3 direction = player.transform.position - excitedChild.transform.position;
        float magnitute = direction.magnitude;
        direction.Normalize();
        if(magnitute > PandaInReactionRange)
        {
            //Debug.Log("The child is out of view range");
            anim.SetTrigger("PandaOutOfRange");
        }
        else
        {
            //Debug.Log("The child is in view range");
            anim.SetTrigger("PandaInRange");
        }



    } // End Update 

    #endregion  // Main Methods

    #region Help or detailed methods/procedures

    void VisitorBehaviourOnPandaFocus()
    {

    } // End of VisitorBehaviourOnPandaFocus

    void FollowPath()
    {
        transform.LookAt(new Vector3(pointInPath.Current.position.x,
               transform.position.y, pointInPath.Current.position.z));

        if (Type == MovementType.MoveTowards)
        {
            // Move to the next point in the path using MoveTowards
            transform.position = Vector3.MoveTowards(transform.position, pointInPath.Current.position, Time.deltaTime * VisitorSpeed);
        }
        else if (Type == MovementType.LerpTowards)
        {
            // Move to the next point in the path using Lerp
            transform.position = Vector3.Lerp(transform.position, pointInPath.Current.position, Time.deltaTime * VisitorSpeed);
        }

        // Check if the object is close enough using .sqrMagnitude (faster according to Unity Documentation)
        // (Vector3.Distance could replace this, only potentially slower due to square root calculations)
        var distanceSquared = (transform.position - pointInPath.Current.position).sqrMagnitude;
        if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
        {
            pointInPath.MoveNext();
        }      
    }

    #endregion  // Help or detailed methods/procedures

}  // end class FollowPath

