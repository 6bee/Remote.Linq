using System;
using System.Collections.Generic;
using System.Linq;
using Remote.Linq.TestClassLibrary;

namespace Remote.Linq.TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // translate expressions forth and back and execute
            var valueList = new[] { new Obj<int> { Value = 13 }, new Obj<int> { Value = 14 }, new Obj<int> { Value = 87 }, new Obj<int> { Value = 98 }, new Obj<int> { Value = 99 } };
            var sortedValueList = valueList.Sort(i => i.Value % 2);

            //var idList = new[] { 87, 98, 99 };
            //var filteredLIdist = idList.Filter(x => x % 2 == 0);
            //var match = idList.Filter(x => idList.Contains(x));

            var filteredValueList = valueList.Filter(i => i.Value % 2 != 0);

            var idList = new[] { 87, 98, 99 };
            var filteredLIdist = idList.Filter(x => x % 2 == 0);
            var match = idList.Filter(x => idList.Contains(x));

            var q =
                from item in valueList
                where item.Value % 2 == 0
                orderby item.Value
                select item.Value;


            // transpalte and print expressions
            var nameList = new[] { "Peter", "Sue", "Marc" };

            var obj = new BusinessObject
            {
                Id = 99,
                Name = "BO 99",
                CreatedAt = DateTime.Now,
                OtherObj = new OtherObj
                {
                    Id = 2,
                    NameX = "Other name"
                }
            };

            var id = 99;
            var name = "sali";

            var n = 0;

            Remote.Linq.Expressions.Expression expresison;
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => i.Flag);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => (int)i.Parent.Parent.Id == 77);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => (int)i["Id"] == 77);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => (int)i.Parent.Parent.Parent.Parent["Id"] == 77);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => i.OtherObj.Id == id);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => n == i.Id);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => true != i.Flag);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => i.Flag == false);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => false != i.Flag);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => i.Name == null);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => null != i.Name);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => !i.Flag);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => (long)i.Id == 77L);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => i.Id == id && i.Name.StartsWith(name));
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => idList.Contains(i.Id));
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => nameList.Contains(i.Name));
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => ((string)i.OtherObj["NameX"]).Contains("textstr") == false);
            expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => "textstr" == i["Name"] == true);
            //expresison = QueryExtensions.TranslateAndPrint<BusinessObject>(i => i[98] == "textstr");

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
