using UnityEngine;
using System.Collections;

/// <summary>
/// Class that allows to check if something is already spawned at a particular spawn point.
/// If an object is at a spawn point it will be considered occupied and unable to be moved to.
/// </summary>
public class SpawnPoint
{
	private bool isOccupied;
	public bool IsOccupied
	{
		get
		{
			return this.isOccupied;
		}
		set
		{
			this.isOccupied = value;
		}
	}
	private Transform xForm;
	public Transform XForm
	{
		get
		{
			return this.xForm;
		}
		set
		{
			this.xForm = value;
		}
	}

	public SpawnPoint(Transform transform)
	{
		// Unoccupied by default.
		this.isOccupied = false;
		this.xForm = transform;
	}
}
