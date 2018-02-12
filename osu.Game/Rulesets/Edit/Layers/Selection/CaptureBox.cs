﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using OpenTK;

namespace osu.Game.Rulesets.Edit.Layers.Selection
{
    /// <summary>
    /// A box which encapsulates captured <see cref="DrawableHitObject"/>s.
    /// </summary>
    public class CaptureBox : VisibilityContainer
    {
        private readonly IDrawable captureArea;
        private readonly IReadOnlyList<DrawableHitObject> capturedObjects;

        private readonly Container borderContainer;

        public CaptureBox(IDrawable captureArea, IReadOnlyList<DrawableHitObject> capturedObjects)
        {
            this.captureArea = captureArea;
            this.capturedObjects = capturedObjects;

            Origin = Anchor.Centre;

            InternalChild = borderContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                BorderThickness = 3,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true,
                    Alpha = 0
                }
            };

            State = Visibility.Visible;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            borderContainer.BorderColour = colours.Yellow;

            // Move the rectangle to cover the hitobjects
            var topLeft = new Vector2(float.MaxValue, float.MaxValue);
            var bottomRight = new Vector2(float.MinValue, float.MinValue);

            foreach (var obj in capturedObjects)
            {
                topLeft = Vector2.ComponentMin(topLeft, captureArea.ToLocalSpace(obj.SelectionQuad.TopLeft));
                bottomRight = Vector2.ComponentMax(bottomRight, captureArea.ToLocalSpace(obj.SelectionQuad.BottomRight));
            }

            topLeft -= new Vector2(5);
            bottomRight += new Vector2(5);

            Size = bottomRight - topLeft;
            Position = topLeft + Size / 2f;
        }

        protected override void PopIn() => this.ScaleTo(1.1f)
                                               .Then()
                                               .ScaleTo(1f, 300, Easing.OutQuint).FadeIn(300, Easing.OutQuint);

        protected override void PopOut() => this.FadeOut(300, Easing.OutQuint);
    }
}
