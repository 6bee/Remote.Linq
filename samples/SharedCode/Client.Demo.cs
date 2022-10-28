// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using System;
    using System.Linq;

    public class Demo : IDemo
    {
        private readonly Func<IRemoteRepository> _repoProvider;

        public Demo(Func<IRemoteRepository> repoProvider)
            => _repoProvider = repoProvider;

        public void Run()
        {
            using IRemoteRepository repo = _repoProvider();

            var q = repo.ProductGroups.Where(x => x.Products.Any(y => x.Id == y.RelatedPosition.Id)).ToList();
        }
    }
}