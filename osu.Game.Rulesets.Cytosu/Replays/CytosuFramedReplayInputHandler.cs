﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Framework.Utils;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.Cytosu.Replays
{
    public class CytosuFramedReplayInputHandler : FramedReplayInputHandler<CytosuReplayFrame>
    {
        public CytosuFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(CytosuReplayFrame frame) => frame.Actions.Any();

        protected Vector2 Position
        {
            get
            {
                var frame = CurrentFrame;

                if (frame == null)
                    return Vector2.Zero;

                Debug.Assert(CurrentTime != null);

                return Interpolation.ValueAt(CurrentTime.Value, frame.Position, NextFrame.Position, frame.Time, NextFrame.Time);
            }
        }

        public override List<IInput> GetPendingInputs()
        {
            return new List<IInput>
            {
                new MousePositionAbsoluteInput
                {
                    Position = GamefieldToScreenSpace(Position),
                },
                new ReplayState<CytosuAction>
                {
                    PressedActions = CurrentFrame?.Actions ?? new List<CytosuAction>(),
                }
            };
        }
    }
}
