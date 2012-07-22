// *****************************************************************************
// 
//  (c) Crownwood Consulting Limited 2002 
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Crownwood Consulting 
//	Limited, Haxey, North Lincolnshire, England and are supplied subject to 
//	licence terms.
// 
//  DotNetMagic Version 1.8
// *****************************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;
using SharpClient.UI.Common;

namespace SharpClient.UI.Docking
{
    public class HotZoneNull : HotZone
    {
        public HotZoneNull(Rectangle hotArea)
            : base(hotArea, hotArea)
        {
        }

        public override void DrawIndicator(Point mousePos) {}
        public override void RemoveIndicator(Point mousePos) {}
    }
}