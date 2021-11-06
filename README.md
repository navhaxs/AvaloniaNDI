# AvaloniaNDI

Output NDI® from your Avalonia application!

A crude port of the WPF sample code from the NewTek NDI® 5 SDK.

This library provides a `NDISendContainer` Avalonia component. In the sample, I have wrapped the `NDISendContainer` itself within a ViewBox, which seems to ensure that the rendered output frames remain at the expected size and resolution.

`NDILibDotNet2` is based on the NDI SDK C# sample code, with the WPF dependencies removed so to target .NET 5

## Project Status: Experimental

I have not done much testing. This code is likely wildly inefficient. I have not tested audio yet.

But it does work well enough to send out frames correctly, with alpha channel - pretty cool!!

From a cursory glance, performance is quite acceptable... it can only improve from here, right? :) :) :)

![Screenshot of animated progress bar and transparent background](screenshot.png?raw=true "Screenshot of animated progress bar and transparent background")

NDI® (Network Device Interface) is a standard developed by NewTek, Inc that enables applications to deliver video streams via a local area network. Please refer to ndi.tv for further information about the technology.
