﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Game.Rulesets.Cytosu.Objects.Drawables;
using osu.Game.Rulesets.Cytosu.Scoring;
using osu.Game.Rulesets.Cytosu.UI.HUD;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Cytosu.UI
{
    [Cached]
    public class CytosuPlayfield : Playfield
    {
        private readonly JudgementContainer<DrawableCytosuJudgement> judgementLayer;

        private readonly IDictionary<HitResult, DrawablePool<DrawableCytosuJudgement>> poolDictionary = new Dictionary<HitResult, DrawablePool<DrawableCytosuJudgement>>();

        public CytosuPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                judgementLayer = new JudgementContainer<DrawableCytosuJudgement>
                {
                    RelativeSizeAxes = Axes.Both,
                },
                HitObjectContainer,
                new HUD.PlayfieldBorder
                {
                    RelativeSizeAxes = Axes.Both
                },
                new ScanLine
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Width = 5000f,
                    LineColour = Color4.White,
                }
            };

            var hitWindows = new CytosuHitWindows();

            foreach (var result in Enum.GetValues(typeof(HitResult)).OfType<HitResult>().Where(r => r > HitResult.None && hitWindows.IsHitResultAllowed(r)))
                poolDictionary.Add(result, new DrawableJudgementPool(result));

            AddRangeInternal(poolDictionary.Values);
        }

        protected override GameplayCursorContainer CreateCursor() => new CytosuCursorContainer();

        public override void Add(DrawableHitObject h)
        {
            h.OnNewResult += onNewResult;
            h.OnLoadComplete += d =>
            {
            };

            base.Add(h);
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            DrawableCytosuJudgement explosion = poolDictionary[result.Type].Get(doj => doj.Apply(result, judgedObject));

            judgementLayer.Add(explosion);
        }

        private class DrawableJudgementPool : DrawablePool<DrawableCytosuJudgement>
        {
            private readonly HitResult result;

            public DrawableJudgementPool(HitResult result)
                : base(10)
            {
                this.result = result;
            }

            protected override DrawableCytosuJudgement CreateNewDrawable()
            {
                var judgement = base.CreateNewDrawable();
                judgement.Apply(new JudgementResult(new HitObject(), new Judgement()) { Type = result }, null);

                return judgement;
            }
        }
    }
}
