using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;
using UnrealEngine.HeadMountedDisplay;
using UnrealEngine.Runtime;

namespace TP_Puzzle
{
    [UClass]
    class APuzzlePlayerCharacter : ACharacter
    {
        [UProperty, Tooltip("How many blocks have been clicked")]
        private APuzzleBlock CurrenBlockFocus { get; set; }

        [UProperty]
        private UCameraComponent FpCamera { get; set; }

        [UProperty]
        private UCameraComponent VrCamera { get; set; }

        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);

            VrCamera = initializer.CreateDefaultSubobject<UCameraComponent>(this, new FName("VR_Camera"));
            VrCamera.RelativeLocation = new FVector(-100, 0, 270);
            VrCamera.AutoActivate = false;
            VrCamera.AttachToComponent(CapsuleComponent, FName.None, EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, false);

            FpCamera = initializer.CreateDefaultSubobject<UCameraComponent>(this, new FName("FP_Camera"));
            FpCamera.RelativeLocation = new FVector(600, 0, 230);
            FpCamera.RelativeRotation = new FRotator(-89.9000015f, -1.5f, 1.5f);
            FpCamera.AutoActivate = false;
            FpCamera.AttachToComponent(CapsuleComponent, FName.None, EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, false);

            CharacterMovement.GravityScale = 0;
            UseControllerRotationYaw = false;

            // Enable tick
            FTickFunction tickFunction = PrimaryActorTick;
            tickFunction.StartWithTickEnabled = true;
            tickFunction.CanEverTick = true;
        }

        protected override void BeginPlay()
        {
            base.BeginPlay();

            // Choose Camera based on whether HMD is enabled
            if (UHeadMountedDisplayFunctionLibrary.IsHeadMountedDisplayEnabled())
            {
                VrCamera.Activate();
            }
            else
            {
                FpCamera.Activate();
            }
        }

        protected override void SetupPlayerInputComponent(UInputComponent playerInputComponent)
        {
            base.SetupPlayerInputComponent(playerInputComponent);

            // Use R Controller Trigger to process click events
            playerInputComponent.BindAction("TriggerClick", EInputEventType.Pressed, OnTriggerClick);
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            base.ReceiveTick_Implementation(DeltaSeconds);

            // Determine which Puzzle Block the player is looking at and highlight it.
            if (UHeadMountedDisplayFunctionLibrary.IsHeadMountedDisplayEnabled())
            {
                UCameraComponent camera = VrCamera;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                FVector start = camera.GetWorldLocation();
                FVector end = start + (camera.GetWorldRotation().GetForwardVector() * 8000);
                FHitResult hitResult;
                bool hit = USystemLibrary.LineTraceSingle(this, start, end, UEngineTypes.ConvertToTraceType(ECollisionChannel.Camera),
                    false, null, EDrawDebugTrace.None, out hitResult, true, FLinearColor.Red, FLinearColor.Green, 0);
                APuzzleBlock block = hit ? hitResult.Actor.Value as APuzzleBlock : null;
                if (block != null)
                {
                    CurrenBlockFocus = block;
                    block.DoHighlight(true);
                }
                else if (CurrenBlockFocus != null)
                {
                    CurrenBlockFocus.DoHighlight(false);
                    CurrenBlockFocus = null;
                }
            }
        }

        // Must be marked as a [UFunction] so that the engine can call this function when the button is pressed
        [UFunction]
        private void OnTriggerClick()
        {
            if (CurrenBlockFocus != null)
            {
                CurrenBlockFocus.HandleControllerClick();
            }
        }
    }
}
