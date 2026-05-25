using UnityEngine;
using UnityEngine.Rendering;

internal class CameraData
{
	public RenderingMethod method;

	public CommandBuffer maskBuffer;

	public CommandBuffer projectionBuffer;

	public CullingGroup maskCulling;

	public CullingGroup projectionCulling;

	public bool enabled;

	public bool sceneCamera;

	public bool previewCamera;

	public CustomDepthTextureMode customDTM;

	public DepthTextureMode? originalDTM;

	public DepthTextureMode? desiredDTM;

	public CameraData(Camera Camera)
	{
		sceneCamera = ((Object)Camera).name == "SceneCamera";
		previewCamera = ((Object)Camera).name == "Preview Camera";
	}

	public void Initialize(Camera Camera, DynamicDecals System)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		maskBuffer = new CommandBuffer();
		maskBuffer.name = "Dynamic Decals - Masking";
		projectionBuffer = new CommandBuffer();
		projectionBuffer.name = "Dynamic Decals - Projection";
		maskCulling = new CullingGroup();
		projectionCulling = new CullingGroup();
		maskCulling.targetCamera = Camera;
		projectionCulling.targetCamera = Camera;
		enabled = true;
		InitializeRenderingMethod(Camera);
	}

	public void Terminate(Camera Camera)
	{
		RestoreDepthTextureMode(Camera);
		TerminateRenderingMethod(Camera);
		if (maskCulling != null)
		{
			maskCulling.Dispose();
			maskCulling = null;
		}
		if (projectionCulling != null)
		{
			projectionCulling.Dispose();
			maskCulling = null;
		}
		enabled = false;
	}

	public void InitializeRenderingMethod(Camera Camera)
	{
		if (method == RenderingMethod.ForwardLow && (sceneCamera || previewCamera))
		{
			method = RenderingMethod.ForwardHigh;
		}
		switch (method)
		{
		case RenderingMethod.ForwardLow:
			Camera.AddCommandBuffer((CameraEvent)3, maskBuffer);
			customDTM = CustomDepthTextureMode.None;
			desiredDTM = (DepthTextureMode)2;
			SetDepthTextureMode(Camera);
			break;
		case RenderingMethod.ForwardHigh:
			Camera.AddCommandBuffer((CameraEvent)1, maskBuffer);
			customDTM = CustomDepthTextureMode.Normal;
			desiredDTM = (DepthTextureMode)1;
			SetDepthTextureMode(Camera);
			break;
		case RenderingMethod.ForwardForced:
			Camera.AddCommandBuffer((CameraEvent)10, maskBuffer);
			customDTM = CustomDepthTextureMode.Normal;
			desiredDTM = (DepthTextureMode)1;
			SetDepthTextureMode(Camera);
			break;
		case RenderingMethod.Deferred:
			Camera.AddCommandBuffer((CameraEvent)21, maskBuffer);
			Camera.AddCommandBuffer((CameraEvent)21, projectionBuffer);
			customDTM = CustomDepthTextureMode.None;
			RestoreDepthTextureMode(Camera);
			break;
		}
	}

	public void TerminateRenderingMethod(Camera Camera)
	{
		if (!((Object)(object)Camera != (Object)null))
		{
			return;
		}
		switch (method)
		{
		case RenderingMethod.ForwardLow:
			if (maskBuffer != null)
			{
				Camera.RemoveCommandBuffer((CameraEvent)3, maskBuffer);
			}
			break;
		case RenderingMethod.ForwardHigh:
			if (maskBuffer != null)
			{
				Camera.RemoveCommandBuffer((CameraEvent)1, maskBuffer);
			}
			break;
		case RenderingMethod.ForwardForced:
			if (maskBuffer != null)
			{
				Camera.RemoveCommandBuffer((CameraEvent)10, maskBuffer);
			}
			break;
		case RenderingMethod.Deferred:
			if (maskBuffer != null)
			{
				Camera.RemoveCommandBuffer((CameraEvent)21, maskBuffer);
			}
			if (projectionBuffer != null)
			{
				Camera.RemoveCommandBuffer((CameraEvent)21, projectionBuffer);
			}
			break;
		}
	}

	public void UpdateRenderingMethod(Camera Camera, DynamicDecals System)
	{
		RenderingMethod renderingMethod = ((System.renderingPath == SystemPath.Deferred) ? ((!System.Settings.forceForward) ? RenderingMethod.Deferred : RenderingMethod.ForwardForced) : (System.Settings.highPrecision ? RenderingMethod.ForwardHigh : RenderingMethod.ForwardLow));
		if (method != renderingMethod)
		{
			TerminateRenderingMethod(Camera);
			method = renderingMethod;
			InitializeRenderingMethod(Camera);
		}
	}

	public void SetDepthTextureMode(Camera Camera)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (desiredDTM.HasValue)
		{
			if ((DepthTextureMode?)Camera.depthTextureMode != desiredDTM)
			{
				if (!originalDTM.HasValue)
				{
					originalDTM = Camera.depthTextureMode;
				}
				else
				{
					Camera.depthTextureMode = originalDTM.Value;
				}
				Camera.depthTextureMode |= desiredDTM.Value;
			}
		}
		else
		{
			RestoreDepthTextureMode(Camera);
		}
	}

	public void RestoreDepthTextureMode(Camera Camera)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (originalDTM.HasValue && (Object)(object)Camera != (Object)null)
		{
			Camera.depthTextureMode = originalDTM.Value;
		}
	}
}
