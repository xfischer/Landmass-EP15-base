using UnityEngine;
using System.Collections;

public class UpdatableData : ScriptableObject {

	public event System.Action OnValuesUpdated;

	public bool autoUpdate;

	protected virtual void OnValidate() {
		if (autoUpdate) {
			NotifyOnUpdatedValues();
		}
	}

	public void NotifyOnUpdatedValues() {
		if (OnValuesUpdated != null			) {
			OnValuesUpdated();
		}
	}


}
 