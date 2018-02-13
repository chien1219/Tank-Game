using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCountLife : StateMachineBehaviour {

	public GameObject attackEffect;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log ("OnStateEnter Melee Enemy");
		PlayerControl.instance.Life -= meleeEnemyBehavior.instance.attack;
		GameObject effect = Instantiate (attackEffect, PlayerControl.instance.turret.position, new Quaternion (0, 0, 0, 1)) as GameObject;
		var e = effect.GetComponent<AudioSource> ();
		e.Play ();
		Destroy (effect, 2);

	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
