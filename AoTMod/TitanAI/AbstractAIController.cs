using UnityEngine;
using System.Collections;

public abstract class AbstractAIController : AIController{
	protected TITAN controlledTitan;

	public AbstractAIController (TITAN controlledTitan){
		this.controlledTitan = controlledTitan;
	}

	public abstract void Update();

	public abstract void LateUpdate();

	public abstract void FixedUpdate();

}

