using System;
using NUnit.Framework;
using ProtoCore.DSASM.Mirror;
using ProtoFFI;
using ProtoTestFx.TD;
namespace ProtoTest.TD
{
    class DefectRegress
    {
        public ProtoCore.Core core;
        public TestFrameWork thisTest = new TestFrameWork();
        string testCasePath = "..\\..\\..\\Tests\\ProtoTest\\ImportFiles\\";
        [SetUp]
        public void Setup()
        {
            core = new ProtoCore.Core(new ProtoCore.Options());
            core.Executives.Add(ProtoCore.Language.kAssociative, new ProtoAssociative.Executive(core));
            core.Executives.Add(ProtoCore.Language.kImperative, new ProtoImperative.Executive(core));
            DLLFFIHandler.Register(FFILanguage.CSharp, new CSModuleHelper());
            DLLFFIHandler.Register(FFILanguage.CPlusPlus, new PInvokeModuleHelper());
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455643()
        {
            string code = @"class Tuple4{    X : var;    Y : var;    Z : var;    H : var;            constructor ByCoordinates4 (coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = coordinates[3];        }        public def Equals : bool (other : Tuple4)    {        return =   X == other.X &&                   Y == other.Y &&                   Z == other.Z &&                   H == other.H;    }}class Transform{    public C0 : Tuple4;     public C1 : Tuple4;     public C2 : Tuple4;     public C3 : Tuple4;         public constructor ByData(data : double[][])    {        C0 = Tuple4.ByCoordinates4(data[0]);        C1 = Tuple4.ByCoordinates4(data[1]);        C2 = Tuple4.ByCoordinates4(data[2]);        C3 = Tuple4.ByCoordinates4(data[3]);    }      def Equals : bool (other : Transform )   {        return =C0.Equals(other.C0) &&                C1.Equals(other.C1) &&                C2.Equals(other.C2) &&                C3.Equals(other.C3);   }}data1 = {    {1.0,0,0,0},            {0.0,1,0,0},            {0.0,0,1,0},            {0.0,0,0,1}        };        data2 = {    {1.0,0,0,0},    {1.0,1,0,0},    {0.0,0,1,0},    {0.0,0,0,1}};                xform1 = Transform.ByData(data1);xform2 = Transform.ByData(data2);areEqual1 = xform1.Equals(xform1);areEqual2 = xform1.Equals(xform2);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("areEqual1", true);
            thisTest.Verify("areEqual2", false);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455621()
        {
            string code = @"class Tuple4{    X : var;    Y : var;    Z : var;    H : var;        constructor XYZH(xValue : double, yValue : double, zValue : double, hValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = hValue;            }        constructor XYZ(xValue : double, yValue : double, zValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = 1.0;            }    constructor ByCoordinates3(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = 1.0;            }    constructor ByCoordinates4(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = coordinates[3];        }        def get_X : double ()     {        return = X;    }        def get_Y : double ()     {        return = Y;    }        def get_Z : double ()     {        return = Z;    }        def get_H : double ()     {        return = H;    }        public def Multiply : double (other : Tuple4)    {        return = X * other.X + Y * other.Y + Z * other.Z + H * other.H;    }        public def Coordinates3 : double[] ()    {         return = {X, Y, Z };    }        public def Coordinates4 : double[] ()     {         return = {X, Y, Z, H };    }}/*t1 = Tuple4.XYZH(0,0,0,0);t2 = Tuple4.XYZ(0,0,0);t3 = Tuple4.ByCoordinates3({0.0,0,0});t4 = Tuple4.ByCoordinates4({0.0,0,0,0});mult = t1.Multiply(t2);c3 = t3.Coordinates3();c4 = t3.Coordinates4();*/class Vector{    X : var;    Y : var;    Z : var;        public constructor ByCoordinates(xx : double, yy : double, zz : double)    {        X = xx;        Y = yy;        Z = zz;    }}class Transform{    public C0 : Tuple4;     public C1 : Tuple4;     public C2 : Tuple4;     public C3 : Tuple4;             public constructor ByTuples(C0Value : Tuple4, C1Value : Tuple4, C2Value : Tuple4, C3Value : Tuple4)    {        C0 = C0Value;        C1 = C1Value;        C2 = C2Value;        C3 = C3Value;    }    public constructor ByData(data : double[][])    {        C0 = Tuple4.ByCoordinates4(data[0]);        C1 = Tuple4.ByCoordinates4(data[1]);        C2 = Tuple4.ByCoordinates4(data[2]);        C3 = Tuple4.ByCoordinates4(data[3]);    }        public def ApplyTransform : Tuple4 (t : Tuple4)    {        tx = Tuple4.XYZH(C0.X, C1.X, C2.X, C3.X);        RX = tx.Multiply(t);        ty = Tuple4.XYZH(C0.Y, C1.Y, C2.Y, C3.Y);        RY = ty.Multiply(t);        tz = Tuple4.XYZH(C0.Z, C1.Z, C2.Z, C3.Z);        RZ = tz.Multiply(t);        th = Tuple4.XYZH(C0.H, C1.H, C2.H, C3.H);        RH = th.Multiply(t);                return = Tuple4.XYZH(RX, RY, RZ, RH);    }    public def NativeMultiply : Transform(other : Transform)    {                      tc0 = ApplyTransform(other.C0);        tc1 = ApplyTransform(other.C1);        tc2 = ApplyTransform(other.C2);        tc3 = ApplyTransform(other.C3);        return = Transform.ByTuples(tc0, tc1, tc2, tc3);    }        public def NativePreMultiply : Transform (other : Transform)    {             //  as we don't have this now let's do it longer way!                //return = other.NativeMultiply(this);        //        tc0 = other.ApplyTransform(C0);        tc1 = other.ApplyTransform(C1);        tc2 = other.ApplyTransform(C2);        tc3 = other.ApplyTransform(C3);        return = Transform.ByTuples(tc0, tc1, tc2, tc3);    }}data = {    {1.0,0,0,0},            {0.0,1,0,0},            {0.0,0,1,0},            {0.0,0,0,1}        };        xform = Transform.ByData(data);c0 = xform.C0;c0_X = c0.X;c0_Y = c0.Y;c0_Z = c0.Z;c0_H = c0.H;vec111 = Vector.ByCoordinates(1,1,1);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455158()
        {
            string code = @"	class TestPoint	{        X : var;        Y : var;                     constructor Create(xx : int, yy : int)        {			X = xx;             Y = yy;        }                    def Modify : TestPoint()        {            tempX = X + 1;            tempY = Y + 1;            return = TestPoint.Create(tempX, tempY);        }            	}			pt1 = TestPoint.Create(1, 2);	pt2 = pt1.Modify();	x2 = pt2.X;	y2 = pt2.Y;	pt3 = pt2.Modify();	x3 = pt3.X;	y3 = pt3.Y;		";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x3", 3);
            thisTest.Verify("y3", 4);
        }

        [Test]
        public void Regress_1455618()
        {
            //Assert.Fail("1467188 - Sprint24 : rev 3170: REGRESSION : instantiating a class more than once with same argument is causing DS to go into infinite loop!");
            string code = @"class Tuple4{    X : var;    Y : var;    Z : var;    H : var;        constructor XYZH(xValue : double, yValue : double, zValue : double, hValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = hValue;            }        constructor XYZ(xValue : double, yValue : double, zValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = 1.0;            }    constructor ByCoordinates3(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = 1.0;            }    constructor ByCoordinates4(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = coordinates[3];        }        def get_X : double ()     {        return = X;    }        def get_Y : double ()     {        return = Y;    }        def get_Z : double ()     {        return = Z;    }        def get_H : double ()     {        return = H;    }        public def Multiply : double (other : Tuple4)    {        return = X * other.X + Y * other.Y + Z * other.Z + H * other.H;    }        public def Coordinates3 : double[] ()    {         return = {X, Y, Z };    }        public def Coordinates4 : double[] ()     {         return = {X, Y, Z, H };    }}class Vector{    X : var;    Y : var;    Z : var;        public constructor ByCoordinates(xx : double, yy : double, zz : double)    {        X = xx;        Y = yy;        Z = zz;    }}class Transform{    public C0 : Tuple4;     public C1 : Tuple4;     public C2 : Tuple4;     public C3 : Tuple4;             public constructor ByTuples(C0Value : Tuple4, C1Value : Tuple4, C2Value : Tuple4, C3Value : Tuple4)    {        C0 = C0Value;        C1 = C1Value;        C2 = C2Value;        C3 = C3Value;    }    public constructor ByData(data : double[][])    {        C0 = Tuple4.ByCoordinates4(data[0]);        C1 = Tuple4.ByCoordinates4(data[1]);        C2 = Tuple4.ByCoordinates4(data[2]);        C3 = Tuple4.ByCoordinates4(data[3]);    }        public def ApplyTransform : Tuple4 (t : Tuple4)    {        //a = C0.X;        tx = Tuple4.XYZH(C0.X, C1.X, C2.X, C3.X);        RX = tx.Multiply(t);        ty = Tuple4.XYZH(C0.Y, C1.Y, C2.Y, C3.Y);        RY = ty.Multiply(t);        tz = Tuple4.XYZH(C0.Z, C1.Z, C2.Z, C3.Z);        RZ = tz.Multiply(t);        th = Tuple4.XYZH(C0.H, C1.H, C2.H, C3.H);        RH = th.Multiply(t);        return = Tuple4.XYZH(RX, RY, RZ, RH);    }        public def TransformVector : Vector (p: Vector)    {            tpa = Tuple4.XYZH(p.X, p.Y, p.Z, 0.0);        tpcv = ApplyTransform(tpa);        return = Vector.ByCoordinates(tpcv.X, tpcv.Y, tpcv.Z);        }}data = {    {1.0,0,0,0},            {0.0,1,0,0},            {0.0,0,1,0},            {0.0,0,0,1}        };        xform = Transform.ByData(data);c0 = xform.C0;c0_X = c0.X;c0_Y = c0.Y;c0_Z = c0.Z;c0_H = c0.H;vec111 = Vector.ByCoordinates(1,1,1);tempTuple = Tuple4.XYZH(vec111.X, vec111.Y, vec111.Z, 0.0);tempcv = xform.ApplyTransform(tempTuple);x = tempcv.X;y = tempcv.Y;z = tempcv.Z;h = tempcv.H;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", 1.0);
            thisTest.Verify("y", 1.0);
            thisTest.Verify("z", 1.0);
            thisTest.Verify("h", 0.0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455584()
        {
            string code = @"class Tuple4{    X : var;    Y : var;    Z : var;    H : var;        constructor XYZH(xValue : double, yValue : double, zValue : double, hValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = hValue;            }        constructor XYZ(xValue : double, yValue : double, zValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = 1.0;            }    constructor ByCoordinates3(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = 1.0;            }    constructor ByCoordinates4(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = coordinates[3];        }        def get_X : double ()     {        return = X;    }        def get_Y : double ()     {        return = Y;    }        def get_Z : double ()     {        return = Z;    }        def get_H : double ()     {        return = H;    }        public def Multiply : double(other : Tuple4)    {        return = X * other.X + Y * other.Y + Z * other.Z + H * other.H;    }        public def Coordinates3 : double[] ()    {         return = {X, Y, Z };    }        public def Coordinates4 :double[] ()     {         return = {X, Y, Z, H };    }        }cor1 = {10.0, 10.0, 10.0, 10.0};cor2 = {10.0, 10.0, 10.0, 10.0};tuple1 = Tuple4.ByCoordinates4 (cor1);tuple1 = Tuple4.XYZH (1,1,1,1);tuple2 = Tuple4.ByCoordinates4 (cor2);result1 = tuple1.Coordinates4();result2 = tuple2.Coordinates4();multiply1 = tuple1.Multiply(tuple2);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("multiply1", 40.0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455729()
        {
            string code = @"def cos : double(val : double){    return = val;}def sin : double ( val : double){    return = val;}class Geometry{    private id : var;}class Plane extends Geometry{}class Curve extends Geometry{}class Surface extends Geometry{}class Solid extends Geometry{}class Color{}class CoordinateSystem extends Geometry{}class Vector{}class Tuple4{    X : var;    Y : var;    Z : var;    H : var;        constructor XYZH(xValue : double, yValue : double, zValue : double, hValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = hValue;            }        constructor XYZ(xValue : double, yValue : double, zValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = 1.0;            }    constructor ByCoordinates3(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = 1.0;            }    constructor ByCoordinates4(coordinates : double[] )    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        H = coordinates[3];        }        def get_X : double ()     {        return = X;    }        def get_Y : double ()     {        return = Y;    }        def get_Z : double ()     {        return = Z;    }        def get_H : double ()     {        return = H;    }        public def Multiply : double (other : Tuple4)    {        return = X * other.X + Y * other.Y + Z * other.Z + H * other.H;    }        public def Coordinates3 : double[] ()    {         return = {X, Y, Z };    }        public def Coordinates4 : double[] ()     {         return = {X, Y, Z, H };    }}class Point extends Geometry{    public color                    : var;    //= Color.Yellow;    public XTranslation             : var; // double = 0;    public YTranslation             : var; //double = 0;    public ZTranslation             : var; //double = 0;    public ParentCoordinateSystem   : var; //CoordinateSystem = CoordinateSystem.BaseCoordinateSystem;    public GlobalCoordinates        : var; //= 0.0;    public X                        : var; //double = GlobalCoordinates[0];    public Y                        : var; //double = GlobalCoordinates[1];    public Z                        : var; //double = GlobalCoordinates[2];    public Radius                   : var; //double = 0.0;    public Theta                    : var; //double = 0.0;    public Height                   : var; //double = 0.0;    public Phi                      : var; //double = 0.0;    //  properties due to various AtParameter/Project constructors    public MySurface                  : var;//Surface = null;    public MyCurve                    : var;//Curve = null;    public U                        : var; //double = null;    public V                        : var;//double = null;    public T                        : var;//double = null;    public Distance                 : var;// double = null;    public Direction                : var;//Vector = null;    public MyPlane                    : var; //Plane = null;    private bHostEntityCreated      : var;// = DC_Point_updateHostPoint(hostEntityID, {X, Y, Z});    public def ComputeGlobalCoords : double[] (cs: CoordinateSystem, x : double, y : double, z : double)    {        /*localCoordsTuple = Tuple4.XYZ(x, y, z);        globalCoordsTuple = cs.Matrix.ApplyTransform(localCoordsTuple);        return = {globalCoordsTuple.X, globalCoordsTuple.Y, globalCoordsTuple.Z};*/        return = {x, y, z};    }    private def init : bool ()    {        color                  =  null;        XTranslation           =  0.0;        YTranslation           =  0.0;        ZTranslation           =  0.0;        ParentCoordinateSystem =  null;        GlobalCoordinates      =  null;        X                      =  0.0;        Y                      =  0.0;        Z                      =  0.0;        Radius                 =  0.0;        Theta                  =  0.0;        Height                 =  0.0;        Phi                    =  0.0;                MySurface                 =   null;        MyCurve                   =   null;        U                       = 0.0;        V                       = 0.0;        T                       = 0.0;        Distance                = 0.0;        Direction               = null;        MyPlane                   = null;        bHostEntityCreated      = false;                // id is a private member in base class        // id                      = null;                return = true;    }            public constructor ByCylindricalCoordinates(cs : CoordinateSystem, radius : double, theta : double, height : double)    {        neglect = init();                ParentCoordinateSystem = cs;        Radius = radius;        Theta = theta;        Height = height;        XTranslation = Radius*cos(Theta);        YTranslation = Radius*sin(theta);        ZTranslation = Height;        //GlobalCoordinates = ComputeGlobalCoords(ParentCoordinateSystem, XTranslation, YTranslation, ZTranslation);        X = GlobalCoordinates[0];        Y = GlobalCoordinates[1];        Z = GlobalCoordinates[2];    }}a = 2;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 2);//Compilation test.
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455738()
        {
            string code = @"b;[Associative]{    a = 3;    b = a * 2;    a = 4;}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("b", 8);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455276()
        {
            string code = @"class Point{    x : var;    y : var;    z : var;        public constructor Create(xx : double, yy : double, zz: double)    {        x = xx;        y = yy;        z = zz;    }        public def SquaredDistance : double (otherPt : Point)    {        //distx = (otherPt.x -x) * (otherPt.x -x);        distx = (otherPt.x-x);        distx = distx * distx;           disty = otherPt.y -y;        //disty = disty * disty;                distz = otherPt.z -z;        //distz = distz * distz;                        return = distx + disty + distz;    }}pt1 = Point.Create(0,0,0);pt2 = Point.Create(10.0, 0 ,0 );dist = pt1.SquaredDistance(pt2);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("dist", 100.0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454980()
        {
            string code = @"import (""TestImport.ds"");class Math{                //external (""ffi_library"") def dc_sqrt : double (val : double );                //external (""ffi_library"") def dc_factorial : int (val : int );                           constructor GetInstance()                {}                                def Sqrt : double ( val : double )                {                                return = dc_sqrt(val);                }                def Factorial : int ( val : int )                {                                return = dc_factorial(val); //issue is here. the below line will pass                                //return = dc_factorial(10);                }}                math = Math.GetInstance();                result = math.Factorial(3);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("result", 9);
        }

        [Test]
        public void Regress_1455568()
        {
            //Assert.Fail("1467188 - Sprint24 : rev 3170: REGRESSION : instantiating a class more than once with same argument is causing DS to go into infinite loop!");
            string code = @"class Tuple4{    X : var;    Y : var;    Z : var;    H : var;        constructor XYZH(xValue : double, yValue : double, zValue : double, hValue : double)    {        X = xValue;        Y = yValue;        Z = zValue;        H = hValue;            }        public def Multiply  (other : Tuple4)    {        return = X * other.X + Y * other.Y + Z * other.Z + H * other.H;    }}C0 = Tuple4.XYZH(1.0,0,0,0);C1 = Tuple4.XYZH(0,1.0,0,0);C2 = Tuple4.XYZH(0,0,1.0,0);C3 = Tuple4.XYZH(0,0,0,1.0);t = Tuple4.XYZH(1,1,1,1);tx = Tuple4.XYZH(C0.X, C1.X, C2.X, C3.X);RX = tx.Multiply(t);ty = Tuple4.XYZH(C0.Y, C1.Y, C2.Y, C3.Y);RY = ty.Multiply(t);tz = Tuple4.XYZH(C0.Z, C1.Z, C2.Z, C3.Z);RZ = tz.Multiply(t);th = Tuple4.XYZH(C0.H, C1.H, C2.H, C3.H);RH = th.Multiply(t);       result1 =  Tuple4.XYZH(RX, RY, RZ, RH);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("RX", 1.0);
            thisTest.Verify("RY", 1.0);
            thisTest.Verify("RZ", 1.0);
            thisTest.Verify("RH", 1.0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455291()
        {
            string code = @"class CoordinateSystem{}class Vector{    public GlobalCoordinates : var;    public X : var;    public Y : var;    public Z : var;    public Length : var;    public Normalized : var;    public ParentCoordinateSystem : var;    public XLocal : var;    public YLocal : var;    public ZLocal : var;    public constructor ByCoordinates(x : double, y : double, z : double)    {        X = x;        Y = y;        Z = z;    }        public constructor ByCoordinates(cs: CoordinateSystem, xLocal : double, yLocal : double, zLocal : double )    {        ParentCoordinateSystem = cs;        XLocal = xLocal;        YLocal = yLocal;        ZLocal = zLocal;    }    public constructor ByCoordinateArray(coordinates : double[])    {        X = coordinates[0];        Y = coordinates[1];        Z = coordinates[2];        }    public def Cross : Vector (otherVector : Vector)    {        return = Vector.ByCoordinates(            Y*otherVector.Z - Z*otherVector.Y,            Z*otherVector.X - X*otherVector.Z,            X*otherVector.Y - Y*otherVector.X);    }}    a = 5;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 5);
            //This is more of a test for compilation. 
        }

        [Test]
        public void Regress_1454075()
        {
            string dscode = @"                class Vector{	constructor Vector() {}}v = Vector.Vector();";
            ExecutionMirror mirror = thisTest.RunScriptSource(dscode);
            //Compilation test. 
        }

        [Test]
        public void Regress_1454723()
        {
            string dscode = @"def sqrt : double  (val : double ){    result = 0.0;    result = [Imperative]             {                return = 10.0 * val;             }    return = result;}ten;[Imperative]{    val = 10;    ten = sqrt(val);}";
            ExecutionMirror mirror = thisTest.RunScriptSource(dscode);
            thisTest.Verify("ten", 100.0);
        }

        [Test]
        public void Regress_1457064()
        {
            string dscode = @"def Scale (arr : double[], scalingFactor : double){    scaledArr = [Imperative]    {        counter = 0;        for(val in arr)        {            arr[counter] = scalingFactor * val;            counter = counter + 1;        }        return = arr;    }    return = scaledArr;}xCount =3;dummy = 1;rangeExpression = 0.0..(180*dummy)..#xCount;result = Scale(rangeExpression, 2);";
            ExecutionMirror mirror = thisTest.RunScriptSource(dscode);
            object[] expectedResult = { 0.0, 180.0, 360.0 };
            thisTest.Verify("result", expectedResult, 0);
        }

        [Test]
        public void Regress_1456921()
        {
            string dscode = @"b = 10.0;a = 0.0;d = a..b..c;";
            // Assert.Fail("1456921 - Sprint 20: Rev 2088: (negative), null expected when using an undefined variable ranged expression"); 
            string errmsg = "DNL-1467454 Rev 4596 : Regression : CompilerInternalException coming from undefined variable used in range expression";
            ExecutionMirror mirror = thisTest.VerifyRunScriptSource(dscode, errmsg);
            object expectedResultc = null;
            thisTest.Verify("d", expectedResultc);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454697()
        {
            String code =
            @"    def foo : double (array : double[])    {        return = 1.0 ;    }        arr = {1,2,3};    arr2 = foo(arr);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            //Verification
            object arr2 = 1.0;
            thisTest.Verify("arr2", arr2, 0);
        }

        [Test]
        public void Regress_1457179()
        {
            string code = @"import (""TestImport.ds"");//external (""libmath"") def dc_sin : double (val : double);def Sin : double (val : double){    return = dc_sin(val);}result1 = Sin(90);result2 = Sin(90.0);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            //Verification
            object result1 = 180.0;
            object result2 = 180.0;
            thisTest.Verify("result1", result1, 0);
            thisTest.Verify("result2", result2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458561()
        {
            Object[] t1 = new Object[] { 10, 20 };
            string code = @"class A{ 	x1 : int[] = {10,20};	constructor A () 	{				}}a = A.A();t1 = a.x1;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t1", t1, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458785()
        {
            string code = @"class A{    x : var;	constructor A ()	{	    x = 1;	}		def foo ( i )	{		return = i;	}}	a1 = A.A();a2 = a1.foo();a3 = 2;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a3", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458785_2()
        {
            string code = @"def foo ( i:int[]){return = i;}x =  1;a1 = foo(x);a2 = 3;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object[] expectedResult = new Object[] { 1 };
            thisTest.Verify("a1", expectedResult, 0);
            thisTest.Verify("a2", 3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458785_3()
        {
            string code = @"class A{    x : int;	constructor A (i)	{	    x = i;		y = 2;	}	}	a1 = A.A(1);x1 = a1.x;y1 = a1.y;z1 = 2;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object expectedResult = null;
            thisTest.Verify("y1", expectedResult, 0);
            thisTest.Verify("z1", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458785_4()
        {
            string code = @"class A{    private x:int;    constructor A()    {        x = 3;    }    def testPublic()     {        x=x+2; // x= 5         return= x;    }     private def testprivate()    {        x=x-1;          return =x;    }    def testmethod() // to test calling private methods    {        a=testprivate();        return=a;    }    }test1=A.A();test2=z.x;// private member must not be exposed test3=test1.testPublic();test4=test1.testprivate();// private method must not be exposed test5= test1.testmethod(); ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454692()
        {
            string code = @"x;y;[Imperative]{	x = 0;	b = 0..3; //{ 0, 1, 2, 3 }	for( y in b )	{		x = y + x;	}	}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", 6, 0);
            thisTest.Verify("y", 3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454692_2()
        {
            string code = @"def length : int (pts : double[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = numPts;}    arr = 0.0..3.0;//{0.0,1.0,2.0,3.0};num = length(arr);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] expectedresult = new Object[] { 0.0, 1.0, 2.0, 3.0 };
            thisTest.Verify("num", 4, 0);
            thisTest.Verify("arr", expectedresult, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455935()
        {
            string code = @"b;c;d;[Imperative]{	def foo:int ( a : bool )	{		if(a)			return = 1;		else			return = 0;	}		b = foo( 1 );	c = foo( 1.5 );	d = 0;	if(1.5 == true ) 	{	    d = 3;	}}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("b", 1, 0);
            thisTest.Verify("c", 1, 0);
            thisTest.Verify("d", 3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457862()
        {
            string code = @"class point{		x : var;				constructor point1(a : int[])		{			x = a;		}				def foo(a : int)		{			return = a;		}}def foo1(a : int)		{			return = a;		}def foo2(a : int[])		{			return = a[2];		}a3;a4;[Imperative]{	//x1 = 1..4;	x1 = { 1, 2, 3, 4 };	a = point.point1(x1);	a1 = a.x;	a2 = a.foo(x1);		a3 = foo1(x1[0]);	a4 = foo2(x1);}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] x1 = new Object[] { 1.0, 2.0, 3.0, 4.0 };
            Object[] a = new Object[] { 1.0, 2.0, 3.0, 4.0 };
            Object[] a1 = new Object[] { 1, 2, 3, 4 };
            Object[] a2 = new Object[] { 1, 2, 3, 4 };
            thisTest.Verify("a3", 1, 0);
            thisTest.Verify("a4", 3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457885()
        {
            string code = @"c = 5..7..#1;a = 0.2..0.3..~0.2;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] c1 = new Object[] { 5 };
            // Object[] c2 = new Object[] {0.2,0.3};
            thisTest.Verify("c", c1, 0);
            //               thisTest.Verify("a",c2, 0);
        }

        [Test]
        public void Regress_1454966()
        {
            string code = @"class A{	a : var;	constructor CreateA ( a1 : int )	{		a = a1;	}	}a1;[Associative]{		a1 = A.CreateA(1).a;	}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            //Assert.Fail("1460965 - Sprint 18 : Rev 1700 : Design Issue : Accessing properties from collection of class instances using the dot operator should yield a collection of the class properties");
            //thisTest.Verify("a1", new Object[] { new Object[] { 1, 1, 1 }, new Object[] { 2, 2, 2 }, new Object[] { 3, 3, 3 } });
            thisTest.Verify("a1", 1);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454966_2()
        {
            string code = @"class A{    a : var;	constructor A ( i : int)	{	    a = i;	}}def create:A( b ){    a = A.A(b);	return = a;}x = create(3).a;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", 3, 0);
        }

        [Test]
        [Category("Dot Op")]
        public void Regress_1454966_3()
        {
            //    Assert.Fail("1454966 - Sprint15: Rev 720 : [Geometry Porting] Assignment operation where the right had side is Class.Constructor.Property is not working"); 
            string errmsg = "DNL-1467177 sprint24: rev 3152 : REGRESSION : Replication should not be supported in Imperative scope";
            string src = @"class A
{    
	a : var[];    
	constructor A ( b : int )    
	{        
		a = { b, b, b };    
	}		
	
}
[Imperative]
{
	x = { 1, 2, 3 };
	a1 = A.A( x ).a;
	a2 = A.A( x );
	t1 = a2[0].a[0];
	t2 = a2[1].a[1];
	t3 = a2[2].a[2];
	a3 = a2[0].a[0] + a2[1].a[1] +a2[2].a[2];
	
}";
            ExecutionMirror mirror = thisTest.VerifyRunScriptSource(src, errmsg);
            Object[] x = new Object[] { 1, 2, 3 };
            //Object[] a1 = new Object[] { 1, 1, 1 };
            Object[][] a1 = new Object[][] { new object[] { 1, 1, 1 }, new object[] { 2, 2, 2 }, new object[] { 3, 3, 3 } };
            thisTest.Verify("x", x);
            thisTest.Verify("a1", a1);
            //thisTest.Verify("a2", a2);
            thisTest.Verify("t1", 1);
            thisTest.Verify("t2", 2);
            thisTest.Verify("t3", 3);
            thisTest.Verify("a3", 6);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454966_4()
        {
            string code = @"class Test{ A: double[];         constructor Test (a : double[]) {  A = a;  }} value = Test.Test ({1.3,3.0,5.0}); value2 = Test.Test (1.3); getval= value.A;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] x = new Object[] { 1.3, 3.0, 5.0 };
            object c = null;
            thisTest.Verify("getval", x, 0);
        }

        [Test]
        public void Regress_1454966_5()
        {
            string code = @"class Test{ A : double[];         constructor Test (a : double[]) {  A = a;  }} value = Test.Test ({1.3,3.0,5.0}); getval= value.A; getval2= value.A[0]; b=1; getval3= value.A[b]; b=2; getval4= value.A[b]; b=-1; getval5= value.A[b];";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] x = new Object[] { 1.3, 3.0, 5.0 };
            //Assert.Fail("1454966 - Sprint15: Rev 720 : [Geometry Porting] Assignment operation where the right had side is Class.Constructor.Property is not working");
            thisTest.Verify("getval", x, 0);
            thisTest.Verify("getval2", 1.3, 0);
            thisTest.Verify("getval3", 5.0, 0);
            thisTest.Verify("getval4", 5.0, 0);
            thisTest.Verify("getval5", 5.0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454966_6()
        {
            string code = @"class Test{ A : double;         constructor Test (a : double) {  A = a;  }}; value = Test.Test (1.3);def call:double(b:Test){ getval= b.A; return= getval; }c= call(value);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("c", 1.3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454966_7()
        {
            Object[] x = new Object[] { 1.0, 2.0, 3.0 };
            string code = @"class Test{ A : double;        constructor Test (a : double) {  A = a;  }};a1;[Imperative]{		d = { 1,2,3 };		val={0,0,0};	j = 0;		for( i in d )		{			    val[j]=Test.Test(i).A;	    j = j + 1;		}		a1 = val;		}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a1", x, 0);
        }

        [Test]
        public void Regress_1454966_8()
        {
            string code = @"class A{ 	x : int[];			constructor A ( i :int[])	{		x = i;       		} 	public def foo ()	{	    return = x;	}}class B{ 	public x : A[] ;			constructor B (i:A[])	{		x = i;       	} 	public def foo ()	{	    return = x;	}		}x = { 1, 2, 3 };y = { 4, 5, 6 };a1 = A.A(x);a2 = A.A(y);b1 = B.B({a1,a2});t1 = b1.x[0].x[1];";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t1", 2, 0);
        }

        [Test]
        public void Regress_1454966_9()
        {
            string code = @"class A{a:int;constructor A (){}}class B{public x : var ;constructor B (i){x = i;}}a1 = A.A();a2 = A.A();a3 = {a1,a2};b1 = B.B(a3);b2=b1[0].x.a;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("b2", 0, 0);
        }

        [Test]
        public void Regress_1454966_10()
        {
            string code = @"class A{a : var[];constructor A ( b : int ){a = { b, b, b };}}t1;[Imperative]{	x = { 1, 2, 3 };	//a1 = A.A( x ).a; // error here	a2 = A.A( x );	t1 = a2[0].a[0];}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t1", 1, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456895()
        {
            string code = @"class collection{                                public a : int[];                                constructor create( b : int)                {                                a = { b , b};                }                                def ret_col ( )                {                                return = a;                }}d;[Associative]{                c1 = collection.create( 3 );                d = c1.ret_col();}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] x = new Object[] { 3, 3 };
            thisTest.Verify("d", x, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456895_2()
        {
            string code = @"class collection{                                	public a : var[]..[];                                	constructor create( b : int[]..[])                	{		a = b;                	}                                	def ret_col ( )                	{		return = a[0];                	}}c;d;[Associative]{                    c = { 3, 3 };	c1 = collection.create( c );                	d = c1.ret_col();}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] x = new Object[] { 3, 3 };
            thisTest.Verify("c", x, 0);
            thisTest.Verify("d", 3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456895_3()
        {
            string code = @"class A{	Pts : var[];	constructor A ( pts : double[] )	{	    Pts = pts;	}	def length ()	{		numPts = [Imperative]		{			counter = 0;			for(pt in Pts)			{				counter = counter + 1;			}						return = counter;		}		return = numPts;	}}    arr = {0.0,1.0,2.0,3.0};a1 = A.A(arr);num = a1.length(); // expected 4, recieved 1";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] arr = new Object[] { 0.0, 1.0, 2.0, 3.0 };
            thisTest.Verify("arr", arr, 0);
            thisTest.Verify("num", 4, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456713()
        {
            string code = @"a = 2.3;b = a * 3;c = 2.32;d = c * 3;e1=0.31;f=3*e1;g=1.1;h=g*a;i=0.99999;j=2*i;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 2.3, 0);
            thisTest.Verify("b", 6.9, 0);
            thisTest.Verify("c", 2.32, 0);
            thisTest.Verify("d", 6.96, 0);
            thisTest.Verify("e1", 0.31, 0);
            thisTest.Verify("f", 0.93, 0);
            thisTest.Verify("g", 1.1, 0);
            thisTest.Verify("h", 2.53, 0);
            thisTest.Verify("i", 0.99999, 0);
            thisTest.Verify("j", 1.99998, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454511()
        {
            string code = @"x;[Imperative]{	x = 0;		for ( i in b )	{		x = x + 1;	}}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", 0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456758()
        {
            string code = @"a = true && true ? -1 : 1;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", -1, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459175()
        {
            string code = @"class Point{    X : double;	Y : double;		constructor ByCoordinates( x : double, y: double )	{	    X = x;	    Y = y;				}	}p1 = Point.ByCoordinates( 5.0, 10.0);a1 = p1.create(4.0,5.0);a2 = a1.X; // expected null here!!a3 = a1.Y; // expected null here!!";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object expectedResultc = null;
            thisTest.Verify("a1", expectedResultc, 0);
            thisTest.Verify("a2", expectedResultc, 0);
            thisTest.Verify("a3", expectedResultc, 0);
        }
        [Test, Ignore]
        [Category("Type System")]
        public void Regress_1459175_2()
        {
            Assert.Inconclusive();
            string code = @"class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : A (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = pts;}pt1 = A.A( 1 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts);test =numpts[0].X;test2 =numpts[1].X;	";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("test", 1, 0);
            thisTest.Verify("test2", 10, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457903()
        {
            string code = @"a = 1..7..#2.5;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object expectedResultc = null;
            thisTest.Verify("a", expectedResultc, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458918_1()
        {
            // Assert.Fail("1467247 - Sprint25: rev 3448 : REGRESSION : Runtime exception thrown when setting a private property of a class instance");
            string code = @"class A{ 	public x : var ;		private y : var ;	//protected z : var = 0 ;	constructor A ()	{		   		}	public def foo1 (a)	{	    x = a;		y = a + 1;		return = x + y;	} 	private def foo2 (a)	{	    x = a;		y = a + 1;		return = x + y;	}	}a = A.A();a1 = a.foo1(1);a2 = a.foo2(1);a.x = 4;a.y = 5;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a1", 3, 0);
            thisTest.Verify("a2", null, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458918_2()
        {
            string code = @"class A{    private x:int;    constructor A()    {        x = 3;    }    def testPublic()     {        x=x+2; // x= 5         return= x;    }     private def testprivate()    {        x=x-1;          return =x;    }    def testmethod() // to test calling private methods    {        a=testprivate();        return=a;    }    }test1=A.A();//test2=test1.x;// private member must not be exposed test3=test1.testPublic();//test4=test1.testprivate();// private method must not be exposed test5= test1.testmethod();class B extends A{        constructor B()    {        x = 4;         }    def foo()    {        x=1;       return = x;    }}a=B.B();// x is private so would not be assigned any vb=a.x;// private member must not be exposed ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("test3", 5, 0);
            thisTest.Verify("test5", 4, 0);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1454918_1()
        {
            //Assert.Fail("1467156 - Sprint 25 - Rev 3026 type checking of return types at run time ");
            string code = @"d;[Associative] // expected 2.5{	 def Divide : double (a:int, b:int)	 {	  return = a/b;	 }	 d = Divide (5,2);}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("d", 2.5);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454918_2()
        {
            string code = @"d;[Associative] // expected error{	 def foo : int (a:double)	 {		  return = a;	 }	 d = foo (5.5);}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("d", 6, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454918_3()
        {
            string code = @"d;[Associative] // expected d = 5.0{	 def foo : double (a:double)	 {		  return = a;	 }	 d = foo (5.0);} ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("d", 5.0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454918_4()
        {
            string code = @"d;[Associative] // expected error{	 def foo : double (a:bool)	 {		  return = a;	 }	 d = foo (true);}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("d", c, 0);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1454918_5()
        {
            //Assert.Fail("1467156 - Sprint 25 - Rev 3026 type checking of return types at run time ");
            string code = @"d;class A{	a : var;	constructor CreateA ( a1 : int )	{		a = a1;	}	}[Associative] {	 def foo : int (a : A)	 {             return = a;	 }	 a1 = A.CreateA(1);	 d = foo (a1);}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("d", c, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454918_6()
        {
            string code = @"     def foo : double ()	 {		  return = 5;	 }	 d = foo ();";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("d", 5.0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611()
        {
            string code = @"class A{    a : var;	constructor A ( i : int)	{	    a = i;	}}def create( b ){    a = A.A(b);	return = a;}x = create(3);y = x.a;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("y", 3, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611_2()
        {
            string code = @" class A{    X : int;            constructor A(x : int)    {        X = x;            }}numpts;[Imperative]{    def length : int (pts : A[])    {        numPts = [Associative]        {            return = [Imperative]            {                counter = 0;                for(pt in pts)                {                    counter = counter + 1;                }                        return = counter;            }        }        return = numPts;    }    pt1 = A.A( 0 );    pt2 = A.A( 10 );    pts = {pt1, pt2};    numpts = length(pts); // getting null}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("numpts", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611_3()
        {
            string code = @"// function test -return class array, argument as class array class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : A[] (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = pts;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); // getting null";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611_4()
        {
            string code = @"//  function test return int , multiple arguments class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : int (pts : A[],num:int){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = num;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts,5); // getting null";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("numpts", 5, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611_5()
        {
            string code = @"// function test pass an item in hte array as argument , no return type specifiedclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length(pts : A[],num:int ){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = num;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};a={1,2,3};numpts = length(pts,a[0]);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("numpts", 1, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611_6()
        {
            string code = @"// function test pass an item in the array as argument , no return type specifiedclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length(pts : A[],num:int ){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = null;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};a={1,2,3};numpts = length(pts,a[0]);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("numpts", c, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1456611_7()
        {
            string code = @"// no return type specified ad no return statement class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length(pts : A[],num:int ){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }//    return = null;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};a={1,2,3};numpts = length(pts,a[0]);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("numpts", c, 0);
        }

        [Test]
        public void Regress_1456611_8()
        {
            //Assert.Fail("Sub-recursion calls with auto promotion on jagged arrays is not working");
            string code = @"// function test pass an item in the array as argument , no return type specifiedclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length :A[](pts : A[]){       return = pts;}def nested(pts:A[] ){    pt1 = A.A( 5 );    pts2={pts,pt1};    return =length(pts2);}gpt1 = A.A( 0 );gpt2 = A.A( 10 );gpts = {gpt1, gpt2};a={1,2,3};numpts = nested(gpts);t1 = numpts[0][0].X;t2 = numpts[0][1].X;t3 = numpts[1][0].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t1", 0, 0);
            thisTest.Verify("t2", 10, 0);
            thisTest.Verify("t3", 5, 0);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1456611_9()
        {
            //Assert.Fail("DNL-1467208 Auto-upcasting of int -> int[] is not happening on function return");
            string code = @"// test rank of return type class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length :A[](pts : A[]){       return = pts;}def nested:A[][](pts:A[] )//return type 2 dimensional{    pt1 = A.A( 5 );    pt2 = A.A( 5 );  //  pts2={pts,{pt1,pt2}};    return =length(pts); // returned array 1 dimensional}gpt1 = A.A( 0 );gpt2 = A.A( 10 );gpts = {gpt1, gpt2};a={1,2,3};res = nested(gpts);numpts=res[0][0].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("numpts", 0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459372()
        {
            string code = @"collection = { 2, 2, 2 };collection[1] = 3;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] x = new Object[] { 2, 3, 2 };
            thisTest.Verify("collection", x, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459512()
        {
            string code = @"def length : int (pts : int[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = numPts;}z=length({1,2});";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459171_1()
        {
            string code = @"class Point{    X : double;	Y : double;	constructor ByCoordinates( x : double, y: double )	{	    X = x;		Y = y;				}		def create( )	{	    		return = Point.ByCoordinates( X, Y );			}	}p1 = Point.ByCoordinates( 5.0, 10.0);t1 = p1.X;t2 = p1.Y;a1 = p1.create();a2 = a1.X;a3 = a1.Y;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459171_2()
        {
            string code = @"e1;[Imperative]{	def even : int (a : int) 	{			if(( a % 2 ) > 0 )			return = a + 1;				else 			return = a;	}	x = 1..3..1;	y = 1..9..2;	z = 11..19..2;	c = even(x);	d = even(x)..even(c)..(even(0)+0.5);	e1 = even(y)..even(z)..1;	f = even(e1[0])..even(e1[1]); 	g = even(y)..even(z)..f[0][1]; }";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object[] e1 = {  new object[] {2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12},                               new object[] {4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14},                               new object[] {6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16},                               new object[] {8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18},                               new object[] {10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20}                             };
            thisTest.Verify("e1", e1);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458916()
        {
            string code = @"class A{ 	x : int = 5 ;	}a1 = A.A();x1 = a1.x;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458915()
        {
            string code = @"class A{ 	x : var = 0 ;			constructor A ()	{		 x = 1;     		}	def foo ()	{	    return = x + 1;	}	}class C extends A{ 	y : var ;			constructor C () : base.A()	{		 y = foo();         	 	}	}c = C.C();c1 = c.x;c2 = c.y;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("c1", 1, 0);
            thisTest.Verify("c2", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458915_2()
        {
            string code = @"class CA{    x:int = 1;    constructor CA()    {    }}class CB extends CA{    y:int = 2;    constructor CB()    {    }}b = CB.CB();t = b.x; // expected 1 here";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t", 1, 0);
        }


        [Test]
        [Category("SmokeTest")]
        public void Regress_1458915_3()
        {
            string code = @"class A{ 	x : int = 5 ;	}class B extends A    {    y : int = 5 ;	}a1 = A.A();x1 = a1.x;b1 = B.B();y1 = b1.x;y2 = b1.y;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x1", 5, 0);
            thisTest.Verify("y1", 5, 0);
            thisTest.Verify("y2", 5, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459584()
        {
            string code = @"class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : A[] (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = pts;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts);test=numpts[0].X;test2= numpts[1].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] numpts = new object[] { 0, 10 };
            thisTest.Verify("test", 0, 0);
            thisTest.Verify("test2", 10, 0);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1459584_1()
        {
            string code = @"//return type class and return an array of class-class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : A[] (pts : A[]){    c1 = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = pts;}pt1 = A.A( 0 );pt2 = A.A( 10 );c1 = 0;pts = {pt1, pt2};numpts = length(pts); a=numpts[0].X;b=numpts[1].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 0, 0);
            thisTest.Verify("b", 10, 0);
            thisTest.Verify("c1", 2, 0);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1459584_2()
        {
            //Assert.Fail("1467196 Sprint 25 - Rev 3216 - [Design Issue] when rank of return type does not match the value returned what is the expected result ");
            string code = @"//return type class and return an array of class-class A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : A[] (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = pts[0];}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); a=numpts[0].X;b=numpts[1].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 0, 0);
            thisTest.Verify("b", null, 0);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1459584_3()
        {
            //Assert.Fail("1467196 Sprint 25 - Rev 3216 - [Design Issue] when rank of return type does not match the value returned what is the expected result ");
            string code = @"//return type class and return a doubleclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : A (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = 1.0;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); a=numpts[0].X;b=numpts[1].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            //   thisTest.Verify("a", 0, 0);
            //  thisTest.Verify("b", 10, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459584_4()
        {
            string code = @"//return type int and return a doubleclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : int (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = 1.0;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("numpts", 1, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459584_5()
        {
            string code = @"//return type int and return a doubleclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length : double (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = 1;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("numpts", 1.0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459584_6()
        {
            string code = @"//no return type definedclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length  (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = pts;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); test=numpts[0].X;test2=numpts[1].X;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] numpts = new object[] { 0, 10 };
            thisTest.Verify("test", 0, 0);
            thisTest.Verify("test2", 10, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459584_7()
        {
            string code = @"//no return type defined and return nullclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length  (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = null;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("numpts", c, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459584_8()
        {
            string code = @"//no return statementclass A{    X : int;            constructor A(x : int)    {        X = x;            }}def length  (pts : A[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }   // return = null;}pt1 = A.A( 0 );pt2 = A.A( 10 );pts = {pt1, pt2};numpts = length(pts); ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("numpts", c, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458475()
        {
            string code = @"a = { 1,2 };b1 = a[-1];//b1=2";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("b1", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458475_2()
        {
            string code = @"a = { {1,2},{3,4,5}};b1 = a[0][-1];// b1=2";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("b1", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458187()
        {
            string code = @"//b=true;  //              x = (b == 0) ? b : b+1;def foo1 ( b  ){                x = (b == 0) ? b : b+1;                return = x;}a=foo1(5.0);b=foo1(5);c=foo1(0);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 6.0, 0);
            thisTest.Verify("b", 6, 0);
            thisTest.Verify("c", 0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458187_2()
        {
            string code = @"def foo1 ( b ){x = (b == 0) ? b : b+1;return = x;}a=foo1(true); ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1458187_3()
        {
            string code = @"def foo1 ( b ){x = (b == 0) ? b : b+1;return = x;}a=foo1(null); ";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object c = null;
            thisTest.Verify("a", c, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1454926()
        {
            string code = @"result;result2;[Imperative]{	 	 d1 = null;	 d2 = 0.5;	 	 result = d1 * d2; 	 result2 = d1 + d2;  }";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object result = null;
            thisTest.Verify("result", result, 0);
            thisTest.Verify("result2", result, 0);
        }

        [Test]
        [Category("Escalate")]
        [Category("SmokeTest")]
        public void Regress_1455283()
        {
            string code = @"class MyPoint        {        X: double;    Y: double;        constructor CreateByXY(x : double, y : double)                        {        X = x;        Y = y;                }       }    class MyNewPoint extends MyPoint    {        Z : double;        constructor Create (x: double, y: double, z : double) : base.CreateByXY(x, y)        {         Z = z;        }    }   test = MyNewPoint.Create (10, 20, 30);x = test.X;y = test.Y;z = test.Z;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", 10.0, 0);
            thisTest.Verify("y", 20.0, 0);
            thisTest.Verify("z", 30.0, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1455283_1()
        {
            string code = @"class B{ 	x3 : int ;			constructor B () 	{			x3 = 2;	}}class A extends B{ 	x1 : int ;	x2 : double;		constructor A () : base.B ()	{			x1 = 1; 		x2 = 1.5;			}}a1 = A.A();b1 = a1.x1;b2 = a1.x2;b3 = a1.x3;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("b1", 1, 0);
            thisTest.Verify("b2", 1.5, 0);
            thisTest.Verify("b3", 2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459900()
        {
            string code = @"a:int = 1.3;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459900_1()
        {
            string code = @"def atan ( a : double ) = 0.5 * a;class A{	theta:bool  = atan(5.0) ;        //theta : double = atan(5.0) ; => this works fine	}a1 = A.A();";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1459762()
        {
            string code = @"class A {    a : var;	constructor A ( )	{	    a = 5;	}}r1 = A.A();r2 = r1+1;// expected : r2 = null// recieved : r2 = ptr: 1";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            object r2 = null;
            thisTest.Verify("r2", r2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1452951()
        {
            string code = @"x;[Associative]{	a = { 4,5 };   	[Imperative]	{	       //a = { 4,5 }; // works fine		x = 0;		for( y in a )		{			x = x + y;		}	}}";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", 9, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457023()
        {
            string code = @"class Utils{    public def length : int (pts : double[])    {        numPts = [Imperative]        {            counter = 0;            for(pt in pts)            {                counter = counter + 1;            }                        return = counter;        }        return = numPts;    }        constructor Create()    {}}utils = Utils.Create();arr = {0.0,1.0,2.0,3.0};num = utils.length(arr);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] arr = new object[] { 0.0, 1.0, 2.0, 3.0 };
            thisTest.Verify("arr", arr, 0);
            thisTest.Verify("num", 4, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457023_1()
        {
            string code = @"class Utils{    C : double[];		public def length : int ()    {        counter = 0;				[Imperative]        {                      for(pt in C)            {                counter = counter + 1;            }                   }        return = counter;    }        constructor Create(a : double[])    {		C = a;	}}arr = { 0.0, 1.0, 2.0, 3.0 };utils = Utils.Create(arr);num = utils.length();";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] arr = new object[] { 0.0, 1.0, 2.0, 3.0 };
            thisTest.Verify("arr", arr, 0);
            thisTest.Verify("num", 4, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457023_2()
        {
            string code = @"class A{    b : double[];		constructor A (x : double[])	{		b = x;	}		def add_2:double[]( )	{		j = 0;		x = [Imperative]		{			for ( i in b )			{				b[j] = b[j] + 1;				j = j + 1 ;			}			return = b;		}				return = x;	}}c = { 1.0, 2, 3 };a1 = A.A( c );b2 = a1.add_2( );";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            Object[] b2 = new object[] { 2.0, 3.0, 4.0 };
            thisTest.Verify("b2", b2, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457023_3()
        {
            string code = @"import ( ""testImport.ds"" );class Vector{    //external (""libmath"") def dc_sqrt : double (val : double);    public Length : var;        private def init : bool ()    {        Length = null;        return = true;            }        X : var;    Y : var;    Z : var;        public constructor ByCoordinates(x : double, y : double, z : double)    {        neglect = init();                X = x;        Y = y;        Z = z;    }        public def GetLength ()    {        return = [Imperative]        {            if( Length == null )            {                Length = dc_sqrt(X*X + Y*Y + Z*Z);            }            return = Length;        }    }    }vec =  Vector.ByCoordinates(3.0,4.0,0.0);vec_len = vec.GetLength();";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("vec_len", 12.5, 0);
        }

        [Test]
        [Category("SmokeTest")]
        public void Regress_1457023_4()
        {
            //Assert.Fail("1463372 - Sprint 20 : Rev 2088 : Imperative code is not allowed in class constructor "); 
            string code = @"class A{	a : var[]..[];		constructor create(i:int)	{		[Imperative]		{			if( i == 1 )			{				a = { { 1,2,3 } , { 4,5,6 } };			}			else			{			    a = { { 1,2,3 } , { 1,2,3 } };			}		}		}	}A1 = A.create(1);a1 = A1.a[0][0];";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a1", 1);
        }

        [Test]
        [Category("SmokeTest")]
        public void myTest()
        {
            string code = @"def length : int (pts : int[]){    numPts = [Imperative]    {        counter = 0;        for(pt in pts)        {            counter = counter + 1;        }                return = counter;    }    return = numPts;}z=length({1,2});";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
        }

        [Test]
        [Category("Type System")]
        public void Regress_1462308()
        {
            string code = @"import(TestData from ""ProtoTest.dll"");f = TestData.IncrementByte(101); F = TestData.ToUpper(f);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            // Assert.Fail("1467119 Sprint24 : rev 2807 : Type conversion issue with char");
            thisTest.Verify("f", 102);
            thisTest.Verify("F", 70);
        }

        [Test]
        public void Regress_1467091()
        {
            string code = @"def foo(x:int){    return =  x + 1;}y1 = test.foo(2);y2 = ding().foo(3);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            TestFrameWork.VerifyBuildWarning(ProtoCore.BuildData.WarningID.kIdUnboundIdentifier);
            if (!core.Options.SuppressFunctionResolutionWarning)
            {
                TestFrameWork.VerifyBuildWarning(ProtoCore.BuildData.WarningID.kFunctionNotFound);
            }
            thisTest.Verify("y1", null);
            thisTest.Verify("y2", null);
        }

        [Test]
        public void Regress_1467094_1()
        {
            string code = @"t = {};x = t[3];t[2] = 1;y = t[3];z = t[-1];";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("x", null);
            thisTest.Verify("y", null);
            thisTest.Verify("z", 1);
        }

        [Test]
        public void Regress_1467094_2()
        {
            string code = @"class A  {    x : var;    constructor A ( y : var )    {        x = y;    }}c = { A.A(0), A.A(1) };p = {};d = [Imperative]{    if(c[0].x == 0 )    {        c[0] = 0;    p[0] = 0;    }    if(c[0].x == 0 )    {        p[1] = 1;    }    return = 0;}t1 = c[0];t2 = c[1].x;t3=p[0];t4=p[1];";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t4", null);
        }

        [Test]
        public void Regress_1467104()
        {
            string code = @"class Point{    x : var;            constructor Create(xx : double)    {        x = xx;            }}pts = Point.Create( { 1, 2} );aa = pts[null].x;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("aa", null);
        }

        [Test]
        public void Regress_1467107()
        {
            string code = @"def foo(x:int){    return =  x + 1;}m=null;y = m.foo(2);";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("y", null);
            TestFrameWork.VerifyRuntimeWarning(ProtoCore.RuntimeData.WarningID.kDereferencingNonPointer);
        }

        [Test]
        public void Regress_1467117()
        {
            string code = @"/*/**/a = 1;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("a", 1);
            thisTest.VerifyBuildWarningCount(0);
        }

        [Test]
        public void Regress_1467273()
        {
            string code = @"def foo(x:var[]..[]) { return = 2; }def foo(x:var[]) { return = 1; }d = foo({1,2});";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("d", 1);
            thisTest.VerifyBuildWarningCount(0);
        }

        [Test]
        public void Regress_1467318()
        {
            string code = @"class A {x;constructor A() {x = {2, 3}; }}a = A.A();t = a.x;";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            thisTest.Verify("t", new object[] { 2, 3 });
            thisTest.VerifyBuildWarningCount(0);
        }

        [Test]
        public void Regress_1467505_gc_issue()
        {
            string code = @"class A{    def _Dispose()    {        Print(""A._Dispose()"");    }}def foo(){    [Imperative]    {        a = A.A();    }}foo();";
            ExecutionMirror mirror = thisTest.RunScriptSource(code);
            //thisTest.Verify("t", new object[] { 2, 3 });
            thisTest.VerifyBuildWarningCount(0);
        }
    }
}
