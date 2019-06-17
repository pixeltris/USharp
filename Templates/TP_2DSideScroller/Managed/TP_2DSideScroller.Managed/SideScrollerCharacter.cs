using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.InputCore;
using UnrealEngine.Plugins.Paper2D;
using UnrealEngine.Runtime;

namespace TP_2DSideScroller
{
    [UClass]
    class ASideScrollerCharacter : APaperCharacter
    {
        /// <summary>
        /// Side view camera
        /// </summary>
        [UProperty, VisibleAnywhere, BlueprintReadOnly, Category("Camera")]
        public UCameraComponent SideViewCameraComponent { get; set; }

        /// <summary>
        /// Camera boom positioning the camera beside the character
        /// </summary>
        [UProperty, VisibleAnywhere, BlueprintReadOnly, Category("Camera")]
        public USpringArmComponent CameraBoom { get; set; }

        /// <summary>
        /// The animation to play while running around
        /// </summary>
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Animations")]
        public UPaperFlipbook RunningAnimation { get; set; }

        /// <summary>
        /// The animation to play while idle (standing still)
        /// </summary>
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Animations")]
        public UPaperFlipbook IdleAnimation { get; set; }

        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);

            UCharacterMovementComponent characterMovement = CharacterMovement;
            UCapsuleComponent capsuleComponent = CapsuleComponent;

            // Use only Yaw from the controller and ignore the rest of the rotation.
            UseControllerRotationPitch = false;
            UseControllerRotationYaw = true;
            UseControllerRotationRoll = false;

            // Set the size of our collision capsule.
            capsuleComponent.SetCapsuleHalfHeight(96.0f);
            capsuleComponent.SetCapsuleRadius(40.0f);

            // Create a camera boom attached to the root (capsule)
            CameraBoom = initializer.CreateDefaultSubobject<USpringArmComponent>(this, (FName)"CameraBoom");
            CameraBoom.SetupAttachment(RootComponent);
            CameraBoom.TargetArmLength = 500.0f;
            CameraBoom.SocketOffset = new FVector(0.0f, 0.0f, 75.0f);
            CameraBoom.AbsoluteRotation = true;
            CameraBoom.DoCollisionTest = false;
            CameraBoom.RelativeRotation = new FRotator(0.0f, -90.0f, 0.0f);

            // Create an orthographic camera (no perspective) and attach it to the boom
            SideViewCameraComponent = initializer.CreateDefaultSubobject<UCameraComponent>(this, (FName)"SideViewCamera");
            SideViewCameraComponent.ProjectionMode = ECameraProjectionMode.Orthographic;
            SideViewCameraComponent.OrthoWidth = 2048.0f;
            SideViewCameraComponent.SetupAttachment(CameraBoom, USpringArmComponent.SocketName);

            // Prevent all automatic rotation behavior on the camera, character, and camera component
            CameraBoom.AbsoluteRotation = true;
            SideViewCameraComponent.UsePawnControlRotation = false;
            SideViewCameraComponent.AutoActivate = true;
            characterMovement.OrientRotationToMovement = false;

            // Configure character movement
            characterMovement.GravityScale = 2.0f;
            characterMovement.AirControl = 0.80f;
            characterMovement.JumpZVelocity = 1000.0f;
            characterMovement.GroundFriction = 3.0f;
            characterMovement.MaxWalkSpeed = 600.0f;
            characterMovement.MaxFlySpeed = 600.0f;

            // Lock character motion onto the XZ plane, so the character can't move in or out of the screen
            characterMovement.ConstrainToPlane = true;
            characterMovement.SetPlaneConstraintNormal(new FVector(0.0f, -1.0f, 0.0f));

            // Behave like a traditional 2D platformer character, with a flat bottom instead of a curved capsule bottom
            // Note: This can cause a little floating when going up inclines; you can choose the tradeoff between better
            // behavior on the edge of a ledge versus inclines by setting this to true or false
            characterMovement.UseFlatBaseForFloorChecks = true;

            // Enable replication on the Sprite component so animations show up when networked
            Sprite.SetIsReplicated(true);
            Replicates = true;
        }

        private void UpdateAnimation()
        {
            FVector playerVelocity = GetVelocity();
            float playerSpeedSqr = playerVelocity.SizeSquared();

            // Are we moving or standing still?
            UPaperFlipbook desiredAnimation = playerSpeedSqr > 0.0f ? RunningAnimation : IdleAnimation;
            if (Sprite.GetFlipbook() != desiredAnimation)
            {
                Sprite.SetFlipbook(desiredAnimation);
            }
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            base.ReceiveTick_Implementation(DeltaSeconds);

            UpdateCharacter();
        }

        protected override void SetupPlayerInputComponent(UInputComponent playerInputComponent)
        {
            base.SetupPlayerInputComponent(playerInputComponent);

            // Note: the 'Jump' action and the 'MoveRight' axis are bound to actual keys/buttons/sticks in DefaultInput.ini (editable from Project Settings..Input)
            playerInputComponent.BindAction("Jump", EInputEvent.Pressed, Jump);
            playerInputComponent.BindAction("Jump", EInputEvent.Released, StopJumping);
            playerInputComponent.BindAxis("MoveRight", MoveRight);

            playerInputComponent.BindTouch(EInputEvent.Pressed, TouchStart);
            playerInputComponent.BindTouch(EInputEvent.Released, TouchStopped);
        }

        [UFunction]
        private void MoveRight(float value)
        {
            // Apply the input to the character motion
            AddMovementInput(new FVector(1.0f, 0.0f, 0.0f), value);
        }

        [UFunction]
        private void TouchStart(ETouchIndex fingerIndex, FVector location)
        {
            // Jump on any touch
            Jump();
        }

        [UFunction]
        private void TouchStopped(ETouchIndex fingerIndex, FVector location)
        {
            // Cease jumping once touch stopped
            StopJumping();
        }

        private void UpdateCharacter()
        {
            // Update animation to match the motion
            UpdateAnimation();

            // Now setup the rotation of the controller based on the direction we are travelling
            FVector playerVelocity = GetVelocity();
            float travelDirection = playerVelocity.X;
            // Set the rotation so that the character faces his direction of travel.
            AController controller = GetController();
            if (controller != null)
            {
                if (travelDirection < 0.0f)
                {
                    controller.SetControlRotation(new FRotator(0.0f, 180.0f, 0.0f));
                }
                else if (travelDirection > 0.0f)
                {
                    controller.SetControlRotation(new FRotator(0.0f, 0.0f, 0.0f));
                }
            }
        }
    }
}
