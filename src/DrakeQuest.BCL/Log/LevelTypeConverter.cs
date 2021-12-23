// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// John R. Hicks (angryjohn69@nc.rr.com)
// Gerry Shaw (gerry_shaw@yahoo.com)
// William E. Caputo (wecaputo@thoughtworks.com | logosity@yahoo.com)
// Gert Driesen (drieseng@users.sourceforge.net)
//
// Some of this class was based on code from the Mono class library.
// Copyright (C) 2002 John R. Hicks <angryjohn69@nc.rr.com>
//
// The events described in this file are based on the comments and
// structure of Ant.
// Copyright (C) Copyright (c) 2000,2002 The Apache Software Foundation.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;


namespace DrakeQuest.Log
{

	/// <summary>
	/// Specialized <see cref="EnumConverter" /> for <see cref="Level" />
	/// that ignores case when converting from string.
	/// </summary>
	internal class LevelConverter : EnumConverter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LevelConverter" />
		/// class.
		/// </summary>
		public LevelConverter() : base(typeof(LevelEnum))
		{
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the 
		/// specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="CultureInfo"/> object. If a <see langword="null"/> is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <returns>
		/// An <see cref="Object"/> that represents the converted value.
		/// </returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string stringValue)
			{
				return Enum.Parse(EnumType, stringValue, true);
			}

			// default to EnumConverter behavior
			return base.ConvertFrom(context, culture, value);
		}
	}
}
