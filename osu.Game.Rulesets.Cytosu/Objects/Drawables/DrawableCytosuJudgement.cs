﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Cytosu.Objects.Drawables
{
    public class DrawableCytosuJudgement : DrawableJudgement
    {
        [Resolved]
        private OsuColour colours { get; set; }

        private OsuSpriteText judgementText;

        public DrawableCytosuJudgement(JudgementResult result, DrawableCytosuHitObject judgedObject)
            : base(result, judgedObject)
        {
        }

        public DrawableCytosuJudgement()
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Child = judgementText = new OsuSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = Result.Type.GetDescription().ToUpperInvariant(),
                    Font = OsuFont.Numeric.With(size: 20),
                    Colour = colours.ForHitResult(Result.Type),
                    Scale = new Vector2(0.7f),
                }
            };

            switch (Result.Type)
            {
                case HitResult.Meh:
                    judgementText.Text = "Bad";
                    break;

                case HitResult.Good:
                    judgementText.Text = "Good";
                    break;

                case HitResult.Great:
                    judgementText.Text = "Perfect";
                    break;
            }
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();
            if (JudgedObject?.HitObject is CytosuHitObject hitObject)
            {
                Position = hitObject.Position;
                Scale = new Vector2(hitObject.Scale);
            }
        }

        protected override void ApplyHitAnimations() => animateJudgements();
        protected override void ApplyMissAnimations() => animateJudgements();

        private void animateJudgements()
        {
            this.FadeOut(500).Expire(true);
        }
    }
}
