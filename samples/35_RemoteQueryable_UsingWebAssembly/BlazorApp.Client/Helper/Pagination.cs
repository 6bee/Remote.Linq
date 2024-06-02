// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Client.Helper;

using System.ComponentModel;

public sealed class Pagination : INotifyPropertyChanged
{
    private readonly Func<Task> _callback;

    private int _page = 1;
    private int _numberOfPages;
    private int _numberOrRecords;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Pagination(int pageSize, Func<Task> callback)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must not be smaller than one.");
        }

        PageSize = pageSize;
        _callback = callback;
    }

    public bool CanMoveBack => Page > 1;

    public bool CanMoveForward => Page < NumberOfPages;

    public int PageSize { get; }

    public int Page
    {
        get => _page;
        private set
        {
            _page = value;
            RaisePropertyChanged(nameof(Page));
        }
    }

    public int NumberOfPages
    {
        get => _numberOfPages;
        private set
        {
            _numberOfPages = value;
            RaisePropertyChanged(nameof(NumberOfPages));
        }
    }

    public int NumberOrRecords
    {
        get => _numberOrRecords;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(NumberOrRecords), "Number of records must not be smaller than zero.");
            }

            _numberOrRecords = value;
            RaisePropertyChanged(nameof(NumberOrRecords));

            NumberOfPages = Math.Max(1, (int)Math.Ceiling((double)value / PageSize));
        }
    }

    public Task First()
        => SetPage(1);

    public Task Next()
        => SetPage(Page + 1);

    public Task Previous()
        => SetPage(Page - 1);

    public Task Last()
        => SetPage(NumberOfPages);

    public void Reset()
        => Page = 1;

    public Task SetPage(int page)
    {
        Page = page;
        return _callback();
    }

    public IQueryable<TSource> Apply<TSource>(IQueryable<TSource> source)
        => source
        .Skip((Page - 1) * PageSize)
        .Take(PageSize);

    private void RaisePropertyChanged(string name)
    {
        if (PropertyChanged is PropertyChangedEventHandler handler)
        {
            handler(this, new(name));

            switch (name)
            {
                case nameof(NumberOfPages):
                    handler(this, new(nameof(CanMoveForward)));
                    break;

                case nameof(Page):
                    handler(this, new(nameof(CanMoveBack)));
                    handler(this, new(nameof(CanMoveForward)));
                    break;
            }
        }
    }
}