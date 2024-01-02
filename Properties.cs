// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.Properties.Resources
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Pololu.MaestroControlCenter.Properties
{
    [CompilerGenerated]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [DebuggerNonUserCode]
    internal class Resources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal Resources()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)Pololu.MaestroControlCenter.Properties.Resources.resourceMan, (object)null))
                    Pololu.MaestroControlCenter.Properties.Resources.resourceMan = new ResourceManager("Pololu.MaestroControlCenter.Properties.Resources", typeof(Pololu.MaestroControlCenter.Properties.Resources).Assembly);
                return Pololu.MaestroControlCenter.Properties.Resources.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => Pololu.MaestroControlCenter.Properties.Resources.resourceCulture;
            set => Pololu.MaestroControlCenter.Properties.Resources.resourceCulture = value;
        }

        internal static Bitmap about_box
        {
            get
            {
                return (Bitmap)Pololu.MaestroControlCenter.Properties.Resources.ResourceManager.GetObject(nameof(about_box), Pololu.MaestroControlCenter.Properties.Resources.resourceCulture);
            }
        }
    }
}
