using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngineTestHarness
{
    class TestBase { }
    class TestChild : TestBase { }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var engine = new RMUD.RuleEngine(RMUD.NewRuleQueueingMode.ImmediatelyAddNewRules);

                engine.DeclarePerformRuleBook<TestBase>("test", "");

                engine.Perform<TestBase>("test")
                    .Do((thing) => { Console.WriteLine("Base"); return RMUD.PerformResult.Continue; });

                engine.Perform<TestChild>("test")
                    .Do((thing) => { Console.WriteLine("Child"); return RMUD.PerformResult.Continue; });

                Console.WriteLine("Output when given a child:");
                engine.ConsiderPerformRule("test", new TestChild());

                Console.WriteLine("Output when given a base:");
                engine.ConsiderPerformRule("test", new TestBase());

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            Console.ReadKey();
        }
    }
}
