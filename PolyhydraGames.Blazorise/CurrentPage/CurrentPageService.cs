﻿using System.Reactive.Linq;
using System.Reactive.Subjects;
using PolyhydraGames.Blazor.Data.CurrentPage;

namespace PolyhydraGames.BlazorComponents.CurrentPage;

public class CurrentPageService : ICurrentPage
{

    public CurrentPageService()
    {
        NameChanged = _name.AsObservable();
    }

    private Subject<string> _name = new();

    public void SetName(string name)
    {
        _name.OnNext(name);
    }

    public IObservable<string> NameChanged { get; }


}