﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateManager
{
	private List<NPCState> states;
	
	private StateID currentStateID;
	public StateID CurrentStateID { get { return currentStateID; } }
	private NPCState currentState;
	public NPCState CurrentState { get { return currentState; } }
	
	public NPCStateManager()
	{
		states = new List<NPCState>();
	}
	
	public void AddState(NPCState newState)
	{
		if (newState == null)
		{
			Debug.LogError("StateManager AddState(): Null state");
		}
		
		if (states.Count == 0)
		{
			states.Add(newState);
			currentState = newState;
			currentStateID = newState.ID;
			return;
		}
		
		foreach (NPCState state in states)
		{
			if (state.ID == newState.ID)
			{
				Debug.LogError("StateManager AddState(): " + newState.ID.ToString() + " already exists");
				return;
			}
		}
		states.Add(newState);
	}
	
	public void DeleteState(StateID id)
	{
		if (id == StateID.NullStateID)
		{
			Debug.LogError("StateManager DeleteState(): NullStateID!");
			return;
		}
		
		foreach (NPCState state in states)
		{
			if (state.ID == id)
			{
				states.Remove(state);
				return;
			}
		}
		Debug.LogError("StateManager DeleteState(): " + id.ToString() + " not in list");
	}
	
	public void PerformTransition(Transition transition)
	{
		if (transition == Transition.NullTransition)
		{
			Debug.LogError("StateManager PerformTransition(): NullTransition not allowed");
			return;
		}
		
		StateID id = currentState.GetOutputState(transition);
		if (id == StateID.NullStateID)
		{
			Debug.LogError("StateManager PerformTransition(): " + currentStateID.ToString() +
			               " does not have a next state for transition " + transition.ToString());
			return;
		}
		
		currentStateID = id;
		foreach (NPCState state in states)
		{
			if (state.ID == currentStateID)
			{
				currentState.OnStateExit();
				
				currentState = state;
				
				currentState.OnStateEntered();
				break;
			}
		}
		
	}	
}

public abstract class NPCState
{
	protected Dictionary<Transition, StateID> transitionMap = new Dictionary<Transition, StateID>();
	protected StateID stateID;
	public StateID ID { get { return stateID; } }
	private NPCState currentState;
	public NPCState CurrentState { get { return currentState; } }
	protected GameObject player, npc;
	
	public void AddTransition(Transition transition, StateID id)
	{
		if (transition == Transition.NullTransition)
		{
			Debug.LogError("NPCState AddTransition(): NullTransition is not allowed for a real transition");
			return;
		}
		
		if (id == StateID.NullStateID)
		{
			Debug.LogError("NPCState AddTransition(): NullStateID is not allowed for a real ID");
			return;
		}
		
		if (transitionMap.ContainsKey(transition))
		{
			Debug.LogError("NPCState AddTransition(): " + stateID.ToString() + " already has transition " + 
			               transition.ToString());
			return;
		}
		
		transitionMap.Add(transition, id);
	}
	
	public void DeleteTransition(Transition transition)
	{
		if (transition == Transition.NullTransition)
		{
			Debug.LogError("NPCState DeleteTransition(): NullTransition is not allowed");
			return;
		}
		
		if (transitionMap.ContainsKey(transition))
		{
			transitionMap.Remove(transition);
			return;
		}
		Debug.LogError("NPCState DeleteTransition(): " + transition.ToString() + " is not on " + 
		               stateID.ToString() + " transition list");
	}
	
	public StateID GetOutputState(Transition transition)
	{
		if (transitionMap.ContainsKey(transition))
		{
			return transitionMap[transition];
		}
		return StateID.NullStateID;
	}
	
	public virtual void OnStateEntered() { }
	
	public virtual void OnStateExit() { } 
	
	public abstract void TransitionCondition();
	
	public abstract void StateUpdate();
}