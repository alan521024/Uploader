using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Resources;
using System.Xml;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("DoubleXUI For WPF")]
#if NET4
[assembly: AssemblyDescription("DoubleXUI For WPF 4")]
#else
[assembly: AssemblyDescription("DoubleXUI For WPF 4.5")]
#endif

// 配置文件，如零售、发布、调试等信息。程序集在运行时不会使用该信息(eg:retail 零售)
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DoubleXUI")]
[assembly: AssemblyProduct("DoubleXUI for WPF")]
[assembly: AssemblyCopyright("Copyright ©  2017")]

// 合法商标
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("edf55977-ac98-4db4-856e-8654f999ed2b")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
// 可以指定所有值，也可以使用以下所示的 "*" 预置版本号和修订号
//通过使用 "*"，如下所示:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]


[assembly: XmlnsDefinition("http://doublex.com/doublexui", "DoubleXUI")]
//[assembly: XmlnsDefinition("http://doublex.com/doublexui", "DoubleXUI.Presentation")]
[assembly: XmlnsDefinition("http://doublex.com/doublexui", "DoubleXUI.Controls")]
[assembly: XmlnsPrefix("http://doublex.com/doublexui", "dxui")]

[assembly: ThemeInfo(
    //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.None,
    //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly
)]
[assembly: NeutralResourcesLanguageAttribute("zh-CN")]
