using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening;

[AddComponentMenu("DOTween/DOTween Animation")]
public class DOTweenAnimation : ABSAnimationComponent
{
	public bool targetIsSelf = true;

	public GameObject targetGO;

	public bool tweenTargetIsTargetGO = true;

	public float delay;

	public float duration = 1f;

	public Ease easeType = (Ease)6;

	public AnimationCurve easeCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public LoopType loopType;

	public int loops = 1;

	public string id = string.Empty;

	public bool isRelative;

	public bool isFrom;

	public bool isIndependentUpdate;

	public bool autoKill = true;

	public bool isActive = true;

	public bool isValid;

	public Component target;

	public DOTweenAnimationType animationType;

	public TargetType targetType;

	public TargetType forcedTargetType;

	public bool autoPlay = true;

	public bool useTargetAsV3;

	public float endValueFloat;

	public Vector3 endValueV3;

	public Vector2 endValueV2;

	public Color endValueColor = new Color(1f, 1f, 1f, 1f);

	public string endValueString = string.Empty;

	public Rect endValueRect = new Rect(0f, 0f, 0f, 0f);

	public Transform endValueTransform;

	public bool optionalBool0;

	public float optionalFloat0;

	public int optionalInt0;

	public RotateMode optionalRotationMode;

	public ScrambleMode optionalScrambleMode;

	public string optionalString;

	private bool _tweenCreated;

	private int _playCount = -1;

	private void Awake()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		if (isActive && isValid && ((int)animationType != 1 || !useTargetAsV3))
		{
			CreateTween();
			_tweenCreated = true;
		}
	}

	private void Start()
	{
		if (!_tweenCreated && isActive && isValid)
		{
			CreateTween();
			_tweenCreated = true;
		}
	}

	private void OnDestroy()
	{
		if (base.tween != null && TweenExtensions.IsActive(base.tween))
		{
			TweenExtensions.Kill(base.tween, false);
		}
		base.tween = null;
	}

	public void CreateTween()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected I4, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Invalid comparison between Unknown and I4
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Expected I4, but got Unknown
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Expected I4, but got Unknown
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Invalid comparison between Unknown and I4
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Invalid comparison between Unknown and I4
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Unknown result type (might be due to invalid IL or missing references)
		//IL_099c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Invalid comparison between Unknown and I4
		//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Expected O, but got Unknown
		//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Expected O, but got Unknown
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Expected O, but got Unknown
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Expected O, but got Unknown
		//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4e: Expected O, but got Unknown
		//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b75: Expected O, but got Unknown
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected I4, but got Unknown
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Expected O, but got Unknown
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Invalid comparison between Unknown and I4
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Expected O, but got Unknown
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Expected O, but got Unknown
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Expected O, but got Unknown
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Expected O, but got Unknown
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Expected O, but got Unknown
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Expected O, but got Unknown
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected O, but got Unknown
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Expected O, but got Unknown
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Expected O, but got Unknown
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Expected O, but got Unknown
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Expected O, but got Unknown
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Invalid comparison between Unknown and I4
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Expected O, but got Unknown
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Invalid comparison between Unknown and I4
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Expected O, but got Unknown
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Expected O, but got Unknown
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Expected O, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Invalid comparison between Unknown and I4
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Expected O, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Invalid comparison between Unknown and I4
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Invalid comparison between Unknown and I4
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0909: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Expected O, but got Unknown
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Expected O, but got Unknown
		//IL_04b3: Expected O, but got Unknown
		//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bac: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Expected O, but got Unknown
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Invalid comparison between Unknown and I4
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c32: Expected O, but got Unknown
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c52: Invalid comparison between Unknown and I4
		//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce6: Expected O, but got Unknown
		//IL_0d1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d25: Expected O, but got Unknown
		//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d64: Expected O, but got Unknown
		//IL_0d99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Expected O, but got Unknown
		//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de2: Expected O, but got Unknown
		//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e21: Expected O, but got Unknown
		GameObject tweenGO = GetTweenGO();
		if ((Object)(object)target == (Object)null || (Object)(object)tweenGO == (Object)null)
		{
			if (targetIsSelf && (Object)(object)target == (Object)null)
			{
				Debug.LogWarning((object)$"{((Object)((Component)this).gameObject).name} :: This DOTweenAnimation's target is NULL, because the animation was created with a DOTween Pro version older than 0.9.255. To fix this, exit Play mode then simply select this object, and it will update automatically", (Object)(object)((Component)this).gameObject);
			}
			else
			{
				Debug.LogWarning((object)$"{((Object)((Component)this).gameObject).name} :: This DOTweenAnimation's target/GameObject is unset: the tween will not be created.", (Object)(object)((Component)this).gameObject);
			}
			return;
		}
		if ((int)forcedTargetType != 0)
		{
			targetType = forcedTargetType;
		}
		if ((int)targetType == 0)
		{
			targetType = TypeToDOTargetType(((object)target).GetType());
		}
		DOTweenAnimationType val = animationType;
		switch ((int)val)
		{
		case 1:
		{
			if (useTargetAsV3)
			{
				isRelative = false;
				if ((Object)(object)endValueTransform == (Object)null)
				{
					Debug.LogWarning((object)$"{((Object)((Component)this).gameObject).name} :: This tween's TO target is NULL, a Vector3 of (0,0,0) will be used instead", (Object)(object)((Component)this).gameObject);
					endValueV3 = Vector3.zero;
				}
				else if ((int)targetType == 5)
				{
					Transform obj = endValueTransform;
					RectTransform val3 = (RectTransform)(object)((obj is RectTransform) ? obj : null);
					if ((Object)(object)val3 == (Object)null)
					{
						Debug.LogWarning((object)$"{((Object)((Component)this).gameObject).name} :: This tween's TO target should be a RectTransform, a Vector3 of (0,0,0) will be used instead", (Object)(object)((Component)this).gameObject);
						endValueV3 = Vector3.zero;
					}
					else
					{
						Component obj2 = target;
						RectTransform val4 = (RectTransform)(object)((obj2 is RectTransform) ? obj2 : null);
						if ((Object)(object)val4 == (Object)null)
						{
							Debug.LogWarning((object)$"{((Object)((Component)this).gameObject).name} :: This tween's target and TO target are not of the same type. Please reassign the values", (Object)(object)((Component)this).gameObject);
						}
						else
						{
							endValueV3 = Vector2.op_Implicit(DOTweenModuleUI.Utils.SwitchToRectTransform(val3, val4));
						}
					}
				}
				else
				{
					endValueV3 = endValueTransform.position;
				}
			}
			TargetType val5 = targetType;
			switch (val5 - 5)
			{
			case 6:
				base.tween = (Tween)(object)ShortcutExtensions.DOMove((Transform)target, endValueV3, duration, optionalBool0);
				break;
			case 0:
				base.tween = (Tween)(object)DOTweenModuleUI.DOAnchorPos3D((RectTransform)target, endValueV3, duration, optionalBool0);
				break;
			case 3:
				base.tween = (Tween)(object)DOTweenModulePhysics.DOMove((Rigidbody)target, endValueV3, duration, optionalBool0);
				break;
			case 4:
				base.tween = (Tween)(object)DOTweenModulePhysics2D.DOMove((Rigidbody2D)target, Vector2.op_Implicit(endValueV3), duration, optionalBool0);
				break;
			}
			break;
		}
		case 2:
			base.tween = (Tween)(object)ShortcutExtensions.DOLocalMove(tweenGO.transform, endValueV3, duration, optionalBool0);
			break;
		case 3:
		{
			TargetType val11 = targetType;
			if ((int)val11 != 11)
			{
				if ((int)val11 != 8)
				{
					if ((int)val11 == 9)
					{
						base.tween = (Tween)(object)DOTweenModulePhysics2D.DORotate((Rigidbody2D)target, endValueFloat, duration);
					}
				}
				else
				{
					base.tween = (Tween)(object)DOTweenModulePhysics.DORotate((Rigidbody)target, endValueV3, duration, optionalRotationMode);
				}
			}
			else
			{
				base.tween = (Tween)(object)ShortcutExtensions.DORotate((Transform)target, endValueV3, duration, optionalRotationMode);
			}
			break;
		}
		case 4:
			base.tween = (Tween)(object)ShortcutExtensions.DOLocalRotate(tweenGO.transform, endValueV3, duration, optionalRotationMode);
			break;
		case 5:
			base.tween = (Tween)(object)ShortcutExtensions.DOScale(tweenGO.transform, (Vector3)((!optionalBool0) ? endValueV3 : new Vector3(endValueFloat, endValueFloat, endValueFloat)), duration);
			break;
		case 21:
			base.tween = (Tween)(object)DOTweenModuleUI.DOSizeDelta((RectTransform)target, (Vector2)((!optionalBool0) ? endValueV2 : new Vector2(endValueFloat, endValueFloat)), duration);
			break;
		case 6:
		{
			isRelative = false;
			TargetType val8 = targetType;
			switch (val8 - 3)
			{
			case 3:
				base.tween = (Tween)(object)ShortcutExtensions.DOColor(((Renderer)target).material, endValueColor, duration);
				break;
			case 1:
				base.tween = (Tween)(object)ShortcutExtensions.DOColor((Light)target, endValueColor, duration);
				break;
			case 4:
				base.tween = (Tween)(object)DOTweenModuleSprite.DOColor((SpriteRenderer)target, endValueColor, duration);
				break;
			case 0:
				base.tween = (Tween)(object)DOTweenModuleUI.DOColor((Image)target, endValueColor, duration);
				break;
			case 7:
				base.tween = (Tween)(object)DOTweenModuleUI.DOColor((Text)target, endValueColor, duration);
				break;
			case 12:
				base.tween = (Tween)(object)((TextMeshProUGUI)(object)target).DOColor(endValueColor, duration);
				break;
			case 11:
				base.tween = (Tween)(object)((TextMeshPro)(object)target).DOColor(endValueColor, duration);
				break;
			}
			break;
		}
		case 7:
		{
			isRelative = false;
			TargetType val7 = targetType;
			switch (val7 - 2)
			{
			case 4:
				base.tween = (Tween)(object)ShortcutExtensions.DOFade(((Renderer)target).material, endValueFloat, duration);
				break;
			case 2:
				base.tween = (Tween)(object)ShortcutExtensions.DOIntensity((Light)target, endValueFloat, duration);
				break;
			case 5:
				base.tween = (Tween)(object)DOTweenModuleSprite.DOFade((SpriteRenderer)target, endValueFloat, duration);
				break;
			case 1:
				base.tween = (Tween)(object)DOTweenModuleUI.DOFade((Image)target, endValueFloat, duration);
				break;
			case 8:
				base.tween = (Tween)(object)DOTweenModuleUI.DOFade((Text)target, endValueFloat, duration);
				break;
			case 0:
				base.tween = (Tween)(object)DOTweenModuleUI.DOFade((CanvasGroup)target, endValueFloat, duration);
				break;
			case 13:
				base.tween = (Tween)(object)((TextMeshProUGUI)(object)target).DOFade(endValueFloat, duration);
				break;
			case 12:
				base.tween = (Tween)(object)((TextMeshPro)(object)target).DOFade(endValueFloat, duration);
				break;
			}
			break;
		}
		case 8:
		{
			TargetType val9 = targetType;
			if ((int)val9 == 10)
			{
				base.tween = (Tween)(object)DOTweenModuleUI.DOText((Text)target, endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
			}
			TargetType val10 = targetType;
			if ((int)val10 != 15)
			{
				if ((int)val10 == 14)
				{
					base.tween = (Tween)(object)((TextMeshPro)(object)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
				}
			}
			else
			{
				base.tween = (Tween)(object)((TextMeshProUGUI)(object)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
			}
			break;
		}
		case 9:
		{
			TargetType val6 = targetType;
			if ((int)val6 != 11)
			{
				if ((int)val6 == 5)
				{
					base.tween = (Tween)(object)DOTweenModuleUI.DOPunchAnchorPos((RectTransform)target, Vector2.op_Implicit(endValueV3), duration, optionalInt0, optionalFloat0, optionalBool0);
				}
			}
			else
			{
				base.tween = (Tween)(object)ShortcutExtensions.DOPunchPosition((Transform)target, endValueV3, duration, optionalInt0, optionalFloat0, optionalBool0);
			}
			break;
		}
		case 11:
			base.tween = (Tween)(object)ShortcutExtensions.DOPunchScale(tweenGO.transform, endValueV3, duration, optionalInt0, optionalFloat0);
			break;
		case 10:
			base.tween = (Tween)(object)ShortcutExtensions.DOPunchRotation(tweenGO.transform, endValueV3, duration, optionalInt0, optionalFloat0);
			break;
		case 12:
		{
			TargetType val2 = targetType;
			if ((int)val2 != 11)
			{
				if ((int)val2 == 5)
				{
					base.tween = (Tween)(object)DOTweenModuleUI.DOShakeAnchorPos((RectTransform)target, duration, Vector2.op_Implicit(endValueV3), optionalInt0, optionalFloat0, optionalBool0);
				}
			}
			else
			{
				base.tween = (Tween)(object)ShortcutExtensions.DOShakePosition((Transform)target, duration, endValueV3, optionalInt0, optionalFloat0, optionalBool0, true);
			}
			break;
		}
		case 14:
			base.tween = (Tween)(object)ShortcutExtensions.DOShakeScale(tweenGO.transform, duration, endValueV3, optionalInt0, optionalFloat0, true);
			break;
		case 13:
			base.tween = (Tween)(object)ShortcutExtensions.DOShakeRotation(tweenGO.transform, duration, endValueV3, optionalInt0, optionalFloat0, true);
			break;
		case 15:
			base.tween = (Tween)(object)ShortcutExtensions.DOAspect((Camera)target, endValueFloat, duration);
			break;
		case 16:
			base.tween = (Tween)(object)ShortcutExtensions.DOColor((Camera)target, endValueColor, duration);
			break;
		case 17:
			base.tween = (Tween)(object)ShortcutExtensions.DOFieldOfView((Camera)target, endValueFloat, duration);
			break;
		case 18:
			base.tween = (Tween)(object)ShortcutExtensions.DOOrthoSize((Camera)target, endValueFloat, duration);
			break;
		case 19:
			base.tween = (Tween)(object)ShortcutExtensions.DOPixelRect((Camera)target, endValueRect, duration);
			break;
		case 20:
			base.tween = (Tween)(object)ShortcutExtensions.DORect((Camera)target, endValueRect, duration);
			break;
		}
		if (base.tween == null)
		{
			return;
		}
		if (isFrom)
		{
			TweenSettingsExtensions.From<Tweener>((Tweener)base.tween, isRelative);
		}
		else
		{
			TweenSettingsExtensions.SetRelative<Tween>(base.tween, isRelative);
		}
		GameObject val12 = ((!targetIsSelf && tweenTargetIsTargetGO) ? targetGO : ((Component)this).gameObject);
		TweenSettingsExtensions.OnKill<Tween>(TweenSettingsExtensions.SetAutoKill<Tween>(TweenSettingsExtensions.SetLoops<Tween>(TweenSettingsExtensions.SetDelay<Tween>(TweenSettingsExtensions.SetTarget<Tween>(base.tween, (object)val12), delay), loops, loopType), autoKill), (TweenCallback)delegate
		{
			base.tween = null;
		});
		if (base.isSpeedBased)
		{
			TweenSettingsExtensions.SetSpeedBased<Tween>(base.tween);
		}
		if ((int)easeType == 37)
		{
			TweenSettingsExtensions.SetEase<Tween>(base.tween, easeCurve);
		}
		else
		{
			TweenSettingsExtensions.SetEase<Tween>(base.tween, easeType);
		}
		if (!string.IsNullOrEmpty(id))
		{
			TweenSettingsExtensions.SetId<Tween>(base.tween, id);
		}
		TweenSettingsExtensions.SetUpdate<Tween>(base.tween, isIndependentUpdate);
		if (base.hasOnStart)
		{
			if (base.onStart != null)
			{
				TweenSettingsExtensions.OnStart<Tween>(base.tween, new TweenCallback(base.onStart.Invoke));
			}
		}
		else
		{
			base.onStart = null;
		}
		if (base.hasOnPlay)
		{
			if (base.onPlay != null)
			{
				TweenSettingsExtensions.OnPlay<Tween>(base.tween, new TweenCallback(base.onPlay.Invoke));
			}
		}
		else
		{
			base.onPlay = null;
		}
		if (base.hasOnUpdate)
		{
			if (base.onUpdate != null)
			{
				TweenSettingsExtensions.OnUpdate<Tween>(base.tween, new TweenCallback(base.onUpdate.Invoke));
			}
		}
		else
		{
			base.onUpdate = null;
		}
		if (base.hasOnStepComplete)
		{
			if (base.onStepComplete != null)
			{
				TweenSettingsExtensions.OnStepComplete<Tween>(base.tween, new TweenCallback(base.onStepComplete.Invoke));
			}
		}
		else
		{
			base.onStepComplete = null;
		}
		if (base.hasOnComplete)
		{
			if (base.onComplete != null)
			{
				TweenSettingsExtensions.OnComplete<Tween>(base.tween, new TweenCallback(base.onComplete.Invoke));
			}
		}
		else
		{
			base.onComplete = null;
		}
		if (base.hasOnRewind)
		{
			if (base.onRewind != null)
			{
				TweenSettingsExtensions.OnRewind<Tween>(base.tween, new TweenCallback(base.onRewind.Invoke));
			}
		}
		else
		{
			base.onRewind = null;
		}
		if (autoPlay)
		{
			TweenExtensions.Play<Tween>(base.tween);
		}
		else
		{
			TweenExtensions.Pause<Tween>(base.tween);
		}
		if (base.hasOnTweenCreated && base.onTweenCreated != null)
		{
			base.onTweenCreated.Invoke();
		}
	}

	public override void DOPlay()
	{
		DOTween.Play((object)((Component)this).gameObject);
	}

	public override void DOPlayBackwards()
	{
		DOTween.PlayBackwards((object)((Component)this).gameObject);
	}

	public override void DOPlayForward()
	{
		DOTween.PlayForward((object)((Component)this).gameObject);
	}

	public override void DOPause()
	{
		DOTween.Pause((object)((Component)this).gameObject);
	}

	public override void DOTogglePause()
	{
		DOTween.TogglePause((object)((Component)this).gameObject);
	}

	public override void DORewind()
	{
		_playCount = -1;
		DOTweenAnimation[] components = ((Component)this).gameObject.GetComponents<DOTweenAnimation>();
		for (int num = components.Length - 1; num > -1; num--)
		{
			Tween tween = ((ABSAnimationComponent)components[num]).tween;
			if (tween != null && TweenExtensions.IsInitialized(tween))
			{
				TweenExtensions.Rewind(((ABSAnimationComponent)components[num]).tween, true);
			}
		}
	}

	public override void DORestart(bool fromHere = false)
	{
		_playCount = -1;
		if (base.tween == null)
		{
			if (Debugger.logPriority > 1)
			{
				Debugger.LogNullTween(base.tween);
			}
			return;
		}
		if (fromHere && isRelative)
		{
			ReEvaluateRelativeTween();
		}
		DOTween.Restart((object)((Component)this).gameObject, true, -1f);
	}

	public override void DOComplete()
	{
		DOTween.Complete((object)((Component)this).gameObject, false);
	}

	public override void DOKill()
	{
		DOTween.Kill((object)((Component)this).gameObject, false);
		base.tween = null;
	}

	public void DOPlayById(string id)
	{
		DOTween.Play((object)((Component)this).gameObject, (object)id);
	}

	public void DOPlayAllById(string id)
	{
		DOTween.Play((object)id);
	}

	public void DOPauseAllById(string id)
	{
		DOTween.Pause((object)id);
	}

	public void DOPlayBackwardsById(string id)
	{
		DOTween.PlayBackwards((object)((Component)this).gameObject, (object)id);
	}

	public void DOPlayBackwardsAllById(string id)
	{
		DOTween.PlayBackwards((object)id);
	}

	public void DOPlayForwardById(string id)
	{
		DOTween.PlayForward((object)((Component)this).gameObject, (object)id);
	}

	public void DOPlayForwardAllById(string id)
	{
		DOTween.PlayForward((object)id);
	}

	public void DOPlayNext()
	{
		DOTweenAnimation[] components = ((Component)this).GetComponents<DOTweenAnimation>();
		while (_playCount < components.Length - 1)
		{
			_playCount++;
			DOTweenAnimation dOTweenAnimation = components[_playCount];
			if ((Object)(object)dOTweenAnimation != (Object)null && ((ABSAnimationComponent)dOTweenAnimation).tween != null && !TweenExtensions.IsPlaying(((ABSAnimationComponent)dOTweenAnimation).tween) && !TweenExtensions.IsComplete(((ABSAnimationComponent)dOTweenAnimation).tween))
			{
				TweenExtensions.Play<Tween>(((ABSAnimationComponent)dOTweenAnimation).tween);
				break;
			}
		}
	}

	public void DORewindAndPlayNext()
	{
		_playCount = -1;
		DOTween.Rewind((object)((Component)this).gameObject, true);
		DOPlayNext();
	}

	public void DORewindAllById(string id)
	{
		_playCount = -1;
		DOTween.Rewind((object)id, true);
	}

	public void DORestartById(string id)
	{
		_playCount = -1;
		DOTween.Restart((object)((Component)this).gameObject, (object)id, true, -1f);
	}

	public void DORestartAllById(string id)
	{
		_playCount = -1;
		DOTween.Restart((object)id, true, -1f);
	}

	public List<Tween> GetTweens()
	{
		List<Tween> list = new List<Tween>();
		DOTweenAnimation[] components = ((Component)this).GetComponents<DOTweenAnimation>();
		DOTweenAnimation[] array = components;
		foreach (DOTweenAnimation dOTweenAnimation in array)
		{
			list.Add(((ABSAnimationComponent)dOTweenAnimation).tween);
		}
		return list;
	}

	public static TargetType TypeToDOTargetType(Type t)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		string text = t.ToString();
		int num = text.LastIndexOf(".");
		if (num != -1)
		{
			text = text.Substring(num + 1);
		}
		if (text.IndexOf("Renderer") != -1 && text != "SpriteRenderer")
		{
			text = "Renderer";
		}
		return (TargetType)Enum.Parse(typeof(TargetType), text);
	}

	public Tween CreateEditorPreview()
	{
		if (Application.isPlaying)
		{
			return null;
		}
		CreateTween();
		return base.tween;
	}

	private GameObject GetTweenGO()
	{
		return (!targetIsSelf) ? targetGO : ((Component)this).gameObject;
	}

	private void ReEvaluateRelativeTween()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		GameObject tweenGO = GetTweenGO();
		if ((Object)(object)tweenGO == (Object)null)
		{
			Debug.LogWarning((object)$"{((Object)((Component)this).gameObject).name} :: This DOTweenAnimation's target/GameObject is unset: the tween will not be created.", (Object)(object)((Component)this).gameObject);
		}
		else if ((int)animationType == 1)
		{
			((Tweener)base.tween).ChangeEndValue((object)(tweenGO.transform.position + endValueV3), true);
		}
		else if ((int)animationType == 2)
		{
			((Tweener)base.tween).ChangeEndValue((object)(tweenGO.transform.localPosition + endValueV3), true);
		}
	}
}
