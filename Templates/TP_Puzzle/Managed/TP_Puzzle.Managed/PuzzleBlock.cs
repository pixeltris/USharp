using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;
using UnrealEngine.InputCore;
using UnrealEngine.Runtime;

namespace TP_Puzzle
{
    [UClass]
    class APuzzleBlock : AActor
    {
        // These are private fields. These aren't visible to UE4 in any way.
        // This may be undesirable; such as in cases where you may want to duplicate a block. These wouldn't get serialized.
        private bool isActive;
        private bool isHighlighted;

        [UProperty, EditAnywhere, BlueprintReadWrite, ExposeOnSpawn]
        public APuzzleBlockGrid OwningGrid { get; set; }

        [UProperty]
        public UStaticMeshComponent BlockMesh { get; set; }

        // Cache some materials (GameStaticVar values are cleared each time the game is restarted)
        static GameStaticVar<UMaterialInterface> baseMaterial = new GameStaticVar<UMaterialInterface>();
        static GameStaticVar<UMaterialInterface> blueMaterial = new GameStaticVar<UMaterialInterface>();
        static GameStaticVar<UMaterialInterface> orangeMaterial = new GameStaticVar<UMaterialInterface>();

        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);

            if (baseMaterial.Value == null)
            {
                baseMaterial.Value = ConstructorHelpers.FObjectFinder<UMaterialInterface>.Find("Material'/Game/Puzzle/Meshes/BaseMaterial.BaseMaterial'");
            }
            if (blueMaterial.Value == null)
            {
                blueMaterial.Value = ConstructorHelpers.FObjectFinder<UMaterialInterface>.Find("MaterialInstanceConstant'/Game/Puzzle/Meshes/BlueMaterial.BlueMaterial'");
            }
            if (orangeMaterial.Value == null)
            {
                orangeMaterial.Value = ConstructorHelpers.FObjectFinder<UMaterialInterface>.Find("MaterialInstanceConstant'/Game/Puzzle/Meshes/OrangeMaterial.OrangeMaterial'");
            }

            UStaticMesh mesh = ConstructorHelpers.FObjectFinder<UStaticMesh>.Find("StaticMesh'/Game/Puzzle/Meshes/PuzzleCube.PuzzleCube'");
            if (mesh != null)
            {
                BlockMesh = initializer.CreateDefaultSubobject<UStaticMeshComponent>(this, new FName("BlockMesh"));
                BlockMesh.SetStaticMesh(mesh);
                BlockMesh.RelativeLocation = new FVector(0, 0, 25);
                BlockMesh.RelativeScale3D = new FVector(1, 1, 0.25f);
                RootComponent = BlockMesh;

                if (blueMaterial.Value != null)
                {
                    BlockMesh.SetMaterial(0, blueMaterial.Value);
                }
            }
        }

        protected override void ReceiveActorOnClicked_Implementation(FKey ButtonPressed)
        {
            base.ReceiveActorOnClicked_Implementation(ButtonPressed);
            HandleControllerClick();
        }

        protected override void ReceiveActorOnInputTouchBegin_Implementation(ETouchIndex FingerIndex)
        {
            base.ReceiveActorOnInputTouchBegin_Implementation(FingerIndex);
            HandleControllerClick();
        }

        // Inactive blocks turn white when hovered by mouse and back to blue if the mouse isn't hovering.
        protected override void ReceiveActorBeginCursorOver_Implementation()
        {
            base.ReceiveActorBeginCursorOver_Implementation();
            DoHighlight(true);
        }

        protected override void ReceiveActorEndCursorOver_Implementation()
        {
            base.ReceiveActorEndCursorOver_Implementation();
            DoHighlight(false);
        }

        /// <summary>
        /// Handle block being clicked
        /// </summary>
        public void HandleControllerClick()
        {
            if (!isActive)
            {
                isActive = true;
                BlockMesh.SetMaterial(0, orangeMaterial.Value);
                if (OwningGrid != null)
                {
                    OwningGrid.AddScore();
                }
            }
        }

        public void DoHighlight(bool on)
        {
            if (!isActive && isHighlighted != on)
            {
                isHighlighted = on;
                BlockMesh.SetMaterial(0, isHighlighted ? baseMaterial.Value : blueMaterial.Value);
            }
        }
    }
}
