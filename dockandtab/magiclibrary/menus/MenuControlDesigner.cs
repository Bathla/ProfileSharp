// *****************************************************************************
// 
//  (c) Crownwood Consulting Limited 2002 
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Crownwood Consulting 
//	Limited, Haxey, North Lincolnshire, England and are supplied subject to 
//	licence terms.
// 
//  Magic Version 1.7 	www.dotnetmagic.com
// *****************************************************************************

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using SharpClient.UI.Controls;

namespace SharpClient.UI.Menus
{
    public class MenuControlDesigner :  System.Windows.Forms.Design.ParentControlDesigner
    {
        public override ICollection AssociatedComponents
        {
            get 
            {
                if (base.Control is SharpClient.UI.Menus.MenuControl)
                    return ((SharpClient.UI.Menus.MenuControl)base.Control).MenuCommands;
                else
                    return base.AssociatedComponents;
            }
        }

        protected override bool DrawGrid
        {
            get { return false; }
        }
    }
}
