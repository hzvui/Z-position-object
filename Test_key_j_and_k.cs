#region Math Variables
#if UNIGINE_DOUBLE
	using Scalar = System.Double;
	using Vec2 = Unigine.dvec2;
	using Vec3 = Unigine.dvec3;
	using Vec4 = Unigine.dvec4;
	using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ca2d39d48695fe3a4463a06462e7de03ae3bbf2a")]
public class Test_key_j_and_k : Component
{
	    [ShowInEditor]
    [ParameterSlider(Title = "поднять конструкцию", Group = "VR Object Switch")]
    public Node buttonUp;

    [ShowInEditor]
    [ParameterSlider(Title = "опустить конструкцию", Group = "VR Object Switch")]
    public Node buttonDown;

	[ShowInEditor]
    [ParameterSlider(Title = "изначальное положение кнопки", Group = "VR Object Switch")]
    public float ThresholdZ = 0.61f;

	[ShowInEditor]
	[ParameterSlider(Title = "Move Speed", Group = "Height Control", Min = 0.1f)]
	private Scalar moveSpeed = 2.0f;

	[ShowInEditor]
	[ParameterSlider(Title = "Move Smoothness", Group = "Height Control", Min = 0.0f, Max = 1.0f)]
	private Scalar smoothness = 0.1f;

	private Vec3 targetPosition;
	private bool isMoving = false;

	[ShowInEditor]
    public Scalar DeltaZ { get; private set; } = 0;
	private Vec3 initialPosition; // запомним исходную позицию

	private AssemblyGuide assemblyGuide;

	protected override void OnReady()
	{
		initialPosition = node.Position;
		targetPosition = node.Position;
		DeltaZ = 0; // начальная дельта = 0
	}

	private void Update()
	{
		assemblyGuide = FindComponentInWorld<AssemblyGuide>();
		if (assemblyGuide == null)
		{
			Log.Warning("Test_key_j_and_k: AssemblyGuide not found!");
		}
		HandleKeyboardInput();
		SmoothMove();
		// Обновляем DeltaZ как разницу по Z между текущей и начальной позицией
        DeltaZ = node.Position.z - initialPosition.z;
	}

	private void HandleKeyboardInput()
	{
		        bool isUpActive = false;
        bool isDownActive = false;

        if (buttonUp != null)
            isUpActive = buttonUp.Position.z <= ThresholdZ;

        if (buttonDown != null)
            isDownActive = buttonDown.Position.z <= ThresholdZ;

		// Поднимаем объект при нажатии J
		if (isUpActive && !isDownActive)
		{
			targetPosition.z += moveSpeed * Game.IFps;
			var currentStep = assemblyGuide.Steps[assemblyGuide.currentStepIndex];
			currentStep.SourceObject.WorldPosition = currentStep.InitialPosition;
			isMoving = true;
		}

		// Опускаем объект при нажатии K
		else if (isDownActive && !isUpActive)
		{
			targetPosition.z -= moveSpeed * Game.IFps;
			var currentStep = assemblyGuide.Steps[assemblyGuide.currentStepIndex];
			currentStep.SourceObject.WorldPosition = currentStep.InitialPosition;
			isMoving = true;
		}
	}

	private void SmoothMove()
	{
		if (isMoving)
		{
			//перемещение к целевой позиции
			node.Position = new Vec3(MathLib.Lerp(node.Position, targetPosition, smoothness));
			
			if (MathLib.Length2(node.Position - targetPosition) < 0.001f)
			{
				node.Position = targetPosition;
				isMoving = false;
			}
		}
	}

}