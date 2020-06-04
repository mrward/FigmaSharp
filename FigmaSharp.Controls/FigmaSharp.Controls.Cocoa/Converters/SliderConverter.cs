﻿// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa
{
	public abstract class SliderConverter : CocoaConverter
    {
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSSlider);

		public override bool CanSetAccessibilityLabel => false;

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var slider = new NSSlider();

			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlType(out var controlType);
			frame.TryGetNativeControlVariant(out var controlVariant);

			slider.ControlSize = CocoaHelpers.GetNSControlSize(controlVariant);

			slider.MinValue = 0;
			slider.MaxValue = 1;

			if (controlType == FigmaControlType.SliderLinear)
				slider.DoubleValue = 0.618;

			FigmaGroup group = frame.children
				.OfType<FigmaGroup>()
				.FirstOrDefault(s => s.visible);

			if (group?.name == ComponentString.STATE_DISABLED)
				slider.Enabled = false;

			return new View(slider);
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var code = new StringBuilder();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			var frame = (FigmaFrame)currentNode.Node;
			currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
			currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			code.WriteEquality(name, nameof(NSButton.ControlSize), CocoaHelpers.GetNSControlSize(controlVariant));

			code.WriteEquality(name, nameof(NSProgressIndicator.MinValue), "0");
			code.WriteEquality(name, nameof(NSProgressIndicator.MaxValue), "1");

            if (controlType == FigmaControlType.SliderLinear)
				code.WriteEquality(name, nameof(NSProgressIndicator.DoubleValue), "0.618");

			FigmaGroup group = frame.children
				.OfType<FigmaGroup>()
				.FirstOrDefault(s => s.visible);

			if (group?.name == ComponentString.STATE_DISABLED)
				code.WriteEquality(name, nameof(NSSlider.Enabled), false);

			return code;
		}
	}


	public class SliderLinearConverter : SliderConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == FigmaControlType.SliderLinear;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			IView view = base.OnConvertToView(currentNode, parentNode, rendererService);

			var slider = (NSSlider) view.NativeObject;
			slider.SliderType = NSSliderType.Linear;

			var frame = (FigmaFrame) currentNode;

            if (frame.absoluteBoundingBox.Height > frame.absoluteBoundingBox.Width)
				slider.IsVertical = 1;

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			StringBuilder code = base.OnConvertToCode(currentNode, parentNode, rendererService);
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			var frame = (FigmaFrame)currentNode.Node;

			code.WriteEquality(name, nameof(NSSlider.SliderType), NSSliderType.Linear);

			if (frame.absoluteBoundingBox.Height > frame.absoluteBoundingBox.Width)
				code.WriteEquality(name, nameof(NSSlider.IsVertical), "1", inQuotes: false);

			return code;
		}
	}


	public class SliderCircularConverter : SliderConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == FigmaControlType.SliderCircular;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			IView view = base.OnConvertToView(currentNode, parentNode, rendererService);
			(view.NativeObject as NSSlider).SliderType = NSSliderType.Circular;

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			StringBuilder code = base.OnConvertToCode(currentNode, parentNode, rendererService);
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			code.WriteEquality(name, nameof(NSSlider.SliderType), NSSliderType.Circular);

			return code;
		}
	}
}
