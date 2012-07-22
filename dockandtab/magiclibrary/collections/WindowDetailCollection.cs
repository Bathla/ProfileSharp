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
using SharpClient.UI.Docking;
using SharpClient.UI.Collections;

namespace SharpClient.UI.Collections
{
    public class WindowDetailCollection : CollectionWithEvents
    {
        public WindowDetail Add(WindowDetail value)
        {
            // Use base class to process actual collection operation
            base.List.Add(value as object);

            return value;
        }

        public void AddRange(WindowDetail[] values)
        {
            // Use existing method to add each array entry
            foreach(WindowDetail page in values)
                Add(page);
        }

        public void Remove(WindowDetail value)
        {
            // Use base class to process actual collection operation
            base.List.Remove(value as object);
        }

        public void Insert(int index, WindowDetail value)
        {
            // Use base class to process actual collection operation
            base.List.Insert(index, value as object);
        }

        public bool Contains(WindowDetail value)
        {
            // Use base class to process actual collection operation
            return base.List.Contains(value as object);
        }

        public WindowDetail this[int index]
        {
            // Use base class to process actual collection operation
            get { return (base.List[index] as WindowDetail); }
        }

        public int IndexOf(WindowDetail value)
        {
            // Find the 0 based index of the requested entry
            return base.List.IndexOf(value);
        }
    }
}
