//
// Copyright (c) 2011 Scott Clayton
//
// This file is part of the C# to PHP Encryption Library.
//   
// The C# to PHP Encryption Library is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//   
// The C# to PHP Encryption Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with the C# to PHP Encryption Library.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Secure
{
    public static class Utility
    {
        public static string ToUrlSafeBase64(byte[] input)
        {
			string result = "";
			try 
			{
				result = Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_");
			}
			catch(Exception e)
			{
				string infoStr = string.Format("ToBase64 Error : {0}", System.Text.UTF8Encoding.UTF8.GetString(input));
				Debug.Log(infoStr);
			}
            
			return result;
        }

        public static byte[] FromUrlSafeBase64(string input)
        {
			byte[] result = null;
			try
			{
				result = Convert.FromBase64String(input.Replace("-", "+").Replace("_", "/"));
			}
			catch(Exception e)
			{
				string infoStr = string.Format("FromBase64 Error : {0}", input);
				Debug.Log(infoStr);
			}
			
            return result;
        }
    }
}
