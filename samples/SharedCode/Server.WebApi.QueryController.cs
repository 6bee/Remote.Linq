// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.Model;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    public class QueryController : ApiController
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public IEnumerable<DynamicObject> Query([FromBody] Query query)
        {
            try
            {
                return query.Expression.Execute(DataStore.QueryableByTypeProvider);
            }
            catch (Exception ex)
            {
                string errorMessage = $"{ex.GetType()}: {ex.Message}";
                byte[] errorMessageData = Encoding.UTF8.GetBytes(errorMessage);

                var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ByteArrayContent(errorMessageData),
                };

                throw new HttpResponseException(httpResponse);
            }
        }
    }
}
