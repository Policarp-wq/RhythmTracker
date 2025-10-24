using System;
using System.Collections;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing.Drawing;

public class Scene<T> : IEnumerable<T>, IRenderable
    where T : IRenderable
{
    private Dictionary<int, T> _renderables = [];
    private int _id = 1;

    public bool IsVisible { get; set; } = true;

    public IEnumerator<T> GetEnumerator() => _renderables.Values.GetEnumerator();

    public IEnumerable<KeyValuePair<int, T>> Entries => _renderables;

    public int Add(T renderable)
    {
        int key = _id++;
        _renderables.Add(key, renderable);
        return key;
    }

    public void Remove(int key)
    {
        _renderables.Remove(key);
    }

    public void Remove(T renderable)
    {
        var entry = _renderables.SingleOrDefault(entry => entry.Value.Equals(renderable));
        if (entry.Equals(default(KeyValuePair<int, T>)))
            return;
        Remove(entry.Key);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Render(CanvasInfo info)
    {
        if (!IsVisible)
            return;
        foreach (var el in _renderables.Values)
        {
            el.Render(info);
        }
    }
}

public class Scene : Scene<IRenderable> { }
