using System;
using System.Collections.Generic;
using UnityEngine;

public class RiverWaves : MonoBehaviour
{
	[Serializable]
	public class WaveContainer
	{
		public Vector3 Center;

		public float Amplitude;

		public float Speed = 2f;

		public float Frequency = 0.04f;

		internal float ScaledDistance;

		internal float Timer;

		public WaveContainer(Vector3 _Position, float _Amplitude = 2f, float _Frequency = 0.04f, float _Speed = 2f)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Center = _Position;
			Amplitude = _Amplitude;
			Frequency = _Frequency;
			Speed = _Speed;
		}
	}

	public List<WaveContainer> CurrentWaves = new List<WaveContainer>();

	private Mesh WaterMesh;

	private MeshCollider WaterMeshCollider;

	[SerializeField]
	private float waveScale;

	[SerializeField]
	private float Turbulance_A;

	[SerializeField]
	private float Turbulance_B;

	[SerializeField]
	private float Speed;

	private Vector3[] StartingVertPositions;

	private Vector3[] newVertPositions;

	private void Start()
	{
		WaterMesh = ((Component)this).GetComponent<MeshFilter>().mesh;
		WaterMeshCollider = ((Component)this).GetComponent<MeshCollider>();
		StartingVertPositions = WaterMesh.vertices;
		newVertPositions = (Vector3[])StartingVertPositions.Clone();
	}

	public void AddWave(Vector3 _Position, float _Amplitude, float _Frequency = 0.04f, float _Speed = 2f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		CurrentWaves.Add(new WaveContainer(_Position, _Amplitude, _Frequency, _Speed));
	}

	private void Update()
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		for (int num = CurrentWaves.Count - 1; num >= 0; num--)
		{
			WaveContainer waveContainer = CurrentWaves[num];
			waveContainer.Timer += Time.deltaTime;
			waveContainer.ScaledDistance += Time.deltaTime * 10f;
			waveContainer.Amplitude = Mathf.Lerp(waveContainer.Amplitude, 0f, waveContainer.ScaledDistance / 3000f * waveContainer.Speed);
			waveContainer.Frequency = Mathf.Lerp(waveContainer.Frequency, 0f, waveContainer.ScaledDistance / 3000f * waveContainer.Speed);
			if ((double)waveContainer.Amplitude < 0.01)
			{
				CurrentWaves.Remove(waveContainer);
			}
		}
		for (int i = 0; i < StartingVertPositions.Length; i++)
		{
			Vector3 val = StartingVertPositions[i];
			Vector3 val2 = ((Component)this).transform.TransformPoint(StartingVertPositions[i]);
			val.y = Mathf.Sin(val2.x * 1.2f * Turbulance_B + val2.z * -0.75f * Turbulance_A + Time.time * Speed * Turbulance_A) * waveScale;
			val.y += Mathf.Sin(val2.x * -1.16f * Turbulance_A + val2.z * 1.24f * Turbulance_B + Time.time * Speed * Turbulance_B) * waveScale;
			val.y += Mathf.Sin(val2.x + val2.z) * waveScale;
			for (int j = 0; j < CurrentWaves.Count; j++)
			{
				WaveContainer waveContainer2 = CurrentWaves[j];
				Vector3 val3 = val2 - waveContainer2.Center;
				float num2 = Mathf.Clamp01(1f - ((Vector3)(ref val3)).magnitude / waveContainer2.ScaledDistance);
				float num3 = (val2.x - waveContainer2.Center.x) * (val2.x - waveContainer2.Center.x);
				float num4 = (val2.z - waveContainer2.Center.z) * (val2.z - waveContainer2.Center.z);
				val.y += waveContainer2.Amplitude * (Mathf.Sin((num3 + num4) * waveContainer2.Frequency) + Mathf.Sin(waveContainer2.Timer)) * num2;
			}
			newVertPositions[i] = val;
		}
		WaterMesh.vertices = newVertPositions;
		WaterMesh.RecalculateNormals();
		if (Object.op_Implicit((Object)(object)WaterMeshCollider))
		{
			WaterMeshCollider.sharedMesh = WaterMesh;
		}
	}
}
