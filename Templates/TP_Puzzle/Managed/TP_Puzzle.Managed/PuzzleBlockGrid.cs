using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;

namespace TP_Puzzle
{
    [UClass]
    class APuzzleBlockGrid : AActor
    {
        [UProperty, EditAnywhere, Tooltip("Used to display score")]
        public UTextRenderComponent ScoreText { get; set; }

        [UProperty, EditAnywhere, Tooltip("Number of blocks along each side of grid")]
        public int Size { get; set; }

        [UProperty, EditAnywhere, Tooltip("Spacing of blocks")]
        public float BlockSpacing { get; set; }

        [UProperty, Tooltip("How many blocks have been clicked")]
        private int Score { get; set; }

        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);

            RootComponent = initializer.CreateDefaultSubobject<USceneComponent>(this, new FName("DummyRoot"));

            ScoreText = initializer.CreateDefaultSubobject<UTextRenderComponent>(this, new FName("ScoreText"));
            ScoreText.RelativeLocation = new FVector(320, 60, 0);
            ScoreText.RelativeRotation = new FRotator(90, 0, 0);
            ScoreText.RelativeScale3D = new FVector(4, 4, 4);
            ScoreText.AttachToComponent(RootComponent, FName.None, EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, false);

            // Care needs to be taken when dealing with FText. In C++ FText lifetime is automatically managed via shared pointers.
            // Any FText which has a lifetime maintained by C# (FText.OwnsReference==true) needs to be destroyed manually. This can be
            // done via by calling 'Dispose()', or in a 'using' statement. If neither is used, the finalizer will do the clean up when
            // the C# GC runs.
            using (FText text = FText.FromString("Score: 0"))
            {
                ScoreText.SetText(text);

                // - text needs to be disposed as the lifetime is maintained by C#
                // - ScoreText.Text doesn't need to be disposed as the lifetime is maintained by native code
                Debug.Assert(text.OwnsReference && !ScoreText.Text.OwnsReference);
            }
        }

        protected override void BeginPlay()
        {
            base.BeginPlay();

            // Spawn blocks
            for (int i = 0; i < Size * Size; i++)
            {
                float x = (i % Size) * BlockSpacing;
                float y = (i / Size) * BlockSpacing;

                FVector location = GetActorLocation() + new FVector(x, y, 0);
                APuzzleBlock block = World.SpawnActor<APuzzleBlock>(location, default(FRotator));
                block.OwningGrid = this;
            }
        }

        /// <summary>
        /// Handle scoring when block is clicked
        /// </summary>
        public void AddScore()
        {
            Score++;
            using (FText text = FText.FromString("Score: " + Score))
            {
                // NOTE:
                // ScoreText.SetText() calls UActorComponent::MarkRenderStateDirty() which updates the actual render of the text.
                // If ScoreText.Text=text; was used the text may not visually change
                ScoreText.SetText(text);
            }
        }
    }
}
