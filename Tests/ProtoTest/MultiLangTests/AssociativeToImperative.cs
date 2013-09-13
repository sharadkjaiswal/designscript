using System;
using NUnit.Framework;
using ProtoCore.DSASM.Mirror;
using ProtoCore.Lang;
namespace ProtoTest.MultiLangTests
{
    [TestFixture]
    public class AssociativeToImperative
    {
        public ProtoCore.Core core;
        [SetUp]
        public void Setup()
        {
            core = new ProtoCore.Core(new ProtoCore.Options());
            core.Executives.Add(ProtoCore.Language.kAssociative, new ProtoAssociative.Executive(core));
            core.Executives.Add(ProtoCore.Language.kImperative, new ProtoImperative.Executive(core));
        }

        [Test]
        public void EmbeddedTest001()
        {
            String code =
@"x;[Associative]{	x = 0;    [Imperative]    {        x = x + 5;    }}";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("x");
            Assert.IsTrue((Int64)o.Payload == 5);
        }

        [Test]
        public void EmbeddedTest002()
        {
            String code =
@"x;[Associative]{	x =     {         0 => x@first;    }    [Imperative]    {        x = x + 5;    }}";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("x");
            Assert.IsTrue((Int64)o.Payload == 5);
        }

        [Test]
        public void EmbeddedTest003()
        {
            String code =
@"x;[Associative]{	x =     {        0 => x@first;        +1 => x@second;    }    [Imperative]    {        x = x + 5;    }}";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("x");
            Assert.IsTrue((Int64)o.Payload == 6);
        }

        [Test]
        [Category("Modifier Block")]
        public void EmbeddedTest004()
        {

            Assert.Fail("This code should fail as x@first should be read only, however it doesn't");
            String code =
@"x;[Associative]{	x = {        0 => x@first;        +1 => x@second;}    [Imperative]    {        x@first = x + 5;    }}";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("x");
            Assert.IsTrue((Int64)o.Payload == 7); //0 => x@first, 1 x@second, 6 -> x@first 7 -> x@second
        }

        [Test]
        [Category("Modifier Block")]
        public void EmbeddedTest005()
        {
            Assert.Fail("This code should fail as x@second should be read only, however it doesn't");
            String code =
@"x;[Associative]{	x = {        0 => x@first;        +1 => x@second;}    [Imperative]    {        x@second = x + 5;    }}";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("x");
            Assert.IsTrue((Int64)o.Payload == 6);
        }

        [Test]
        public void EmbeddedTest006()
        {
            String code =
                @"c = 0;x = c > 5 ? 1 : 2;[Imperative]{    c = 10;            }                ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("x").Payload == 1);
        }

        [Test]
        public void ImperativeUpdate001()
        {
            String code =
@"a = 1;b;[Imperative]{    b = a;}a = 10;";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("b");
            Assert.IsTrue((Int64)o.Payload == 10);
        }

        [Test]
        public void ImperativeUpdate002()
        {
            String code =
@"class C{    x : int = 1;}p = C.C();a;[Imperative]{    a = p.x;}p.x = 10;";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Obj o = mirror.GetValue("a");
            Assert.IsTrue((Int64)o.Payload == 10);
        }

        [Test]
        public void LanguageBlockReturn01()
        {
            String code =
@"a;[Associative]{    a = 4;    b = a*2;	    x = [Imperative]    {        i=0;		        return = i;     }    a = x;    temp = 5;}    ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("a").Payload == 0);
        }

        [Test]
        public void LanguageBlockReturn02()
        {
            String code =
@"a;[Associative]{    def DoSomthing : int(p : int)    {        ret = p;               d = [Imperative]        {            local = 20;            return = local;        }        return = ret * 100 + d;    }    a = DoSomthing(10);   }    ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("a").Payload == 1020);
        }

        [Test]
        public void NestedBlockInFunction01()
        {
            String code =
@"def f : int (p : int){    loc = 32;	s = [Imperative]	{        n = loc + p;        return = n;	}	return = s;}a = f(16);	    ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("a").Payload == 48);
        }

        [Test]
        public void NestedBlockInFunction02()
        {
            String code =
@"def clampRange : int(i : int, rangeMin : int, rangeMax : int){    result = [Imperative]    {	    clampedValue = i;	    if(i < rangeMin) 	    {		    clampedValue = rangeMin;	    }	    elseif( i > rangeMax ) 	    {		    clampedValue = rangeMax; 	    }         return = clampedValue;    }	return = result;}a = clampRange(101, 10, 100);    ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("a").Payload == 100);
        }

        [Test]
        public void NestedBlockInFunction03()
        {
            String code =
@"def foo  (){        t = [Imperative]    {          t1 = [Associative]         {                                     t2 = 6;                    return = t2;          }             return = t1;                     }    return = t;    }p = foo();     ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("p").Payload == 6);
        }

        [Test]
        public void NestedBlockInFunction04()
        {
            String code =
@"def foo  (){        t = [Associative]    {          t1 = [Imperative]         {                                     t2 = 6;                    return = t2;          }             return = t1;                     }    return = t;    }p = foo();    ";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("p").Payload == 6);
        }

        [Test]
        public void AccessGlobalVariableInsideFunction()
        {
            string code = @"                                arr = { 1, 2, 3 };                                factor = 10;                                def foo : int[]()                                {	                                f = factor;	                                [Imperative]	                                {		                                ix = 0;		                                for(i in arr)		                                {			                                arr[ix] = arr[ix] * factor * f;			                                ix = ix + 1;		                                }	                                }	                                return = arr;                                }                                w = foo();                                w0 = w[0];                                w1 = w[1];                                w2 = w[2];";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("w0").Payload == 100);
            Assert.IsTrue((Int64)mirror.GetValue("w1").Payload == 200);
            Assert.IsTrue((Int64)mirror.GetValue("w2").Payload == 300);
        }


        [Test]
        public void Nestedblocks_Inside_1467358()
        {
            string code = @"                             def foo()                                {                                    t = [Imperative]                                    {                                        t1 = [Associative]                                        {                                            t2 = 6;                                            return = t2;                                        }                                        return = t1;                                    }                                    return = t;                                }                                def foo2()                                {                                    t = [Associative]                                    {                                        t1 = [Imperative]                                        {                                            t2 = 6;                                            return = t2;                                        }                                        return = t1;                                    }                                    return = t;                                }                                p1 = foo(); // expected 6, got null                                p2 = foo2(); // expected 6, got 6";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("p1").Payload == 6);
            Assert.IsTrue((Int64)mirror.GetValue("p2").Payload == 6);

        }

        [Test]
        public void Nestedblocks_Inside_1467358_2()
        {
            string code = @"                             def foo(){    t = [Imperative]    {        t1 = [Associative]        {            t2 = 6;            return = t2;        }        return = t1;    }    return = t;}def foo2(){    t = [Associative]    {        t1 = [Imperative]        {                        t2 = [Associative]            {             t3   = 6;                return = t3;            }            return = t2;        }        return = t1;    }    return = t;}p1 = foo(); // expected 6, got nullp2 = foo2(); // expected 6, got 6";
            ProtoScript.Runners.ProtoScriptTestRunner fsr = new ProtoScript.Runners.ProtoScriptTestRunner();
            ExecutionMirror mirror = fsr.Execute(code, core);
            Assert.IsTrue((Int64)mirror.GetValue("p1").Payload == 6);
            Assert.IsTrue((Int64)mirror.GetValue("p2").Payload == 6);
        }
    }
}
