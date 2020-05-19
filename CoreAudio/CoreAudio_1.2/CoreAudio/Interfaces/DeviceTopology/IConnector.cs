﻿/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace CoreAudio.Interfaces
{
    [Guid("9c2c4058-23f5-41de-877a-df3af236a09e"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IConnector
    {
        [PreserveSig]
        int GetType(out ConnectorType type);
        [PreserveSig]
        int GetDataFlow(out EDataFlow flow);
        [PreserveSig]
        int ConnectTo(IConnector connectTo);
        [PreserveSig]
        int Disconnect();
        [PreserveSig]
        int IsConnected(out bool connected);
        [PreserveSig]
        int GetConnectedTo(out IConnector connectedTo);
        [PreserveSig]
        int GetConnectorIdConnectedTo([Out(), MarshalAs(UnmanagedType.LPWStr)] out string connectorId);
        [PreserveSig]
        int GetDeviceIdConnectedTo([Out(), MarshalAs(UnmanagedType.LPWStr)] out string deviceId);
    }
}
