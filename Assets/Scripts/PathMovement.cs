using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovement: MonoBehaviour
{
    #region Definitions, public and private variables

    public enum PathTypes
    {
        Linear,
        loop
    }

    public PathTypes PathType;
    public int movementDirection = 1;  // 1 clockwise/forward or -1 opposite
    public int movingTo = 0;  // point in the path sequence that we are moving towards
    public Transform[] PathSequence;  // Array of all the points in the path sequence

    #endregion  // Definitions, public and private variables

    #region Main Methods
    // Check points exist and then draw lines between points on path for visual clues
    // If the path is a loop, draw a line from the last to first point
    public void OnDrawGizmos()
    {
        if (PathSequence == null || PathSequence.Length < 2)
        {
            return;  // Exit if line cannot be drawn
        }

        for (var i = 1; i < PathSequence.Length; i++)
        {
            Gizmos.DrawLine(PathSequence[i - 1].position, PathSequence[i].position);
        }

        if (PathType == PathTypes.loop)
        {
            Gizmos.DrawLine(PathSequence[0].position, PathSequence[PathSequence.Length - 1].position);
        }
    } // End OnDrawGizmos

    #endregion  // Main Methods

    //Coroutines run parallel to other functions
    #region Coroutines

    // Return the transform component of the next point in the path after checking points exist and exit if not
    public IEnumerator<Transform> GetNextPathPoint()
    {
        if (PathSequence == null || PathSequence.Length < 1)
        {
            yield break;  // exit
        }

        while (true) // will not create an infinit loop due to yield return
        {
            yield return PathSequence[movingTo];
            // *** code will pause here to wait for next call of enumerator, preventing infinit loop ***

            if (PathSequence.Length == 1)
            {
                continue;
            }


            if (PathType == PathTypes.Linear)
            {
                if (movingTo <= 0)
                {
                    movementDirection = 1;  // forward
                }
                else if (movingTo >= PathSequence.Length - 1)
                {
                    movementDirection = -1;  // backward
                }
            }

            movingTo = movingTo + movementDirection;  // 1 or -1 (to be called before pathTypes.loop below)

            if (PathType == PathTypes.loop)
            {
                if (movingTo >= PathSequence.Length)
                {
                    movingTo = 0;  // set the next point to start
                }

                if (movingTo < 0)  // moved backward past the first point
                {
                    movingTo = PathSequence.Length - 1;  // move to last point 
                }
            }

        } // end while

    } // End IEnumerator<Transform> GetNextPathPoint()

    #endregion  // End Coroutines

} // End Class