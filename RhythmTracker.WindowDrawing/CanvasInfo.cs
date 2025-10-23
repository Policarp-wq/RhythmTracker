using System;
using Gtk;

namespace RhythmTracker.WindowDrawing;

public record CanvasInfo(DrawingArea DrawingArea, Cairo.Context Context, int Width, int Height) { }
