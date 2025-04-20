using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [SerializeField] private Paddle goalOwner;

    private void OnTriggerEnter2D(Collider2D other)
    {   
        // If the ball enters the goal area...
        if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {   
            // Score a goal for the goal owner.
            GameManager.Instance.GoalScored(goalOwner);
        }
    }
}
